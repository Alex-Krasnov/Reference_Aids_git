using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reference_Aids.Models
{
    public partial class TblPatientCard
    {
        public TblPatientCard()
        {
            TblIncomingBloods = new HashSet<TblIncomingBlood>();
        }

        public int PatientId { get; set; }
        public string? FirstName { get; set; }
        public string? FamilyName { get; set; }
        public string? ThirdName { get; set; }
        public DateOnly BirthDate { get; set; }
        public int? SexId { get; set; }
        public int? RegionId { get; set; }
        public string? CityName { get; set; }
        public string? AreaName { get; set; }
        public string? PatientCom { get; set; }
        public string? PhoneNum { get; set; }
        public DateTime? DateEdit { get; set; }
        public int? UserEdit { get; set; }
        public string? AddrHome { get; set; }
        public string? AddrCorps { get; set; }
        public string? AddrFlat { get; set; }
        public string? AddrStreat { get; set; }

        public virtual ListSex? Sex { get; set; }
        public virtual ListRegion? Region { get; set; }
        public virtual ICollection<TblIncomingBlood> TblIncomingBloods { get; set; }

        public string? SexName()
        {
            if (SexId != null)
            {
                return Sex.SexNameShort;
            }
            return null; 
        }
        public string? RegionName()
        {
            if (RegionId != null)
            {
                return Region.RegionName;
            }
            return null;
        }
    }
}
