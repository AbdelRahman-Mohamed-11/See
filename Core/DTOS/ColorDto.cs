using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.DTOS
{
    public class ColorDto
    {
        public string ColorName { get; set; }
        public Guid ColorId { get; set; }
    }
}