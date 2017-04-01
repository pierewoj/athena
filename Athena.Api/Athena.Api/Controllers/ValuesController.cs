using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Athena.Api.Infrastructure.Services;
using Athena.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace Athena.Api.Controllers
{
    [Route("/pages/crawlable")]
    public class ValuesController : Controller
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly ICrawlablePageService _crawlablePageService;

        public ValuesController(ILogger<ValuesController> logger, ICrawlablePageService crawlablePageService)
        {
            _crawlablePageService = crawlablePageService;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult GetPage()
        {
            var pageOption = _crawlablePageService.GetPage();
            var result = pageOption.Match<IActionResult>(uri => Ok(new CrawlablePage(){Uri = uri}), NoContent);
            return result;
        }

        [HttpPut]
        public IActionResult PutPages([FromBody] CrawlablePageCollection pages)
        {
            if (pages == null)
                return BadRequest("Valid json body with page info has to be provided with request.");
            if (pages.CrawlablePages == null)
                return BadRequest("Invalid format");
            var uris = pages.CrawlablePages.Select(x => x.Uri.ToString());
            _crawlablePageService.Save(uris);
            return Ok();
        }
    }
}
