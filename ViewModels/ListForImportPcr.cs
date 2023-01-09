using Reference_Aids.Models;

namespace Reference_Aids.ViewModels
{
    public class ListForImportPcr
    {
        public int IfaStart { get; set; }
        public int IfaEnd { get; set; }
        public string TestSystemName { get; set; }
        public string Date { get; set; }
        public string DateId { get; set; }
        public IEnumerable<ListTestSystem>? ListTestSystems { get; set; }
        public IEnumerable<ListResult>? ListResults { get; set; }

    }
}
