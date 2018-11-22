namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class Scan : LogEvent
    {
        [JsonProperty("BodyName")]
        public string BodyName { get; set; }

        [JsonProperty("BodyID")]
        public long? BodyID { get; set; }

        [JsonProperty("DistanceFromArrivalLS")]
        public double DistanceFromArrivalLs { get; set; }

        [JsonProperty("TidalLock")]
        public bool? TidalLock { get; set; }

        [JsonProperty("TerraformState")]
        public string TerraformState { get; set; }

        [JsonProperty("PlanetClass")]
        public string PlanetClass { get; set; }

        [JsonProperty("Composition")]
        public GeologicComposition Composition { get; set; }

        [JsonProperty("Atmosphere")]
        public string Atmosphere { get; set; }

        [JsonProperty("AtmosphereType")]
        public string AtmosphereType { get; set; }

        [JsonProperty("AtmosphereComposition")]
        public Composition[] AtmosphereComposition { get; set; }

        [JsonProperty("Volcanism")]
        public string Volcanism { get; set; }

        [JsonProperty("MassEM")]
        public double? MassEm { get; set; }

        [JsonProperty("Radius")]
        public double? Radius { get; set; }

        [JsonProperty("SurfaceGravity")]
        public double? SurfaceGravity { get; set; }

        [JsonProperty("SurfaceTemperature")]
        public double? SurfaceTemperature { get; set; }

        [JsonProperty("SurfacePressure")]
        public double? SurfacePressure { get; set; }

        [JsonProperty("Landable")]
        public bool? Landable { get; set; }

        [JsonProperty("SemiMajorAxis")]
        public double? SemiMajorAxis { get; set; }

        [JsonProperty("Eccentricity")]
        public double? Eccentricity { get; set; }

        [JsonProperty("OrbitalInclination")]
        public double? OrbitalInclination { get; set; }

        [JsonProperty("Periapsis")]
        public double? Periapsis { get; set; }

        [JsonProperty("OrbitalPeriod")]
        public double? OrbitalPeriod { get; set; }

        [JsonProperty("RotationPeriod")]
        public double? RotationPeriod { get; set; }

        [JsonProperty("AxialTilt")]
        public double? AxialTilt { get; set; }

        [JsonProperty("ScanType")]
        public string ScanType { get; set; }

        [JsonProperty("StarType")]
        public string StarType { get; set; }

        [JsonProperty("Age_MY")]
        public long? AgeMegaYears { get; set; }

        [JsonProperty("StellarMass")]
        public double? StellarMass { get; set; }

        [JsonProperty("AbsoluteMagnitude")]
        public double? AbsoluteMagnitude { get; set; }

        [JsonProperty("Luminosity")]
        public double? Luminosity { get; set; }

        [JsonProperty("Rings")]
        public Ring[] Rings { get; set; }

        [JsonProperty("Materials")]
        public Composition[] Materials { get; set; }
    }

    public class Composition
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Percent")]
        public double Percent { get; set; }
    }

    public class GeologicComposition
    {
        [JsonProperty("Ice")]
        public double Ice { get; set; }

        [JsonProperty("Rock")]
        public double Rock { get; set; }

        [JsonProperty("Metal")]
        public double Metal { get; set; }
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
