using System;
using System.Collections.Generic;

namespace Reference_Aids.Models
{
    public partial class TblResultPcr
    {
        public int? BloodId { get; set; }
        public int? ResultPcrId { get; set; }
        public DateOnly ResultPcrDate { get; set; } = new DateOnly();
        public int? ResultPcrTestSysId { get; set; }
        public int? ResultPcrResultId { get; set; }

        public virtual TblIncomingBlood Blood { get; set; } = null!;
        public virtual ListResult ResultPcrResult { get; set; } = null!;
        public virtual ListTestSystem? TestSystem { get; set; }

        public string? ResultPcrTestSysName()
        {
            if (ResultPcrTestSysId != null)
            {
                return TestSystem.TestSystemName;
            }
            return null;
        }

        public string? ResultPcrResultName()
        {
            if (ResultPcrResultId != null)
            {
                return ResultPcrResult.ResultName;
            }
            return null;
        }
    }
}
