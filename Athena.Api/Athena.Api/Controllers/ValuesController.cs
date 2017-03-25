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
        [Route("{type}")]
        public IActionResult GetPage(string type)
        {
            var pageOption = _crawlablePageService.GetPage();
            var result = pageOption.Match<IActionResult>(uri => Ok(new CrawlablePage(){Uri = uri}), NoContent);
            return result;
        }

        [HttpPut]
        [Route("{type}")]
        public IActionResult PutPage(string type, [FromBody] CrawlablePage page)
        {
            if (page == null)
                return BadRequest("Valid json body with page info has to be provided with request.");
            if (page.Uri == null)
                return BadRequest("Page.Uri cannot be null.");
            try
            {
                var uri = new Uri(page.Uri);
            }
            catch (Exception)
            {
                return BadRequest("Page.Uri has to be a valid absolute uri.");
            }

            _crawlablePageService.Save(page.Uri);
            return Ok();
        }
    }
}
