﻿using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeoCop.Api.Validation.Interlis
{
    /// <summary>
    /// Validates an INTERLIS transfer <see cref="File"/> provided through an <see cref="IFileProvider"/>.
    /// </summary>
    public class InterlisValidator : IValidator
    {
        private const string UploadUrl = "/api/v1/upload";
        private static readonly TimeSpan pollInterval = TimeSpan.FromSeconds(2);
        private static readonly JsonSerializerOptions jsonOptions = new ()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        };

        private readonly ILogger<InterlisValidator> logger;
        private readonly IConfiguration configuration;
        private readonly IFileProvider fileProvider;

        /// <inheritdoc/>
        public string Name => "ilicheck";

        /// <summary>
        /// Initializes a new instance of the <see cref="InterlisValidator"/> class.
        /// </summary>
        public InterlisValidator(ILogger<InterlisValidator> logger, IConfiguration configuration, IFileProvider fileProvider)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.fileProvider = fileProvider;
        }

        /// <inheritdoc/>
        public async Task<ValidatorResult> ExecuteAsync(ValidationJob validationJob, CancellationToken cancellationToken)
        {
            if (validationJob == null) throw new ArgumentNullException(nameof(validationJob));
            if (string.IsNullOrWhiteSpace(validationJob.TempFileName)) throw new ArgumentException("Transfer file name cannot be empty.", nameof(validationJob));

            fileProvider.Initialize(validationJob.Id);
            if (!fileProvider.Exists(validationJob.TempFileName)) throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Transfer file with the specified name <{0}> not found for validation id <{1}>.", validationJob.TempFileName, validationJob.Id));

            var checkServiceUrl = configuration.GetValue<string>("Validation:InterlisCheckServiceUrl")
                ?? throw new InvalidOperationException("Missing InterlisCheckServiceUrl to validate INTERLIS transfer files.");
            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri(checkServiceUrl),
                DefaultRequestHeaders =
                {
                    { "Accept", "application/json" },
                },
            };

            logger.LogInformation("Validating transfer file <{File}>...", validationJob.TempFileName);
            var uploadResponse = await UploadTransferFileAsync(httpClient, validationJob.TempFileName, cancellationToken).ConfigureAwait(false);
            var statusResponse = await PollStatusAsync(httpClient, uploadResponse.StatusUrl!, cancellationToken).ConfigureAwait(false);
            var logFiles = await DownloadLogFilesAsync(httpClient, statusResponse, validationJob.TempFileName, cancellationToken).ConfigureAwait(false);

            return new ValidatorResult(statusResponse.Status, statusResponse.StatusMessage)
            {
                LogFiles = logFiles,
            };
        }

        private async Task<IliCheckUploadResponse> UploadTransferFileAsync(HttpClient httpClient, string transferFile, CancellationToken cancellationToken)
        {
            using var streamContent = new StreamContent(fileProvider.Open(transferFile));
            using var formData = new MultipartFormDataContent
            {
                { streamContent, "file", transferFile },
            };
            using var response = await httpClient.PostAsync(UploadUrl, formData, cancellationToken).ConfigureAwait(false);

            logger.LogInformation("Uploaded transfer file <{TransferFile}> to interlis-check-service. Status code <{StatusCode}>.", transferFile, response.StatusCode);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(jsonOptions, cancellationToken).ConfigureAwait(false);
                throw new ValidationFailedException(problemDetails?.Detail ?? "Invalid transfer file");
            }

            response.EnsureSuccessStatusCode();

            var uploadResponse = await response.Content.ReadFromJsonAsync<IliCheckUploadResponse>(jsonOptions, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Invalid response from interlis-check-service");
            return uploadResponse;
        }

        private async Task<IliCheckStatusResponse> PollStatusAsync(HttpClient httpClient, string statusUrl, CancellationToken cancellationToken)
        {
            while (true)
            {
                using var response = await httpClient.GetAsync(statusUrl, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var statusResponse = await response.Content.ReadFromJsonAsync<IliCheckStatusResponse>(jsonOptions, cancellationToken).ConfigureAwait(false)
                    ?? throw new InvalidOperationException("Invalid response from interlis-check-service");

                if (statusResponse.Status == Status.Completed
                    || statusResponse.Status == Status.CompletedWithErrors
                    || statusResponse.Status == Status.Failed)
                {
                    return statusResponse;
                }

                await Task.Delay(pollInterval, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<IDictionary<string, string>> DownloadLogFilesAsync(HttpClient httpClient, IliCheckStatusResponse statusResponse, string transferFile, CancellationToken cancellationToken)
        {
            var logFiles = new Dictionary<string, string>();
            var tasks = new List<Task>();
            var transferFileWithoutExtension = Path.GetFileNameWithoutExtension(transferFile);

            if (statusResponse.LogUrl != null)
            {
                var logFile = $"{transferFileWithoutExtension}_log.log";
                logFiles["Log"] = logFile;
                tasks.Add(DownloadLogAsFileAsync(httpClient, statusResponse.LogUrl.ToString(), logFile, cancellationToken));
            }

            if (statusResponse.XtfLogUrl != null)
            {
                var xtfLogFile = $"{transferFileWithoutExtension}_log.xtf";
                logFiles["Xtf-Log"] = xtfLogFile;
                tasks.Add(DownloadLogAsFileAsync(httpClient, statusResponse.XtfLogUrl.ToString(), xtfLogFile, cancellationToken));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return logFiles;
        }

        private async Task DownloadLogAsFileAsync(HttpClient httpClient, string url, string destination, CancellationToken cancellationToken)
        {
            using var logDownloadStream = await httpClient.GetStreamAsync(url, cancellationToken).ConfigureAwait(false);
            using var logFileStream = fileProvider.CreateFile(destination);
            await logDownloadStream.CopyToAsync(logFileStream, cancellationToken).ConfigureAwait(false);
        }
    }
}