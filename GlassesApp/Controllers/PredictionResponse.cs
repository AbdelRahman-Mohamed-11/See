using Newtonsoft.Json;

namespace GlassesApp.Controllers
{
    public class PredictionResponse
    {
        [JsonProperty("predicted_class")]
        public string PredictedClass { get; set; }
    }
}