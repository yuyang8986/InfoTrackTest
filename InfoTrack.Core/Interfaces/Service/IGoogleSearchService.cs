using System.Threading.Tasks;
using InfoTrackTest.Core.Interfaces.ViewModels;

namespace InfoTrackTest.Core.Interfaces.Service
{
    public interface IGoogleSearchService
    {
        Task<IResultViewModel> Search(string keywords);
    }
}