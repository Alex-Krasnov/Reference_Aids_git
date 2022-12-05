namespace Reference_Aids.ModelsForInput
{
    public class InputDistricBlot
    {
        public int PatientId { get; set; }
        public string DBlot { get; set; }
        public string CutOff { get; set; }//
        public string BlotResult { get; set; }//
        public double? BlotCoefficient { get; set; }
        public string? TestSystemId { get; set; }
        public string? SendDistrictId { get; set; }
        public int DistrictBlotId { get; set; }
        public string? SendLabId { get; set; }
    }
}
