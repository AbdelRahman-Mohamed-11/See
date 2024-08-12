using Core.Entities;

namespace Application.Services
{
    public class GlassUpdateImageDTO
    {
        public Guid Id { get; set; }
        public List<ImageDTO> PicturesUrl { get; set; }
    }
}