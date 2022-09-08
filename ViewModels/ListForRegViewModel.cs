using Reference_Aids.Models;

namespace Reference_Aids.ViewModels
{
    public class ListForRegViewModel
    {
        public int NextPatientID;
        public IEnumerable<ListSex> ListSexes { get; set; }
        public IEnumerable<ListRegion> ListRegions { get; set; }
        public IEnumerable<ListSendLab> ListSendLabs { get; set; }
        public IEnumerable<ListTestSystem> ListTestSystems { get; set; }
        public IEnumerable<ListSendDistrict> ListSendDistricts { get; set; }
    }
}
