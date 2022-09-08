using Reference_Aids.Data;

namespace Reference_Aids.ModelsForInput
{
    public class InputIfa
    {
        public int BloodId { get; set; }
        public string ResultIfaDate { get; set; } = null!;
        public string? ResultIfaTestSysName { get; set; }
        public double? ResultIfaCutOff { get; set; }
        public double? ResultIfaOp { get; set; }

        public int TestSysId(Reference_AIDSContext _context)
        {
            return _context.ListTestSystems.Where(e => e.TestSystemName == ResultIfaTestSysName).ToList()[0].TestSystemId;
        }
    }
}
