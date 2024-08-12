using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class ReviewDtoResponse : BaseEntity
    {
        public string? Comment { get; set; }
        public double? Rating { get; set; }  
    }
}
