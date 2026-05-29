using API.BackgroundServices;
using Infrastructure;
using Shared.Results;
var builder = WebApplication.CreateBuilder(args);

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

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();