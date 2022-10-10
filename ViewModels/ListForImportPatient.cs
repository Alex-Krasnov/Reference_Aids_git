using Reference_Aids.Models;
using Reference_Aids.ModelsForInput;
namespace Reference_Aids.ViewModels
{
    public class ListForImportPatient
    {
        public IEnumerable<ListSex> ListSexes { get; set; } = null!;
        public IEnumerable<ListRegion> ListRegions { get; set; } = null!;
        public IEnumerable<InputPatients> Patients { get; set; }
        public IEnumerable<ListSendDistrict> ListSendDistricts { get; set; }
        public IEnumerable<ListSendLab> ListSendLabs { get; set; }
        public IEnumerable<ListTestSystem> ListTestSystems { get; set; }
        public IEnumerable<ListCategory> ListCategories { get; set; }
    }
}
