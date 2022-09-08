using System;
using System.Collections.Generic;

namespace Reference_Aids.Models
{
    public partial class ListSendLab
    {
        public ListSendLab()
        {
            TblIncomingBloods = new HashSet<TblIncomingBlood>();
        }

        public int SendLabId { get; set; }
        public string? SendLabName { get; set; }

        public virtual ICollection<TblIncomingBlood> TblIncomingBloods { get; set; }
    }
}
