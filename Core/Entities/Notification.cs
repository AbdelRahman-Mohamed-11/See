using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
        public class Notification : BaseEntity
        {
            public Guid ApplicationUserId { get; set; }
        // Foreign key to associate notification with user
            public string Message { get; set; }

            public string Title { get; set; }
            public bool IsRead { get; set; }
        }

}
