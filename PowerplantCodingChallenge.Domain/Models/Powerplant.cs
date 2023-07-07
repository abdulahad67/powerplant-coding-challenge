namespace PowerplantCodingChallenge.Domain.Models
{
    public class Powerplant
    {
        public string? Name { get; set; }
        public PowerplantType Type { get; set; }
        public double Efficiency { get; set; }
        public int Pmin { get; set; }
        public int Pmax { get; set; }
    }
}
