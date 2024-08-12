using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class VisionTestAddDTO
    {
        public int Score { get; set; }

        public DateTime VisionTestDate { get; set; }

        public int LeftEyeResult { get; set; }

        public int RightEyeResult { get; set; }
    }
}
