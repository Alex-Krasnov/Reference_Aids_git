using Reference_Aids.Data;

namespace Reference_Aids.ModelsForInput
{
    public class InputSearch
    {
        public int? PatientId { get; set; }
        public string? FamilyName { get; set; }
        public string? FirstName { get; set; }
        public string? ThirdName { get; set; }
        public string? SexName { get; set; }
        public string? RegionName { get; set; }
        public string? BirthDate { get; set; }
        public string? NumIfa { get; set; }
        public string? Snils { get; set; }

        public int? SexId(Reference_AIDSContext _context)
        {
            if (SexName != null)
            {
                return _context.ListSexes.Where(e => e.SexNameShort == SexName).ToList()[0].SexId;
            }
            return null;
        }
        public int? RegionId(Reference_AIDSContext _context)
        {
            if (RegionName != null)
            {
                return _context.ListRegions.Where(e => e.RegionName == RegionName).ToList()[0].RegionId;
            }
            return null;
        }
    }
}
