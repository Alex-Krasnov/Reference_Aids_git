using System;
using System.Collections.Generic;

namespace Reference_Aids.Models
{
    public partial class ListQualitySerum
    {
        public ListQualitySerum()
        {
            TblIncomingBloods = new HashSet<TblIncomingBlood>();
        }

        public int QualitySerumId { get; set; }
        public string? QualitySerumName { get; set; }

        public virtual ICollection<TblIncomingBlood> TblIncomingBloods { get; set; }
    }
}
