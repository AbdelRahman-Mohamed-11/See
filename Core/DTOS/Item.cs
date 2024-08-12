using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class Item
    {
        public string Name { get; set; }
        public string AmountCents { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
    }
}
