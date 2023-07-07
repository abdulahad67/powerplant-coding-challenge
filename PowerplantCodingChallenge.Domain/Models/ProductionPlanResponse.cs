using System.Text.Json.Serialization;

namespace PowerplantCodingChallenge.Domain.Models
{
    public class ProductionPlanResponse
    {
        public string? Name { get; set; }

        [JsonPropertyName("p")]
        public double Power { get; set; }
    }
}
