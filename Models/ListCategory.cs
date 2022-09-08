using System;
using System.Collections.Generic;

namespace Reference_Aids.Models
{
    public partial class ListCategory
    {
        public ListCategory()
        {
            TblIncomingBloods = new HashSet<TblIncomingBlood>();
        }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public virtual ICollection<TblIncomingBlood> TblIncomingBloods { get; set; }
    }
}
