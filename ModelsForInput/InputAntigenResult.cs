using Reference_Aids.Data;

namespace Reference_Aids.ModelsForInput
{
    public class InputAntigenResult
    {
        public InputAntigenResult(string ResultAntigenTypeName, string ResultAntigenDate, string ResultAntigenTestSysName, double? ResultAntigenCutOff, double? ResultAntigenOp, double? ResultAntigenConfirmOp, int BloodId)
        {
            this.BloodId = BloodId;
            this.ResultAntigenDate = ResultAntigenDate;
            this.ResultAntigenTestSysName = ResultAntigenTestSysName;
            this.ResultAntigenCutOff = ResultAntigenCutOff;
            this.ResultAntigenOp = ResultAntigenOp;
            this.ResultAntigenConfirmOp = ResultAntigenConfirmOp;
            this.ResultAntigenTypeName = ResultAntigenTypeName;
        }

        public InputAntigenResult()
        {
            BloodId = 0;
            ResultAntigenDate = "01/01/1900";
            ResultAntigenTestSysName = "err";
            ResultAntigenTypeName = "err";
        }

        public int BloodId { get; set; }
        public string ResultAntigenDate { get; set; } = null!;
        public string ResultAntigenTestSysName { get; set; } = null!;
        public double? ResultAntigenCutOff { get; set; }
        public double? ResultAntigenOp { get; set; }
        public double? ResultAntigenConfirmOp { get; set; }
        public string ResultAntigenTypeName { get; set; } = null!;

        public int TypeAntigen(Reference_AIDSContext _context)
        {
            return _context.ListTypeAntigens.Where(e => e.TypeAntigenName == ResultAntigenTypeName).ToList()[0].TypeAntigenId;
        }
        public int TestSysId(Reference_AIDSContext _context)
        {
            return _context.ListTestSystems.Where(e => e.TestSystemName == ResultAntigenTestSysName).ToList()[0].TestSystemId;
        }
    }
}
