using Core.Products;

namespace Core.Entities
{

    public class FrameType : BaseEntity
    {

        public string FrameTypeName { get; set; }

        public List<Glass> Glasses { get; set; }

    }
}