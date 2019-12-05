using System.Collections.Generic;
using InfoTrackTest.Application.Implementations.Models;
using InfoTrackTest.Core.Interfaces;
using InfoTrackTest.Core.Interfaces.ViewModels;

namespace InfoTrackTest.Application.ViewModels
{
    public class SearchResultViewModel : IResultViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IList<BaseGoogleSearchResult> Results { get; set; }
    }
}