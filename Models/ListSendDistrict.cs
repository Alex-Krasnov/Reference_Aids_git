using System;
using System.Collections.Generic;

namespace Reference_Aids.Models
{
    public partial class ListSendDistrict
    {
        public ListSendDistrict()
        {
            TblIncomingBloods = new HashSet<TblIncomingBlood>();
        }

        public int SendDistrictId { get; set; }
        public string? SendDistrictName { get; set; }

        public virtual ICollection<TblIncomingBlood> TblIncomingBloods { get; set; }
    }
}
