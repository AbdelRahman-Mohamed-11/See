using Core.Entities;

namespace Core.DTOS.ProductDTOS
{
    public class ImageDTOResponse
    {        
        public Guid ColorId { get; set; }

        public List<PictureDTO> PictureDTO { get; set; } = new List<PictureDTO>();
    }
}