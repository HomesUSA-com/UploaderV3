namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Counties
    {
        [EnumMember(Value = "ANDRSN")]
        [Description("Anderson")]
        Anderson,
        [EnumMember(Value = "ANDWS")]
        [Description("Andrews")]
        Andrews,
        [EnumMember(Value = "ANGELNA")]
        [Description("Angelina")]
        Angelina,
        [EnumMember(Value = "ARANSAS")]
        [Description("Aransas")]
        Aransas,
        [EnumMember(Value = "ARCHER")]
        [Description("Archer")]
        Archer,
        [EnumMember(Value = "ARMSTRG")]
        [Description("Armstrong")]
        Armstrong,
        [EnumMember(Value = "ATASC")]
        [Description("Atascosa")]
        Atascosa,
        [EnumMember(Value = "AUSTIN")]
        [Description("Austin")]
        Austin,
        [EnumMember(Value = "BAILEY")]
        [Description("Bailey")]
        Bailey,
        [EnumMember(Value = "BANDE")]
        [Description("Bandera")]
        Bandera,
        [EnumMember(Value = "BASTR")]
        [Description("Bastrop")]
        Bastrop,
        [EnumMember(Value = "BAYLR")]
        [Description("Baylor")]
        Baylor,
        [EnumMember(Value = "BEE")]
        [Description("Bee")]
        Bee,
        [EnumMember(Value = "BELL")]
        [Description("Bell")]
        Bell,
        [EnumMember(Value = "BEXAR")]
        [Description("Bexar")]
        Bexar,
        [EnumMember(Value = "BLANC")]
        [Description("Blanco")]
        Blanco,
        [EnumMember(Value = "BORDEN")]
        [Description("Borden")]
        Borden,
        [EnumMember(Value = "BOSQUE")]
        [Description("Bosque")]
        Bosque,
        [EnumMember(Value = "BOWIE")]
        [Description("Bowie")]
        Bowie,
        [EnumMember(Value = "BRAZRA")]
        [Description("Brazoria")]
        Brazoria,
        [EnumMember(Value = "BRAZOS")]
        [Description("Brazos")]
        Brazos,
        [EnumMember(Value = "BREWSTR")]
        [Description("Brewster")]
        Brewster,
        [EnumMember(Value = "BRISCOE")]
        [Description("Briscoe")]
        Briscoe,
        [EnumMember(Value = "BROOKS")]
        [Description("Brooks")]
        Brooks,
        [EnumMember(Value = "BROWN")]
        [Description("Brown")]
        Brown,
        [EnumMember(Value = "BURLESN")]
        [Description("Burleson")]
        Burleson,
        [EnumMember(Value = "BURNET")]
        [Description("Burnet")]
        Burnet,
        [EnumMember(Value = "CALDW")]
        [Description("Caldwell")]
        Caldwell,
        [EnumMember(Value = "CALHO")]
        [Description("Calhoun")]
        Calhoun,
        [EnumMember(Value = "CALAHN")]
        [Description("Callahan")]
        Callahan,
        [EnumMember(Value = "CAMERN")]
        [Description("Cameron")]
        Cameron,
        [EnumMember(Value = "CAMP")]
        [Description("Camp")]
        Camp,
        [EnumMember(Value = "CARSON")]
        [Description("Carson")]
        Carson,
        [EnumMember(Value = "CASS")]
        [Description("Cass")]
        Cass,
        [EnumMember(Value = "CASTRO")]
        [Description("Castro")]
        Castro,
        [EnumMember(Value = "CHMBRS")]
        [Description("Chambers")]
        Chambers,
        [EnumMember(Value = "CHEROK")]
        [Description("Cherokee")]
        Cherokee,
        [EnumMember(Value = "CHIDRS")]
        [Description("Childress")]
        Childress,
        [EnumMember(Value = "CLAY")]
        [Description("Clay")]
        Clay,
        [EnumMember(Value = "COCHRN")]
        [Description("Cochran")]
        Cochran,
        [EnumMember(Value = "COKE")]
        [Description("Coke")]
        Coke,
        [EnumMember(Value = "COLEMN")]
        [Description("Coleman")]
        Coleman,
        [EnumMember(Value = "COLLIN")]
        [Description("Collin")]
        Collin,
        [EnumMember(Value = "CLNGSWTH")]
        [Description("Collingsworth")]
        Collingsworth,
        [EnumMember(Value = "CO")]
        [Description("Colorado")]
        Colorado,
        [EnumMember(Value = "COMAL")]
        [Description("Comal")]
        Comal,
        [EnumMember(Value = "COMCHE")]
        [Description("Comanche")]
        Comanche,
        [EnumMember(Value = "CONCHO")]
        [Description("Concho")]
        Concho,
        [EnumMember(Value = "COOKE")]
        [Description("Cooke")]
        Cooke,
        [EnumMember(Value = "CORYELL")]
        [Description("Coryell")]
        Coryell,
        [EnumMember(Value = "COTTLE")]
        [Description("Cottle")]
        Cottle,
        [EnumMember(Value = "CRANE")]
        [Description("Crane")]
        Crane,
        [EnumMember(Value = "CRCKTT")]
        [Description("Crockett")]
        Crockett,
        [EnumMember(Value = "CROSBY")]
        [Description("Crosby")]
        Crosby,
        [EnumMember(Value = "CLBRSN")]
        [Description("Culberson")]
        Culberson,
        [EnumMember(Value = "DALLAM")]
        [Description("Dallam")]
        Dallam,
        [EnumMember(Value = "DALLAS")]
        [Description("Dallas")]
        Dallas,
        [EnumMember(Value = "DAWSN")]
        [Description("Dawson")]
        Dawson,
        [EnumMember(Value = "DEAFSMTH")]
        [Description("Deaf Smith")]
        DeafSmith,
        [EnumMember(Value = "DELTA")]
        [Description("Delta")]
        Delta,
        [EnumMember(Value = "DENTON")]
        [Description("Denton")]
        Denton,
        [EnumMember(Value = "DEWIT")]
        [Description("Dewitt")]
        Dewitt,
        [EnumMember(Value = "DICKNS")]
        [Description("Dickens")]
        Dickens,
        [EnumMember(Value = "DIMMIT")]
        [Description("Dimmit")]
        Dimmit,
        [EnumMember(Value = "DONLEY")]
        [Description("Donley")]
        Donley,
        [EnumMember(Value = "DUVAL")]
        [Description("Duval")]
        Duval,
        [EnumMember(Value = "ESTLND")]
        [Description("Eastland")]
        Eastland,
        [EnumMember(Value = "ECTOR")]
        [Description("Ector")]
        Ector,
        [EnumMember(Value = "EDWAR")]
        [Description("Edwards")]
        Edwards,
        [EnumMember(Value = "ELPASO")]
        [Description("El Paso")]
        ElPaso,
        [EnumMember(Value = "ELLIS")]
        [Description("Ellis")]
        Ellis,
        [EnumMember(Value = "ERATH")]
        [Description("Erath")]
        Erath,
        [EnumMember(Value = "FALLS")]
        [Description("Falls")]
        Falls,
        [EnumMember(Value = "FANNIN")]
        [Description("Fannin")]
        Fannin,
        [EnumMember(Value = "FAYET")]
        [Description("Fayette")]
        Fayette,
        [EnumMember(Value = "FISHER")]
        [Description("Fisher")]
        Fisher,
        [EnumMember(Value = "FLOYD")]
        [Description("Floyd")]
        Floyd,
        [EnumMember(Value = "FOARD")]
        [Description("Foard")]
        Foard,
        [EnumMember(Value = "FTBEND")]
        [Description("Fort Bend")]
        FortBend,
        [EnumMember(Value = "FRNKLIN")]
        [Description("Franklin")]
        Franklin,
        [EnumMember(Value = "FREESTN")]
        [Description("Freestone")]
        Freestone,
        [EnumMember(Value = "FRIO")]
        [Description("Frio")]
        Frio,
        [EnumMember(Value = "GAINES")]
        [Description("Gaines")]
        Gaines,
        [EnumMember(Value = "GALVSTN")]
        [Description("Galveston")]
        Galveston,
        [EnumMember(Value = "GARZA")]
        [Description("Garza")]
        Garza,
        [EnumMember(Value = "GILLE")]
        [Description("Gillespie")]
        Gillespie,
        [EnumMember(Value = "GLASSCK")]
        [Description("Glasscock")]
        Glasscock,
        [EnumMember(Value = "GOLIAD")]
        [Description("Goliad")]
        Goliad,
        [EnumMember(Value = "GONZA")]
        [Description("Gonzales")]
        Gonzales,
        [EnumMember(Value = "GRAY")]
        [Description("Gray")]
        Gray,
        [EnumMember(Value = "GRAYSN")]
        [Description("Grayson")]
        Grayson,
        [EnumMember(Value = "GREGG")]
        [Description("Gregg")]
        Gregg,
        [EnumMember(Value = "GRIMES")]
        [Description("Grimes")]
        Grimes,
        [EnumMember(Value = "GUADA")]
        [Description("Guadalupe")]
        Guadalupe,
        [EnumMember(Value = "HALE")]
        [Description("Hale")]
        Hale,
        [EnumMember(Value = "HALL")]
        [Description("Hall")]
        Hall,
        [EnumMember(Value = "HAMLTN")]
        [Description("Hamilton")]
        Hamilton,
        [EnumMember(Value = "HANSFD")]
        [Description("Hansford")]
        Hansford,
        [EnumMember(Value = "HRDEMN")]
        [Description("Hardeman")]
        Hardeman,
        [EnumMember(Value = "HARDIN")]
        [Description("Hardin")]
        Hardin,
        [EnumMember(Value = "HARRIS")]
        [Description("Harris")]
        Harris,
        [EnumMember(Value = "HARRISN")]
        [Description("Harrison")]
        Harrison,
        [EnumMember(Value = "HARTLY")]
        [Description("Hartley")]
        Hartley,
        [EnumMember(Value = "HASKEL")]
        [Description("Haskell")]
        Haskell,
        [EnumMember(Value = "HAYS")]
        [Description("Hays")]
        Hays,
        [EnumMember(Value = "HEMPHL")]
        [Description("Hemphill")]
        Hemphill,
        [EnumMember(Value = "HENDRSN")]
        [Description("Henderson")]
        Henderson,
        [EnumMember(Value = "HIDALGO")]
        [Description("Hidalgo")]
        Hidalgo,
        [EnumMember(Value = "HILL")]
        [Description("Hill")]
        Hill,
        [EnumMember(Value = "HCKLEY")]
        [Description("Hockley")]
        Hockley,
        [EnumMember(Value = "HOOD")]
        [Description("Hood")]
        Hood,
        [EnumMember(Value = "HOPKNS")]
        [Description("Hopkins")]
        Hopkins,
        [EnumMember(Value = "HOUSTN")]
        [Description("Houston")]
        Houston,
        [EnumMember(Value = "HOWRD")]
        [Description("Howard")]
        Howard,
        [EnumMember(Value = "HUDSPTH")]
        [Description("Hudspeth")]
        Hudspeth,
        [EnumMember(Value = "HUNT")]
        [Description("Hunt")]
        Hunt,
        [EnumMember(Value = "HTCHINSN")]
        [Description("Hutchinson")]
        Hutchinson,
        [EnumMember(Value = "IRION")]
        [Description("Irion")]
        Irion,
        [EnumMember(Value = "JACK")]
        [Description("Jack")]
        Jack,
        [EnumMember(Value = "JACKSN")]
        [Description("Jackson")]
        Jackson,
        [EnumMember(Value = "JASPER")]
        [Description("Jasper")]
        Jasper,
        [EnumMember(Value = "JEFFDAV")]
        [Description("Jeff Davis")]
        JeffDavis,
        [EnumMember(Value = "JEFFRSN")]
        [Description("Jefferson")]
        Jefferson,
        [EnumMember(Value = "JIMHOG")]
        [Description("Jim Hogg")]
        JimHogg,
        [EnumMember(Value = "JIMWEL")]
        [Description("Jim Wells")]
        JimWells,
        [EnumMember(Value = "JOHNSN")]
        [Description("Johnson")]
        Johnson,
        [EnumMember(Value = "JONES")]
        [Description("Jones")]
        Jones,
        [EnumMember(Value = "KARNE")]
        [Description("Karnes")]
        Karnes,
        [EnumMember(Value = "KAUFMN")]
        [Description("Kaufman")]
        Kaufman,
        [EnumMember(Value = "KENDA")]
        [Description("Kendall")]
        Kendall,
        [EnumMember(Value = "KENEDY")]
        [Description("Kenedy")]
        Kenedy,
        [EnumMember(Value = "KENT")]
        [Description("Kent")]
        Kent,
        [EnumMember(Value = "KERR")]
        [Description("Kerr")]
        Kerr,
        [EnumMember(Value = "KIMBLE")]
        [Description("Kimble")]
        Kimble,
        [EnumMember(Value = "KING")]
        [Description("King")]
        King,
        [EnumMember(Value = "KINNEY")]
        [Description("Kinney")]
        Kinney,
        [EnumMember(Value = "KLEBRG")]
        [Description("Kleberg")]
        Kleberg,
        [EnumMember(Value = "KNOX")]
        [Description("Knox")]
        Knox,
        [EnumMember(Value = "LASAL")]
        [Description("La Salle")]
        LaSalle,
        [EnumMember(Value = "LAMAR")]
        [Description("Lamar")]
        Lamar,
        [EnumMember(Value = "LAMB")]
        [Description("Lamb")]
        Lamb,
        [EnumMember(Value = "LMPASAS")]
        [Description("Lampasas")]
        Lampasas,
        [EnumMember(Value = "LAVAC")]
        [Description("Lavaca")]
        Lavaca,
        [EnumMember(Value = "LEE")]
        [Description("Lee")]
        Lee,
        [EnumMember(Value = "LEON")]
        [Description("Leon")]
        Leon,
        [EnumMember(Value = "LIBRTY")]
        [Description("Liberty")]
        Liberty,
        [EnumMember(Value = "LIMESTN")]
        [Description("Limestone")]
        Limestone,
        [EnumMember(Value = "LIPSCMB")]
        [Description("Lipscomb")]
        Lipscomb,
        [EnumMember(Value = "LIVEOAK")]
        [Description("Live Oak")]
        LiveOak,
        [EnumMember(Value = "LLANO")]
        [Description("Llano")]
        Llano,
        [EnumMember(Value = "LOVING")]
        [Description("Loving")]
        Loving,
        [EnumMember(Value = "LUBBCK")]
        [Description("Lubbock")]
        Lubbock,
        [EnumMember(Value = "LYNN")]
        [Description("Lynn")]
        Lynn,
        [EnumMember(Value = "MADISN")]
        [Description("Madison")]
        Madison,
        [EnumMember(Value = "MARION")]
        [Description("Marion")]
        Marion,
        [EnumMember(Value = "MARTIN")]
        [Description("Martin")]
        Martin,
        [EnumMember(Value = "MASON")]
        [Description("Mason")]
        Mason,
        [EnumMember(Value = "MATAG")]
        [Description("Matagorda")]
        Matagorda,
        [EnumMember(Value = "MAVRCK")]
        [Description("Maverick")]
        Maverick,
        [EnumMember(Value = "MCCULCH")]
        [Description("McCulloch")]
        McCulloch,
        [EnumMember(Value = "MCLENAN")]
        [Description("McLennan")]
        McLennan,
        [EnumMember(Value = "MCMULEN")]
        [Description("McMullen")]
        McMullen,
        [EnumMember(Value = "MEDIN")]
        [Description("Medina")]
        Medina,
        [EnumMember(Value = "MENRD")]
        [Description("Menard")]
        Menard,
        [EnumMember(Value = "MIDLND")]
        [Description("Midland")]
        Midland,
        [EnumMember(Value = "MILAM")]
        [Description("Milam")]
        Milam,
        [EnumMember(Value = "MILLS")]
        [Description("Mills")]
        Mills,
        [EnumMember(Value = "MTCHEL")]
        [Description("Mitchell")]
        Mitchell,
        [EnumMember(Value = "MNTAGUE")]
        [Description("Montague")]
        Montague,
        [EnumMember(Value = "MNTGMRY")]
        [Description("Montgomery")]
        Montgomery,
        [EnumMember(Value = "MOORE")]
        [Description("Moore")]
        Moore,
        [EnumMember(Value = "MORRIS")]
        [Description("Morris")]
        Morris,
        [EnumMember(Value = "MOTLEY")]
        [Description("Motley")]
        Motley,
        [EnumMember(Value = "NCODCHS")]
        [Description("Nacogdoches")]
        Nacogdoches,
        [EnumMember(Value = "NAVARO")]
        [Description("Navarro")]
        Navarro,
        [EnumMember(Value = "NEWTON")]
        [Description("Newton")]
        Newton,
        [EnumMember(Value = "NOLAN")]
        [Description("Nolan")]
        Nolan,
        [EnumMember(Value = "NUECE")]
        [Description("Nueces")]
        Nueces,
        [EnumMember(Value = "OCHLTR")]
        [Description("Ochiltree")]
        Ochiltree,
        [EnumMember(Value = "OLDHAM")]
        [Description("Oldham")]
        Oldham,
        [EnumMember(Value = "ORANGE")]
        [Description("Orange")]
        Orange,
        [EnumMember(Value = "OTH")]
        [Description("Other")]
        Other,
        [EnumMember(Value = "PALOPNTO")]
        [Description("Palo Pinto")]
        PaloPinto,
        [EnumMember(Value = "PANOLA")]
        [Description("Panola")]
        Panola,
        [EnumMember(Value = "PARKER")]
        [Description("Parker")]
        Parker,
        [EnumMember(Value = "PARMER")]
        [Description("Parmer")]
        Parmer,
        [EnumMember(Value = "PECOS")]
        [Description("Pecos")]
        Pecos,
        [EnumMember(Value = "POLK")]
        [Description("Polk")]
        Polk,
        [EnumMember(Value = "POTTER")]
        [Description("Potter")]
        Potter,
        [EnumMember(Value = "PRSDIO")]
        [Description("Presidio")]
        Presidio,
        [EnumMember(Value = "RAINS")]
        [Description("Rains")]
        Rains,
        [EnumMember(Value = "RNDALL")]
        [Description("Randall")]
        Randall,
        [EnumMember(Value = "REAGAN")]
        [Description("Reagan")]
        Reagan,
        [EnumMember(Value = "REAL")]
        [Description("Real")]
        Real,
        [EnumMember(Value = "REDRV")]
        [Description("Red River")]
        RedRiver,
        [EnumMember(Value = "REEVES")]
        [Description("Reeves")]
        Reeves,
        [EnumMember(Value = "REFGIO")]
        [Description("Refugio")]
        Refugio,
        [EnumMember(Value = "ROBRTS")]
        [Description("Roberts")]
        Roberts,
        [EnumMember(Value = "ROBRTSN")]
        [Description("Robertson")]
        Robertson,
        [EnumMember(Value = "RCKWALL")]
        [Description("Rockwall")]
        Rockwall,
        [EnumMember(Value = "RUNELS")]
        [Description("Runnels")]
        Runnels,
        [EnumMember(Value = "RUSK")]
        [Description("Rusk")]
        Rusk,
        [EnumMember(Value = "SABINE")]
        [Description("Sabine")]
        Sabine,
        [EnumMember(Value = "SANAUG")]
        [Description("San Augustine")]
        SanAugustine,
        [EnumMember(Value = "SANJCNTO")]
        [Description("San Jacinto")]
        SanJacinto,
        [EnumMember(Value = "SANPTRO")]
        [Description("San Patricio")]
        SanPatricio,
        [EnumMember(Value = "SANSABO")]
        [Description("San Saba")]
        SanSaba,
        [EnumMember(Value = "SCHLCHER")]
        [Description("Schleicher")]
        Schleicher,
        [EnumMember(Value = "SCURRY")]
        [Description("Scurry")]
        Scurry,
        [EnumMember(Value = "SHCKLFRD")]
        [Description("Shackelford")]
        Shackelford,
        [EnumMember(Value = "SHELBY")]
        [Description("Shelby")]
        Shelby,
        [EnumMember(Value = "SHERMN")]
        [Description("Sherman")]
        Sherman,
        [EnumMember(Value = "SMITH")]
        [Description("Smith")]
        Smith,
        [EnumMember(Value = "SOMRVEL")]
        [Description("Somervell")]
        Somervell,
        [EnumMember(Value = "STARR")]
        [Description("Starr")]
        Starr,
        [EnumMember(Value = "STPHENS")]
        [Description("Stephens")]
        Stephens,
        [EnumMember(Value = "STRLNG")]
        [Description("Sterling")]
        Sterling,
        [EnumMember(Value = "STNWELL")]
        [Description("Stonewall")]
        Stonewall,
        [EnumMember(Value = "SUTTON")]
        [Description("Sutton")]
        Sutton,
        [EnumMember(Value = "SWISHR")]
        [Description("Swisher")]
        Swisher,
        [EnumMember(Value = "TARRNT")]
        [Description("Tarrant")]
        Tarrant,
        [EnumMember(Value = "TAYLOR")]
        [Description("Taylor")]
        Taylor,
        [EnumMember(Value = "TERREL")]
        [Description("Terrell")]
        Terrell,
        [EnumMember(Value = "TERRY")]
        [Description("Terry")]
        Terry,
        [EnumMember(Value = "THRCKMRTN")]
        [Description("Throckmorton")]
        Throckmorton,
        [EnumMember(Value = "TITUS")]
        [Description("Titus")]
        Titus,
        [EnumMember(Value = "TOMGRN")]
        [Description("Tom Green")]
        TomGreen,
        [EnumMember(Value = "TRAVI")]
        [Description("Travis")]
        Travis,
        [EnumMember(Value = "TRINITY")]
        [Description("Trinity")]
        Trinity,
        [EnumMember(Value = "TYLER")]
        [Description("Tyler")]
        Tyler,
        [EnumMember(Value = "UPSHUR")]
        [Description("Upshur")]
        Upshur,
        [EnumMember(Value = "UPTON")]
        [Description("Upton")]
        Upton,
        [EnumMember(Value = "UVALDE")]
        [Description("Uvalde")]
        Uvalde,
        [EnumMember(Value = "VALVERD")]
        [Description("Val Verde")]
        ValVerde,
        [EnumMember(Value = "VANZAN")]
        [Description("Van Zandt")]
        VanZandt,
        [EnumMember(Value = "VICTRA")]
        [Description("Victoria")]
        Victoria,
        [EnumMember(Value = "WALKE")]
        [Description("Walker")]
        Walker,
        [EnumMember(Value = "WALLER")]
        [Description("Waller")]
        Waller,
        [EnumMember(Value = "WARD")]
        [Description("Ward")]
        Ward,
        [EnumMember(Value = "WSHNGTN")]
        [Description("Washington")]
        Washington,
        [EnumMember(Value = "WEBB")]
        [Description("Webb")]
        Webb,
        [EnumMember(Value = "WHARTN")]
        [Description("Wharton")]
        Wharton,
        [EnumMember(Value = "WHEELR")]
        [Description("Wheeler")]
        Wheeler,
        [EnumMember(Value = "WICHITA")]
        [Description("Wichita")]
        Wichita,
        [EnumMember(Value = "WILBRGR")]
        [Description("Wilbarger")]
        Wilbarger,
        [EnumMember(Value = "WILLCY")]
        [Description("Willacy")]
        Willacy,
        [EnumMember(Value = "WILMSON")]
        [Description("Williamson")]
        Williamson,
        [EnumMember(Value = "WILSO")]
        [Description("Wilson")]
        Wilson,
        [EnumMember(Value = "WINKLR")]
        [Description("Winkler")]
        Winkler,
        [EnumMember(Value = "WISE")]
        [Description("Wise")]
        Wise,
        [EnumMember(Value = "WOOD")]
        [Description("Wood")]
        Wood,
        [EnumMember(Value = "YOAKUM")]
        [Description("Yoakum")]
        Yoakum,
        [EnumMember(Value = "YOUNG")]
        [Description("Young")]
        Young,
        [EnumMember(Value = "ZAPATA")]
        [Description("Zapata")]
        Zapata,
        [EnumMember(Value = "ZAVALA")]
        [Description("Zavala")]
        Zavala,
    }
}
