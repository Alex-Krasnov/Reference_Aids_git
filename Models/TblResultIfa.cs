using System;
using System.Collections.Generic;

namespace Reference_Aids.Models
{
    public partial class TblResultIfa
    {
        public int BloodId { get; set; }
        public int ResultIfaId { get; set; }
        public DateOnly ResultIfaDate { get; set; }
        public int? ResultIfaTestSysId { get; set; }
        public double? ResultIfaCutOff { get; set; }
        public double? ResultIfaOp { get; set; }
        public double? ResultIfaKp { get; set; }
        public int? ResultIfaResultId { get; set; }
        

        public virtual TblIncomingBlood Blood { get; set; } = null!;
        public virtual ListResult ResultIfaResult { get; set; } = null!;
        public virtual ListTestSystem? TestSystem { get; set; }

        public string? ResultIfaTestSysName()
        {
            if (ResultIfaTestSysId != null)
            {
                return TestSystem.TestSystemName;
            }
            return null;
        }
    }
}
