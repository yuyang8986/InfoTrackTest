namespace InfoTrackTest.Core.Interfaces.ViewModels
{
    public interface IResultViewModel
    {
        bool Success { get; set; }
        string Message { get; set; }
    }
}