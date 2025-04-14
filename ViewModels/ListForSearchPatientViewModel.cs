using Reference_Aids.Models;

namespace Reference_Aids.ViewModels
{
    public class ListForSearchPatientViewModel
    {
        public IEnumerable<TblPatientCard>? TblPatientCards { get; set; }
        public IEnumerable<ListSex> ListSexes { get; set; } = null!;
        public IEnumerable<ListRegion> ListRegions { get; set; } = null!;

        public int? ID { get; set; }
        public string? FamilyName { get; set; }
        public string? FirstName { get; set; }
        public string? ThirdName { get; set; }
        public string? BirthDate { get; set; }
        public string? SexName { get; set; }
        public string? RegoinName { get; set; }
        public string? NumIfa { get; set; }
        public string? Snils { get; set; }
    }
}
