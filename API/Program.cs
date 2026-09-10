using API.BackgroundServices;
using Infrastructure;
using Shared.Results;
using Microsoft.Extensions.FileProviders;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

var builder = WebApplication.CreateBuilder(args);
// FE
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Đăng ký services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    });
// Gọi DependencyInjection của Infrastructure (đã có DbContext + Services bên trong)
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<BoothMonitorWorker>();

var app = builder.Build();
var imagesPath = Path.Combine(builder.Environment.ContentRootPath, "Images");

app.UseCors("AllowFrontend");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPath),
    RequestPath = "/images"
});

var framePath = Path.Combine(builder.Environment.ContentRootPath, "Frame");
app.UseCors("AllowFrontEnd");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(framePath),
    RequestPath = "/frame"
});

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();