using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class ReturnPaymentInformation
    {
        public string OrderPaymentId { get; set; } // order id from paymob
        
        public string Token { get; set; } // token that we are using in redreict to paymob paymnet page
    }
}
