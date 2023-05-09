namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StreetType
    {
        [EnumMember(Value = "ALY")]
        [Description("Alley")]
        Alley,
        [EnumMember(Value = "ARC")]
        [Description("Arcade")]
        Arcade,
        [EnumMember(Value = "AVE")]
        [Description("Avenue")]
        Avenue,
        [EnumMember(Value = "BCH")]
        [Description("Beach")]
        Beach,
        [EnumMember(Value = "BND")]
        [Description("Bend")]
        Bend,
        [EnumMember(Value = "BLF")]
        [Description("Bluff")]
        Bluff,
        [EnumMember(Value = "BLFS")]
        [Description("Bluffs")]
        Bluffs,
        [EnumMember(Value = "BLVD")]
        [Description("Boulevard")]
        Boulevard,
        [EnumMember(Value = "BR")]
        [Description("Branch")]
        Branch,
        [EnumMember(Value = "BRG")]
        [Description("Bridge")]
        Bridge,
        [EnumMember(Value = "BRK")]
        [Description("Brook")]
        Brook,
        [EnumMember(Value = "BYP")]
        [Description("ByPass")]
        ByPass,
        [EnumMember(Value = "CP")]
        [Description("Camp")]
        Camp,
        [EnumMember(Value = "CYN")]
        [Description("Canyon")]
        Canyon,
        [EnumMember(Value = "CPE")]
        [Description("Cape")]
        Cape,
        [EnumMember(Value = "CSWY")]
        [Description("Causeway")]
        Causeway,
        [EnumMember(Value = "CHAS")]
        [Description("Chase")]
        Chase,
        [EnumMember(Value = "CIR")]
        [Description("Circle")]
        Circle,
        [EnumMember(Value = "CLF")]
        [Description("Cliff")]
        Cliff,
        [EnumMember(Value = "CLFS")]
        [Description("Cliffs")]
        Cliffs,
        [EnumMember(Value = "CLB")]
        [Description("Club")]
        Club,
        [EnumMember(Value = "CMN")]
        [Description("Common")]
        Common,
        [EnumMember(Value = "COR")]
        [Description("Corner")]
        Corner,
        [EnumMember(Value = "CORS")]
        [Description("Corners")]
        Corners,
        [EnumMember(Value = "CT")]
        [Description("Court")]
        Court,
        [EnumMember(Value = "CV")]
        [Description("Cove")]
        Cove,
        [EnumMember(Value = "CRK")]
        [Description("Creek")]
        Creek,
        [EnumMember(Value = "CRES")]
        [Description("Crescent")]
        Crescent,
        [EnumMember(Value = "CRST")]
        [Description("Crest")]
        Crest,
        [EnumMember(Value = "XING")]
        [Description("Crossing")]
        Crossing,
        [EnumMember(Value = "CURV")]
        [Description("Curve")]
        Curve,
        [EnumMember(Value = "DL")]
        [Description("Dale")]
        Dale,
        [EnumMember(Value = "DM")]
        [Description("Dam")]
        Dam,
        [EnumMember(Value = "DR")]
        [Description("Drive")]
        Drive,
        [EnumMember(Value = "EST")]
        [Description("Estate")]
        Estate,
        [EnumMember(Value = "ESTS")]
        [Description("Estates")]
        Estates,
        [EnumMember(Value = "EXPY")]
        [Description("Expressway")]
        Expressway,
        [EnumMember(Value = "EXT")]
        [Description("Extension")]
        Extension,
        [EnumMember(Value = "FALL")]
        [Description("Fall")]
        Fall,
        [EnumMember(Value = "FLS")]
        [Description("Falls")]
        Falls,
        [EnumMember(Value = "FLD")]
        [Description("Field")]
        Field,
        [EnumMember(Value = "FLDS")]
        [Description("Fields")]
        Fields,
        [EnumMember(Value = "FLT")]
        [Description("Flat")]
        Flat,
        [EnumMember(Value = "FLTS")]
        [Description("Flats")]
        Flats,
        [EnumMember(Value = "FRD")]
        [Description("Ford")]
        Ford,
        [EnumMember(Value = "FRST")]
        [Description("Forest")]
        Forest,
        [EnumMember(Value = "FRG")]
        [Description("Forge")]
        Forge,
        [EnumMember(Value = "FRK")]
        [Description("Fork")]
        Fork,
        [EnumMember(Value = "FRKS")]
        [Description("Forks")]
        Forks,
        [EnumMember(Value = "GDN")]
        [Description("Garden")]
        Garden,
        [EnumMember(Value = "GDNS")]
        [Description("Gardens")]
        Gardens,
        [EnumMember(Value = "GLN")]
        [Description("Glen")]
        Glen,
        [EnumMember(Value = "GRN")]
        [Description("Green")]
        Green,
        [EnumMember(Value = "GRV")]
        [Description("Grove")]
        Grove,
        [EnumMember(Value = "HBR")]
        [Description("Harbor")]
        Harbor,
        [EnumMember(Value = "HVN")]
        [Description("Haven")]
        Haven,
        [EnumMember(Value = "HTS")]
        [Description("Heights")]
        Heights,
        [EnumMember(Value = "HWY")]
        [Description("Highway")]
        Highway,
        [EnumMember(Value = "HL")]
        [Description("Hill")]
        Hill,
        [EnumMember(Value = "HLS")]
        [Description("Hills")]
        Hills,
        [EnumMember(Value = "HOLW")]
        [Description("Hollow")]
        Hollow,
        [EnumMember(Value = "IS")]
        [Description("Island")]
        Island,
        [EnumMember(Value = "ISLE")]
        [Description("Isle")]
        Isle,
        [EnumMember(Value = "JUNCTION")]
        [Description("Junction")]
        Junction,
        [EnumMember(Value = "KY")]
        [Description("Key")]
        Key,
        [EnumMember(Value = "KNL")]
        [Description("Knoll")]
        Knoll,
        [EnumMember(Value = "KNLS")]
        [Description("Knolls")]
        Knolls,
        [EnumMember(Value = "LK")]
        [Description("Lake")]
        Lake,
        [EnumMember(Value = "LKS")]
        [Description("Lakes")]
        Lakes,
        [EnumMember(Value = "LAND")]
        [Description("Land")]
        Land,
        [EnumMember(Value = "LNDG")]
        [Description("Landing")]
        Landing,
        [EnumMember(Value = "LN")]
        [Description("Lane")]
        Lane,
        [EnumMember(Value = "LGT")]
        [Description("Light")]
        Light,
        [EnumMember(Value = "LDG")]
        [Description("Lodge")]
        Lodge,
        [EnumMember(Value = "LOOP")]
        [Description("Loop")]
        Loop,
        [EnumMember(Value = "MNR")]
        [Description("Manor")]
        Manor,
        [EnumMember(Value = "MDW")]
        [Description("Meadow")]
        Meadow,
        [EnumMember(Value = "MDWS")]
        [Description("Meadows")]
        Meadows,
        [EnumMember(Value = "MEWS")]
        [Description("Mews")]
        Mews,
        [EnumMember(Value = "ML")]
        [Description("Mill")]
        Mill,
        [EnumMember(Value = "MLS")]
        [Description("Mills")]
        Mills,
        [EnumMember(Value = "MIST")]
        [Description("Mist")]
        Mist,
        [EnumMember(Value = "MT")]
        [Description("Mount")]
        Mount,
        [EnumMember(Value = "MTN")]
        [Description("Mountain")]
        Mountain,
        [EnumMember(Value = "OAK")]
        [Description("Oak")]
        Oak,
        [EnumMember(Value = "ORCH")]
        [Description("Orchard")]
        Orchard,
        [EnumMember(Value = "OVAL")]
        [Description("Oval")]
        Oval,
        [EnumMember(Value = "PARK")]
        [Description("Park")]
        Park,
        [EnumMember(Value = "PKWY")]
        [Description("Parkway")]
        Parkway,
        [EnumMember(Value = "PASS")]
        [Description("Pass")]
        Pass,
        [EnumMember(Value = "PATH")]
        [Description("Path")]
        Path,
        [EnumMember(Value = "PEAK")]
        [Description("Peak")]
        Peak,
        [EnumMember(Value = "PIKE")]
        [Description("Pike")]
        Pike,
        [EnumMember(Value = "PNE")]
        [Description("Pine")]
        Pine,
        [EnumMember(Value = "PL")]
        [Description("Place")]
        Place,
        [EnumMember(Value = "PLN")]
        [Description("Plain")]
        Plain,
        [EnumMember(Value = "PLNS")]
        [Description("Plains")]
        Plains,
        [EnumMember(Value = "PLZ")]
        [Description("Plaza")]
        Plaza,
        [EnumMember(Value = "PT")]
        [Description("Point")]
        Point,
        [EnumMember(Value = "PRT")]
        [Description("Port")]
        Port,
        [EnumMember(Value = "PR")]
        [Description("Prairie")]
        Prairie,
        [EnumMember(Value = "RNCH")]
        [Description("Ranch")]
        Ranch,
        [EnumMember(Value = "RPDS")]
        [Description("Rapids")]
        Rapids,
        [EnumMember(Value = "RDG")]
        [Description("Ridge")]
        Ridge,
        [EnumMember(Value = "RIV")]
        [Description("River")]
        River,
        [EnumMember(Value = "RD")]
        [Description("Road")]
        Road,
        [EnumMember(Value = "ROW")]
        [Description("Row")]
        Row,
        [EnumMember(Value = "RUN")]
        [Description("Run")]
        Run,
        [EnumMember(Value = "SHRS")]
        [Description("Shores")]
        Shores,
        [EnumMember(Value = "SKWY")]
        [Description("Skyway")]
        Skyway,
        [EnumMember(Value = "SPG")]
        [Description("Spring")]
        Spring,
        [EnumMember(Value = "SPGS")]
        [Description("Springs")]
        Springs,
        [EnumMember(Value = "SPUR")]
        [Description("Spur")]
        Spur,
        [EnumMember(Value = "SQ")]
        [Description("Square")]
        Square,
        [EnumMember(Value = "STA")]
        [Description("Station")]
        Station,
        [EnumMember(Value = "STRM")]
        [Description("Stream")]
        Stream,
        [EnumMember(Value = "ST")]
        [Description("Street")]
        Street,
        [EnumMember(Value = "SMT")]
        [Description("Summit")]
        Summit,
        [EnumMember(Value = "TER")]
        [Description("Terrace")]
        Terrace,
        [EnumMember(Value = "TRCE")]
        [Description("Trace")]
        Trace,
        [EnumMember(Value = "TRAK")]
        [Description("Track")]
        Track,
        [EnumMember(Value = "TRL")]
        [Description("Trail")]
        Trail,
        [EnumMember(Value = "VLY")]
        [Description("Valley")]
        Valley,
        [EnumMember(Value = "VW")]
        [Description("View")]
        View,
        [EnumMember(Value = "VLG")]
        [Description("Village")]
        Village,
        [EnumMember(Value = "VIS")]
        [Description("Vista")]
        Vista,
        [EnumMember(Value = "WALK")]
        [Description("Walk")]
        Walk,
        [EnumMember(Value = "WAY")]
        [Description("Way")]
        Way,
        [EnumMember(Value = "WLS")]
        [Description("Wells")]
        Wells,
        [EnumMember(Value = "WOOD")]
        [Description("Wood")]
        Wood,
    }
}
