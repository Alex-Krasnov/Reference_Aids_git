using Reference_Aids.Data;
namespace Reference_Aids.ModelsForInput
{
    public class UpdIncomingBlood
    {
        public int PatientId { get; set; }
        public int BloodId { get; set; }
        public string? SendDistrictId { get; set; }
        public string? SendLabId { get; set; }
        public int? CategoryPatientId { get; set; }
        public string? DateBloodSampling { get; set; }
        public string? QualitySerumId { get; set; }
        public string? DateBloodImport { get; set; }
        public int NumIfa { get; set; }
        public int NumInList { get; set; }
    }
}
