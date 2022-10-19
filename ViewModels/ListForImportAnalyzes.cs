using Reference_Aids.Models;

namespace Reference_Aids.ViewModels
{
    public class ListForImportAnalyzes
    {
        public IEnumerable<ListTestSystem>? ListTestSystems { get; set; }
        public IEnumerable<string> ListTypeAnalyzes { get; set; }
    }
}
