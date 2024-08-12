namespace GlassesApp.Controllers
{
    public class VisionTestResponse 
    {
        public Guid Id { get; set; }
        public int Score { get; set; }

        public DateTime VisionTestDate { get; set; }

        public int LeftEyeResult { get; set; }

        public int RightEyeResult { get; set; }
    }
}