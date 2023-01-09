using Reference_Aids.Data;

namespace Reference_Aids.ModelsForInput
{
    public class InputPcr
    {
        public int? BloodId { get; set; }
        public int ResultPcrId { get; set; }
        public string ResultPcrDate { get; set; } = null!;
        public string? ResultPcrTestSysName { get; set; }
        public string? ResultPcrResultName { get; set; }
        public string DateId { get; set; }

        public int TestSysId(Reference_AIDSContext _context)
        {
            return _context.ListTestSystems.Where(e => e.TestSystemName == ResultPcrTestSysName).ToList()[0].TestSystemId;
        }

        public int ResultId(Reference_AIDSContext _context)
        {
            return _context.ListResults.Where(e => e.ResultName == ResultPcrResultName).ToList()[0].ResultId;
        }
    }
}
