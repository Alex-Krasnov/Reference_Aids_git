namespace Reference_Aids.ModelsForInput
{
    public class InputDistricBlot
    {
        public int PatientId { get; set; }
        public DateOnly DBlot { get; set; }
        public double CutOff { get; set; }
        public double BlotResult { get; set; }
        public double? BlotCoefficient { get; set; }
        public int? TestSystemId { get; set; }
        public int? SendDistrictId { get; set; }
        public int DistrictBlotId { get; set; }
        public int? SendLabId { get; set; }
    }
}
