using UrlSh.Data;

namespace UrlSh
{
    public class Worker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly SqidsService _sqidsService;
        private readonly ILogger<Worker> _logger;

        public Worker(IServiceScopeFactory serviceScopeFactory, SqidsService sqidsService, ILogger<Worker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _sqidsService = sqidsService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while(!ct.IsCancellationRequested)
            {
                await using var scope = _serviceScopeFactory.CreateAsyncScope();
                await using var context = scope.ServiceProvider.GetRequiredService<UrlShContext>();
                {
                    foreach (var redirect in context.Redirects.Where(t => t.Code == null).OrderBy(t => t.CreatedAt).Take(1000))
                    {
                        _logger.LogInformation("Adding sqid for redirect {id} to {url}", redirect.Id, redirect.Url);
                        redirect.Code = _sqidsService.Encode(redirect.Id);
                    }

                    await context.SaveChangesAsync(ct);
                }

                await Task.Delay(10000, ct);
            }
        }
    }
}
