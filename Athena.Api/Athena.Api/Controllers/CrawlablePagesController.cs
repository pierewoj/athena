﻿using System;
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
    public class CrawlablePagesController : Controller
    {
        private readonly ILogger<CrawlablePagesController> _logger;
        private readonly ICrawlablePageService _crawlablePageService;

        public CrawlablePagesController(ILogger<CrawlablePagesController> logger, ICrawlablePageService crawlablePageService)
        {
            _crawlablePageService = crawlablePageService;
            _logger = logger;
        }

        [HttpGet]
        [Route("/pages/crawlable/random")]
        public IActionResult GetPage()
        {
            var pageOption = _crawlablePageService.GetPage();
            var result = pageOption.Match<IActionResult>(
                    uri =>
                    {
                        _logger.LogInformation($"GetPage returns OK with pageUri {uri}");
                        return Ok(new CrawlablePage() {Uri = uri});
                    },() =>
                    {
                        _logger.LogInformation($"GetPage returns NoContent.");
                        return NoContent();
                    }
                );
            
            return result;
        }

        [HttpPut]
        [Route("/pages/crawlable")]
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

        [HttpDelete]
        [Route("/pages/crawlable")]
        public IActionResult DeletePage([FromBody] CrawlablePage page)
        {
            if (page == null)
                return BadRequest("Valid json body with page info has to be provided with request.");
            if (page.Uri == null)
                return BadRequest("Invalid format");
            _crawlablePageService.RemoveFromCrawlQueue(page.Uri);
            return Ok();
        }
    }
}
