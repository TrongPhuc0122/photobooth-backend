using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Gọi DependencyInjection của Infrastructure (đã có DbContext + Services bên trong)
builder.Services.AddInfrastructure(builder.Configuration);

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