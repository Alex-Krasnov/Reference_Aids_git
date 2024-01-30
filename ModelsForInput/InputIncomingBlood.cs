using Reference_Aids.Data;
namespace Reference_Aids.ModelsForInput
{
    public class InputIncomingBlood
    {
        public int PatientId { get; set; }
        public string? SendDistrict { get; set; }
        public string? SendLab { get; set; }
        public int? CategoryPatient { get; set; }
        public string? Repeat { get; set; }
        public string DateBloodSampling { get; set; } = null!;
        public string? QualitySerum { get; set; }
        public string DateBloodImport { get; set; } = null!;
        public int NumIfa { get; set; }
        public int NumInList { get; set; }
    }
}
