using BuffetPortfolioTracker.Interfaces;
using BuffetPortfolioTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace BuffetPortfolioTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;
        private readonly ILogger<PortfolioController> _logger;

        public PortfolioController(IPortfolioService portfolioService, ILogger<PortfolioController> logger)
        {
            _portfolioService = portfolioService;
            _logger = logger;
        }

        [HttpGet]
        [Route("Update")]
        public async Task<IEnumerable<Stock>> UpdateAsync()
        {
            return await _portfolioService.UpdateLocalDataAsync();
        }

        [HttpGet]
        [Route("CheckForUpdates")]
        public async Task<IEnumerable<Stock>> CheckForUpdatesAsync()
        {
            return await _portfolioService.CheckForUpdatesAsync();
        }
    }
}