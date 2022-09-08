namespace Reference_Aids.Models
{
    public class ListSex
    {
        public ListSex()
        {
            TblPatientCards = new HashSet<TblPatientCard>();
        }
        public int SexId { get; set; }
        public string? SexNameShort { get; set; }
        public string? SexNameLong { get; set; }
        public virtual ICollection<TblPatientCard> TblPatientCards { get; set; }
    }
}
