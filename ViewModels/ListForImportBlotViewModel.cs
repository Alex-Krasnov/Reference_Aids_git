using Reference_Aids.Models;

namespace Reference_Aids.ViewModels
{
    public class ListForImportBlotViewModel
    {
        public int CountRow { get; set; }
        public IEnumerable<ListTestSystem>? ListTestSystems { get; set; }
    }
}
