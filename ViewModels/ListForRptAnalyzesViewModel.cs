using Reference_Aids.Models;

namespace Reference_Aids.ViewModels
{
    public class ListForRptAnalyzesViewModel
    {
        public IEnumerable<ListPatientForRpt>? TblPatientCards { get; set; }
        public IEnumerable<ListRecForRpt> ListRecForRpts { get; set; }
        public int IfaStart { get; set; }
        public int IfaEnd { get; set; }
        public string Doctor { get; set; }
        public string DateId { get; set; }

    }

    public class ListPatientForRpt
    {
        public int NumIfa { get; set; }
        public int PatientId { get; set; }
        public string? FirstName { get; set; }
        public string? FamilyName { get; set; }
        public string? ThirdName { get; set; }
        public string BirthDate { get; set; }
        public int? CategoryPatientId { get; set; }
        public string AddrFull { get; set; }
        public string? ResFull { get; set; }
    }
}
