using Reference_Aids.Data;

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
        public virtual ListTestSystem TestSystem { get; set; } = null!;

        public string? ResultIfaTestSysName()
        {
            if (ResultIfaTestSysId != null)
            {
                return TestSystem.TestSystemName;//_context.ListTestSystems.Where(e => e.TestSystemId == ResultIfaTestSysId).ToList()[0].TestSystemName;
            }
            return null;
        }
    }
}
