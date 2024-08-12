using Core.Entities;
using Core.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Products
{
    public class Glass : Product
    {
        public Guid ShapeID { get; set; }
        public Shape Shape { get; set; }  // frameshape


        public Guid FrameTypeID { get; set; }
        
        public FrameType FrameType { get; set; }   // frametype

        public FrameSize FrameSize { get; set; }  // frameSize
    }
}
