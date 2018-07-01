using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.LogModel.Events
{
    public class Scan : LogEvent
    {
        [JsonProperty("BodyName")]
        public string BodyName { get; set; }

        [JsonProperty("DistanceFromArrivalLS")]
        public double DistanceFromArrivalLs { get; set; }

        [JsonProperty("TidalLock")]
        public bool TidalLock { get; set; }

        [JsonProperty("TerraformState")]
        public string TerraformState { get; set; }

        [JsonProperty("PlanetClass")]
        public string PlanetClass { get; set; }

        [JsonProperty("Atmosphere")]
        public string Atmosphere { get; set; }

        [JsonProperty("Volcanism")]
        public string Volcanism { get; set; }

        [JsonProperty("MassEM")]
        public double MassEm { get; set; }

        [JsonProperty("Radius")]
        public long Radius { get; set; }

        [JsonProperty("SurfaceGravity")]
        public double SurfaceGravity { get; set; }

        [JsonProperty("SurfaceTemperature")]
        public double SurfaceTemperature { get; set; }

        [JsonProperty("SurfacePressure")]
        public long SurfacePressure { get; set; }

        [JsonProperty("Landable")]
        public bool Landable { get; set; }

        [JsonProperty("SemiMajorAxis")]
        public long SemiMajorAxis { get; set; }

        [JsonProperty("Eccentricity")]
        public double Eccentricity { get; set; }

        [JsonProperty("OrbitalInclination")]
        public double OrbitalInclination { get; set; }

        [JsonProperty("Periapsis")]
        public double Periapsis { get; set; }

        [JsonProperty("OrbitalPeriod")]
        public long OrbitalPeriod { get; set; }

        [JsonProperty("RotationPeriod")]
        public double RotationPeriod { get; set; }

        [JsonProperty("Rings")]
        public Ring[] Rings { get; set; }
    }
    
    public class Ring
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("RingClass")]
        public string RingClass { get; set; }

        [JsonProperty("MassMT")]
        public long MassMt { get; set; }

        [JsonProperty("InnerRad")]
        public long InnerRad { get; set; }

        [JsonProperty("OuterRad")]
        public long OuterRad { get; set; }
    }
}
