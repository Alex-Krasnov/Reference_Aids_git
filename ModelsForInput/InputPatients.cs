namespace Reference_Aids.ModelsForInput
{
    public class InputPatients
    {
        public string SendLab { get; set; } = null!;
        public string SendDistrict { get; set; } = null!;
        public string DateBloodSampling { get; set; }
        public string Category { get; set; } = null!;
        public string Anon { get; set; }
        public string? FamilyName { get; set; }
        public string? FirstName { get; set; }
        public string? ThirdName { get; set; }
        public string BirthDate { get; set; }
        public string Sex { get; set; } = null!;
        public string? Phone { get; set; }
        public string? RegionName { get; set; }
        public string? CityName { get; set; }
        public string? AreaName { get; set; }
        public string? AddrHome { get; set; }
        public string? AddrCorps { get; set; }
        public string? AddrFlat { get; set; }
        public string? AddrStreat { get; set; }
        public string Blotdate { get; set; }
        public string TestSys { get; set; } = null!;
        public string CutOff { get; set; }
        public string Result { get; set; }
    }
}
