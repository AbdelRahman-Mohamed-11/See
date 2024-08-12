using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.filteredDTO
{
    public class AccessoryFilterDTO
    {
        public decimal? MinPrice { get; set; }  
        
        public decimal? MaxPrice { get; set; }


        public List<Guid>? GendersId { get; set; }

        
        public List<Guid> ? CategoryIds { get; set; }


        public List<Guid> ?BrandIds { get; set; }

        public List<Guid>? ColorsIds { get; set; }

        public string? Search { get; set; }
    }
}
