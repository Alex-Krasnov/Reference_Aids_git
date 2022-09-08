using System;
using System.Collections.Generic;

namespace Reference_Aids.Models
{
    public partial class TblDistrictBlot
    {
        public int PatientId { get; set; }
        public DateOnly DBlot { get; set; }
        public double CutOff { get; set; }
        public double BlotResult { get; set; }
        public double? BlotCoefficient { get; set; }
        public int? TestSystemId { get; set; }
        public int? SendDistrictId { get; set; }
        public int DistrictBlotId { get; set; }
        public int? SendLabId { get; set; }

        public virtual TblPatientCard Patient { get; set; } = null!;
        public virtual ListSendDistrict? SendDistrict { get; set; }
        public virtual ListSendLab? SendLab { get; set; }
        public virtual ListTestSystem? TestSystem { get; set; }

        public string? TestSystemName()
        {
            if (TestSystemId != null)
            {
                return TestSystem.TestSystemName;
            }
            return null;
        }

        public string? SendDistrictName()
        {
            if (SendDistrictId != null)
            {
                return SendDistrict.SendDistrictName;
            }
            return null;
        }

        public string? SendLabName()
        {
            if (SendLabId != null)
            {
                return SendLab.SendLabName;
            }
            return null;
        }
    }
}
