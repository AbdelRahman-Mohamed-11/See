using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.VisionTest
{
    public class VisionTest : BaseEntity
    {
        public Guid ApplicationUserId { get; set; }
        
        public int Score { get; set; }

        
        public DateTime VisionTestDate = DateTime.Now;

        public int LeftEyeResult { get; set; }

        public int RightEyeResult { get; set; }
    }
}
