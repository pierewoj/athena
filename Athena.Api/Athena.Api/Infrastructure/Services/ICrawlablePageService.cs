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
            var page = _redisProvider.GetDatabase().SetRandomMember("to_crawl");
            if (!page.HasValue)
            {
                _logger.LogInformation("No more pages to crawl. Returning None.");
                return Option<string>.None;
            }
            
            return page.ToString();
        }
        

        public void Save(IEnumerable<string> pages)
        {
            _logger.LogInformation("Saving pages for future crawling.");
            var toAdd = pages.Where(_shouldCrawlDecider.ShouldCrawl);
            var toAddRedis = toAdd.Select(s => (RedisValue) s).ToArray();
            var db = _redisProvider.GetDatabase();
            var toAddKey = Guid.NewGuid().ToString();
            var tmpKey = Guid.NewGuid().ToString();
            db.SetAdd(toAddKey, toAddRedis);
            db.SetCombineAndStore(SetOperation.Difference, tmpKey, toAddKey, "crawled");
            db.SetCombineAndStore(SetOperation.Union, "to_crawl", "to_crawl", tmpKey);
            db.KeyDelete(tmpKey);
            db.KeyDelete(toAddKey);
        }

        public void RemoveFromCrawlQueue(string page)
        {
            _logger.LogInformation($"Removing [{page}] from crawl queue");
            var r1 = _redisProvider.GetDatabase().SetRemove("to_crawl", page);
            var r2 = _redisProvider.GetDatabase().SetAdd("crawled", page);
        }
    }
}
