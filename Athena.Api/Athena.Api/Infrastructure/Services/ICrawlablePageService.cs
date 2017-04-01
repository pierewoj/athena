using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Athena.Api.Configuration;
using Athena.Api.Model;
using Microsoft.Extensions.Logging;
using LanguageExt;
using Microsoft.Extensions.Options;

namespace Athena.Api.Infrastructure.Services
{
    public interface ICrawlablePageService
    {
        Option<string> GetPage();
        void Save(IEnumerable<string> pages);
    }

    class CrawlablePageService : ICrawlablePageService
    {
        private readonly Queue<string> _crawlablePagesQueue = new Queue<string>();
        private readonly HashSet<string> _savedPages = new HashSet<string>();
        private readonly IShouldCrawlDecider _shouldCrawlDecider;
        private readonly ILogger<CrawlablePageService> _logger;
        private readonly AthenaConfiguration _athenaConfiguration;

        public CrawlablePageService(IShouldCrawlDecider shouldCrawlDecider, ILogger<CrawlablePageService> logger, IOptions<AthenaConfiguration> configuration)
        {
            _logger = logger;
            _shouldCrawlDecider = shouldCrawlDecider;
            _athenaConfiguration = configuration.Value;

            var startPage = _athenaConfiguration.BaseUri;
            _crawlablePagesQueue.Enqueue(startPage);
        }

        public Option<string> GetPage()
        {
            if (!_crawlablePagesQueue.Any())
            {
                _logger.LogInformation("No more pages to crawl. Returning None.");
                return Option<string>.None;
            }

            var page = _crawlablePagesQueue.Dequeue();
            return page;
        }

        public void Save(string page)
        {
            bool pageSavedBefore = _savedPages.Contains(page);
            var shouldCrawlPage = _shouldCrawlDecider.ShouldCrawl(page);

            if (!pageSavedBefore && shouldCrawlPage)
            {
                _logger.LogDebug($"Saving page [{page}] and adding it to the queue. SavedCount={_savedPages.Count}");
                _savedPages.Add(page);
                _crawlablePagesQueue.Enqueue(page);
            }
            else
            {
                _logger.LogDebug($"Ignoring saving page [{page}] and adding it to the queue. PageSavedBefore={pageSavedBefore}, ShouldCrawl={shouldCrawlPage}");
            }
        }

        public void Save(IEnumerable<string> pages)
        {
            foreach (var crawlablePage in pages)
            {
                Save(crawlablePage);
            }
        }
    }
}
