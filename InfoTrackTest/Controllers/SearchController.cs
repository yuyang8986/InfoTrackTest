using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InfoTrackTest.Core.Interfaces.Service;
using InfoTrackTest.Core.Interfaces.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Strings = InfoTrackTest.Core.Constants.Strings;

namespace InfoTrackTest.Controllers
{
    [Route("search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IGoogleSearchService _searchService;

        public SearchController(IGoogleSearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> Search()
        {
            try
            {
                IResultViewModel viewModel = await _searchService.Search(Strings.CEOSearchKeyWords);

                return Ok(viewModel);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}