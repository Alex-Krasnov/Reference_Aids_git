namespace Reference_Aids.ModelsForInput
{
    public class InputDistricBlot
    {
        public int PatientId { get; set; }
        public string DBlot { get; set; }
        public double CutOff { get; set; }
        public double BlotResult { get; set; }
        public double? BlotCoefficient { get; set; }
        public string? TestSystemId { get; set; }
        public string? SendDistrictId { get; set; }
        public int DistrictBlotId { get; set; }
        public string? SendLabId { get; set; }
    }
}
