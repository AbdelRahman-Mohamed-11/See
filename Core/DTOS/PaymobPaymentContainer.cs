using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class PaymobPaymentContainer
    {
        public PaymobPaymentDetails obj { get; set; }
        public string type { get; set; }
    }
    public class PaymobPaymentDetails
    {
        public long Id { get; set; }  // transactionID is the id of the operation done in paymob
        public long integration_id { get; set; }
        public long owner { get; set; }
        public bool Pending { get; set; }
        public bool is_3d_secure { get; set; }
        public bool is_auth { get; set; }
        public bool is_capture { get; set; }
        public bool is_refunded { get; set; }
        public bool is_standalone_payment { get; set; }
        public bool is_voided { get; set; }
        public long amount_cents { get; set; }
        public bool Success { get; set; }
        public string created_at { get; set; }
        public string Currency { get; set; }
        public bool error_occured { get; set; }
        public bool has_parent_transaction { get; set; }
        public OrderPyamentDetails Order { get; set; }
        public SourceData source_data { get; set; }
        public Data Data { get; set; }
    }

    public class OrderPyamentDetails
    {
        public long Id { get; set; } // 
        public string created_at { get; set; }
    }

    public class SourceData
    {
        public string Pan { get; set; }
        public string sub_type { get; set; }
        public string Type { get; set; }
    }

    public class Data
    {
        public string secure_hash { get; set; }
        public string transaction_no { get; set; }
    }
}
