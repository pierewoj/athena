using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Athena.Api.Configuration;
using Athena.Api.Model;
using Microsoft.Extensions.Logging;
using LanguageExt;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Athena.Api.Infrastructure.Services
{
    public interface ICrawlablePageService
    {
        Option<string> GetPage();
        void Save(IEnumerable<string> pages);
        void RemoveFromCrawlQueue(string page);
    }

    class CrawlablePageService : ICrawlablePageService
    {
        private readonly IShouldCrawlDecider _shouldCrawlDecider;
        private readonly ILogger<CrawlablePageService> _logger;
        private readonly IRedisProvider _redisProvider;

        public CrawlablePageService(IShouldCrawlDecider shouldCrawlDecider, ILogger<CrawlablePageService> logger, 
            IRedisProvider redisProvider)
        {
            _redisProvider = redisProvider;
            _logger = logger;
            _shouldCrawlDecider = shouldCrawlDecider;
        }

        public Option<string> GetPage()
        {
            using (var client = _redisProvider.GetDatabase().GetReadOnlyClient())
            {
                var page = client.GetRandomItemFromSet("to_crawl");
                if (page == null)
                {
                    _logger.LogInformation("No more pages to crawl. Returning None.");
                    return Option<string>.None;
                }
                return page;
            }
        }
        

        public void Save(IEnumerable<string> pages)
        {
            var toAdd = pages.Where(_shouldCrawlDecider.ShouldCrawl).ToList();
            var toAddKey = Guid.NewGuid().ToString();
            var tmpKey = Guid.NewGuid().ToString();
            using (var client = _redisProvider.GetDatabase().GetClient())
            {
                _logger.LogInformation("Saving pages for future crawling.");
                client.AddRangeToSet(toAddKey, toAdd);
                client.StoreDifferencesFromSet(tmpKey, toAddKey, "crawled");
                client.StoreUnionFromSets("to_crawl","to_crawl", tmpKey);
                client.RemoveEntry(tmpKey, toAddKey);
            }
        }

        public void RemoveFromCrawlQueue(string page)
        {
            _logger.LogInformation($"Removing [{page}] from crawl queue");
            using (var client = _redisProvider.GetDatabase().GetClient())
            {
                client.RemoveItemFromSet("to_crawl", page);
                client.AddItemToSet("crawled",page);
            }
        }
    }
}
