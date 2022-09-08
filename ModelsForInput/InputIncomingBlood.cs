namespace Reference_Aids.ModelsForInput
{
    public class InputIncomingBlood
    {
        public int PatientId { get; set; }
        public int? SendDistrictId { get; set; }
        public int? SendLabId { get; set; }
        public int? CategoryPatientId { get; set; }
        public bool? AnonymousPatient { get; set; }
        public string DateBloodSampling { get; set; } = null!;
        public int? QualitySerumId { get; set; }
        public string DateBloodImport { get; set; } = null!;
        public int NumIfa { get; set; }
        public int NumInList { get; set; }
    }
}
