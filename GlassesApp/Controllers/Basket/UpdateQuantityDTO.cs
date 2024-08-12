namespace GlassesApp.Controllers.Basket
{
    public class UpdateQuantityDTO
    {
        public string BasketID { get; set; }

        public List<ItemQuantityDTO> itemQuantityDTO { get; set; } = new List<ItemQuantityDTO>()
;    }
}