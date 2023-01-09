namespace Reference_Aids.ViewModels
{
    public class ListForImportPhotometr
    {
        public string TestSystem { get; set; }
        public string TypeAnalyze { get; set; }
        public float CutOff { get; set; }
        public IEnumerable<PhotometrResult> ListPhotometrResult { get; set; }
    }
    public class PhotometrResult
    {
        public int PatientId { get; set; }
        public float PatientResult { get; set; }
        public string Result { get; set; }
        public string DateId { get; set; }
    }
}
