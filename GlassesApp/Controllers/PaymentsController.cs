using Microsoft.AspNetCore.Mvc;
using X.Paymob.CashIn;

namespace GlassesApp.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymobCashInBroker _paymobBroker;

        public PaymentsController(IPaymobCashInBroker paymobBroker)
        {
            _paymobBroker = paymobBroker;
        }

      
    }
}
