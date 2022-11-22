using Reference_Aids.Models;
namespace Reference_Aids.ModelsForInput
{
    public class InputPatients
    {
        public string? PatienId { get; set; }
        public int NumPatient { get; set; }
        public string SendLab { get; set; }
        public string SendDistrict { get; set; }
        public string DateBloodSampling { get; set; }
        public string Category { get; set; }
        public string Anon { get; set; }
        public string? FamilyName { get; set; }
        public string? FirstName { get; set; }
        public string? ThirdName { get; set; }
        public string BirthDate { get; set; }
        public string Sex { get; set; }
        public string? Phone { get; set; }
        public string? RegionName { get; set; }
        public string? CityName { get; set; }
        public string? AreaName { get; set; }
        public string? AddrHome { get; set; }
        public string? AddrCorps { get; set; }
        public string? AddrFlat { get; set; }
        public string? AddrStreat { get; set; }
        public string Blotdate { get; set; }
        public string TestSys { get; set; }
        public string CutOff { get; set; }
        public string Result { get; set; }
        public int? NumIfa { get; set; }
        public string NumInList { get; set; }
        public string? PatientCom { get; set; }
        public IEnumerable<TblPatientCard> PossiblePatients { get; set; }
    }
}
