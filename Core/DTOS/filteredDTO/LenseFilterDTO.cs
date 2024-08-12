using Core.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.filteredDTO
{
    public class LenseFilterDTO
    {
        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }


        public List<Guid>? GendersId { get; set; }


        public List<Guid>? CategoryIds { get; set; }


        public List<Guid>? BrandIds { get; set; }

        public double? LensBaseCurve { get; set; }

        public double? Lensdiameter { get; set; }

        public int? LensUsage { get; set; }

        public int? lensType { get; set; }

        public double? WaterContent { get; set; }
        
        public List<Guid>? ColorsIds { get; set; }

        public string? Search { get; set; }

    }
}
