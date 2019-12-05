using System;
using System.Linq;
using System.Threading.Tasks;
using InfoTrackTest.Application.Crawler;
using InfoTrackTest.Application.ViewModels;
using InfoTrackTest.Core.Constants;
using InfoTrackTest.Core.Interfaces.Service;
using InfoTrackTest.Core.Interfaces.ViewModels;

namespace InfoTrackTest.Services.Services
{
    public class InfoTrackSearchService : IGoogleSearchService
    {
        public async Task<IResultViewModel> Search(string keyword)
        {
            try
            {
                InfoTrackCrawler crawler = new InfoTrackCrawler(keyword);
                var results = await crawler.Load();

                IResultViewModel viewModel = new SearchResultViewModel
                {
                    Results = results.Where(s => s.Title.ToLower().Contains(Strings.KeyWordCriteria)).ToList(),
                    Message = "Crawled..Successful!",
                    Success = true
                };
                return viewModel;
            }
            catch (Exception e)
            {
                return new SearchResultViewModel
                {
                    Results = null,
                    Message = $"Crawling failed..: {e.Message}",
                    Success = false
                };
            }
        }
    }
}