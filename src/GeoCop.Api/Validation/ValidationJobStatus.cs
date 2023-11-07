﻿namespace GeoCop.Api.Validation
{
    /// <summary>
    /// Represents the status and log files of a validation job.
    /// </summary>
    public class ValidationJobStatus
    {
        /// <summary>
        /// Status of the validation job.
        /// </summary>
        public Status Status { get; }

        /// <summary>
        /// Human-readable status message.
        /// </summary>
        public string? StatusMessage { get; }

        /// <summary>
        /// Available log files to download.
        /// </summary>
        public IDictionary<string, string> LogFiles { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationJobStatus"/> class.
        /// </summary>
        public ValidationJobStatus(Status status, string? statusMessage, IDictionary<string, string>? logFiles = null)
        {
            Status = status;
            StatusMessage = statusMessage;
            LogFiles = logFiles ?? new Dictionary<string, string>();
        }
    }
}