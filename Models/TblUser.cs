using System;
using System.Collections.Generic;

namespace Reference_Aids.Models
{
    public partial class TblUser
    {
        public int UserId { get; set; }
        public string? UserFio { get; set; }
        public string? UserPosition { get; set; }
        public string UserPassword { get; set; } = null!;
    }
}
