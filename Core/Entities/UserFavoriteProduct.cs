using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class UserFavoriteProduct
    {
        // cannot refernce to navigations properties
        public Guid ApplicationUserId { get; set; }

        public Guid ProductId { get; set; }
    }
}
