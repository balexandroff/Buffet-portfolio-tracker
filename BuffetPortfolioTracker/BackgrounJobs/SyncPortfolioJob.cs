using BuffetPortfolioTracker.Interfaces;
using Quartz;

namespace BuffetPortfolioTracker.BackgrounJobs
{
    public class SyncPortfolioJob : IJob
    {
        private readonly IPortfolioService _portfolioService;
        public SyncPortfolioJob(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _portfolioService.UpdateLocalDataAsync();
        }
    }
}
