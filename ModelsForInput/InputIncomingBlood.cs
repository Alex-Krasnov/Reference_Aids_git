using Reference_Aids.Data;
namespace Reference_Aids.ModelsForInput
{
    public class InputIncomingBlood
    {
        public int PatientId { get; set; }
        public string? SendDistrict { get; set; }
        public string? SendLab { get; set; }
        public string? CategoryPatient { get; set; }
        public bool? AnonymousPatient { get; set; }
        public string DateBloodSampling { get; set; } = null!;
        public string? QualitySerum { get; set; }
        public string DateBloodImport { get; set; } = null!;
        public int NumIfa { get; set; }
        public int NumInList { get; set; }

        public int? SendDistrictId(Reference_AIDSContext _context)
        {
            if (SendDistrict != null)
            {
                return _context.ListSendDistricts.Where(e => e.SendDistrictName == SendDistrict).ToList()[0].SendDistrictId;
            }
            return null;
        }
        public int? SendLabId(Reference_AIDSContext _context)
        {
            if (SendLab != null)
            {
                return _context.ListSendLabs.Where(e => e.SendLabName == SendLab).ToList()[0].SendLabId;
            }
            return null;
        }
        public int? CategoryPatientId(Reference_AIDSContext _context)
        {
            if (CategoryPatient != null)
            {
                return _context.ListCategories.Where(e => e.CategoryName == CategoryPatient).ToList()[0].CategoryId;
            }
            return null;
        }
        public int? QualitySerumId(Reference_AIDSContext _context)
        {
            if (QualitySerum != null)
            {
                return _context.ListQualitySerums.Where(e => e.QualitySerumName == QualitySerum).ToList()[0].QualitySerumId;
            }
            return null;
        }
    }
}
