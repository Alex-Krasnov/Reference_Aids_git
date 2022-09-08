namespace Reference_Aids.Models
{
    public class ListRegion
    {
        public ListRegion()
        {
            TblPatientCards = new HashSet<TblPatientCard>();
        }
        public int RegionId { get; set; }
        public string? RegionName { get; set; }
        public int? RegionType { get; set; }
        public virtual ICollection<TblPatientCard> TblPatientCards { get; set; }
    }
}
