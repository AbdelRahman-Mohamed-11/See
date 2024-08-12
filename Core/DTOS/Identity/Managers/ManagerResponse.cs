using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS.Identity.Managers
{
   
        public class ManagerResponse : BaseEntity
        {
           
            public string Email { get; set; }

            public string FirstName { get; set; }
            public string LastName { get; set; }

            public string storeName { get; set; }

            public string BusinessLocation { get; set; }

            public PaymentInfo PaymentInfo { get; set; }
        }
    

}
