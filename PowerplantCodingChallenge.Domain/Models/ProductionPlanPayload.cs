namespace PowerplantCodingChallenge.Domain.Models
{
    public class ProductionPlanPayload
    {
        public double Load { get; set; }
        public IDictionary<string, double>? Fuels { get; set; }
        public IList<Powerplant>? Powerplants { get; set; }
    }
}
