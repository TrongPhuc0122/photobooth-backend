using Application.Interfaces;

namespace API.BackgroundServices;
public class BoothMonitorWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BoothMonitorWorker> _logger;

    public BoothMonitorWorker(IServiceProvider serviceProvider, ILogger<BoothMonitorWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var boothService = scope.ServiceProvider.GetRequiredService<IBoothService>();
                try
                {
                    await boothService.SetStatus();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Lỗi kiểm tra trạng thái: {ex.Message}");
                }
            }
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}