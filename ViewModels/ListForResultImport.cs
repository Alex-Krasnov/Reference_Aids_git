namespace Reference_Aids.ViewModels
{
    public class ListForResultImport
    {
        public List<string> Error { get; set; }
        public List<Success> SuccessList { get; set; }
    }

    public class Success
    {
        public int NumIfa { get; set; }
        public string Com { get; set; }
    }
}
