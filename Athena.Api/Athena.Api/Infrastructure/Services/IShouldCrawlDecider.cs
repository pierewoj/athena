using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Athena.Api.Model;

namespace Athena.Api.Infrastructure.Services
{
    public interface IShouldCrawlDecider
    {
        bool ShouldCrawl(string page);
    }

    class ShouldCrawlDecider : IShouldCrawlDecider
    {
        public bool ShouldCrawl(string page)
        {
            return true;
        }
    }
}
