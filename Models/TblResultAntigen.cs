using System;
using System.Collections.Generic;

namespace Reference_Aids.Models
{
    public partial class TblResultAntigen
    {
        public int? BloodId { get; set; }
        public int ResultAntigenId { get; set; }
        public DateOnly ResultAntigenDate { get; set; }
        public int? ResultAntigenTestSysId { get; set; }
        public double? ResultAntigenCutOff { get; set; }
        public double? ResultAntigenOp { get; set; }
        public double? ResultAntigenConfirmOp { get; set; }
        public double? ResultAntigenPercentGash { get; set; }
        public double? ResultAntigenKp { get; set; }
        public int? ResultAntigenResultId { get; set; }
        public int ResultAntigenTypeId { get; set; }
        
        public virtual TblIncomingBlood Blood { get; set; } = null!;
        public virtual ListResult? ResultAntigenResult { get; set; }
        public virtual ListTestSystem? TestSystem { get; set; }
        public virtual ListTypeAntigen TypeAntigen { get; set; } = null!;

        public string? ResultAntigenTypeName()
        {
            return TypeAntigen.TypeAntigenName;
        }

        public string? ResultAntigenTestSysName()
        {
            if (ResultAntigenTestSysId != null)
            {
                return TestSystem.TestSystemName;
            }
            return null;
        }

        public int? ResultAntigen()
        {
            return null;
        }
    }
}
