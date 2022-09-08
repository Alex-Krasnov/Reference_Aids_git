using Reference_Aids.Models;

namespace Reference_Aids.ViewModels
{
    public class ListForInputAnalyzesViewModel
    {
        public IEnumerable<ListTestSystem>? ListTestSystems { get; set; }
        public IEnumerable<ListResult>? ListResults  { get; set; }
        public IEnumerable<ListTypeAntigen>? ListTypeAntigens { get; set; }
        public List<TblIncomingBlood> TblIncomingBloods { get; set; } = null!;
        public IEnumerable<TblResultAntigen>? TblResultAntigens { get; set; }
        public IEnumerable<TblResultPcr>? TblResultPcrs { get; set; }
        public IEnumerable<TblResultIfa>? TblResultIfas { get; set; }
        public IEnumerable<TblResultBlot>? TblResultBlots { get; set; }
    }
}
