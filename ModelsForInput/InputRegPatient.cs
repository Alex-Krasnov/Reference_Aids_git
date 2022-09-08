using Reference_Aids.Data;

namespace Reference_Aids.ModelsForInput
{
    public class InputRegPatient
    {
        public int PatientId { get; set; }
        public string? FirstName { get; set; }
        public string? FamilyName { get; set; }
        public string? ThirdName { get; set; }
        public string? BirthDate { get; set; }
        public string? SexName { get; set; }
        public string? RegionName { get; set; }
        public string? CityName { get; set; }
        public string? AreaName { get; set; }
        public string? PatientCom { get; set; }
        public string? PhoneNum { get; set; }
        public string? AddrHome { get; set; }
        public string? AddrCorps { get; set; }
        public string? AddrFlat { get; set; }
        public string? AddrStreat { get; set; }
        public string? DBlot { get; set; }
        public float CutOff { get; set; }
        public float BlotResult { get; set; }
        public string? TestSystemName { get; set; }
        public string? SendDistrictName { get; set; }
        public string? SendLabName { get; set; }

        public int? SexId(Reference_AIDSContext _context)
        {
            if (SexName != null)
            {
                return _context.ListSexes.Where(e => e.SexNameLong == SexName).ToList()[0].SexId;
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
        public int? TestSystemId(Reference_AIDSContext _context)
        {
            if (TestSystemName != null)
            {
                return _context.ListTestSystems.Where(e => e.TestSystemName == TestSystemName).ToList()[0].TestSystemId;
            }
            return null;
        }
        public int? SendDistrictId(Reference_AIDSContext _context)
        {
            if (SendDistrictName != null)
            {
                return _context.ListSendDistricts.Where(e => e.SendDistrictName == SendDistrictName).ToList()[0].SendDistrictId;
            }
            return null;
        }
        public int? SendLabId(Reference_AIDSContext _context)
        {
            if (SendLabName != null)
            {
                return _context.ListSendLabs.Where(e => e.SendLabName == SendLabName).ToList()[0].SendLabId;
            }
            return null;
        }
    }
}
