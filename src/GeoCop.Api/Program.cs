using Asp.Versioning;
using GeoCop.Api.StacServices;

namespace GeoCop.Api;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // TODO: Only add for STAC Browser
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("All",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
        });

        builder.Services.AddControllers();

        builder.Services.AddSingleton<Context>();

        builder.Services.AddStacData(builder =>
        {
        });

        builder.Services.AddApiVersioning(config =>
        {
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.ReportApiVersions = true;
            config.ApiVersionReader = new HeaderApiVersionReader("api-version");
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseCors("All");

        app.MapControllers();

        app.Run();
    }
}
