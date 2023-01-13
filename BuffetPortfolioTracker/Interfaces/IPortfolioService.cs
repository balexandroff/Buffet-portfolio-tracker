using BuffetPortfolioTracker.Models;

namespace BuffetPortfolioTracker.Interfaces
{
    public interface IPortfolioService
    {
        Task<IEnumerable<Stock>> UpdateLocalDataAsync();
        Task<IEnumerable<Stock>> CheckForUpdatesAsync();
        Task<IEnumerable<Stock>> CompareAsync(IEnumerable<Stock> newData);
    }
}
