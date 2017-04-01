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
            return
                page.StartsWith("http://kwestiasmaku.com")
                && IsValidUri(page)
                && !page.Contains("taxonomy")
                && !page.Contains("sort_by")
                && !page.Contains("login")
                && !page.Contains("user");
        }

        public bool IsValidUri(string page)
        {
            try
            {
                var uri = new Uri(page);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
