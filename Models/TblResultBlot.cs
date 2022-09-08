using System;
using System.Collections.Generic;

namespace Reference_Aids.Models
{
    public partial class TblResultBlot
    {
        public int BloodId { get; set; }
        public int ResultBlotId { get; set; }
        public DateOnly ResultBlotDate { get; set; }
        public DateOnly ExpirationResultBlotDate { get; set; }
        public int? ResultBlotTestSysId { get; set; }
        public int? ResultBlotEnv160 { get; set; }
        public int? ResultBlotEnv120 { get; set; }
        public int? ResultBlotEnv41 { get; set; }
        public int? ResultBlotGag55 { get; set; }
        public int? ResultBlotGag40 { get; set; }
        public int? ResultBlotGag2425 { get; set; }
        public int? ResultBlotGag18 { get; set; }
        public int? ResultBlotPol6866 { get; set; }
        public int? ResultBlotPol5251 { get; set; }
        public int? ResultBlotPol3431 { get; set; }
        public int? ResultBlotHiv2105 { get; set; }
        public int? ResultBlotHiv236 { get; set; }
        public int? ResultBlotHiv0 { get; set; }
        public bool? ResultBlotReturnResult { get; set; }
        public int? ResultBlotResultId { get; set; }

        public virtual ListTestSystem TestSystem { get; set; } = null!;
        public virtual TblIncomingBlood Blood { get; set; } = null!;
        public virtual ListResult? ResultBlotResult { get; set; }
        public virtual ListResult? ResultBlotResultEnv160 { get; set; }
        public virtual ListResult? ResultBlotResultEnv120 { get; set; }
        public virtual ListResult? ResultBlotResultEnv41 { get; set; }
        public virtual ListResult? ResultBlotResultGag55 { get; set; }
        public virtual ListResult? ResultBlotResultGag40 { get; set; }
        public virtual ListResult? ResultBlotResultGag2425 { get; set; }
        public virtual ListResult? ResultBlotResultGag18 { get; set; }
        public virtual ListResult? ResultBlotResultPol6866 { get; set; }
        public virtual ListResult? ResultBlotResultPol5251 { get; set; }
        public virtual ListResult? ResultBlotResultPol3431 { get; set; }
        public virtual ListResult? ResultBlotResultHiv2105 { get; set; }
        public virtual ListResult? ResultBlotResultHiv236 { get; set; }
        public virtual ListResult? ResultBlotResultHiv0 { get; set; }

        public string? ResultBlotEnv160Name()
        {
            if (ResultBlotEnv160 != null)
            {
                return ResultBlotResultEnv160.ResultName;
            }
            return null;
        }

        public string? ResultBlotEnv120Name()
        {
            if (ResultBlotEnv120 != null)
            {
                return ResultBlotResultEnv120.ResultName;
            }
            return null;
        }

        public string? ResultBlotEnv41Name()
        {
            if (ResultBlotEnv41 != null)
            {
                return ResultBlotResultEnv41.ResultName;
            }
            return null;
        }

        public string? ResultBlotGag55Name()
        {
            if (ResultBlotGag55 != null)
            {
                return ResultBlotResultGag55.ResultName;
            }
            return null;
        }

        public string? ResultBlotGag40Name()
        {
            if (ResultBlotGag40 != null)
            {
                return ResultBlotResultGag40.ResultName;
            }
            return null;
        }

        public string? ResultBlotGag2425Name()
        {
            if (ResultBlotGag2425 != null)
            {
                return ResultBlotResultGag2425.ResultName;
            }
            return null;
        }

        public string? ResultBlotGag18Name()
        {
            if (ResultBlotGag18 != null)
            {
                return ResultBlotResultGag18.ResultName;
            }
            return null;
        }

        public string? ResultBlotPol6866Name()
        {
            if (ResultBlotPol6866 != null)
            {
                return ResultBlotResultPol6866.ResultName;
            }
            return null;
        }

        public string? ResultBlotPol5251Name()
        {
            if (ResultBlotPol5251 != null)
            {
                return ResultBlotResultPol5251.ResultName;
            }
            return null;
        }

        public string? ResultBlotPol3431Name()
        {
            if (ResultBlotPol3431 != null)
            {
                return ResultBlotResultPol3431.ResultName;
            }
            return null;
        }

        public string? ResultBlotHiv2105Name()
        {
            if (ResultBlotHiv2105 != null)
            {
                return ResultBlotResultHiv2105.ResultName;
            }
            return null;
        }

        public string? ResultBlotHiv236Name()
        {
            if (ResultBlotHiv236 != null)
            {
                return ResultBlotResultHiv236.ResultName;
            }
            return null;
        }

        public string? ResultBlotHiv0Name()
        {
            if (ResultBlotHiv0 != null)
            {
                return ResultBlotResultHiv0.ResultName;
            }
            return null;
        }

        public string? ResultBlotResultName()
        {
            if (ResultBlotResultId != null)
            {
                return ResultBlotResult.ResultName;
            }
            return null;
        }

        public string TestSystemName()
        {
            return TestSystem.TestSystemName;
        }
    }
}
