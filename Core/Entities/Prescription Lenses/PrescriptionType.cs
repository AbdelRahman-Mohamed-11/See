using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Prescription_Lenses
{
    public class PrescriptionType : BaseEntity
    {
        public string PrescriptionTypeName { get; set; }

        public int PrescriptionCode { get; set; }

        public string Description { get; set; }

    }
}
