using Reference_Aids.Data;

namespace Reference_Aids.ModelsForInput
{
    public class InputBlot
    {
        public int BloodId { get; set; }
        public string ResultBlotDate { get; set; } = null!;        
        public string ExpirationResultBlotDate { get; set; } = null!;
        public string ResultBlotTestSysName { get; set; } = null!;
        public string? ResultBlotEnv160 { get; set; }
        public string? ResultBlotEnv120 { get; set; }
        public string? ResultBlotEnv41 { get; set; }
        public string? ResultBlotGag55 { get; set; }
        public string? ResultBlotGag40 { get; set; }
        public string? ResultBlotGag2425 { get; set; }
        public string? ResultBlotGag18 { get; set; }
        public string? ResultBlotPol6866 { get; set; }
        public string? ResultBlotPol5251 { get; set; }
        public string? ResultBlotPol3431 { get; set; }
        public string? ResultBlotHiv2105 { get; set; }
        public string? ResultBlotHiv236 { get; set; }
        public string? ResultBlotHiv0 { get; set; }
        public bool? ResultBlotReturnResult { get; set; }
        public string? ResultBlotResult { get; set; }

        public int TestSysId(Reference_AIDSContext _context)
        {
            return _context.ListTestSystems.Where(e => e.TestSystemName == ResultBlotTestSysName).ToList()[0].TestSystemId;
        }

        public int? ResultBlotEnv160Id(Reference_AIDSContext _context)
        {
            if (ResultBlotEnv160 != null)
            {
                return _context.ListResults.Where(e => e.ResultName == ResultBlotEnv160).ToList()[0].ResultId;
            }
            return null;
        }

        public int? ResultBlotEnv120Id(Reference_AIDSContext _context)
        {
            if (ResultBlotEnv120 != null)
            {
                return _context.ListResults.Where(e => e.ResultName == ResultBlotEnv120).ToList()[0].ResultId;
            }
            return null;            
        }

        public int? ResultBlotEnv41Id(Reference_AIDSContext _context)
        {
            if (ResultBlotEnv41 != null)
            {
                return _context.ListResults.Where(e => e.ResultName == ResultBlotEnv41).ToList()[0].ResultId;
            }
            return null;            
        }

        public int? ResultBlotGag55Id(Reference_AIDSContext _context)
        {
            if (ResultBlotGag55 != null)
            {
                return _context.ListResults.Where(e => e.ResultName == ResultBlotGag55).ToList()[0].ResultId;
            }
            return null;            
        }

        public int? ResultBlotGag40Id(Reference_AIDSContext _context)
        {
            if (ResultBlotGag40 != null)
            {
                return _context.ListResults.Where(e => e.ResultName == ResultBlotGag40).ToList()[0].ResultId;
            }
            return null;            
        }

        public int? ResultBlotGag2425Id(Reference_AIDSContext _context)
        {
            if (ResultBlotGag2425 != null)
            {
                return _context.ListResults.Where(e => e.ResultName == ResultBlotGag2425).ToList()[0].ResultId;
            }
            return null;            
        }

        public int? ResultBlotGag18Id(Reference_AIDSContext _context)
        {
            if (ResultBlotGag18 != null)
            {
                return _context.ListResults.Where(e => e.ResultName == ResultBlotGag18).ToList()[0].ResultId;
            }
            return null;            
        }

        public int? ResultBlotPol6866Id(Reference_AIDSContext _context)
        {
            if (ResultBlotPol6866 != null)
            {
                return _context.ListResults.Where(e => e.ResultName == ResultBlotPol6866).ToList()[0].ResultId;
            }
            return null;            
        }

        public int? ResultBlotPol5251Id(Reference_AIDSContext _context)
        {
            if (ResultBlotPol5251 != null)
            {
                return _context.ListResults.Where(e => e.ResultName == ResultBlotPol5251).ToList()[0].ResultId;
            }
            return null;            
        }

        public int? ResultBlotPol3431Id(Reference_AIDSContext _context)
        {
            if (ResultBlotPol3431 != null)
            {
                return _context.ListResults.Where(e => e.ResultName == ResultBlotPol3431).ToList()[0].ResultId;
            }
            return null;            
        }

        public int? ResultBlotHiv2105Id(Reference_AIDSContext _context)
        {
            if (ResultBlotHiv2105 != null)
            {
                return _context.ListResults.Where(e => e.ResultName == ResultBlotHiv2105).ToList()[0].ResultId;
            }
            return null;            
        }

        public int? ResultBlotHiv236Id(Reference_AIDSContext _context)
        {
            if (ResultBlotHiv236 != null)
            {
                return _context.ListResults.Where(e => e.ResultName == ResultBlotHiv236).ToList()[0].ResultId;
            }
            return null;            
        }

        public int? ResultBlotHiv0Id(Reference_AIDSContext _context)
        {
            if (ResultBlotHiv236 != null)
            {
                return _context.ListResults.Where(e => e.ResultName == ResultBlotHiv0).ToList()[0].ResultId;
            }
            return null;            
        }

        public int? ResultId(Reference_AIDSContext _context)
        {
            if (ResultBlotResult != null)
            {
                return _context.ListResults.Where(e => e.ResultName == ResultBlotResult).ToList()[0].ResultId;
            }
            return null;            
        }
    }
}
