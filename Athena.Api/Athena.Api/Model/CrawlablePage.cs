using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Athena.Api.Model
{
    public class CrawlablePageCollection
    {
        public List<CrawlablePage> CrawlablePages { get; set; }
    }

    public class CrawlablePage
    {
        public string Uri { get; set; }
    }
}
