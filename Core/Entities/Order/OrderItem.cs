using Core.Entities.Prescription_Lenses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Order
{
    public class OrderItem : BaseEntity
    {
        public OrderItem() { }
        public OrderItem(ProductItemOrdered itemOrdered, int quantity, 
            decimal price , string color , Guid managerID,
            Guid? prescriptionId)
        {
            ItemOrdered = itemOrdered;
            Quantity = quantity;
            Price = price;
            this.ManagerID = managerID;
            this.Color = color;
            this.UserPrescriptionId = prescriptionId;
        }

        public ProductItemOrdered ItemOrdered { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string Color { get; set; }    
        
        public Guid ManagerID { get; set; }

        public Guid? UserPrescriptionId { get; set; }
        
        public UserPrescription UserPrescription { get; set; }
    }
}
