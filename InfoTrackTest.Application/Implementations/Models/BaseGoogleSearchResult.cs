using InfoTrackTest.Core.Interfaces;
using InfoTrackTest.Core.Interfaces.Core;

namespace InfoTrackTest.Application.Implementations.Models
{
    public class BaseGoogleSearchResult : IIndex, ITitle
    {
        public int Index { get; set; }
        public string Title { get; set; }
    }
}