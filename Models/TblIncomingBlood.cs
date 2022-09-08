namespace Reference_Aids.Models
{
    public partial class TblIncomingBlood
    {
        public TblIncomingBlood()
        {
            TblResultIfas = new HashSet<TblResultIfa>();
        }

        public int PatientId { get; set; }
        public int BloodId { get; set; }
        public int? SendDistrictId { get; set; }
        public int? SendLabId { get; set; }
        public int? CategoryPatientId { get; set; }
        public bool? AnonymousPatient { get; set; }
        public DateOnly DateBloodSampling { get; set; }
        public int? QualitySerumId { get; set; }
        public DateOnly DateBloodImport { get; set; }
        public int NumIfa { get; set; }
        public int NumInList { get; set; }

        public virtual ListCategory? CategoryPatientNavigation { get; set; }
        public virtual TblPatientCard Patient { get; set; } = null!;
        public virtual ListQualitySerum? QualitySerumNavigation { get; set; }
        public virtual ListSendDistrict? SendDistrictNavigation { get; set; }
        public virtual ListSendLab? SendLabNavigation { get; set; }
        public virtual ICollection<TblResultIfa> TblResultIfas { get; set; }

        public string? SendDistrictName()
        {
            if (SendDistrictId != null)
            {
                return SendDistrictNavigation.SendDistrictName;
            }
            return null;
        }

        public string? SendLabName()
        {
            if (SendLabId != null)
            {
                return SendLabNavigation.SendLabName;
            }
            return null;
        }

        public string? CategoryPatientName()
        {
            if (CategoryPatientId != null)
            {
                return CategoryPatientNavigation.CategoryName;
            }
            return null;
        }

        public string? QualitySerumName()
        {
            if (QualitySerumId != null)
            {
                return QualitySerumNavigation.QualitySerumName;
            }
            return null;
        }
    }
}
