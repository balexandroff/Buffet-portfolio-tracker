using BuffetPortfolioTracker.Interfaces;
using BuffetPortfolioTracker.Models;
using BuffetPortfolioTracker.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BuffetPortfolioTracker.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IEmailSender _emailSender;
        private readonly Configuration _configuration;
        private readonly ILogger<PortfolioService> _logger;

        public PortfolioService(IEmailSender emailSender, IOptions<Configuration> configuration, ILogger<PortfolioService> logger)
        {
            _emailSender = emailSender;
            _configuration = configuration.Value;
            _logger = logger;
        }

        public async Task<IEnumerable<Stock>> UpdateLocalDataAsync()
        {
            try
            {
                var onlineData = await FetchDataAsync();

                var json = JsonConvert.SerializeObject(onlineData);

                if (!Directory.Exists(_configuration.JsonStoragePath))
                    Directory.CreateDirectory(_configuration.JsonStoragePath);

                File.WriteAllText(Path.Combine(_configuration.JsonStoragePath, _configuration.FileName), json);
                File.WriteAllText(Path.Combine(_configuration.JsonStoragePath, string.Format(_configuration.FileNamePerDate, DateTime.Now)), json);

                var difference = await this.CompareAsync(onlineData);

                if (difference.Any())
                    await _emailSender.SendEmailAsync(_configuration.MailerReceiverEmail, "Buffet Portfolio Update", $"Hi receiver," +
                        $"<br />" +
                        $"We are happy to inform you that there is an update in the Buffet's portfolio!" +
                        $"<br />" +
                        $"Here is a quick summary of what have been changed:" +
                        $"<br /><br />" +
                        $"{string.Join(string.Empty, difference.Select((d, i) => $"<b>{i+1}) {d.Name} - {d.Ticker} -> {d.Action}{Environment.NewLine}</b><br />").ToList())}" +
                        $"<br />" +
                        $"Happy investing :)");

                return await Task.FromResult(onlineData);
            }
            catch(Exception ex) 
            {
                return await Task.FromResult(new List<Stock>());
            }
        }

        public async Task<IEnumerable<Stock>> CheckForUpdatesAsync()
        {
            try
            {
                var onlineData = await FetchDataAsync();

                var difference = await this.CompareAsync(onlineData);

                return await Task.FromResult(difference);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new List<Stock>());
            }
        }

        public async Task<IEnumerable<Stock>> CompareAsync(IEnumerable<Stock> newData)
        {
            List<Stock> differences = new List<Stock>();

            var lastSavedRawData = File.ReadAllText(Path.Combine(_configuration.JsonStoragePath, _configuration.FileName));
            var lastSavedObjectData = !string.IsNullOrEmpty(lastSavedRawData) ? JsonConvert.DeserializeObject<List<Stock>>(lastSavedRawData) : new List<Stock>();

            foreach (var oldItem in lastSavedObjectData)
            {
                var newItem = newData.FirstOrDefault(nd => string.Compare(nd.Name, oldItem.Name, true) == 0 &&
                                                           string.Compare(nd.Ticker, oldItem.Ticker, true) == 0);

                if (newItem is not null)
                {
                    if (!newItem.Equals(oldItem))
                    {
                        differences.Add(newItem);
                        differences.Last().Action = $"Updated -> Old position: {oldItem.Holdings}, New position: {newItem.Holdings}";
                    }
                }
                else
                {
                    differences.Add(newItem);
                    differences.Last().Action = $"Removed -> Old position: {oldItem.Holdings}, New position: 0";
                }
            }

            foreach (var newItem in newData)
            {
                var oldItem = lastSavedObjectData.FirstOrDefault(nd => string.Compare(nd.Name, newItem.Name, true) == 0 &&
                                                                       string.Compare(nd.Ticker, newItem.Ticker, true) == 0);

                if (oldItem is not null)
                {
                    if (!newItem.Equals(oldItem))
                    {
                        differences.Add(newItem);
                        differences.Last().Action = $"Updated -> Old position: {oldItem.Holdings}, New position: {newItem.Holdings}";
                    }
                }
                else
                {
                    differences.Add(newItem);
                    differences.Last().Action = $"Added -> Old position: 0, New position: {newItem.Holdings}";
                }
            }

            return await Task.FromResult(differences);
        }

        private async Task<IEnumerable<Stock>> FetchDataAsync()
        {
            List<Stock> result = new List<Stock>();

            using (HttpClient httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync(_configuration.PortfolioUrl);

                if (!string.IsNullOrEmpty(json))
                {
                    json = json.Replace("/*O_o*/", string.Empty);
                    json = json.Replace("\n", string.Empty);
                    json = json.Replace("google.visualization.Query.setResponse(", string.Empty);
                    json = json.Replace(");", string.Empty);

                    JObject jsonObject = JObject.Parse(json);
                    var rows = jsonObject["table"]["rows"];

                    foreach (var row in rows)
                    {
                        if (rows.ToList().IndexOf(row) < rows.Count() - 2)
                        {
                            var stock = new Stock
                            {
                                Name = row["c"][0]["v"]?.Value<string>(),
                                Ticker = row["c"][1]["v"]?.Value<string>(),
                                Holdings = row["c"][2]["f"]?.Value<string>(),
                                Stake = row["c"][3]["f"]?.Value<string>(),
                                MarketPrice = row["c"][4]["f"]?.Value<string>(),
                                Value = row["c"][5]["f"]?.Value<string>(),
                                PercentOfPortfolio = row["c"][6]["f"]?.Value<string>(),
                            };

                            result.Add(stock);
                        }
                    }
                }

                return await Task.FromResult(result);
            }
        }
    }
}
