using InfoHub.Entity.Models;

namespace InfoHub.Entity.Entities
{
    public class Ambience : BaseEntity
    {
        public virtual double Temperature { get; set; }
        public virtual double Pressure { get; set; }
        public virtual double Humidity { get; set; }
        public virtual double Visibility { get; set; }
        public virtual double WindSpeed { get; set; }
        public virtual double WindDirection { get; set; }
        public virtual double CloudCover { get; set; }
        public virtual double Turbulence { get; set; }
        public virtual double Acidity { get; set; }
        public virtual double Salinity { get; set; }
        public virtual double Turbidity { get; set; }
    }
}