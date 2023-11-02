﻿using Asp.Versioning;
using GeoCop.Api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApiVersioning(config =>
{
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.ReportApiVersions = true;
    config.ApiVersionReader = new HeaderApiVersionReader("api-version");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DeliveryContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DeliveryContext"), o =>
    {
        o.UseNetTopologySuite();
    });
});

var app = builder.Build();

// Migrate db changes on startup
using var scope = app.Services.CreateScope();
using var context = scope.ServiceProvider.GetRequiredService<DeliveryContext>();
context.Database.Migrate();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
