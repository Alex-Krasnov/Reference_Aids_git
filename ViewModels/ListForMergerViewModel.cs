using Reference_Aids.Models;

namespace Reference_Aids.ViewModels
{
    public class ListForMergerViewModel
    {
        public IEnumerable<ListSex> ListSexes { get; set; }
        public IEnumerable<ListRegion> ListRegions { get; set; }
        public IEnumerable<ListSendDistrict> ListSendDistricts { get; set; }
        public IEnumerable<ListSendLab> ListSendLabs { get; set; }
        public IEnumerable<ListTestSystem> ListTestSystems { get; set; }
        public IEnumerable<ListCategory> ListCategories { get; set; }
        public IEnumerable<ListQualitySerum> ListQualitySerums { get; set; }

        public List<PatientFull> Patients { get; set; }
        
        
    }
    public class PatientFull
    {
        public List<TblPatientCard> TblPatientCards { get; set; }
        public IEnumerable<TblDistrictBlot> TblDistrictBlots { get; set; }
        public IEnumerable<TblIncomingBlood> TblIncomingBloods { get; set; }
    }
}
