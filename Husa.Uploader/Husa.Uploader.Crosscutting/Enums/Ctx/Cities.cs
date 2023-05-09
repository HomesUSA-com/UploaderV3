namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Cities
    {
        [EnumMember(Value = "Abbott")]
        [Description("Abbott")]
        Abbott,
        [EnumMember(Value = "Abernathy")]
        [Description("Abernathy")]
        Abernathy,
        [EnumMember(Value = "Abilene")]
        [Description("Abilene")]
        Abilene,
        [EnumMember(Value = "Abram")]
        [Description("Abram")]
        Abram,
        [EnumMember(Value = "Ackerly")]
        [Description("Ackerly")]
        Ackerly,
        [EnumMember(Value = "Adamsville")]
        [Description("Adamsville")]
        Adamsville,
        [EnumMember(Value = "Addison")]
        [Description("Addison")]
        Addison,
        [EnumMember(Value = "ADKIN")]
        [Description("Adkins")]
        Adkins,
        [EnumMember(Value = "Adrian")]
        [Description("Adrian")]
        Adrian,
        [EnumMember(Value = "AguaDulce")]
        [Description("Agua Dulce")]
        AguaDulce,
        [EnumMember(Value = "Aguilares")]
        [Description("Aguilares")]
        Aguilares,
        [EnumMember(Value = "AirportHeights")]
        [Description("Airport Heights")]
        AirportHeights,
        [EnumMember(Value = "AirportRoadAddition")]
        [Description("Airport Road Addition")]
        AirportRoadAddition,
        [EnumMember(Value = "ALAMO")]
        [Description("Alamo Heights")]
        AlamoHeights,
        [EnumMember(Value = "Alba")]
        [Description("Alba")]
        Alba,
        [EnumMember(Value = "Albany")]
        [Description("Albany")]
        Albany,
        [EnumMember(Value = "Aldine")]
        [Description("Aldine")]
        Aldine,
        [EnumMember(Value = "Aledo")]
        [Description("Aledo")]
        Aledo,
        [EnumMember(Value = "Alfred")]
        [Description("Alfred")]
        Alfred,
        [EnumMember(Value = "Alice")]
        [Description("Alice")]
        Alice,
        [EnumMember(Value = "AliceAcres")]
        [Description("Alice Acres")]
        AliceAcres,
        [EnumMember(Value = "Allen")]
        [Description("Allen")]
        Allen,
        [EnumMember(Value = "Alleyton")]
        [Description("Alleyton")]
        Alleyton,
        [EnumMember(Value = "Alma")]
        [Description("Alma")]
        Alma,
        [EnumMember(Value = "Alpine")]
        [Description("Alpine")]
        Alpine,
        [EnumMember(Value = "Alto")]
        [Description("Alto")]
        Alto,
        [EnumMember(Value = "AltoBonitoHeights")]
        [Description("Alto Bonito Heights")]
        AltoBonitoHeights,
        [EnumMember(Value = "Alton")]
        [Description("Alton")]
        Alton,
        [EnumMember(Value = "Alvarado")]
        [Description("Alvarado")]
        Alvarado,
        [EnumMember(Value = "ALVIN")]
        [Description("Alvin")]
        Alvin,
        [EnumMember(Value = "Alvord")]
        [Description("Alvord")]
        Alvord,
        [EnumMember(Value = "AmadaAcres")]
        [Description("Amada Acres")]
        AmadaAcres,
        [EnumMember(Value = "Amargosa")]
        [Description("Amargosa")]
        Amargosa,
        [EnumMember(Value = "Amaya")]
        [Description("Amaya")]
        Amaya,
        [EnumMember(Value = "Ames")]
        [Description("Ames")]
        Ames,
        [EnumMember(Value = "Amherst")]
        [Description("Amherst")]
        Amherst,
        [EnumMember(Value = "Amistad")]
        [Description("Amistad")]
        Amistad,
        [EnumMember(Value = "Anacua")]
        [Description("Anacua")]
        Anacua,
        [EnumMember(Value = "Anahuac")]
        [Description("Anahuac")]
        Anahuac,
        [EnumMember(Value = "Anderson")]
        [Description("Anderson")]
        Anderson,
        [EnumMember(Value = "Andice")]
        [Description("Andice")]
        Andice,
        [EnumMember(Value = "Andrews")]
        [Description("Andrews")]
        Andrews,
        [EnumMember(Value = "Angleton")]
        [Description("Angleton")]
        Angleton,
        [EnumMember(Value = "Angus")]
        [Description("Angus")]
        Angus,
        [EnumMember(Value = "Anna")]
        [Description("Anna")]
        Anna,
        [EnumMember(Value = "Annetta")]
        [Description("Annetta")]
        Annetta,
        [EnumMember(Value = "AnnettaNorth")]
        [Description("Annetta North")]
        AnnettaNorth,
        [EnumMember(Value = "AnnettaSouth")]
        [Description("Annetta South")]
        AnnettaSouth,
        [EnumMember(Value = "Annona")]
        [Description("Annona")]
        Annona,
        [EnumMember(Value = "Anson")]
        [Description("Anson")]
        Anson,
        [EnumMember(Value = "Anthony")]
        [Description("Anthony")]
        Anthony,
        [EnumMember(Value = "Anton")]
        [Description("Anton")]
        Anton,
        [EnumMember(Value = "Appleby")]
        [Description("Appleby")]
        Appleby,
        [EnumMember(Value = "Aquilla")]
        [Description("Aquilla")]
        Aquilla,
        [EnumMember(Value = "AransasPass")]
        [Description("Aransas Pass")]
        AransasPass,
        [EnumMember(Value = "ArcherCity")]
        [Description("Archer City")]
        ArcherCity,
        [EnumMember(Value = "Arcola")]
        [Description("Arcola")]
        Arcola,
        [EnumMember(Value = "Argyle")]
        [Description("Argyle")]
        Argyle,
        [EnumMember(Value = "Arlington")]
        [Description("Arlington")]
        Arlington,
        [EnumMember(Value = "Arp")]
        [Description("Arp")]
        Arp,
        [EnumMember(Value = "ArroyoColoradoEstates")]
        [Description("Arroyo Colorado Estates")]
        ArroyoColoradoEstates,
        [EnumMember(Value = "ArroyoGardens")]
        [Description("Arroyo Gardens")]
        ArroyoGardens,
        [EnumMember(Value = "Asherton")]
        [Description("Asherton")]
        Asherton,
        [EnumMember(Value = "Aspermont")]
        [Description("Aspermont")]
        Aspermont,
        [EnumMember(Value = "Atascocita")]
        [Description("Atascocita")]
        Atascocita,
        [EnumMember(Value = "ATASC")]
        [Description("Atascosa")]
        Atascosa,
        [EnumMember(Value = "Athens")]
        [Description("Athens")]
        Athens,
        [EnumMember(Value = "Atlanta")]
        [Description("Atlanta")]
        Atlanta,
        [EnumMember(Value = "Aubrey")]
        [Description("Aubrey")]
        Aubrey,
        [EnumMember(Value = "Aurora")]
        [Description("Aurora")]
        Aurora,
        [EnumMember(Value = "AUSTI")]
        [Description("Austin")]
        Austin,
        [EnumMember(Value = "Austwell")]
        [Description("Austwell")]
        Austwell,
        [EnumMember(Value = "Avery")]
        [Description("Avery")]
        Avery,
        [EnumMember(Value = "Avinger")]
        [Description("Avinger")]
        Avinger,
        [EnumMember(Value = "Axtell")]
        [Description("Axtell")]
        Axtell,
        [EnumMember(Value = "Azle")]
        [Description("Azle")]
        Azle,
        [EnumMember(Value = "Bacliff")]
        [Description("Bacliff")]
        Bacliff,
        [EnumMember(Value = "Bailey")]
        [Description("Bailey")]
        Bailey,
        [EnumMember(Value = "BaileysPrairie")]
        [Description("Bailey\"s Prairie")]
        BaileysPrairie,
        [EnumMember(Value = "Baird")]
        [Description("Baird")]
        Baird,
        [EnumMember(Value = "BalchSprings")]
        [Description("Balch Springs")]
        BalchSprings,
        [EnumMember(Value = "BalconesHeights")]
        [Description("Balcones Heights")]
        BalconesHeights,
        [EnumMember(Value = "Ballinger")]
        [Description("Ballinger")]
        Ballinger,
        [EnumMember(Value = "Balmorhea")]
        [Description("Balmorhea")]
        Balmorhea,
        [EnumMember(Value = "BANDE")]
        [Description("Bandera")]
        Bandera,
        [EnumMember(Value = "Bangs")]
        [Description("Bangs")]
        Bangs,
        [EnumMember(Value = "Banquete")]
        [Description("Banquete")]
        Banquete,
        [EnumMember(Value = "Barclay")]
        [Description("Barclay")]
        Barclay,
        [EnumMember(Value = "Bardwell")]
        [Description("Bardwell")]
        Bardwell,
        [EnumMember(Value = "Barrera")]
        [Description("Barrera")]
        Barrera,
        [EnumMember(Value = "Barrett")]
        [Description("Barrett")]
        Barrett,
        [EnumMember(Value = "Barry")]
        [Description("Barry")]
        Barry,
        [EnumMember(Value = "Barstow")]
        [Description("Barstow")]
        Barstow,
        [EnumMember(Value = "Bartlett")]
        [Description("Bartlett")]
        Bartlett,
        [EnumMember(Value = "BartonCreek")]
        [Description("Barton Creek")]
        BartonCreek,
        [EnumMember(Value = "Bartonville")]
        [Description("Bartonville")]
        Bartonville,
        [EnumMember(Value = "BASTR")]
        [Description("Bastrop")]
        Bastrop,
        [EnumMember(Value = "Batesville")]
        [Description("Batesville")]
        Batesville,
        [EnumMember(Value = "BayCity")]
        [Description("Bay City")]
        BayCity,
        [EnumMember(Value = "BayouVista")]
        [Description("Bayou Vista")]
        BayouVista,
        [EnumMember(Value = "Bayside")]
        [Description("Bayside")]
        Bayside,
        [EnumMember(Value = "BAYTO")]
        [Description("Baytown")]
        Baytown,
        [EnumMember(Value = "Bayview")]
        [Description("Bayview")]
        Bayview,
        [EnumMember(Value = "BEACH")]
        [Description("Beach City")]
        BeachCity,
        [EnumMember(Value = "BearCreek")]
        [Description("Bear Creek")]
        BearCreek,
        [EnumMember(Value = "Beasley")]
        [Description("Beasley")]
        Beasley,
        [EnumMember(Value = "Beaumont")]
        [Description("Beaumont")]
        Beaumont,
        [EnumMember(Value = "BEBE")]
        [Description("Bebe")]
        Bebe,
        [EnumMember(Value = "Beckville")]
        [Description("Beckville")]
        Beckville,
        [EnumMember(Value = "Bedford")]
        [Description("Bedford")]
        Bedford,
        [EnumMember(Value = "Bedias")]
        [Description("Bedias")]
        Bedias,
        [EnumMember(Value = "BeeCave")]
        [Description("Bee Cave")]
        BeeCave,
        [EnumMember(Value = "Beeville")]
        [Description("Beeville")]
        Beeville,
        [EnumMember(Value = "Bellaire")]
        [Description("Bellaire")]
        Bellaire,
        [EnumMember(Value = "Bellevue")]
        [Description("Bellevue")]
        Bellevue,
        [EnumMember(Value = "Bellfalls")]
        [Description("Bellfalls")]
        Bellfalls,
        [EnumMember(Value = "Bellmead")]
        [Description("Bellmead")]
        Bellmead,
        [EnumMember(Value = "Bells")]
        [Description("Bells")]
        Bells,
        [EnumMember(Value = "Bellville")]
        [Description("Bellville")]
        Bellville,
        [EnumMember(Value = "BELMO")]
        [Description("Belmont")]
        Belmont,
        [EnumMember(Value = "Belton")]
        [Description("Belton")]
        Belton,
        [EnumMember(Value = "Benavides")]
        [Description("Benavides")]
        Benavides,
        [EnumMember(Value = "Benbrook")]
        [Description("Benbrook")]
        Benbrook,
        [EnumMember(Value = "Bend")]
        [Description("Bend")]
        Bend,
        [EnumMember(Value = "Benjamin")]
        [Description("Benjamin")]
        Benjamin,
        [EnumMember(Value = "BenjaminPerez")]
        [Description("Benjamin Perez")]
        BenjaminPerez,
        [EnumMember(Value = "Berclair")]
        [Description("Berclair")]
        Berclair,
        [EnumMember(Value = "BERGH")]
        [Description("Bergheim")]
        Bergheim,
        [EnumMember(Value = "Berryville")]
        [Description("Berryville")]
        Berryville,
        [EnumMember(Value = "Bertram")]
        [Description("Bertram")]
        Bertram,
        [EnumMember(Value = "BeverlyHills")]
        [Description("Beverly Hills")]
        BeverlyHills,
        [EnumMember(Value = "BevilOaks")]
        [Description("Bevil Oaks")]
        BevilOaks,
        [EnumMember(Value = "BigLake")]
        [Description("Big Lake")]
        BigLake,
        [EnumMember(Value = "BigSandy")]
        [Description("Big Sandy")]
        BigSandy,
        [EnumMember(Value = "BigSpring")]
        [Description("Big Spring")]
        BigSpring,
        [EnumMember(Value = "BigThicketLakeEstates")]
        [Description("Big Thicket Lake Estates")]
        BigThicketLakeEstates,
        [EnumMember(Value = "BIGWE")]
        [Description("Big Wells")]
        BigWells,
        [EnumMember(Value = "Bigfoot")]
        [Description("Bigfoot")]
        Bigfoot,
        [EnumMember(Value = "BishopHills")]
        [Description("Bishop Hills")]
        BishopHills,
        [EnumMember(Value = "Bixby")]
        [Description("Bixby")]
        Bixby,
        [EnumMember(Value = "Blackwell")]
        [Description("Blackwell")]
        Blackwell,
        [EnumMember(Value = "BLANC")]
        [Description("Blanco")]
        Blanco,
        [EnumMember(Value = "Blanket")]
        [Description("Blanket")]
        Blanket,
        [EnumMember(Value = "Blessing")]
        [Description("Blessing")]
        Blessing,
        [EnumMember(Value = "Bloomburg")]
        [Description("Bloomburg")]
        Bloomburg,
        [EnumMember(Value = "BloomingGrove")]
        [Description("Blooming Grove")]
        BloomingGrove,
        [EnumMember(Value = "Bloomington")]
        [Description("Bloomington")]
        Bloomington,
        [EnumMember(Value = "Blossom")]
        [Description("Blossom")]
        Blossom,
        [EnumMember(Value = "BlueBerryHill")]
        [Description("Blue Berry Hill")]
        BlueBerryHill,
        [EnumMember(Value = "BlueMound")]
        [Description("Blue Mound")]
        BlueMound,
        [EnumMember(Value = "BlueRidge")]
        [Description("Blue Ridge")]
        BlueRidge,
        [EnumMember(Value = "Bluetown")]
        [Description("Bluetown")]
        Bluetown,
        [EnumMember(Value = "Blum")]
        [Description("Blum")]
        Blum,
        [EnumMember(Value = "BOERN")]
        [Description("Boerne")]
        Boerne,
        [EnumMember(Value = "Bogata")]
        [Description("Bogata")]
        Bogata,
        [EnumMember(Value = "BOLIN")]
        [Description("Boling")]
        Boling,
        [EnumMember(Value = "BolivarPeninsula")]
        [Description("Bolivar Peninsula")]
        BolivarPeninsula,
        [EnumMember(Value = "BonanzaHills")]
        [Description("Bonanza Hills")]
        BonanzaHills,
        [EnumMember(Value = "Bonham")]
        [Description("Bonham")]
        Bonham,
        [EnumMember(Value = "Bonney")]
        [Description("Bonney")]
        Bonney,
        [EnumMember(Value = "Booker")]
        [Description("Booker")]
        Booker,
        [EnumMember(Value = "Borger")]
        [Description("Borger")]
        Borger,
        [EnumMember(Value = "Botines")]
        [Description("Botines")]
        Botines,
        [EnumMember(Value = "Bovina")]
        [Description("Bovina")]
        Bovina,
        [EnumMember(Value = "Bowie")]
        [Description("Bowie")]
        Bowie,
        [EnumMember(Value = "BoxCanyon")]
        [Description("Box Canyon")]
        BoxCanyon,
        [EnumMember(Value = "Boyd")]
        [Description("Boyd")]
        Boyd,
        [EnumMember(Value = "BoysRanch")]
        [Description("Boys Ranch")]
        BoysRanch,
        [EnumMember(Value = "BRACK")]
        [Description("Bracken")]
        Bracken,
        [EnumMember(Value = "Brackettville")]
        [Description("Brackettville")]
        Brackettville,
        [EnumMember(Value = "Brady")]
        [Description("Brady")]
        Brady,
        [EnumMember(Value = "Brandon")]
        [Description("Brandon")]
        Brandon,
        [EnumMember(Value = "Brazoria")]
        [Description("Brazoria")]
        Brazoria,
        [EnumMember(Value = "BrazosBend")]
        [Description("Brazos Bend")]
        BrazosBend,
        [EnumMember(Value = "BrazosCountry")]
        [Description("Brazos Country")]
        BrazosCountry,
        [EnumMember(Value = "Breckenridge")]
        [Description("Breckenridge")]
        Breckenridge,
        [EnumMember(Value = "Bremond")]
        [Description("Bremond")]
        Bremond,
        [EnumMember(Value = "Brenham")]
        [Description("Brenham")]
        Brenham,
        [EnumMember(Value = "Briar")]
        [Description("Briar")]
        Briar,
        [EnumMember(Value = "Briarcliff")]
        [Description("Briarcliff")]
        Briarcliff,
        [EnumMember(Value = "Briaroaks")]
        [Description("Briaroaks")]
        Briaroaks,
        [EnumMember(Value = "BridgeCity")]
        [Description("Bridge City")]
        BridgeCity,
        [EnumMember(Value = "Bridgeport")]
        [Description("Bridgeport")]
        Bridgeport,
        [EnumMember(Value = "Briggs")]
        [Description("Briggs")]
        Briggs,
        [EnumMember(Value = "Bristol")]
        [Description("Bristol")]
        Bristol,
        [EnumMember(Value = "Broaddus")]
        [Description("Broaddus")]
        Broaddus,
        [EnumMember(Value = "Bronte")]
        [Description("Bronte")]
        Bronte,
        [EnumMember(Value = "Brookshire")]
        [Description("Brookshire")]
        Brookshire,
        [EnumMember(Value = "BrooksideVillage")]
        [Description("Brookside Village")]
        BrooksideVillage,
        [EnumMember(Value = "Browndell")]
        [Description("Browndell")]
        Browndell,
        [EnumMember(Value = "Brownfield")]
        [Description("Brownfield")]
        Brownfield,
        [EnumMember(Value = "Brownsboro")]
        [Description("Brownsboro")]
        Brownsboro,
        [EnumMember(Value = "Brownsville")]
        [Description("Brownsville")]
        Brownsville,
        [EnumMember(Value = "Brownwood")]
        [Description("Brownwood")]
        Brownwood,
        [EnumMember(Value = "BrucevilleEddy")]
        [Description("Bruceville-Eddy")]
        BrucevilleEddy,
        [EnumMember(Value = "Brundage")]
        [Description("Brundage")]
        Brundage,
        [EnumMember(Value = "Bruni")]
        [Description("Bruni")]
        Bruni,
        [EnumMember(Value = "BrushyCreek")]
        [Description("Brushy Creek")]
        BrushyCreek,
        [EnumMember(Value = "Bryan")]
        [Description("Bryan")]
        Bryan,
        [EnumMember(Value = "Bryson")]
        [Description("Bryson")]
        Bryson,
        [EnumMember(Value = "BuchananDam")]
        [Description("Buchanan Dam")]
        BuchananDam,
        [EnumMember(Value = "BuchananLakeVillage")]
        [Description("Buchanan Lake Village")]
        BuchananLakeVillage,
        [EnumMember(Value = "Buckholts")]
        [Description("Buckholts")]
        Buckholts,
        [EnumMember(Value = "BUDA")]
        [Description("Buda")]
        Buda,
        [EnumMember(Value = "BuenaVista")]
        [Description("Buena Vista")]
        BuenaVista,
        [EnumMember(Value = "Buffalo")]
        [Description("Buffalo")]
        Buffalo,
        [EnumMember(Value = "BuffaloGap")]
        [Description("Buffalo Gap")]
        BuffaloGap,
        [EnumMember(Value = "BuffaloSprings")]
        [Description("Buffalo Springs")]
        BuffaloSprings,
        [EnumMember(Value = "Bullard")]
        [Description("Bullard")]
        Bullard,
        [EnumMember(Value = "BULVE")]
        [Description("Bulverde")]
        Bulverde,
        [EnumMember(Value = "Buna")]
        [Description("Buna")]
        Buna,
        [EnumMember(Value = "BunkerHillVillage")]
        [Description("Bunker Hill Village")]
        BunkerHillVillage,
        [EnumMember(Value = "Burkburnett")]
        [Description("Burkburnett")]
        Burkburnett,
        [EnumMember(Value = "Burke")]
        [Description("Burke")]
        Burke,
        [EnumMember(Value = "Burleson")]
        [Description("Burleson")]
        Burleson,
        [EnumMember(Value = "Burlington")]
        [Description("Burlington")]
        Burlington,
        [EnumMember(Value = "Burnet")]
        [Description("Burnet")]
        Burnet,
        [EnumMember(Value = "Burton")]
        [Description("Burton")]
        Burton,
        [EnumMember(Value = "Butterfield")]
        [Description("Butterfield")]
        Butterfield,
        [EnumMember(Value = "Byers")]
        [Description("Byers")]
        Byers,
        [EnumMember(Value = "Bynum")]
        [Description("Bynum")]
        Bynum,
        [EnumMember(Value = "Cactus")]
        [Description("Cactus")]
        Cactus,
        [EnumMember(Value = "CaddoMills")]
        [Description("Caddo Mills")]
        CaddoMills,
        [EnumMember(Value = "Caldwell")]
        [Description("Caldwell")]
        Caldwell,
        [EnumMember(Value = "CallenderLake")]
        [Description("Callender Lake")]
        CallenderLake,
        [EnumMember(Value = "Callisburg")]
        [Description("Callisburg")]
        Callisburg,
        [EnumMember(Value = "Calvert")]
        [Description("Calvert")]
        Calvert,
        [EnumMember(Value = "Camargito")]
        [Description("Camargito")]
        Camargito,
        [EnumMember(Value = "Cameron")]
        [Description("Cameron")]
        Cameron,
        [EnumMember(Value = "CameronPark")]
        [Description("Cameron Park")]
        CameronPark,
        [EnumMember(Value = "CampSwift")]
        [Description("Camp Swift")]
        CampSwift,
        [EnumMember(Value = "CampWood")]
        [Description("Camp Wood")]
        CampWood,
        [EnumMember(Value = "Campbell")]
        [Description("Campbell")]
        Campbell,
        [EnumMember(Value = "CampoVerde")]
        [Description("Campo Verde")]
        CampoVerde,
        [EnumMember(Value = "Canadian")]
        [Description("Canadian")]
        Canadian,
        [EnumMember(Value = "CaneyCity")]
        [Description("Caney City")]
        CaneyCity,
        [EnumMember(Value = "Canton")]
        [Description("Canton")]
        Canton,
        [EnumMember(Value = "CantuAddition")]
        [Description("Cantu Addition")]
        CantuAddition,
        [EnumMember(Value = "Canutillo")]
        [Description("Canutillo")]
        Canutillo,
        [EnumMember(Value = "Canyon")]
        [Description("Canyon")]
        Canyon,
        [EnumMember(Value = "CanyonCreek")]
        [Description("Canyon Creek")]
        CanyonCreek,
        [EnumMember(Value = "CANYO")]
        [Description("Canyon Lake")]
        CanyonLake,
        [EnumMember(Value = "CapeRoyale")]
        [Description("Cape Royale")]
        CapeRoyale,
        [EnumMember(Value = "Carbon")]
        [Description("Carbon")]
        Carbon,
        [EnumMember(Value = "CarlsCorner")]
        [Description("Carl\"s Corner")]
        CarlsCorner,
        [EnumMember(Value = "Carlsbad")]
        [Description("Carlsbad")]
        Carlsbad,
        [EnumMember(Value = "Carmine")]
        [Description("Carmine")]
        Carmine,
        [EnumMember(Value = "CarrizoHill")]
        [Description("Carrizo Hill")]
        CarrizoHill,
        [EnumMember(Value = "CarrizoSprings")]
        [Description("Carrizo Springs")]
        CarrizoSprings,
        [EnumMember(Value = "Carrollton")]
        [Description("Carrollton")]
        Carrollton,
        [EnumMember(Value = "Carthage")]
        [Description("Carthage")]
        Carthage,
        [EnumMember(Value = "CasaBlanca")]
        [Description("Casa Blanca")]
        CasaBlanca,
        [EnumMember(Value = "Casas")]
        [Description("Casas")]
        Casas,
        [EnumMember(Value = "CashionCommunity")]
        [Description("Cashion Community")]
        CashionCommunity,
        [EnumMember(Value = "CastleHills")]
        [Description("Castle Hills")]
        CastleHills,
        [EnumMember(Value = "Castroville")]
        [Description("Castroville")]
        Castroville,
        [EnumMember(Value = "Catarina")]
        [Description("Catarina")]
        Catarina,
        [EnumMember(Value = "CEDAR")]
        [Description("Cedar Creek")]
        CedarCreek,
        [EnumMember(Value = "CedarHill")]
        [Description("Cedar Hill")]
        CedarHill,
        [EnumMember(Value = "CedarPark")]
        [Description("Cedar Park")]
        CedarPark,
        [EnumMember(Value = "CedarPoint")]
        [Description("Cedar Point")]
        CedarPoint,
        [EnumMember(Value = "Celeste")]
        [Description("Celeste")]
        Celeste,
        [EnumMember(Value = "Celina")]
        [Description("Celina")]
        Celina,
        [EnumMember(Value = "censusplacename")]
        [Description("census_place_name")]
        CensusPlaceName,
        [EnumMember(Value = "Center")]
        [Description("Center")]
        Center,
        [EnumMember(Value = "Centerville")]
        [Description("Centerville")]
        Centerville,
        [EnumMember(Value = "CentralGardens")]
        [Description("Central Gardens")]
        CentralGardens,
        [EnumMember(Value = "CésarChávez")]
        [Description("César Chávez")]
        CesarChavez,
        [EnumMember(Value = "Chandler")]
        [Description("Chandler")]
        Chandler,
        [EnumMember(Value = "Channelview")]
        [Description("Channelview")]
        Channelview,
        [EnumMember(Value = "Channing")]
        [Description("Channing")]
        Channing,
        [EnumMember(Value = "Chaparrito")]
        [Description("Chaparrito")]
        Chaparrito,
        [EnumMember(Value = "Chapeno")]
        [Description("Chapeno")]
        Chapeno,
        [EnumMember(Value = "Charlotte")]
        [Description("Charlotte")]
        Charlotte,
        [EnumMember(Value = "Chester")]
        [Description("Chester")]
        Chester,
        [EnumMember(Value = "Chico")]
        [Description("Chico")]
        Chico,
        [EnumMember(Value = "Childress")]
        [Description("Childress")]
        Childress,
        [EnumMember(Value = "Chillicothe")]
        [Description("Chillicothe")]
        Chillicothe,
        [EnumMember(Value = "Chilton")]
        [Description("Chilton")]
        Chilton,
        [EnumMember(Value = "China")]
        [Description("China")]
        China,
        [EnumMember(Value = "ChinaGrove")]
        [Description("China Grove")]
        ChinaGrove,
        [EnumMember(Value = "ChinaSpring")]
        [Description("China Spring")]
        ChinaSpring,
        [EnumMember(Value = "Chireno")]
        [Description("Chireno")]
        Chireno,
        [EnumMember(Value = "Christine")]
        [Description("Christine")]
        Christine,
        [EnumMember(Value = "Christoval")]
        [Description("Christoval")]
        Christoval,
        [EnumMember(Value = "ChulaVista")]
        [Description("Chula Vista")]
        ChulaVista,
        [EnumMember(Value = "CIBOL")]
        [Description("Cibolo")]
        Cibolo,
        [EnumMember(Value = "CienegasTerrace")]
        [Description("Cienegas Terrace")]
        CienegasTerrace,
        [EnumMember(Value = "CincoRanch")]
        [Description("Cinco Ranch")]
        CincoRanch,
        [EnumMember(Value = "CircleDKCEstates")]
        [Description("Circle D-KC Estates")]
        CircleDKceStates,
        [EnumMember(Value = "Cisco")]
        [Description("Cisco")]
        Cisco,
        [EnumMember(Value = "CitrusCity")]
        [Description("Citrus City")]
        CitrusCity,
        [EnumMember(Value = "Clarendon")]
        [Description("Clarendon")]
        Clarendon,
        [EnumMember(Value = "Clarksville")]
        [Description("Clarksville")]
        Clarksville,
        [EnumMember(Value = "ClarksvilleCity")]
        [Description("Clarksville City")]
        ClarksvilleCity,
        [EnumMember(Value = "Claude")]
        [Description("Claude")]
        Claude,
        [EnumMember(Value = "ClearLakeShores")]
        [Description("Clear Lake Shores")]
        ClearLakeShores,
        [EnumMember(Value = "Cleburne")]
        [Description("Cleburne")]
        Cleburne,
        [EnumMember(Value = "Cleveland")]
        [Description("Cleveland")]
        Cleveland,
        [EnumMember(Value = "Clifton")]
        [Description("Clifton")]
        Clifton,
        [EnumMember(Value = "Clint")]
        [Description("Clint")]
        Clint,
        [EnumMember(Value = "Cloverleaf")]
        [Description("Cloverleaf")]
        Cloverleaf,
        [EnumMember(Value = "Clute")]
        [Description("Clute")]
        Clute,
        [EnumMember(Value = "Clyde")]
        [Description("Clyde")]
        Clyde,
        [EnumMember(Value = "Coahoma")]
        [Description("Coahoma")]
        Coahoma,
        [EnumMember(Value = "CockrellHill")]
        [Description("Cockrell Hill")]
        CockrellHill,
        [EnumMember(Value = "CoffeeCity")]
        [Description("Coffee City")]
        CoffeeCity,
        [EnumMember(Value = "Coldspring")]
        [Description("Coldspring")]
        Coldspring,
        [EnumMember(Value = "Coleman")]
        [Description("Coleman")]
        Coleman,
        [EnumMember(Value = "CollegeStation")]
        [Description("College Station")]
        CollegeStation,
        [EnumMember(Value = "Colleyville")]
        [Description("Colleyville")]
        Colleyville,
        [EnumMember(Value = "Collinsville")]
        [Description("Collinsville")]
        Collinsville,
        [EnumMember(Value = "Colmesneil")]
        [Description("Colmesneil")]
        Colmesneil,
        [EnumMember(Value = "ColoradoAcres")]
        [Description("Colorado Acres")]
        ColoradoAcres,
        [EnumMember(Value = "ColoradoCity")]
        [Description("Colorado City")]
        ColoradoCity,
        [EnumMember(Value = "Columbus")]
        [Description("Columbus")]
        Columbus,
        [EnumMember(Value = "Comanche")]
        [Description("Comanche")]
        Comanche,
        [EnumMember(Value = "Combes")]
        [Description("Combes")]
        Combes,
        [EnumMember(Value = "Combine")]
        [Description("Combine")]
        Combine,
        [EnumMember(Value = "COMFO")]
        [Description("Comfort")]
        Comfort,
        [EnumMember(Value = "Commerce")]
        [Description("Commerce")]
        Commerce,
        [EnumMember(Value = "Como")]
        [Description("Como")]
        Como,
        [EnumMember(Value = "Concan")]
        [Description("Concan")]
        Concan,
        [EnumMember(Value = "Concepcion")]
        [Description("Concepcion")]
        Concepcion,
        [EnumMember(Value = "Conroe")]
        [Description("Conroe")]
        Conroe,
        [EnumMember(Value = "CONVE")]
        [Description("Converse")]
        Converse,
        [EnumMember(Value = "Cool")]
        [Description("Cool")]
        Cool,
        [EnumMember(Value = "Coolidge")]
        [Description("Coolidge")]
        Coolidge,
        [EnumMember(Value = "Cooper")]
        [Description("Cooper")]
        Cooper,
        [EnumMember(Value = "Coppell")]
        [Description("Coppell")]
        Coppell,
        [EnumMember(Value = "CopperCanyon")]
        [Description("Copper Canyon")]
        CopperCanyon,
        [EnumMember(Value = "CopperasCove")]
        [Description("Copperas Cove")]
        CopperasCove,
        [EnumMember(Value = "Corinth")]
        [Description("Corinth")]
        Corinth,
        [EnumMember(Value = "CorpusChristi")]
        [Description("Corpus Christi")]
        CorpusChristi,
        [EnumMember(Value = "CorralCity")]
        [Description("Corral City")]
        CorralCity,
        [EnumMember(Value = "Corrigan")]
        [Description("Corrigan")]
        Corrigan,
        [EnumMember(Value = "CORSI")]
        [Description("Corsicana")]
        Corsicana,
        [EnumMember(Value = "COST")]
        [Description("Cost")]
        Cost,
        [EnumMember(Value = "Cottonwood")]
        [Description("Cottonwood")]
        Cottonwood,
        [EnumMember(Value = "CottonwoodShores")]
        [Description("Cottonwood Shores")]
        CottonwoodShores,
        [EnumMember(Value = "Cotulla")]
        [Description("Cotulla")]
        Cotulla,
        [EnumMember(Value = "CountryAcres")]
        [Description("Country Acres")]
        CountryAcres,
        [EnumMember(Value = "Coupland")]
        [Description("Coupland")]
        Coupland,
        [EnumMember(Value = "Cove")]
        [Description("Cove")]
        Cove,
        [EnumMember(Value = "Covington")]
        [Description("Covington")]
        Covington,
        [EnumMember(Value = "Coyanosa")]
        [Description("Coyanosa")]
        Coyanosa,
        [EnumMember(Value = "CoyoteAcres")]
        [Description("Coyote Acres")]
        CoyoteAcres,
        [EnumMember(Value = "CoyoteFlats")]
        [Description("Coyote Flats")]
        CoyoteFlats,
        [EnumMember(Value = "Crandall")]
        [Description("Crandall")]
        Crandall,
        [EnumMember(Value = "Crane")]
        [Description("Crane")]
        Crane,
        [EnumMember(Value = "CranfillsGap")]
        [Description("Cranfills Gap")]
        CranfillsGap,
        [EnumMember(Value = "Crawford")]
        [Description("Crawford")]
        Crawford,
        [EnumMember(Value = "CREED")]
        [Description("Creedmoor")]
        Creedmoor,
        [EnumMember(Value = "Cresson")]
        [Description("Cresson")]
        Cresson,
        [EnumMember(Value = "Crockett")]
        [Description("Crockett")]
        Crockett,
        [EnumMember(Value = "Crosby")]
        [Description("Crosby")]
        Crosby,
        [EnumMember(Value = "Crosbyton")]
        [Description("Crosbyton")]
        Crosbyton,
        [EnumMember(Value = "CrossMountain")]
        [Description("Cross Mountain")]
        CrossMountain,
        [EnumMember(Value = "CrossPlains")]
        [Description("Cross Plains")]
        CrossPlains,
        [EnumMember(Value = "CrossRoads")]
        [Description("Cross Roads")]
        CrossRoads,
        [EnumMember(Value = "CrossTimber")]
        [Description("Cross Timber")]
        CrossTimber,
        [EnumMember(Value = "Crowell")]
        [Description("Crowell")]
        Crowell,
        [EnumMember(Value = "Crowley")]
        [Description("Crowley")]
        Crowley,
        [EnumMember(Value = "CrystalCity")]
        [Description("Crystal City")]
        CrystalCity,
        [EnumMember(Value = "CUERO")]
        [Description("Cuero")]
        Cuero,
        [EnumMember(Value = "Cuevitas")]
        [Description("Cuevitas")]
        Cuevitas,
        [EnumMember(Value = "Cumby")]
        [Description("Cumby")]
        Cumby,
        [EnumMember(Value = "Cumings")]
        [Description("Cumings")]
        Cumings,
        [EnumMember(Value = "Cuney")]
        [Description("Cuney")]
        Cuney,
        [EnumMember(Value = "Cushing")]
        [Description("Cushing")]
        Cushing,
        [EnumMember(Value = "CutandShoot")]
        [Description("Cut and Shoot")]
        CutandShoot,
        [EnumMember(Value = "DHanis")]
        [Description("D\"Hanis")]
        DHanis,
        [EnumMember(Value = "Daingerfield")]
        [Description("Daingerfield")]
        Daingerfield,
        [EnumMember(Value = "Daisetta")]
        [Description("Daisetta")]
        Daisetta,
        [EnumMember(Value = "DALE")]
        [Description("Dale")]
        Dale,
        [EnumMember(Value = "Dalhart")]
        [Description("Dalhart")]
        Dalhart,
        [EnumMember(Value = "Dallas")]
        [Description("Dallas")]
        Dallas,
        [EnumMember(Value = "DalworthingtonGardens")]
        [Description("Dalworthington Gardens")]
        DalworthingtonGardens,
        [EnumMember(Value = "Damon")]
        [Description("Damon")]
        Damon,
        [EnumMember(Value = "Danbury")]
        [Description("Danbury")]
        Danbury,
        [EnumMember(Value = "Darrouzett")]
        [Description("Darrouzett")]
        Darrouzett,
        [EnumMember(Value = "Dawson")]
        [Description("Dawson")]
        Dawson,
        [EnumMember(Value = "Dayton")]
        [Description("Dayton")]
        Dayton,
        [EnumMember(Value = "DaytonLakes")]
        [Description("Dayton Lakes")]
        DaytonLakes,
        [EnumMember(Value = "DeKalb")]
        [Description("De Kalb")]
        DeKalb,
        [EnumMember(Value = "DELEO")]
        [Description("De Leon")]
        DeLeon,
        [EnumMember(Value = "Dean")]
        [Description("Dean")]
        Dean,
        [EnumMember(Value = "Decatur")]
        [Description("Decatur")]
        Decatur,
        [EnumMember(Value = "DeCordova")]
        [Description("DeCordova")]
        DeCordova,
        [EnumMember(Value = "DeerPark")]
        [Description("Deer Park")]
        DeerPark,
        [EnumMember(Value = "DelMarHeights")]
        [Description("Del Mar Heights")]
        DelMarHeights,
        [EnumMember(Value = "DelRio")]
        [Description("Del Rio")]
        DelRio,
        [EnumMember(Value = "DelSol")]
        [Description("Del Sol")]
        DelSol,
        [EnumMember(Value = "DELVA")]
        [Description("Del Valle")]
        DelValle,
        [EnumMember(Value = "DellCity")]
        [Description("Dell City")]
        DellCity,
        [EnumMember(Value = "Delmita")]
        [Description("Delmita")]
        Delmita,
        [EnumMember(Value = "Denison")]
        [Description("Denison")]
        Denison,
        [EnumMember(Value = "Denton")]
        [Description("Denton")]
        Denton,
        [EnumMember(Value = "DenverCity")]
        [Description("Denver City")]
        DenverCity,
        [EnumMember(Value = "Deport")]
        [Description("Deport")]
        Deport,
        [EnumMember(Value = "Desdemona")]
        [Description("Desdemona")]
        Desdemona,
        [EnumMember(Value = "DeSoto")]
        [Description("DeSoto")]
        DeSoto,
        [EnumMember(Value = "Detroit")]
        [Description("Detroit")]
        Detroit,
        [EnumMember(Value = "Devers")]
        [Description("Devers")]
        Devers,
        [EnumMember(Value = "DEVIN")]
        [Description("Devine")]
        Devine,
        [EnumMember(Value = "Deweyville")]
        [Description("Deweyville")]
        Deweyville,
        [EnumMember(Value = "Diboll")]
        [Description("Diboll")]
        Diboll,
        [EnumMember(Value = "Dickens")]
        [Description("Dickens")]
        Dickens,
        [EnumMember(Value = "Dickinson")]
        [Description("Dickinson")]
        Dickinson,
        [EnumMember(Value = "Dilley")]
        [Description("Dilley")]
        Dilley,
        [EnumMember(Value = "Dimmitt")]
        [Description("Dimmitt")]
        Dimmitt,
        [EnumMember(Value = "DISH")]
        [Description("DISH")]
        DISH,
        [EnumMember(Value = "DoddCity")]
        [Description("Dodd City")]
        DoddCity,
        [EnumMember(Value = "Dodson")]
        [Description("Dodson")]
        Dodson,
        [EnumMember(Value = "Doffing")]
        [Description("Doffing")]
        Doffing,
        [EnumMember(Value = "Domino")]
        [Description("Domino")]
        Domino,
        [EnumMember(Value = "Donie")]
        [Description("Donie")]
        Donie,
        [EnumMember(Value = "Donna")]
        [Description("Donna")]
        Donna,
        [EnumMember(Value = "Doolittle")]
        [Description("Doolittle")]
        Doolittle,
        [EnumMember(Value = "Dorchester")]
        [Description("Dorchester")]
        Dorchester,
        [EnumMember(Value = "DoubleOak")]
        [Description("Double Oak")]
        DoubleOak,
        [EnumMember(Value = "Douglassville")]
        [Description("Douglassville")]
        Douglassville,
        [EnumMember(Value = "Doyle")]
        [Description("Doyle")]
        Doyle,
        [EnumMember(Value = "DRIFT")]
        [Description("Driftwood")]
        Driftwood,
        [EnumMember(Value = "DRSP")]
        [Description("Dripping Springs")]
        DrippingSprings,
        [EnumMember(Value = "DRSPR")]
        [Description("Dripping Springs Ranch Rd")]
        DrippingSpringsRanchRd,
        [EnumMember(Value = "Driscoll")]
        [Description("Driscoll")]
        Driscoll,
        [EnumMember(Value = "Dublin")]
        [Description("Dublin")]
        Dublin,
        [EnumMember(Value = "Dumas")]
        [Description("Dumas")]
        Dumas,
        [EnumMember(Value = "Duncanville")]
        [Description("Duncanville")]
        Duncanville,
        [EnumMember(Value = "EagleLake")]
        [Description("Eagle Lake")]
        EagleLake,
        [EnumMember(Value = "EaglePass")]
        [Description("Eagle Pass")]
        EaglePass,
        [EnumMember(Value = "Early")]
        [Description("Early")]
        Early,
        [EnumMember(Value = "Earth")]
        [Description("Earth")]
        Earth,
        [EnumMember(Value = "EastAltoBonito")]
        [Description("East Alto Bonito")]
        EastAltoBonito,
        [EnumMember(Value = "EastBernard")]
        [Description("East Bernard")]
        EastBernard,
        [EnumMember(Value = "EastLopez")]
        [Description("East Lopez")]
        EastLopez,
        [EnumMember(Value = "EastMountain")]
        [Description("East Mountain")]
        EastMountain,
        [EnumMember(Value = "EastTawakoni")]
        [Description("East Tawakoni")]
        EastTawakoni,
        [EnumMember(Value = "Eastland")]
        [Description("Eastland")]
        Eastland,
        [EnumMember(Value = "Easton")]
        [Description("Easton")]
        Easton,
        [EnumMember(Value = "Ector")]
        [Description("Ector")]
        Ector,
        [EnumMember(Value = "Edcouch")]
        [Description("Edcouch")]
        Edcouch,
        [EnumMember(Value = "EDDY")]
        [Description("Eddy")]
        Eddy,
        [EnumMember(Value = "Eden")]
        [Description("Eden")]
        Eden,
        [EnumMember(Value = "EdgecliffVillage")]
        [Description("Edgecliff Village")]
        EdgecliffVillage,
        [EnumMember(Value = "EdgewaterEstates")]
        [Description("Edgewater Estates")]
        EdgewaterEstates,
        [EnumMember(Value = "Edgewood")]
        [Description("Edgewood")]
        Edgewood,
        [EnumMember(Value = "Edinburg")]
        [Description("Edinburg")]
        Edinburg,
        [EnumMember(Value = "Edmonson")]
        [Description("Edmonson")]
        Edmonson,
        [EnumMember(Value = "Edna")]
        [Description("Edna")]
        Edna,
        [EnumMember(Value = "Edom")]
        [Description("Edom")]
        Edom,
        [EnumMember(Value = "Edroy")]
        [Description("Edroy")]
        Edroy,
        [EnumMember(Value = "EidsonRoad")]
        [Description("Eidson Road")]
        EidsonRoad,
        [EnumMember(Value = "ElBrazil")]
        [Description("El Brazil")]
        ElBrazil,
        [EnumMember(Value = "ElCaminoAngosto")]
        [Description("El Camino Angosto")]
        ElCaminoAngosto,
        [EnumMember(Value = "ElCampo")]
        [Description("El Campo")]
        ElCampo,
        [EnumMember(Value = "ElCastillo")]
        [Description("El Castillo")]
        ElCastillo,
        [EnumMember(Value = "ElCenizo")]
        [Description("El Cenizo")]
        ElCenizo,
        [EnumMember(Value = "ElChaparral")]
        [Description("El Chaparral")]
        ElChaparral,
        [EnumMember(Value = "ElIndio")]
        [Description("El Indio")]
        ElIndio,
        [EnumMember(Value = "ElLago")]
        [Description("El Lago")]
        ElLago,
        [EnumMember(Value = "ElMesquite")]
        [Description("El Mesquite")]
        ElMesquite,
        [EnumMember(Value = "ElPaso")]
        [Description("El Paso")]
        ElPaso,
        [EnumMember(Value = "ElQuiote")]
        [Description("El Quiote")]
        ElQuiote,
        [EnumMember(Value = "ElRanchoVela")]
        [Description("El Rancho Vela")]
        ElRanchoVela,
        [EnumMember(Value = "ElRefugio")]
        [Description("El Refugio")]
        ElRefugio,
        [EnumMember(Value = "ElSocio")]
        [Description("El Socio")]
        ElSocio,
        [EnumMember(Value = "ElToro")]
        [Description("El Toro")]
        ElToro,
        [EnumMember(Value = "Elbert")]
        [Description("Elbert")]
        Elbert,
        [EnumMember(Value = "Eldorado")]
        [Description("Eldorado")]
        Eldorado,
        [EnumMember(Value = "Electra")]
        [Description("Electra")]
        Electra,
        [EnumMember(Value = "ELGIN")]
        [Description("Elgin")]
        Elgin,
        [EnumMember(Value = "EliasFelaSolis")]
        [Description("Elias-Fela Solis")]
        EliasFelaSolis,
        [EnumMember(Value = "Elkhart")]
        [Description("Elkhart")]
        Elkhart,
        [EnumMember(Value = "Ellinger")]
        [Description("Ellinger")]
        Ellinger,
        [EnumMember(Value = "ElmCreek")]
        [Description("Elm Creek")]
        ElmCreek,
        [EnumMember(Value = "ElmMott")]
        [Description("Elm Mott")]
        ElmMott,
        [EnumMember(Value = "ELMEN")]
        [Description("Elmendorf")]
        Elmendorf,
        [EnumMember(Value = "Elmo")]
        [Description("Elmo")]
        Elmo,
        [EnumMember(Value = "Elsa")]
        [Description("Elsa")]
        Elsa,
        [EnumMember(Value = "EmeraldBay")]
        [Description("Emerald Bay")]
        EmeraldBay,
        [EnumMember(Value = "Emhouse")]
        [Description("Emhouse")]
        Emhouse,
        [EnumMember(Value = "Emory")]
        [Description("Emory")]
        Emory,
        [EnumMember(Value = "EncantadaRanchitoElCalaboz")]
        [Description("Encantada-Ranchito El Calaboz")]
        EncantadaRanchitoElCalaboz,
        [EnumMember(Value = "EnchantedOaks")]
        [Description("Enchanted Oaks")]
        EnchantedOaks,
        [EnumMember(Value = "Encinal")]
        [Description("Encinal")]
        Encinal,
        [EnumMember(Value = "Encino")]
        [Description("Encino")]
        Encino,
        [EnumMember(Value = "Ennis")]
        [Description("Ennis")]
        Ennis,
        [EnumMember(Value = "Escobares")]
        [Description("Escobares")]
        Escobares,
        [EnumMember(Value = "Estelline")]
        [Description("Estelline")]
        Estelline,
        [EnumMember(Value = "EugenioSaenz")]
        [Description("Eugenio Saenz")]
        EugenioSaenz,
        [EnumMember(Value = "Euless")]
        [Description("Euless")]
        Euless,
        [EnumMember(Value = "Eureka")]
        [Description("Eureka")]
        Eureka,
        [EnumMember(Value = "Eustace")]
        [Description("Eustace")]
        Eustace,
        [EnumMember(Value = "Evadale")]
        [Description("Evadale")]
        Evadale,
        [EnumMember(Value = "Evant")]
        [Description("Evant")]
        Evant,
        [EnumMember(Value = "Evergreen")]
        [Description("Evergreen")]
        Evergreen,
        [EnumMember(Value = "Everman")]
        [Description("Everman")]
        Everman,
        [EnumMember(Value = "Ezzell")]
        [Description("Ezzell")]
        Ezzell,
        [EnumMember(Value = "Fabens")]
        [Description("Fabens")]
        Fabens,
        [EnumMember(Value = "Fabrica")]
        [Description("Fabrica")]
        Fabrica,
        [EnumMember(Value = "FAIRO")]
        [Description("Fair Oaks Ranch")]
        FairOaksRanch,
        [EnumMember(Value = "Fairchilds")]
        [Description("Fairchilds")]
        Fairchilds,
        [EnumMember(Value = "Fairfield")]
        [Description("Fairfield")]
        Fairfield,
        [EnumMember(Value = "Fairview")]
        [Description("Fairview")]
        Fairview,
        [EnumMember(Value = "FalconHeights")]
        [Description("Falcon Heights")]
        FalconHeights,
        [EnumMember(Value = "FalconLakeEstates")]
        [Description("Falcon Lake Estates")]
        FalconLakeEstates,
        [EnumMember(Value = "FalconMesa")]
        [Description("Falcon Mesa")]
        FalconMesa,
        [EnumMember(Value = "FalconVillage")]
        [Description("Falcon Village")]
        FalconVillage,
        [EnumMember(Value = "Falconaire")]
        [Description("Falconaire")]
        Falconaire,
        [EnumMember(Value = "Falfurrias")]
        [Description("Falfurrias")]
        Falfurrias,
        [EnumMember(Value = "FallsCity")]
        [Description("Falls City")]
        FallsCity,
        [EnumMember(Value = "Falman")]
        [Description("Falman")]
        Falman,
        [EnumMember(Value = "Fannett")]
        [Description("Fannett")]
        Fannett,
        [EnumMember(Value = "Fannin")]
        [Description("Fannin")]
        Fannin,
        [EnumMember(Value = "FarmersBranch")]
        [Description("Farmers Branch")]
        FarmersBranch,
        [EnumMember(Value = "Farmersville")]
        [Description("Farmersville")]
        Farmersville,
        [EnumMember(Value = "Farwell")]
        [Description("Farwell")]
        Farwell,
        [EnumMember(Value = "Fate")]
        [Description("Fate")]
        Fate,
        [EnumMember(Value = "Fayetteville")]
        [Description("Fayetteville")]
        Fayetteville,
        [EnumMember(Value = "Faysville")]
        [Description("Faysville")]
        Faysville,
        [EnumMember(Value = "FENTR")]
        [Description("Fentress")]
        Fentress,
        [EnumMember(Value = "FernandoSalinas")]
        [Description("Fernando Salinas")]
        FernandoSalinas,
        [EnumMember(Value = "Ferris")]
        [Description("Ferris")]
        Ferris,
        [EnumMember(Value = "FifthStreet")]
        [Description("Fifth Street")]
        FifthStreet,
        [EnumMember(Value = "FISCH")]
        [Description("Fischer")]
        Fischer,
        [EnumMember(Value = "Flat")]
        [Description("Flat")]
        Flat,
        [EnumMember(Value = "FLATO")]
        [Description("Flatonia")]
        Flatonia,
        [EnumMember(Value = "FlordelRio")]
        [Description("Flor del Rio")]
        FlordelRio,
        [EnumMember(Value = "Florence")]
        [Description("Florence")]
        Florence,
        [EnumMember(Value = "FLORE")]
        [Description("Floresville")]
        Floresville,
        [EnumMember(Value = "Flowella")]
        [Description("Flowella")]
        Flowella,
        [EnumMember(Value = "FlowerMound")]
        [Description("Flower Mound")]
        FlowerMound,
        [EnumMember(Value = "Floydada")]
        [Description("Floydada")]
        Floydada,
        [EnumMember(Value = "Follett")]
        [Description("Follett")]
        Follett,
        [EnumMember(Value = "ForestHill")]
        [Description("Forest Hill")]
        ForestHill,
        [EnumMember(Value = "Forney")]
        [Description("Forney")]
        Forney,
        [EnumMember(Value = "Forsan")]
        [Description("Forsan")]
        Forsan,
        [EnumMember(Value = "FortBliss")]
        [Description("Fort Bliss")]
        FortBliss,
        [EnumMember(Value = "FortClarkSprings")]
        [Description("Fort Clark Springs")]
        FortClarkSprings,
        [EnumMember(Value = "FortDavis")]
        [Description("Fort Davis")]
        FortDavis,
        [EnumMember(Value = "FortHancock")]
        [Description("Fort Hancock")]
        FortHancock,
        [EnumMember(Value = "FortHood")]
        [Description("Fort Hood")]
        FortHood,
        [EnumMember(Value = "FortStockton")]
        [Description("Fort Stockton")]
        FortStockton,
        [EnumMember(Value = "FortWorth")]
        [Description("Fort Worth")]
        FortWorth,
        [EnumMember(Value = "FourCorners")]
        [Description("Four Corners")]
        FourCorners,
        [EnumMember(Value = "FourPoints")]
        [Description("Four Points")]
        FourPoints,
        [EnumMember(Value = "Fowlerton")]
        [Description("Fowlerton")]
        Fowlerton,
        [EnumMember(Value = "Francitas")]
        [Description("Francitas")]
        Francitas,
        [EnumMember(Value = "Franklin")]
        [Description("Franklin")]
        Franklin,
        [EnumMember(Value = "Frankston")]
        [Description("Frankston")]
        Frankston,
        [EnumMember(Value = "Fredericksburg")]
        [Description("Fredericksburg")]
        Fredericksburg,
        [EnumMember(Value = "Freeport")]
        [Description("Freeport")]
        Freeport,
        [EnumMember(Value = "Freer")]
        [Description("Freer")]
        Freer,
        [EnumMember(Value = "Fresno")]
        [Description("Fresno")]
        Fresno,
        [EnumMember(Value = "Friendswood")]
        [Description("Friendswood")]
        Friendswood,
        [EnumMember(Value = "Friona")]
        [Description("Friona")]
        Friona,
        [EnumMember(Value = "Frisco")]
        [Description("Frisco")]
        Frisco,
        [EnumMember(Value = "Fritch")]
        [Description("Fritch")]
        Fritch,
        [EnumMember(Value = "Fronton")]
        [Description("Fronton")]
        Fronton,
        [EnumMember(Value = "FrontonRanchettes")]
        [Description("Fronton Ranchettes")]
        FrontonRanchettes,
        [EnumMember(Value = "Frost")]
        [Description("Frost")]
        Frost,
        [EnumMember(Value = "Fruitvale")]
        [Description("Fruitvale")]
        Fruitvale,
        [EnumMember(Value = "Fulshear")]
        [Description("Fulshear")]
        Fulshear,
        [EnumMember(Value = "Fulton")]
        [Description("Fulton")]
        Fulton,
        [EnumMember(Value = "Gail")]
        [Description("Gail")]
        Gail,
        [EnumMember(Value = "Gainesville")]
        [Description("Gainesville")]
        Gainesville,
        [EnumMember(Value = "GalenaPark")]
        [Description("Galena Park")]
        GalenaPark,
        [EnumMember(Value = "Gallatin")]
        [Description("Gallatin")]
        Gallatin,
        [EnumMember(Value = "Galveston")]
        [Description("Galveston")]
        Galveston,
        [EnumMember(Value = "Ganado")]
        [Description("Ganado")]
        Ganado,
        [EnumMember(Value = "Garceno")]
        [Description("Garceno")]
        Garceno,
        [EnumMember(Value = "Garciasville")]
        [Description("Garciasville")]
        Garciasville,
        [EnumMember(Value = "GardenCity")]
        [Description("Garden City")]
        GardenCity,
        [EnumMember(Value = "GARDE")]
        [Description("Garden Ridge")]
        GardenRidge,
        [EnumMember(Value = "Gardendale")]
        [Description("Gardendale")]
        Gardendale,
        [EnumMember(Value = "Garfield")]
        [Description("Garfield")]
        Garfield,
        [EnumMember(Value = "Garland")]
        [Description("Garland")]
        Garland,
        [EnumMember(Value = "Garrett")]
        [Description("Garrett")]
        Garrett,
        [EnumMember(Value = "Garrison")]
        [Description("Garrison")]
        Garrison,
        [EnumMember(Value = "Garwood")]
        [Description("Garwood")]
        Garwood,
        [EnumMember(Value = "GaryCity")]
        [Description("Gary City")]
        GaryCity,
        [EnumMember(Value = "GarzaSalinasII")]
        [Description("Garza-Salinas II")]
        GarzaSalinasII,
        [EnumMember(Value = "Gatesville")]
        [Description("Gatesville")]
        Gatesville,
        [EnumMember(Value = "Gause")]
        [Description("Gause")]
        Gause,
        [EnumMember(Value = "GeorgeWest")]
        [Description("George West")]
        GeorgeWest,
        [EnumMember(Value = "Georgetown")]
        [Description("Georgetown")]
        Georgetown,
        [EnumMember(Value = "GERON")]
        [Description("Geronimo")]
        Geronimo,
        [EnumMember(Value = "Gholson")]
        [Description("Gholson")]
        Gholson,
        [EnumMember(Value = "Giddings")]
        [Description("Giddings")]
        Giddings,
        [EnumMember(Value = "Gilchrist")]
        [Description("Gilchrist")]
        Gilchrist,
        [EnumMember(Value = "GILLE")]
        [Description("Gillett")]
        Gillett,
        [EnumMember(Value = "Gilmer")]
        [Description("Gilmer")]
        Gilmer,
        [EnumMember(Value = "Girard")]
        [Description("Girard")]
        Girard,
        [EnumMember(Value = "Gladewater")]
        [Description("Gladewater")]
        Gladewater,
        [EnumMember(Value = "GlenRose")]
        [Description("Glen Rose")]
        GlenRose,
        [EnumMember(Value = "GlennHeights")]
        [Description("Glenn Heights")]
        GlennHeights,
        [EnumMember(Value = "Glidden")]
        [Description("Glidden")]
        Glidden,
        [EnumMember(Value = "Godley")]
        [Description("Godley")]
        Godley,
        [EnumMember(Value = "Goldsmith")]
        [Description("Goldsmith")]
        Goldsmith,
        [EnumMember(Value = "GOLDT")]
        [Description("Goldthwaite")]
        Goldthwaite,
        [EnumMember(Value = "Goliad")]
        [Description("Goliad")]
        Goliad,
        [EnumMember(Value = "Golinda")]
        [Description("Golinda")]
        Golinda,
        [EnumMember(Value = "GONZA")]
        [Description("Gonzales")]
        Gonzales,
        [EnumMember(Value = "Goodlow")]
        [Description("Goodlow")]
        Goodlow,
        [EnumMember(Value = "Goodrich")]
        [Description("Goodrich")]
        Goodrich,
        [EnumMember(Value = "Gordon")]
        [Description("Gordon")]
        Gordon,
        [EnumMember(Value = "Goree")]
        [Description("Goree")]
        Goree,
        [EnumMember(Value = "Gorman")]
        [Description("Gorman")]
        Gorman,
        [EnumMember(Value = "Graford")]
        [Description("Graford")]
        Graford,
        [EnumMember(Value = "Graham")]
        [Description("Graham")]
        Graham,
        [EnumMember(Value = "Granbury")]
        [Description("Granbury")]
        Granbury,
        [EnumMember(Value = "GrandAcres")]
        [Description("Grand Acres")]
        GrandAcres,
        [EnumMember(Value = "GrandPrairie")]
        [Description("Grand Prairie")]
        GrandPrairie,
        [EnumMember(Value = "GrandSaline")]
        [Description("Grand Saline")]
        GrandSaline,
        [EnumMember(Value = "Grandfalls")]
        [Description("Grandfalls")]
        Grandfalls,
        [EnumMember(Value = "Grandview")]
        [Description("Grandview")]
        Grandview,
        [EnumMember(Value = "Granger")]
        [Description("Granger")]
        Granger,
        [EnumMember(Value = "GRANI")]
        [Description("Granite Shoals")]
        GraniteShoals,
        [EnumMember(Value = "Granjeno")]
        [Description("Granjeno")]
        Granjeno,
        [EnumMember(Value = "GrapeCreek")]
        [Description("Grape Creek")]
        GrapeCreek,
        [EnumMember(Value = "Grapeland")]
        [Description("Grapeland")]
        Grapeland,
        [EnumMember(Value = "Grapevine")]
        [Description("Grapevine")]
        Grapevine,
        [EnumMember(Value = "GraysPrairie")]
        [Description("Grays Prairie")]
        GraysPrairie,
        [EnumMember(Value = "Greatwood")]
        [Description("Greatwood")]
        Greatwood,
        [EnumMember(Value = "GreenValleyFarms")]
        [Description("Green Valley Farms")]
        GreenValleyFarms,
        [EnumMember(Value = "Greenville")]
        [Description("Greenville")]
        Greenville,
        [EnumMember(Value = "Gregory")]
        [Description("Gregory")]
        Gregory,
        [EnumMember(Value = "GreyForest")]
        [Description("Grey Forest")]
        GreyForest,
        [EnumMember(Value = "Groesbeck")]
        [Description("Groesbeck")]
        Groesbeck,
        [EnumMember(Value = "Groom")]
        [Description("Groom")]
        Groom,
        [EnumMember(Value = "Groves")]
        [Description("Groves")]
        Groves,
        [EnumMember(Value = "Groveton")]
        [Description("Groveton")]
        Groveton,
        [EnumMember(Value = "Gruver")]
        [Description("Gruver")]
        Gruver,
        [EnumMember(Value = "GuadalupeGuerra")]
        [Description("Guadalupe Guerra")]
        GuadalupeGuerra,
        [EnumMember(Value = "Guerra")]
        [Description("Guerra")]
        Guerra,
        [EnumMember(Value = "GunBarrelCity")]
        [Description("Gun Barrel City")]
        GunBarrelCity,
        [EnumMember(Value = "Gunter")]
        [Description("Gunter")]
        Gunter,
        [EnumMember(Value = "Gustine")]
        [Description("Gustine")]
        Gustine,
        [EnumMember(Value = "Guthrie")]
        [Description("Guthrie")]
        Guthrie,
        [EnumMember(Value = "Gutierrez")]
        [Description("Gutierrez")]
        Gutierrez,
        [EnumMember(Value = "HCuellarEstates")]
        [Description("H. Cuellar Estates")]
        HCuellarEstates,
        [EnumMember(Value = "Hackberry")]
        [Description("Hackberry")]
        Hackberry,
        [EnumMember(Value = "HaleCenter")]
        [Description("Hale Center")]
        HaleCenter,
        [EnumMember(Value = "HALLE")]
        [Description("Hallettsville")]
        Hallettsville,
        [EnumMember(Value = "Hallsburg")]
        [Description("Hallsburg")]
        Hallsburg,
        [EnumMember(Value = "Hallsville")]
        [Description("Hallsville")]
        Hallsville,
        [EnumMember(Value = "HaltomCity")]
        [Description("Haltom City")]
        HaltomCity,
        [EnumMember(Value = "Hamilton")]
        [Description("Hamilton")]
        Hamilton,
        [EnumMember(Value = "Hamlin")]
        [Description("Hamlin")]
        Hamlin,
        [EnumMember(Value = "Happy")]
        [Description("Happy")]
        Happy,
        [EnumMember(Value = "Hardin")]
        [Description("Hardin")]
        Hardin,
        [EnumMember(Value = "Hargill")]
        [Description("Hargill")]
        Hargill,
        [EnumMember(Value = "HarkerHeights")]
        [Description("Harker Heights")]
        HarkerHeights,
        [EnumMember(Value = "Harlingen")]
        [Description("Harlingen")]
        Harlingen,
        [EnumMember(Value = "HARPE")]
        [Description("Harper")]
        Harper,
        [EnumMember(Value = "Hart")]
        [Description("Hart")]
        Hart,
        [EnumMember(Value = "Hartley")]
        [Description("Hartley")]
        Hartley,
        [EnumMember(Value = "HARWO")]
        [Description("Harwood")]
        Harwood,
        [EnumMember(Value = "Haskell")]
        [Description("Haskell")]
        Haskell,
        [EnumMember(Value = "Haslet")]
        [Description("Haslet")]
        Haslet,
        [EnumMember(Value = "Havana")]
        [Description("Havana")]
        Havana,
        [EnumMember(Value = "HawkCove")]
        [Description("Hawk Cove")]
        HawkCove,
        [EnumMember(Value = "Hawkins")]
        [Description("Hawkins")]
        Hawkins,
        [EnumMember(Value = "Hawley")]
        [Description("Hawley")]
        Hawley,
        [EnumMember(Value = "Hays")]
        [Description("Hays")]
        Hays,
        [EnumMember(Value = "Hearne")]
        [Description("Hearne")]
        Hearne,
        [EnumMember(Value = "Heath")]
        [Description("Heath")]
        Heath,
        [EnumMember(Value = "Hebbronville")]
        [Description("Hebbronville")]
        Hebbronville,
        [EnumMember(Value = "Hebron")]
        [Description("Hebron")]
        Hebron,
        [EnumMember(Value = "Hedley")]
        [Description("Hedley")]
        Hedley,
        [EnumMember(Value = "HedwigVillage")]
        [Description("Hedwig Village")]
        HedwigVillage,
        [EnumMember(Value = "Heidelberg")]
        [Description("Heidelberg")]
        Heidelberg,
        [EnumMember(Value = "Heidenheimer")]
        [Description("Heidenheimer")]
        Heidenheimer,
        [EnumMember(Value = "HELOT")]
        [Description("Helotes")]
        Helotes,
        [EnumMember(Value = "Hemphill")]
        [Description("Hemphill")]
        Hemphill,
        [EnumMember(Value = "Hempstead")]
        [Description("Hempstead")]
        Hempstead,
        [EnumMember(Value = "Henderson")]
        [Description("Henderson")]
        Henderson,
        [EnumMember(Value = "HENLY")]
        [Description("Henly")]
        Henly,
        [EnumMember(Value = "Henrietta")]
        [Description("Henrietta")]
        Henrietta,
        [EnumMember(Value = "Hereford")]
        [Description("Hereford")]
        Hereford,
        [EnumMember(Value = "Hermleigh")]
        [Description("Hermleigh")]
        Hermleigh,
        [EnumMember(Value = "Hewitt")]
        [Description("Hewitt")]
        Hewitt,
        [EnumMember(Value = "HickoryCreek")]
        [Description("Hickory Creek")]
        HickoryCreek,
        [EnumMember(Value = "Hico")]
        [Description("Hico")]
        Hico,
        [EnumMember(Value = "Hidalgo")]
        [Description("Hidalgo")]
        Hidalgo,
        [EnumMember(Value = "Hideaway")]
        [Description("Hideaway")]
        Hideaway,
        [EnumMember(Value = "Higgins")]
        [Description("Higgins")]
        Higgins,
        [EnumMember(Value = "HIGHL")]
        [Description("Highland Haven")]
        HighlandHaven,
        [EnumMember(Value = "HighlandPark")]
        [Description("Highland Park")]
        HighlandPark,
        [EnumMember(Value = "HighlandVillage")]
        [Description("Highland Village")]
        HighlandVillage,
        [EnumMember(Value = "Highlands")]
        [Description("Highlands")]
        Highlands,
        [EnumMember(Value = "HILLC")]
        [Description("Hill Country Village")]
        HillCountryVillage,
        [EnumMember(Value = "Hillcrest")]
        [Description("Hillcrest")]
        Hillcrest,
        [EnumMember(Value = "Hillsboro")]
        [Description("Hillsboro")]
        Hillsboro,
        [EnumMember(Value = "HillsideAcres")]
        [Description("Hillside Acres")]
        HillsideAcres,
        [EnumMember(Value = "Hilltop")]
        [Description("Hilltop")]
        Hilltop,
        [EnumMember(Value = "HilltopLakes")]
        [Description("Hilltop Lakes")]
        HilltopLakes,
        [EnumMember(Value = "HilshireVillage")]
        [Description("Hilshire Village")]
        HilshireVillage,
        [EnumMember(Value = "Hitchcock")]
        [Description("Hitchcock")]
        Hitchcock,
        [EnumMember(Value = "HOBSO")]
        [Description("Hobson")]
        Hobson,
        [EnumMember(Value = "HolidayBeach")]
        [Description("Holiday Beach")]
        HolidayBeach,
        [EnumMember(Value = "HolidayLakes")]
        [Description("Holiday Lakes")]
        HolidayLakes,
        [EnumMember(Value = "Holland")]
        [Description("Holland")]
        Holland,
        [EnumMember(Value = "Holliday")]
        [Description("Holliday")]
        Holliday,
        [EnumMember(Value = "HollyLakeRanch")]
        [Description("Holly Lake Ranch")]
        HollyLakeRanch,
        [EnumMember(Value = "HOLLY")]
        [Description("Hollywood Park")]
        HollywoodPark,
        [EnumMember(Value = "HomesteadMeadowsNorth")]
        [Description("Homestead Meadows North")]
        HomesteadMeadowsNorth,
        [EnumMember(Value = "HomesteadMeadowsSouth")]
        [Description("Homestead Meadows South")]
        HomesteadMeadowsSouth,
        [EnumMember(Value = "HONDO")]
        [Description("Hondo")]
        Hondo,
        [EnumMember(Value = "HoneyGrove")]
        [Description("Honey Grove")]
        HoneyGrove,
        [EnumMember(Value = "Hooks")]
        [Description("Hooks")]
        Hooks,
        [EnumMember(Value = "HorizonCity")]
        [Description("Horizon City")]
        HorizonCity,
        [EnumMember(Value = "HornsbyBend")]
        [Description("Hornsby Bend")]
        HornsbyBend,
        [EnumMember(Value = "HORSE")]
        [Description("Horseshoe Bay")]
        HorseshoeBay,
        [EnumMember(Value = "HorseshoeBend")]
        [Description("Horseshoe Bend")]
        HorseshoeBend,
        [EnumMember(Value = "Houston")]
        [Description("Houston")]
        Houston,
        [EnumMember(Value = "Howardwick")]
        [Description("Howardwick")]
        Howardwick,
        [EnumMember(Value = "Howe")]
        [Description("Howe")]
        Howe,
        [EnumMember(Value = "Hubbard")]
        [Description("Hubbard")]
        Hubbard,
        [EnumMember(Value = "Hudson")]
        [Description("Hudson")]
        Hudson,
        [EnumMember(Value = "HudsonBend")]
        [Description("Hudson Bend")]
        HudsonBend,
        [EnumMember(Value = "HudsonOaks")]
        [Description("Hudson Oaks")]
        HudsonOaks,
        [EnumMember(Value = "HughesSprings")]
        [Description("Hughes Springs")]
        HughesSprings,
        [EnumMember(Value = "Hull")]
        [Description("Hull")]
        Hull,
        [EnumMember(Value = "Humble")]
        [Description("Humble")]
        Humble,
        [EnumMember(Value = "Hungerford")]
        [Description("Hungerford")]
        Hungerford,
        [EnumMember(Value = "Hunt")]
        [Description("Hunt")]
        Hunt,
        [EnumMember(Value = "HuntersCreekVillage")]
        [Description("Hunters Creek Village")]
        HuntersCreekVillage,
        [EnumMember(Value = "Huntington")]
        [Description("Huntington")]
        Huntington,
        [EnumMember(Value = "HUNTS")]
        [Description("Huntsville")]
        Huntsville,
        [EnumMember(Value = "Hurst")]
        [Description("Hurst")]
        Hurst,
        [EnumMember(Value = "Hutchins")]
        [Description("Hutchins")]
        Hutchins,
        [EnumMember(Value = "Hutto")]
        [Description("Hutto")]
        Hutto,
        [EnumMember(Value = "Huxley")]
        [Description("Huxley")]
        Huxley,
        [EnumMember(Value = "Hye")]
        [Description("Hye")]
        Hye,
        [EnumMember(Value = "Iago")]
        [Description("Iago")]
        Iago,
        [EnumMember(Value = "Idalou")]
        [Description("Idalou")]
        Idalou,
        [EnumMember(Value = "IglesiaAntigua")]
        [Description("Iglesia Antigua")]
        IglesiaAntigua,
        [EnumMember(Value = "Impact")]
        [Description("Impact")]
        Impact,
        [EnumMember(Value = "Imperial")]
        [Description("Imperial")]
        Imperial,
        [EnumMember(Value = "IndianGap")]
        [Description("Indian Gap")]
        IndianGap,
        [EnumMember(Value = "IndianHills")]
        [Description("Indian Hills")]
        IndianHills,
        [EnumMember(Value = "IndianLake")]
        [Description("Indian Lake")]
        IndianLake,
        [EnumMember(Value = "IndianSprings")]
        [Description("Indian Springs")]
        IndianSprings,
        [EnumMember(Value = "Indio")]
        [Description("Indio")]
        Indio,
        [EnumMember(Value = "Industry")]
        [Description("Industry")]
        Industry,
        [EnumMember(Value = "Inez")]
        [Description("Inez")]
        Inez,
        [EnumMember(Value = "Ingleside")]
        [Description("Ingleside")]
        Ingleside,
        [EnumMember(Value = "InglesideontheBay")]
        [Description("Ingleside on the Bay")]
        InglesideontheBay,
        [EnumMember(Value = "Ingram")]
        [Description("Ingram")]
        Ingram,
        [EnumMember(Value = "Iola")]
        [Description("Iola")]
        Iola,
        [EnumMember(Value = "IowaColony")]
        [Description("Iowa Colony")]
        IowaColony,
        [EnumMember(Value = "IowaPark")]
        [Description("Iowa Park")]
        IowaPark,
        [EnumMember(Value = "Iraan")]
        [Description("Iraan")]
        Iraan,
        [EnumMember(Value = "Iredell")]
        [Description("Iredell")]
        Iredell,
        [EnumMember(Value = "Irving")]
        [Description("Irving")]
        Irving,
        [EnumMember(Value = "Italy")]
        [Description("Italy")]
        Italy,
        [EnumMember(Value = "Itasca")]
        [Description("Itasca")]
        Itasca,
        [EnumMember(Value = "Ivanhoe")]
        [Description("Ivanhoe")]
        Ivanhoe,
        [EnumMember(Value = "JacintoCity")]
        [Description("Jacinto City")]
        JacintoCity,
        [EnumMember(Value = "Jacksboro")]
        [Description("Jacksboro")]
        Jacksboro,
        [EnumMember(Value = "Jacksonville")]
        [Description("Jacksonville")]
        Jacksonville,
        [EnumMember(Value = "JamaicaBeach")]
        [Description("Jamaica Beach")]
        JamaicaBeach,
        [EnumMember(Value = "JardindeSanJulian")]
        [Description("Jardin de San Julian")]
        JardindeSanJulian,
        [EnumMember(Value = "Jarrell")]
        [Description("Jarrell")]
        Jarrell,
        [EnumMember(Value = "Jasper")]
        [Description("Jasper")]
        Jasper,
        [EnumMember(Value = "Jayton")]
        [Description("Jayton")]
        Jayton,
        [EnumMember(Value = "Jefferson")]
        [Description("Jefferson")]
        Jefferson,
        [EnumMember(Value = "JerseyVillage")]
        [Description("Jersey Village")]
        JerseyVillage,
        [EnumMember(Value = "Jewett")]
        [Description("Jewett")]
        Jewett,
        [EnumMember(Value = "JFVillarreal")]
        [Description("JF Villarreal")]
        JFVillarreal,
        [EnumMember(Value = "Joaquin")]
        [Description("Joaquin")]
        Joaquin,
        [EnumMember(Value = "JOHNS")]
        [Description("Johnson City")]
        JohnsonCity,
        [EnumMember(Value = "Jolly")]
        [Description("Jolly")]
        Jolly,
        [EnumMember(Value = "JonesCreek")]
        [Description("Jones Creek")]
        JonesCreek,
        [EnumMember(Value = "Jonesboro")]
        [Description("Jonesboro")]
        Jonesboro,
        [EnumMember(Value = "JONES")]
        [Description("Jonestown")]
        Jonestown,
        [EnumMember(Value = "Josephine")]
        [Description("Josephine")]
        Josephine,
        [EnumMember(Value = "Joshua")]
        [Description("Joshua")]
        Joshua,
        [EnumMember(Value = "JOURD")]
        [Description("Jourdanton")]
        Jourdanton,
        [EnumMember(Value = "Juarez")]
        [Description("Juarez")]
        Juarez,
        [EnumMember(Value = "Junction")]
        [Description("Junction")]
        Junction,
        [EnumMember(Value = "Justin")]
        [Description("Justin")]
        Justin,
        [EnumMember(Value = "KBarRanch")]
        [Description("K-Bar Ranch")]
        KBarRanch,
        [EnumMember(Value = "KARNE")]
        [Description("Karnes City")]
        KarnesCity,
        [EnumMember(Value = "Katy")]
        [Description("Katy")]
        Katy,
        [EnumMember(Value = "Kaufman")]
        [Description("Kaufman")]
        Kaufman,
        [EnumMember(Value = "Keene")]
        [Description("Keene")]
        Keene,
        [EnumMember(Value = "Keller")]
        [Description("Keller")]
        Keller,
        [EnumMember(Value = "Kemah")]
        [Description("Kemah")]
        Kemah,
        [EnumMember(Value = "Kemp")]
        [Description("Kemp")]
        Kemp,
        [EnumMember(Value = "Kempner")]
        [Description("Kempner")]
        Kempner,
        [EnumMember(Value = "KENDA")]
        [Description("Kendalia")]
        Kendalia,
        [EnumMember(Value = "Kendleton")]
        [Description("Kendleton")]
        Kendleton,
        [EnumMember(Value = "KENED")]
        [Description("Kenedy")]
        Kenedy,
        [EnumMember(Value = "Kenefick")]
        [Description("Kenefick")]
        Kenefick,
        [EnumMember(Value = "Kennard")]
        [Description("Kennard")]
        Kennard,
        [EnumMember(Value = "Kennedale")]
        [Description("Kennedale")]
        Kennedale,
        [EnumMember(Value = "Kerens")]
        [Description("Kerens")]
        Kerens,
        [EnumMember(Value = "Kermit")]
        [Description("Kermit")]
        Kermit,
        [EnumMember(Value = "KERRV")]
        [Description("Kerrville")]
        Kerrville,
        [EnumMember(Value = "Kilgore")]
        [Description("Kilgore")]
        Kilgore,
        [EnumMember(Value = "Killeen")]
        [Description("Killeen")]
        Killeen,
        [EnumMember(Value = "KNGSB")]
        [Description("Kingsbury")]
        Kingsbury,
        [EnumMember(Value = "KNGSL")]
        [Description("Kingsland")]
        Kingsland,
        [EnumMember(Value = "Kingsville")]
        [Description("Kingsville")]
        Kingsville,
        [EnumMember(Value = "KIRBY")]
        [Description("Kirby")]
        Kirby,
        [EnumMember(Value = "Kirbyville")]
        [Description("Kirbyville")]
        Kirbyville,
        [EnumMember(Value = "Kirvin")]
        [Description("Kirvin")]
        Kirvin,
        [EnumMember(Value = "KNIPP")]
        [Description("Knippa")]
        Knippa,
        [EnumMember(Value = "Knollwood")]
        [Description("Knollwood")]
        Knollwood,
        [EnumMember(Value = "KnoxCity")]
        [Description("Knox City")]
        KnoxCity,
        [EnumMember(Value = "Kopperl")]
        [Description("Kopperl")]
        Kopperl,
        [EnumMember(Value = "Kosse")]
        [Description("Kosse")]
        Kosse,
        [EnumMember(Value = "Kountze")]
        [Description("Kountze")]
        Kountze,
        [EnumMember(Value = "Kress")]
        [Description("Kress")]
        Kress,
        [EnumMember(Value = "Krugerville")]
        [Description("Krugerville")]
        Krugerville,
        [EnumMember(Value = "Krum")]
        [Description("Krum")]
        Krum,
        [EnumMember(Value = "Kurten")]
        [Description("Kurten")]
        Kurten,
        [EnumMember(Value = "KYLE")]
        [Description("Kyle")]
        Kyle,
        [EnumMember(Value = "LaBlanca")]
        [Description("La Blanca")]
        LaBlanca,
        [EnumMember(Value = "LaCarla")]
        [Description("La Carla")]
        LaCarla,
        [EnumMember(Value = "LaCasita")]
        [Description("La Casita")]
        LaCasita,
        [EnumMember(Value = "LaChuparosa")]
        [Description("La Chuparosa")]
        LaChuparosa,
        [EnumMember(Value = "LaComa")]
        [Description("La Coma")]
        LaComa,
        [EnumMember(Value = "LaEscondida")]
        [Description("La Escondida")]
        LaEscondida,
        [EnumMember(Value = "LaEsperanza")]
        [Description("La Esperanza")]
        LaEsperanza,
        [EnumMember(Value = "LaFeria")]
        [Description("La Feria")]
        LaFeria,
        [EnumMember(Value = "LaFeriaNorth")]
        [Description("La Feria North")]
        LaFeriaNorth,
        [EnumMember(Value = "LAGRA")]
        [Description("La Grange")]
        LaGrange,
        [EnumMember(Value = "LaGrulla")]
        [Description("La Grulla")]
        LaGrulla,
        [EnumMember(Value = "LaHoma")]
        [Description("La Homa")]
        LaHoma,
        [EnumMember(Value = "LaJoya")]
        [Description("La Joya")]
        LaJoya,
        [EnumMember(Value = "LaLomadeFalcon")]
        [Description("La Loma de Falcon")]
        LaLomadeFalcon,
        [EnumMember(Value = "LaMarque")]
        [Description("La Marque")]
        LaMarque,
        [EnumMember(Value = "LaMinita")]
        [Description("La Minita")]
        LaMinita,
        [EnumMember(Value = "LaPaloma")]
        [Description("La Paloma")]
        LaPaloma,
        [EnumMember(Value = "LaPalomaAddition")]
        [Description("La Paloma Addition")]
        LaPalomaAddition,
        [EnumMember(Value = "LaPalomaRanchettes")]
        [Description("La Paloma Ranchettes")]
        LaPalomaRanchettes,
        [EnumMember(Value = "LaPalomaLostCreek")]
        [Description("La Paloma-Lost Creek")]
        LaPalomaLostCreek,
        [EnumMember(Value = "LaPorte")]
        [Description("La Porte")]
        LaPorte,
        [EnumMember(Value = "LaPresa")]
        [Description("La Presa")]
        LaPresa,
        [EnumMember(Value = "LaPryor")]
        [Description("La Pryor")]
        LaPryor,
        [EnumMember(Value = "LaPuerta")]
        [Description("La Puerta")]
        LaPuerta,
        [EnumMember(Value = "LaRosita")]
        [Description("La Rosita")]
        LaRosita,
        [EnumMember(Value = "LaTinaRanch")]
        [Description("La Tina Ranch")]
        LaTinaRanch,
        [EnumMember(Value = "LAVER")]
        [Description("La Vernia")]
        LaVernia,
        [EnumMember(Value = "LaVictoria")]
        [Description("La Victoria")]
        LaVictoria,
        [EnumMember(Value = "LaVilla")]
        [Description("La Villa")]
        LaVilla,
        [EnumMember(Value = "LaWard")]
        [Description("La Ward")]
        LaWard,
        [EnumMember(Value = "LacklandAFB")]
        [Description("Lackland AFB")]
        LacklandAFB,
        [EnumMember(Value = "LaCoste")]
        [Description("LaCoste")]
        LaCoste,
        [EnumMember(Value = "LacyLakeview")]
        [Description("Lacy-Lakeview")]
        LacyLakeview,
        [EnumMember(Value = "Ladonia")]
        [Description("Ladonia")]
        Ladonia,
        [EnumMember(Value = "Lago")]
        [Description("Lago")]
        Lago,
        [EnumMember(Value = "LAGOV")]
        [Description("Lago Vista")]
        LagoVista,
        [EnumMember(Value = "LagunaHeights")]
        [Description("Laguna Heights")]
        LagunaHeights,
        [EnumMember(Value = "LagunaPark")]
        [Description("Laguna Park")]
        LagunaPark,
        [EnumMember(Value = "LagunaSeca")]
        [Description("Laguna Seca")]
        LagunaSeca,
        [EnumMember(Value = "LagunaVista")]
        [Description("Laguna Vista")]
        LagunaVista,
        [EnumMember(Value = "LakeBridgeport")]
        [Description("Lake Bridgeport")]
        LakeBridgeport,
        [EnumMember(Value = "LakeBrownwood")]
        [Description("Lake Brownwood")]
        LakeBrownwood,
        [EnumMember(Value = "LakeBryan")]
        [Description("Lake Bryan")]
        LakeBryan,
        [EnumMember(Value = "LakeCherokee")]
        [Description("Lake Cherokee")]
        LakeCherokee,
        [EnumMember(Value = "LakeCity")]
        [Description("Lake City")]
        LakeCity,
        [EnumMember(Value = "LakeColoradoCity")]
        [Description("Lake Colorado City")]
        LakeColoradoCity,
        [EnumMember(Value = "LakeDallas")]
        [Description("Lake Dallas")]
        LakeDallas,
        [EnumMember(Value = "LakeDunlap")]
        [Description("Lake Dunlap")]
        LakeDunlap,
        [EnumMember(Value = "LakeJackson")]
        [Description("Lake Jackson")]
        LakeJackson,
        [EnumMember(Value = "LakeKiowa")]
        [Description("Lake Kiowa")]
        LakeKiowa,
        [EnumMember(Value = "LakeMedinaShores")]
        [Description("Lake Medina Shores")]
        LakeMedinaShores,
        [EnumMember(Value = "LakeMeredithEstates")]
        [Description("Lake Meredith Estates")]
        LakeMeredithEstates,
        [EnumMember(Value = "LakeTanglewood")]
        [Description("Lake Tanglewood")]
        LakeTanglewood,
        [EnumMember(Value = "LakeView")]
        [Description("Lake View")]
        LakeView,
        [EnumMember(Value = "LakeWorth")]
        [Description("Lake Worth")]
        LakeWorth,
        [EnumMember(Value = "LAKEH")]
        [Description("Lakehills")]
        Lakehills,
        [EnumMember(Value = "Lakeport")]
        [Description("Lakeport")]
        Lakeport,
        [EnumMember(Value = "LakeshoreGardensHiddenAcres")]
        [Description("Lakeshore Gardens-Hidden Acres")]
        LakeshoreGardensHiddenAcres,
        [EnumMember(Value = "Lakeside")]
        [Description("Lakeside")]
        Lakeside,
        [EnumMember(Value = "LakesideCity")]
        [Description("Lakeside City")]
        LakesideCity,
        [EnumMember(Value = "LAKEW")]
        [Description("Lakeway")]
        Lakeway,
        [EnumMember(Value = "LakewoodVillage")]
        [Description("Lakewood Village")]
        LakewoodVillage,
        [EnumMember(Value = "Lamar")]
        [Description("Lamar")]
        Lamar,
        [EnumMember(Value = "Lamesa")]
        [Description("Lamesa")]
        Lamesa,
        [EnumMember(Value = "Lampasas")]
        [Description("Lampasas")]
        Lampasas,
        [EnumMember(Value = "Lancaster")]
        [Description("Lancaster")]
        Lancaster,
        [EnumMember(Value = "Lanham")]
        [Description("Lanham")]
        Lanham,
        [EnumMember(Value = "Lantana")]
        [Description("Lantana")]
        Lantana,
        [EnumMember(Value = "Laredo")]
        [Description("Laredo")]
        Laredo,
        [EnumMember(Value = "LaredoRanchettes")]
        [Description("Laredo Ranchettes")]
        LaredoRanchettes,
        [EnumMember(Value = "LaredoRanchettesWest")]
        [Description("Laredo Ranchettes West")]
        LaredoRanchettesWest,
        [EnumMember(Value = "LasHaciendas")]
        [Description("Las Haciendas")]
        LasHaciendas,
        [EnumMember(Value = "LasLomas")]
        [Description("Las Lomas")]
        LasLomas,
        [EnumMember(Value = "LasLomitas")]
        [Description("Las Lomitas")]
        LasLomitas,
        [EnumMember(Value = "LasPalmas")]
        [Description("Las Palmas")]
        LasPalmas,
        [EnumMember(Value = "LasPalmasII")]
        [Description("Las Palmas II")]
        LasPalmasII,
        [EnumMember(Value = "LasPilas")]
        [Description("Las Pilas")]
        LasPilas,
        [EnumMember(Value = "LasQuintasFronterizas")]
        [Description("Las Quintas Fronterizas")]
        LasQuintasFronterizas,
        [EnumMember(Value = "LaSalle")]
        [Description("LaSalle")]
        LaSalle,
        [EnumMember(Value = "Lasana")]
        [Description("Lasana")]
        Lasana,
        [EnumMember(Value = "Lasara")]
        [Description("Lasara")]
        Lasara,
        [EnumMember(Value = "Latexo")]
        [Description("Latexo")]
        Latexo,
        [EnumMember(Value = "LaughlinAFB")]
        [Description("Laughlin AFB")]
        LaughlinAFB,
        [EnumMember(Value = "Laureles")]
        [Description("Laureles")]
        Laureles,
        [EnumMember(Value = "Lavon")]
        [Description("Lavon")]
        Lavon,
        [EnumMember(Value = "Lawn")]
        [Description("Lawn")]
        Lawn,
        [EnumMember(Value = "LeagueCity")]
        [Description("League City")]
        LeagueCity,
        [EnumMember(Value = "LEAKE")]
        [Description("Leakey")]
        Leakey,
        [EnumMember(Value = "LEAND")]
        [Description("Leander")]
        Leander,
        [EnumMember(Value = "Leary")]
        [Description("Leary")]
        Leary,
        [EnumMember(Value = "Ledbetter")]
        [Description("Ledbetter")]
        Ledbetter,
        [EnumMember(Value = "LEESV")]
        [Description("Leesville")]
        Leesville,
        [EnumMember(Value = "Lefors")]
        [Description("Lefors")]
        Lefors,
        [EnumMember(Value = "Leming")]
        [Description("Leming")]
        Leming,
        [EnumMember(Value = "LeonValley")]
        [Description("Leon Valley")]
        LeonValley,
        [EnumMember(Value = "Leona")]
        [Description("Leona")]
        Leona,
        [EnumMember(Value = "Leonard")]
        [Description("Leonard")]
        Leonard,
        [EnumMember(Value = "Leroy")]
        [Description("Leroy")]
        Leroy,
        [EnumMember(Value = "Levelland")]
        [Description("Levelland")]
        Levelland,
        [EnumMember(Value = "Lewisville")]
        [Description("Lewisville")]
        Lewisville,
        [EnumMember(Value = "Lexington")]
        [Description("Lexington")]
        Lexington,
        [EnumMember(Value = "Liberty")]
        [Description("Liberty")]
        Liberty,
        [EnumMember(Value = "LibertyCity")]
        [Description("Liberty City")]
        LibertyCity,
        [EnumMember(Value = "LibertyHill")]
        [Description("Liberty Hill")]
        LibertyHill,
        [EnumMember(Value = "LincolnPark")]
        [Description("Lincoln Park")]
        LincolnPark,
        [EnumMember(Value = "Lindale")]
        [Description("Lindale")]
        Lindale,
        [EnumMember(Value = "Linden")]
        [Description("Linden")]
        Linden,
        [EnumMember(Value = "Lindsay")]
        [Description("Lindsay")]
        Lindsay,
        [EnumMember(Value = "Linn")]
        [Description("Linn")]
        Linn,
        [EnumMember(Value = "Lipan")]
        [Description("Lipan")]
        Lipan,
        [EnumMember(Value = "Lipscomb")]
        [Description("Lipscomb")]
        Lipscomb,
        [EnumMember(Value = "LittleElm")]
        [Description("Little Elm")]
        LittleElm,
        [EnumMember(Value = "LittleRiverAcademy")]
        [Description("Little River-Academy")]
        LittleRiverAcademy,
        [EnumMember(Value = "Littlefield")]
        [Description("Littlefield")]
        Littlefield,
        [EnumMember(Value = "LIVEO")]
        [Description("Live Oak")]
        LiveOak,
        [EnumMember(Value = "Liverpool")]
        [Description("Liverpool")]
        Liverpool,
        [EnumMember(Value = "Livingston")]
        [Description("Livingston")]
        Livingston,
        [EnumMember(Value = "Llano")]
        [Description("Llano")]
        Llano,
        [EnumMember(Value = "LlanoGrande")]
        [Description("Llano Grande")]
        LlanoGrande,
        [EnumMember(Value = "LOCKH")]
        [Description("Lockhart")]
        Lockhart,
        [EnumMember(Value = "Lockney")]
        [Description("Lockney")]
        Lockney,
        [EnumMember(Value = "LogCabin")]
        [Description("Log Cabin")]
        LogCabin,
        [EnumMember(Value = "Lolita")]
        [Description("Lolita")]
        Lolita,
        [EnumMember(Value = "LomaGrande")]
        [Description("Loma Grande")]
        LomaGrande,
        [EnumMember(Value = "LomaLinda")]
        [Description("Loma Linda")]
        LomaLinda,
        [EnumMember(Value = "LomaLindaEast")]
        [Description("Loma Linda East")]
        LomaLindaEast,
        [EnumMember(Value = "LomaLindaWest")]
        [Description("Loma Linda West")]
        LomaLindaWest,
        [EnumMember(Value = "LomaVista")]
        [Description("Loma Vista")]
        LomaVista,
        [EnumMember(Value = "Lometa")]
        [Description("Lometa")]
        Lometa,
        [EnumMember(Value = "LoneOak")]
        [Description("Lone Oak")]
        LoneOak,
        [EnumMember(Value = "LoneStar")]
        [Description("Lone Star")]
        LoneStar,
        [EnumMember(Value = "Longoria")]
        [Description("Longoria")]
        Longoria,
        [EnumMember(Value = "Longview")]
        [Description("Longview")]
        Longview,
        [EnumMember(Value = "Loop")]
        [Description("Loop")]
        Loop,
        [EnumMember(Value = "Lopeño")]
        [Description("Lopeño")]
        Lopeño,
        [EnumMember(Value = "Lopezville")]
        [Description("Lopezville")]
        Lopezville,
        [EnumMember(Value = "Loraine")]
        [Description("Loraine")]
        Loraine,
        [EnumMember(Value = "Lorena")]
        [Description("Lorena")]
        Lorena,
        [EnumMember(Value = "Lorenzo")]
        [Description("Lorenzo")]
        Lorenzo,
        [EnumMember(Value = "LosAltos")]
        [Description("Los Altos")]
        LosAltos,
        [EnumMember(Value = "LosAlvarez")]
        [Description("Los Alvarez")]
        LosAlvarez,
        [EnumMember(Value = "LosAngeles")]
        [Description("Los Angeles")]
        LosAngeles,
        [EnumMember(Value = "LosArcos")]
        [Description("Los Arcos")]
        LosArcos,
        [EnumMember(Value = "LosArrieros")]
        [Description("Los Arrieros")]
        LosArrieros,
        [EnumMember(Value = "LosBarreras")]
        [Description("Los Barreras")]
        LosBarreras,
        [EnumMember(Value = "LosCentenarios")]
        [Description("Los Centenarios")]
        LosCentenarios,
        [EnumMember(Value = "LosCorralitos")]
        [Description("Los Corralitos")]
        LosCorralitos,
        [EnumMember(Value = "LosEbanos")]
        [Description("Los Ebanos")]
        LosEbanos,
        [EnumMember(Value = "LosFresnos")]
        [Description("Los Fresnos")]
        LosFresnos,
        [EnumMember(Value = "LosHuisaches")]
        [Description("Los Huisaches")]
        LosHuisaches,
        [EnumMember(Value = "LosIndios")]
        [Description("Los Indios")]
        LosIndios,
        [EnumMember(Value = "LosMinerales")]
        [Description("Los Minerales")]
        LosMinerales,
        [EnumMember(Value = "LosNopalitos")]
        [Description("Los Nopalitos")]
        LosNopalitos,
        [EnumMember(Value = "LosVeteranosI")]
        [Description("Los Veteranos I")]
        LosVeteranosI,
        [EnumMember(Value = "LosVeteranosII")]
        [Description("Los Veteranos II")]
        LosVeteranosII,
        [EnumMember(Value = "LosYbanez")]
        [Description("Los Ybanez")]
        LosYbanez,
        [EnumMember(Value = "LostCreek")]
        [Description("Lost Creek")]
        LostCreek,
        [EnumMember(Value = "Lott")]
        [Description("Lott")]
        Lott,
        [EnumMember(Value = "Louise")]
        [Description("Louise")]
        Louise,
        [EnumMember(Value = "Lovelady")]
        [Description("Lovelady")]
        Lovelady,
        [EnumMember(Value = "LowryCrossing")]
        [Description("Lowry Crossing")]
        LowryCrossing,
        [EnumMember(Value = "Lozano")]
        [Description("Lozano")]
        Lozano,
        [EnumMember(Value = "Lubbock")]
        [Description("Lubbock")]
        Lubbock,
        [EnumMember(Value = "Lucas")]
        [Description("Lucas")]
        Lucas,
        [EnumMember(Value = "Lueders")]
        [Description("Lueders")]
        Lueders,
        [EnumMember(Value = "Lufkin")]
        [Description("Lufkin")]
        Lufkin,
        [EnumMember(Value = "LULIN")]
        [Description("Luling")]
        Luling,
        [EnumMember(Value = "Lumberton")]
        [Description("Lumberton")]
        Lumberton,
        [EnumMember(Value = "Lyford")]
        [Description("Lyford")]
        Lyford,
        [EnumMember(Value = "LYTLE")]
        [Description("Lytle")]
        Lytle,
        [EnumMember(Value = "LYTTO")]
        [Description("Lytton Springs")]
        LyttonSprings,
        [EnumMember(Value = "Mabank")]
        [Description("Mabank")]
        Mabank,
        [EnumMember(Value = "Macdona")]
        [Description("Macdona")]
        Macdona,
        [EnumMember(Value = "Madisonville")]
        [Description("Madisonville")]
        Madisonville,
        [EnumMember(Value = "Magnolia")]
        [Description("Magnolia")]
        Magnolia,
        [EnumMember(Value = "Malakoff")]
        [Description("Malakoff")]
        Malakoff,
        [EnumMember(Value = "Malone")]
        [Description("Malone")]
        Malone,
        [EnumMember(Value = "MANCH")]
        [Description("Manchaca")]
        Manchaca,
        [EnumMember(Value = "MANOR")]
        [Description("Manor")]
        Manor,
        [EnumMember(Value = "Mansfield")]
        [Description("Mansfield")]
        Mansfield,
        [EnumMember(Value = "ManuelGarcia")]
        [Description("Manuel Garcia")]
        ManuelGarcia,
        [EnumMember(Value = "ManuelGarciaII")]
        [Description("Manuel Garcia II")]
        ManuelGarciaII,
        [EnumMember(Value = "Manvel")]
        [Description("Manvel")]
        Manvel,
        [EnumMember(Value = "Marathon")]
        [Description("Marathon")]
        Marathon,
        [EnumMember(Value = "MarbleFalls")]
        [Description("Marble Falls")]
        MarbleFalls,
        [EnumMember(Value = "Marfa")]
        [Description("Marfa")]
        Marfa,
        [EnumMember(Value = "Marietta")]
        [Description("Marietta")]
        Marietta,
        [EnumMember(Value = "MARIO")]
        [Description("Marion")]
        Marion,
        [EnumMember(Value = "Markham")]
        [Description("Markham")]
        Markham,
        [EnumMember(Value = "Marlin")]
        [Description("Marlin")]
        Marlin,
        [EnumMember(Value = "Marquez")]
        [Description("Marquez")]
        Marquez,
        [EnumMember(Value = "Marshall")]
        [Description("Marshall")]
        Marshall,
        [EnumMember(Value = "Mart")]
        [Description("Mart")]
        Mart,
        [EnumMember(Value = "MARTI")]
        [Description("Martindale")]
        Martindale,
        [EnumMember(Value = "Martinez")]
        [Description("Martinez")]
        Martinez,
        [EnumMember(Value = "Mason")]
        [Description("Mason")]
        Mason,
        [EnumMember(Value = "Matador")]
        [Description("Matador")]
        Matador,
        [EnumMember(Value = "Matagorda")]
        [Description("Matagorda")]
        Matagorda,
        [EnumMember(Value = "Mathis")]
        [Description("Mathis")]
        Mathis,
        [EnumMember(Value = "Maud")]
        [Description("Maud")]
        Maud,
        [EnumMember(Value = "Mauriceville")]
        [Description("Mauriceville")]
        Mauriceville,
        [EnumMember(Value = "MAXWE")]
        [Description("Maxwell")]
        Maxwell,
        [EnumMember(Value = "Maypearl")]
        [Description("Maypearl")]
        Maypearl,
        [EnumMember(Value = "Maysfield")]
        [Description("Maysfield")]
        Maysfield,
        [EnumMember(Value = "McAllen")]
        [Description("McAllen")]
        McAllen,
        [EnumMember(Value = "McCamey")]
        [Description("McCamey")]
        McCamey,
        [EnumMember(Value = "McDade")]
        [Description("McDade")]
        McDade,
        [EnumMember(Value = "McGregor")]
        [Description("McGregor")]
        McGregor,
        [EnumMember(Value = "McKinney")]
        [Description("McKinney")]
        McKinney,
        [EnumMember(Value = "McKinneyAcres")]
        [Description("McKinney Acres")]
        McKinneyAcres,
        [EnumMember(Value = "McLean")]
        [Description("McLean")]
        McLean,
        [EnumMember(Value = "McLendonChisholm")]
        [Description("McLendon-Chisholm")]
        McLendonChisholm,
        [EnumMember(Value = "MCMAH")]
        [Description("McMahan")]
        McMahan,
        [EnumMember(Value = "MCNEI")]
        [Description("McNeil")]
        McNeil,
        [EnumMember(Value = "MCQUE")]
        [Description("McQueeney")]
        McQueeney,
        [EnumMember(Value = "Meadow")]
        [Description("Meadow")]
        Meadow,
        [EnumMember(Value = "Meadowlakes")]
        [Description("Meadowlakes")]
        Meadowlakes,
        [EnumMember(Value = "MeadowsPlace")]
        [Description("Meadows Place")]
        MeadowsPlace,
        [EnumMember(Value = "MEDIN")]
        [Description("Medina ")]
        Medina,
        [EnumMember(Value = "Megargel")]
        [Description("Megargel")]
        Megargel,
        [EnumMember(Value = "Melissa")]
        [Description("Melissa")]
        Melissa,
        [EnumMember(Value = "Melvin")]
        [Description("Melvin")]
        Melvin,
        [EnumMember(Value = "Memphis")]
        [Description("Memphis")]
        Memphis,
        [EnumMember(Value = "MENAR")]
        [Description("Menard")]
        Menard,
        [EnumMember(Value = "Mentone")]
        [Description("Mentone")]
        Mentone,
        [EnumMember(Value = "Mercedes")]
        [Description("Mercedes")]
        Mercedes,
        [EnumMember(Value = "Meridian")]
        [Description("Meridian")]
        Meridian,
        [EnumMember(Value = "Merkel")]
        [Description("Merkel")]
        Merkel,
        [EnumMember(Value = "Mertens")]
        [Description("Mertens")]
        Mertens,
        [EnumMember(Value = "Mertzon")]
        [Description("Mertzon")]
        Mertzon,
        [EnumMember(Value = "Mesquite")]
        [Description("Mesquite")]
        Mesquite,
        [EnumMember(Value = "Mexia")]
        [Description("Mexia")]
        Mexia,
        [EnumMember(Value = "Meyersville")]
        [Description("Meyersville")]
        Meyersville,
        [EnumMember(Value = "MiRanchitoEstate")]
        [Description("Mi Ranchito Estate")]
        MiRanchitoEstate,
        [EnumMember(Value = "Miami")]
        [Description("Miami")]
        Miami,
        [EnumMember(Value = "MICO")]
        [Description("Mico")]
        Mico,
        [EnumMember(Value = "MIDFI")]
        [Description("Midfield")]
        Midfield,
        [EnumMember(Value = "Midland")]
        [Description("Midland")]
        Midland,
        [EnumMember(Value = "Midlothian")]
        [Description("Midlothian")]
        Midlothian,
        [EnumMember(Value = "Midway")]
        [Description("Midway")]
        Midway,
        [EnumMember(Value = "MidwayNorth")]
        [Description("Midway North")]
        MidwayNorth,
        [EnumMember(Value = "MidwaySouth")]
        [Description("Midway South")]
        MidwaySouth,
        [EnumMember(Value = "MiguelBarrera")]
        [Description("Miguel Barrera")]
        MiguelBarrera,
        [EnumMember(Value = "Mikes")]
        [Description("Mikes")]
        Mikes,
        [EnumMember(Value = "MilaDoce")]
        [Description("Mila Doce")]
        MilaDoce,
        [EnumMember(Value = "Milam")]
        [Description("Milam")]
        Milam,
        [EnumMember(Value = "Milano")]
        [Description("Milano")]
        Milano,
        [EnumMember(Value = "Mildred")]
        [Description("Mildred")]
        Mildred,
        [EnumMember(Value = "Miles")]
        [Description("Miles")]
        Miles,
        [EnumMember(Value = "Milford")]
        [Description("Milford")]
        Milford,
        [EnumMember(Value = "MillersCove")]
        [Description("Miller\"s Cove")]
        MillersCove,
        [EnumMember(Value = "Millican")]
        [Description("Millican")]
        Millican,
        [EnumMember(Value = "Millsap")]
        [Description("Millsap")]
        Millsap,
        [EnumMember(Value = "Mineola")]
        [Description("Mineola")]
        Mineola,
        [EnumMember(Value = "MineralWells")]
        [Description("Mineral Wells")]
        MineralWells,
        [EnumMember(Value = "Mingus")]
        [Description("Mingus")]
        Mingus,
        [EnumMember(Value = "MirandoCity")]
        [Description("Mirando City")]
        MirandoCity,
        [EnumMember(Value = "Mission")]
        [Description("Mission")]
        Mission,
        [EnumMember(Value = "MissionBend")]
        [Description("Mission Bend")]
        MissionBend,
        [EnumMember(Value = "MissouriCity")]
        [Description("Missouri City")]
        MissouriCity,
        [EnumMember(Value = "Mobeetie")]
        [Description("Mobeetie")]
        Mobeetie,
        [EnumMember(Value = "MobileCity")]
        [Description("Mobile City")]
        MobileCity,
        [EnumMember(Value = "Moffat")]
        [Description("Moffat")]
        Moffat,
        [EnumMember(Value = "Monahans")]
        [Description("Monahans")]
        Monahans,
        [EnumMember(Value = "MontBelvieu")]
        [Description("Mont Belvieu")]
        MontBelvieu,
        [EnumMember(Value = "Montague")]
        [Description("Montague")]
        Montague,
        [EnumMember(Value = "Montalba")]
        [Description("Montalba")]
        Montalba,
        [EnumMember(Value = "MonteAlto")]
        [Description("Monte Alto")]
        MonteAlto,
        [EnumMember(Value = "Montgomery")]
        [Description("Montgomery")]
        Montgomery,
        [EnumMember(Value = "Moody")]
        [Description("Moody")]
        Moody,
        [EnumMember(Value = "MOORE")]
        [Description("Moore")]
        Moore,
        [EnumMember(Value = "MooreStation")]
        [Description("Moore Station")]
        MooreStation,
        [EnumMember(Value = "Moraida")]
        [Description("Moraida")]
        Moraida,
        [EnumMember(Value = "MoralesSanchez")]
        [Description("Morales-Sanchez")]
        MoralesSanchez,
        [EnumMember(Value = "Moran")]
        [Description("Moran")]
        Moran,
        [EnumMember(Value = "Morgan")]
        [Description("Morgan")]
        Morgan,
        [EnumMember(Value = "MorganFarm")]
        [Description("Morgan Farm")]
        MorganFarm,
        [EnumMember(Value = "MorgansPoint")]
        [Description("Morgans Point")]
        MorgansPoint,
        [EnumMember(Value = "MorgansPointResort")]
        [Description("Morgans Point Resort")]
        MorgansPointResort,
        [EnumMember(Value = "MorningGlory")]
        [Description("Morning Glory")]
        MorningGlory,
        [EnumMember(Value = "Morse")]
        [Description("Morse")]
        Morse,
        [EnumMember(Value = "Morton")]
        [Description("Morton")]
        Morton,
        [EnumMember(Value = "MOULT")]
        [Description("Moulton")]
        Moulton,
        [EnumMember(Value = "Mound")]
        [Description("Mound")]
        Mound,
        [EnumMember(Value = "MountCalm")]
        [Description("Mount Calm")]
        MountCalm,
        [EnumMember(Value = "MountEnterprise")]
        [Description("Mount Enterprise")]
        MountEnterprise,
        [EnumMember(Value = "MountPleasant")]
        [Description("Mount Pleasant")]
        MountPleasant,
        [EnumMember(Value = "MountVernon")]
        [Description("Mount Vernon")]
        MountVernon,
        [EnumMember(Value = "MOUNT")]
        [Description("Mountain City")]
        MountainCity,
        [EnumMember(Value = "MountainHome")]
        [Description("Mountain Home")]
        MountainHome,
        [EnumMember(Value = "Muenster")]
        [Description("Muenster")]
        Muenster,
        [EnumMember(Value = "Muleshoe")]
        [Description("Muleshoe")]
        Muleshoe,
        [EnumMember(Value = "Mullin")]
        [Description("Mullin")]
        Mullin,
        [EnumMember(Value = "Munday")]
        [Description("Munday")]
        Munday,
        [EnumMember(Value = "Muniz")]
        [Description("Muniz")]
        Muniz,
        [EnumMember(Value = "Murchison")]
        [Description("Murchison")]
        Murchison,
        [EnumMember(Value = "Murillo")]
        [Description("Murillo")]
        Murillo,
        [EnumMember(Value = "Murphy")]
        [Description("Murphy")]
        Murphy,
        [EnumMember(Value = "Mustang")]
        [Description("Mustang")]
        Mustang,
        [EnumMember(Value = "MUSTA")]
        [Description("Mustang Ridge")]
        MustangRidge,
        [EnumMember(Value = "MyrtleSprings")]
        [Description("Myrtle Springs")]
        MyrtleSprings,
        [EnumMember(Value = "Nacogdoches")]
        [Description("Nacogdoches")]
        Nacogdoches,
        [EnumMember(Value = "Nada")]
        [Description("Nada")]
        Nada,
        [EnumMember(Value = "Naples")]
        [Description("Naples")]
        Naples,
        [EnumMember(Value = "NarcisoPena")]
        [Description("Narciso Pena")]
        NarcisoPena,
        [EnumMember(Value = "NARUNA")]
        [Description("Naruna")]
        Naruna,
        [EnumMember(Value = "Nash")]
        [Description("Nash")]
        Nash,
        [EnumMember(Value = "NassauBay")]
        [Description("Nassau Bay")]
        NassauBay,
        [EnumMember(Value = "NATAL")]
        [Description("Natalia")]
        Natalia,
        [EnumMember(Value = "Navarro")]
        [Description("Navarro")]
        Navarro,
        [EnumMember(Value = "Navasota")]
        [Description("Navasota")]
        Navasota,
        [EnumMember(Value = "Nazareth")]
        [Description("Nazareth")]
        Nazareth,
        [EnumMember(Value = "Nederland")]
        [Description("Nederland")]
        Nederland,
        [EnumMember(Value = "Needville")]
        [Description("Needville")]
        Needville,
        [EnumMember(Value = "Nesbitt")]
        [Description("Nesbitt")]
        Nesbitt,
        [EnumMember(Value = "Netos")]
        [Description("Netos")]
        Netos,
        [EnumMember(Value = "Nevada")]
        [Description("Nevada")]
        Nevada,
        [EnumMember(Value = "NEWBE")]
        [Description("New Berlin")]
        NewBerlin,
        [EnumMember(Value = "NewBoston")]
        [Description("New Boston")]
        NewBoston,
        [EnumMember(Value = "NEWBR")]
        [Description("New Braunfels")]
        NewBraunfels,
        [EnumMember(Value = "NewChapelHill")]
        [Description("New Chapel Hill")]
        NewChapelHill,
        [EnumMember(Value = "NewDeal")]
        [Description("New Deal")]
        NewDeal,
        [EnumMember(Value = "NewFairview")]
        [Description("New Fairview")]
        NewFairview,
        [EnumMember(Value = "NewFalcon")]
        [Description("New Falcon")]
        NewFalcon,
        [EnumMember(Value = "NewHome")]
        [Description("New Home")]
        NewHome,
        [EnumMember(Value = "NewHope")]
        [Description("New Hope")]
        NewHope,
        [EnumMember(Value = "NewLondon")]
        [Description("New London")]
        NewLondon,
        [EnumMember(Value = "NewSummerfield")]
        [Description("New Summerfield")]
        NewSummerfield,
        [EnumMember(Value = "NewTerritory")]
        [Description("New Territory")]
        NewTerritory,
        [EnumMember(Value = "NewWaverly")]
        [Description("New Waverly")]
        NewWaverly,
        [EnumMember(Value = "Newark")]
        [Description("Newark")]
        Newark,
        [EnumMember(Value = "Newcastle")]
        [Description("Newcastle")]
        Newcastle,
        [EnumMember(Value = "Newton")]
        [Description("Newton")]
        Newton,
        [EnumMember(Value = "Neylandville")]
        [Description("Neylandville")]
        Neylandville,
        [EnumMember(Value = "NIEDE")]
        [Description("Niederwald")]
        Niederwald,
        [EnumMember(Value = "Nina")]
        [Description("Nina")]
        Nina,
        [EnumMember(Value = "NIXON")]
        [Description("Nixon")]
        Nixon,
        [EnumMember(Value = "Nocona")]
        [Description("Nocona")]
        Nocona,
        [EnumMember(Value = "NoconaHills")]
        [Description("Nocona Hills")]
        NoconaHills,
        [EnumMember(Value = "Nolanville")]
        [Description("Nolanville")]
        Nolanville,
        [EnumMember(Value = "Nome")]
        [Description("Nome")]
        Nome,
        [EnumMember(Value = "Noonday")]
        [Description("Noonday")]
        Noonday,
        [EnumMember(Value = "NORDH")]
        [Description("Nordheim")]
        Nordheim,
        [EnumMember(Value = "Normangee")]
        [Description("Normangee")]
        Normangee,
        [EnumMember(Value = "Normanna")]
        [Description("Normanna")]
        Normanna,
        [EnumMember(Value = "NorthAlamo")]
        [Description("North Alamo")]
        NorthAlamo,
        [EnumMember(Value = "NorthCleveland")]
        [Description("North Cleveland")]
        NorthCleveland,
        [EnumMember(Value = "NorthEscobares")]
        [Description("North Escobares")]
        NorthEscobares,
        [EnumMember(Value = "NorthPearsall")]
        [Description("North Pearsall")]
        NorthPearsall,
        [EnumMember(Value = "NorthRichlandHills")]
        [Description("North Richland Hills")]
        NorthRichlandHills,
        [EnumMember(Value = "NorthSanPedro")]
        [Description("North San Pedro")]
        NorthSanPedro,
        [EnumMember(Value = "Northlake")]
        [Description("Northlake")]
        Northlake,
        [EnumMember(Value = "Northridge")]
        [Description("Northridge")]
        Northridge,
        [EnumMember(Value = "Novice")]
        [Description("Novice")]
        Novice,
        [EnumMember(Value = "Nursery")]
        [Description("Nursery")]
        Nursery,
        [EnumMember(Value = "OBrien")]
        [Description("O\"Brien")]
        OBrien,
        [EnumMember(Value = "ODonnell")]
        [Description("O\"Donnell")]
        ODonnell,
        [EnumMember(Value = "OakGrove")]
        [Description("Oak Grove")]
        OakGrove,
        [EnumMember(Value = "OakIsland")]
        [Description("Oak Island")]
        OakIsland,
        [EnumMember(Value = "OakLeaf")]
        [Description("Oak Leaf")]
        OakLeaf,
        [EnumMember(Value = "OakPoint")]
        [Description("Oak Point")]
        OakPoint,
        [EnumMember(Value = "OakRidge")]
        [Description("Oak Ridge")]
        OakRidge,
        [EnumMember(Value = "OakRidgeNorth")]
        [Description("Oak Ridge North")]
        OakRidgeNorth,
        [EnumMember(Value = "OakTrailShores")]
        [Description("Oak Trail Shores")]
        OakTrailShores,
        [EnumMember(Value = "OakValley")]
        [Description("Oak Valley")]
        OakValley,
        [EnumMember(Value = "Oakalla")]
        [Description("Oakalla")]
        Oakalla,
        [EnumMember(Value = "Oakhurst")]
        [Description("Oakhurst")]
        Oakhurst,
        [EnumMember(Value = "Oakwood")]
        [Description("Oakwood")]
        Oakwood,
        [EnumMember(Value = "Odem")]
        [Description("Odem")]
        Odem,
        [EnumMember(Value = "Odessa")]
        [Description("Odessa")]
        Odessa,
        [EnumMember(Value = "Oglesby")]
        [Description("Oglesby")]
        Oglesby,
        [EnumMember(Value = "Oilton")]
        [Description("Oilton")]
        Oilton,
        [EnumMember(Value = "OldRiverWinfree")]
        [Description("Old River-Winfree")]
        OldRiverWinfree,
        [EnumMember(Value = "Olivarez")]
        [Description("Olivarez")]
        Olivarez,
        [EnumMember(Value = "OliviaLopezdeGutierrez")]
        [Description("Olivia Lopez de Gutierrez")]
        OliviaLopezdeGutierrez,
        [EnumMember(Value = "Olmito")]
        [Description("Olmito")]
        Olmito,
        [EnumMember(Value = "OlmitoandOlmito")]
        [Description("Olmito and Olmito")]
        OlmitoandOlmito,
        [EnumMember(Value = "OLMOS")]
        [Description("Olmos Park")]
        OlmosPark,
        [EnumMember(Value = "Olney")]
        [Description("Olney")]
        Olney,
        [EnumMember(Value = "Olton")]
        [Description("Olton")]
        Olton,
        [EnumMember(Value = "Omaha")]
        [Description("Omaha")]
        Omaha,
        [EnumMember(Value = "Onalaska")]
        [Description("Onalaska")]
        Onalaska,
        [EnumMember(Value = "OpdykeWest")]
        [Description("Opdyke West")]
        OpdykeWest,
        [EnumMember(Value = "Orange")]
        [Description("Orange")]
        Orange,
        [EnumMember(Value = "OrangeGrove")]
        [Description("Orange Grove")]
        OrangeGrove,
        [EnumMember(Value = "Orason")]
        [Description("Orason")]
        Orason,
        [EnumMember(Value = "Orchard")]
        [Description("Orchard")]
        Orchard,
        [EnumMember(Value = "OreCity")]
        [Description("Ore City")]
        OreCity,
        [EnumMember(Value = "OTH")]
        [Description("Other")]
        Other,
        [EnumMember(Value = "OTTIN")]
        [Description("Ottine")]
        Ottine,
        [EnumMember(Value = "Overton")]
        [Description("Overton")]
        Overton,
        [EnumMember(Value = "Ovilla")]
        [Description("Ovilla")]
        Ovilla,
        [EnumMember(Value = "OwlRanch")]
        [Description("Owl Ranch")]
        OwlRanch,
        [EnumMember(Value = "OysterCreek")]
        [Description("Oyster Creek")]
        OysterCreek,
        [EnumMember(Value = "Ozona")]
        [Description("Ozona")]
        Ozona,
        [EnumMember(Value = "PabloPena")]
        [Description("Pablo Pena")]
        PabloPena,
        [EnumMember(Value = "Paducah")]
        [Description("Paducah")]
        Paducah,
        [EnumMember(Value = "Paige")]
        [Description("Paige")]
        Paige,
        [EnumMember(Value = "PaintRock")]
        [Description("Paint Rock")]
        PaintRock,
        [EnumMember(Value = "PaisanoPark")]
        [Description("Paisano Park")]
        PaisanoPark,
        [EnumMember(Value = "Palacios")]
        [Description("Palacios")]
        Palacios,
        [EnumMember(Value = "Palestine")]
        [Description("Palestine")]
        Palestine,
        [EnumMember(Value = "Palisades")]
        [Description("Palisades")]
        Palisades,
        [EnumMember(Value = "PalmValley")]
        [Description("Palm Valley")]
        PalmValley,
        [EnumMember(Value = "Palmer")]
        [Description("Palmer")]
        Palmer,
        [EnumMember(Value = "Palmhurst")]
        [Description("Palmhurst")]
        Palmhurst,
        [EnumMember(Value = "Palmview")]
        [Description("Palmview")]
        Palmview,
        [EnumMember(Value = "PalmviewSouth")]
        [Description("Palmview South")]
        PalmviewSouth,
        [EnumMember(Value = "PaloBlanco")]
        [Description("Palo Blanco")]
        PaloBlanco,
        [EnumMember(Value = "PaloPinto")]
        [Description("Palo Pinto")]
        PaloPinto,
        [EnumMember(Value = "PalomaCreek")]
        [Description("Paloma Creek")]
        PalomaCreek,
        [EnumMember(Value = "PalomaCreekSouth")]
        [Description("Paloma Creek South")]
        PalomaCreekSouth,
        [EnumMember(Value = "Pampa")]
        [Description("Pampa")]
        Pampa,
        [EnumMember(Value = "PANDORA")]
        [Description("Pandora")]
        Pandora,
        [EnumMember(Value = "Panhandle")]
        [Description("Panhandle")]
        Panhandle,
        [EnumMember(Value = "PanoramaVillage")]
        [Description("Panorama Village")]
        PanoramaVillage,
        [EnumMember(Value = "Pantego")]
        [Description("Pantego")]
        Pantego,
        [EnumMember(Value = "Paradise")]
        [Description("Paradise")]
        Paradise,
        [EnumMember(Value = "Paris")]
        [Description("Paris")]
        Paris,
        [EnumMember(Value = "Parker")]
        [Description("Parker")]
        Parker,
        [EnumMember(Value = "Pasadena")]
        [Description("Pasadena")]
        Pasadena,
        [EnumMember(Value = "Pattison")]
        [Description("Pattison")]
        Pattison,
        [EnumMember(Value = "PattonVillage")]
        [Description("Patton Village")]
        PattonVillage,
        [EnumMember(Value = "Pawnee")]
        [Description("Pawnee")]
        Pawnee,
        [EnumMember(Value = "PayneSprings")]
        [Description("Payne Springs")]
        PayneSprings,
        [EnumMember(Value = "Pearland")]
        [Description("Pearland")]
        Pearland,
        [EnumMember(Value = "Pearsall")]
        [Description("Pearsall")]
        Pearsall,
        [EnumMember(Value = "PecanAcres")]
        [Description("Pecan Acres")]
        PecanAcres,
        [EnumMember(Value = "PecanGap")]
        [Description("Pecan Gap")]
        PecanGap,
        [EnumMember(Value = "PecanGrove")]
        [Description("Pecan Grove")]
        PecanGrove,
        [EnumMember(Value = "PecanHill")]
        [Description("Pecan Hill")]
        PecanHill,
        [EnumMember(Value = "PecanPlantation")]
        [Description("Pecan Plantation")]
        PecanPlantation,
        [EnumMember(Value = "Pecos")]
        [Description("Pecos")]
        Pecos,
        [EnumMember(Value = "PelicanBay")]
        [Description("Pelican Bay")]
        PelicanBay,
        [EnumMember(Value = "Pena")]
        [Description("Pena")]
        Pena,
        [EnumMember(Value = "Pendleton")]
        [Description("Pendleton")]
        Pendleton,
        [EnumMember(Value = "Penelope")]
        [Description("Penelope")]
        Penelope,
        [EnumMember(Value = "Penitas")]
        [Description("Penitas")]
        Penitas,
        [EnumMember(Value = "Perezville")]
        [Description("Perezville")]
        Perezville,
        [EnumMember(Value = "Perrin")]
        [Description("Perrin")]
        Perrin,
        [EnumMember(Value = "Perry")]
        [Description("Perry")]
        Perry,
        [EnumMember(Value = "Perryton")]
        [Description("Perryton")]
        Perryton,
        [EnumMember(Value = "Petersburg")]
        [Description("Petersburg")]
        Petersburg,
        [EnumMember(Value = "Petrolia")]
        [Description("Petrolia")]
        Petrolia,
        [EnumMember(Value = "Petronila")]
        [Description("Petronila")]
        Petronila,
        [EnumMember(Value = "Pettus")]
        [Description("Pettus")]
        Pettus,
        [EnumMember(Value = "PFLUG")]
        [Description("Pflugerville")]
        Pflugerville,
        [EnumMember(Value = "Pharr")]
        [Description("Pharr")]
        Pharr,
        [EnumMember(Value = "Pidcoke")]
        [Description("Pidcoke")]
        Pidcoke,
        [EnumMember(Value = "PilotPoint")]
        [Description("Pilot Point")]
        PilotPoint,
        [EnumMember(Value = "PineForest")]
        [Description("Pine Forest")]
        PineForest,
        [EnumMember(Value = "PineHarbor")]
        [Description("Pine Harbor")]
        PineHarbor,
        [EnumMember(Value = "PineIsland")]
        [Description("Pine Island")]
        PineIsland,
        [EnumMember(Value = "Pinehurst")]
        [Description("Pinehurst")]
        Pinehurst,
        [EnumMember(Value = "Pineland")]
        [Description("Pineland")]
        Pineland,
        [EnumMember(Value = "PinewoodEstates")]
        [Description("Pinewood Estates")]
        PinewoodEstates,
        [EnumMember(Value = "PineyPointVillage")]
        [Description("Piney Point Village")]
        PineyPointVillage,
        [EnumMember(Value = "PIPEC")]
        [Description("Pipe Creek")]
        PipeCreek,
        [EnumMember(Value = "Pittsburg")]
        [Description("Pittsburg")]
        Pittsburg,
        [EnumMember(Value = "Placedo")]
        [Description("Placedo")]
        Placedo,
        [EnumMember(Value = "Plains")]
        [Description("Plains")]
        Plains,
        [EnumMember(Value = "Plainview")]
        [Description("Plainview")]
        Plainview,
        [EnumMember(Value = "Plano")]
        [Description("Plano")]
        Plano,
        [EnumMember(Value = "Pleak")]
        [Description("Pleak")]
        Pleak,
        [EnumMember(Value = "PleasantHill")]
        [Description("Pleasant Hill")]
        PleasantHill,
        [EnumMember(Value = "PleasantValley")]
        [Description("Pleasant Valley")]
        PleasantValley,
        [EnumMember(Value = "PLEAS")]
        [Description("Pleasanton")]
        Pleasanton,
        [EnumMember(Value = "PlumGrove")]
        [Description("Plum Grove")]
        PlumGrove,
        [EnumMember(Value = "PointBlank")]
        [Description("Point Blank")]
        PointBlank,
        [EnumMember(Value = "PointComfort")]
        [Description("Point Comfort")]
        PointComfort,
        [EnumMember(Value = "POINT")]
        [Description("Point Venture")]
        PointVenture,
        [EnumMember(Value = "Ponder")]
        [Description("Ponder")]
        Ponder,
        [EnumMember(Value = "PRTAL")]
        [Description("Port Alto")]
        PortAlto,
        [EnumMember(Value = "PRTAR")]
        [Description("Port Aransas")]
        PortAransas,
        [EnumMember(Value = "PortArthur")]
        [Description("Port Arthur")]
        PortArthur,
        [EnumMember(Value = "PortIsabel")]
        [Description("Port Isabel")]
        PortIsabel,
        [EnumMember(Value = "PRTLA")]
        [Description("Port Lavaca")]
        PortLavaca,
        [EnumMember(Value = "PortMansfield")]
        [Description("Port Mansfield")]
        PortMansfield,
        [EnumMember(Value = "PortNeches")]
        [Description("Port Neches")]
        PortNeches,
        [EnumMember(Value = "PortOConnor")]
        [Description("Port O\"Connor")]
        PortOConnor,
        [EnumMember(Value = "PorterHeights")]
        [Description("Porter Heights")]
        PorterHeights,
        [EnumMember(Value = "Portland")]
        [Description("Portland")]
        Portland,
        [EnumMember(Value = "Post")]
        [Description("Post")]
        Post,
        [EnumMember(Value = "PostOakBendCity")]
        [Description("Post Oak Bend City")]
        PostOakBendCity,
        [EnumMember(Value = "POTEE")]
        [Description("Poteet")]
        Poteet,
        [EnumMember(Value = "POTH")]
        [Description("Poth")]
        Poth,
        [EnumMember(Value = "Potosi")]
        [Description("Potosi")]
        Potosi,
        [EnumMember(Value = "Pottsboro")]
        [Description("Pottsboro")]
        Pottsboro,
        [EnumMember(Value = "Powderly")]
        [Description("Powderly")]
        Powderly,
        [EnumMember(Value = "Powell")]
        [Description("Powell")]
        Powell,
        [EnumMember(Value = "Poynor")]
        [Description("Poynor")]
        Poynor,
        [EnumMember(Value = "PradoVerde")]
        [Description("Prado Verde")]
        PradoVerde,
        [EnumMember(Value = "PrairieHill")]
        [Description("Prairie Hill")]
        PrairieHill,
        [EnumMember(Value = "PRAIR")]
        [Description("Prairie Lea")]
        PrairieLea,
        [EnumMember(Value = "Premont")]
        [Description("Premont")]
        Premont,
        [EnumMember(Value = "Presidio")]
        [Description("Presidio")]
        Presidio,
        [EnumMember(Value = "Preston")]
        [Description("Preston")]
        Preston,
        [EnumMember(Value = "Primera")]
        [Description("Primera")]
        Primera,
        [EnumMember(Value = "Princeton")]
        [Description("Princeton")]
        Princeton,
        [EnumMember(Value = "Progreso")]
        [Description("Progreso")]
        Progreso,
        [EnumMember(Value = "ProgresoLakes")]
        [Description("Progreso Lakes")]
        ProgresoLakes,
        [EnumMember(Value = "Prosper")]
        [Description("Prosper")]
        Prosper,
        [EnumMember(Value = "ProvidenceVillage")]
        [Description("Providence Village")]
        ProvidenceVillage,
        [EnumMember(Value = "PuebloEast")]
        [Description("Pueblo East")]
        PuebloEast,
        [EnumMember(Value = "PuebloNuevo")]
        [Description("Pueblo Nuevo")]
        PuebloNuevo,
        [EnumMember(Value = "Purmela")]
        [Description("Purmela")]
        Purmela,
        [EnumMember(Value = "Putnam")]
        [Description("Putnam")]
        Putnam,
        [EnumMember(Value = "Pyote")]
        [Description("Pyote")]
        Pyote,
        [EnumMember(Value = "Quail")]
        [Description("Quail")]
        Quail,
        [EnumMember(Value = "QuailCreek")]
        [Description("Quail Creek")]
        QuailCreek,
        [EnumMember(Value = "Quanah")]
        [Description("Quanah")]
        Quanah,
        [EnumMember(Value = "QueenCity")]
        [Description("Queen City")]
        QueenCity,
        [EnumMember(Value = "Quemado")]
        [Description("Quemado")]
        Quemado,
        [EnumMember(Value = "Quesada")]
        [Description("Quesada")]
        Quesada,
        [EnumMember(Value = "Quinlan")]
        [Description("Quinlan")]
        Quinlan,
        [EnumMember(Value = "Quintana")]
        [Description("Quintana")]
        Quintana,
        [EnumMember(Value = "Quitaque")]
        [Description("Quitaque")]
        Quitaque,
        [EnumMember(Value = "Quitman")]
        [Description("Quitman")]
        Quitman,
        [EnumMember(Value = "RadarBase")]
        [Description("Radar Base")]
        RadarBase,
        [EnumMember(Value = "RafaelPena")]
        [Description("Rafael Pena")]
        RafaelPena,
        [EnumMember(Value = "Ralls")]
        [Description("Ralls")]
        Ralls,
        [EnumMember(Value = "Ramireno")]
        [Description("Ramireno")]
        Ramireno,
        [EnumMember(Value = "RamirezPerez")]
        [Description("Ramirez-Perez")]
        RamirezPerez,
        [EnumMember(Value = "Ramos")]
        [Description("Ramos")]
        Ramos,
        [EnumMember(Value = "RanchetteEstates")]
        [Description("Ranchette Estates")]
        RanchetteEstates,
        [EnumMember(Value = "RanchitosDelNorte")]
        [Description("Ranchitos Del Norte")]
        RanchitosDelNorte,
        [EnumMember(Value = "RanchitosEast")]
        [Description("Ranchitos East")]
        RanchitosEast,
        [EnumMember(Value = "RanchitosLasLomas")]
        [Description("Ranchitos Las Lomas")]
        RanchitosLasLomas,
        [EnumMember(Value = "RanchoAlegre")]
        [Description("Rancho Alegre")]
        RanchoAlegre,
        [EnumMember(Value = "RanchoBanquete")]
        [Description("Rancho Banquete")]
        RanchoBanquete,
        [EnumMember(Value = "RanchoChico")]
        [Description("Rancho Chico")]
        RanchoChico,
        [EnumMember(Value = "RanchoViejo")]
        [Description("Rancho Viejo")]
        RanchoViejo,
        [EnumMember(Value = "RanchosPenitasWest")]
        [Description("Ranchos Penitas West")]
        RanchosPenitasWest,
        [EnumMember(Value = "RandolphAFB")]
        [Description("Randolph AFB")]
        RandolphAFB,
        [EnumMember(Value = "Ranger")]
        [Description("Ranger")]
        Ranger,
        [EnumMember(Value = "Rangerville")]
        [Description("Rangerville")]
        Rangerville,
        [EnumMember(Value = "Rankin")]
        [Description("Rankin")]
        Rankin,
        [EnumMember(Value = "RansomCanyon")]
        [Description("Ransom Canyon")]
        RansomCanyon,
        [EnumMember(Value = "Ratamosa")]
        [Description("Ratamosa")]
        Ratamosa,
        [EnumMember(Value = "Ravenna")]
        [Description("Ravenna")]
        Ravenna,
        [EnumMember(Value = "Raymondville")]
        [Description("Raymondville")]
        Raymondville,
        [EnumMember(Value = "Reagan")]
        [Description("Reagan")]
        Reagan,
        [EnumMember(Value = "Realitos")]
        [Description("Realitos")]
        Realitos,
        [EnumMember(Value = "RedLick")]
        [Description("Red Lick")]
        RedLick,
        [EnumMember(Value = "RedOak")]
        [Description("Red Oak")]
        RedOak,
        [EnumMember(Value = "REDRO")]
        [Description("Red Rock")]
        RedRock,
        [EnumMember(Value = "Redfield")]
        [Description("Redfield")]
        Redfield,
        [EnumMember(Value = "Redford")]
        [Description("Redford")]
        Redford,
        [EnumMember(Value = "Redland")]
        [Description("Redland")]
        Redland,
        [EnumMember(Value = "Redwater")]
        [Description("Redwater")]
        Redwater,
        [EnumMember(Value = "Redwood")]
        [Description("Redwood")]
        Redwood,
        [EnumMember(Value = "REEDV")]
        [Description("Reedville")]
        Reedville,
        [EnumMember(Value = "Refugio")]
        [Description("Refugio")]
        Refugio,
        [EnumMember(Value = "ReginoRamirez")]
        [Description("Regino Ramirez")]
        ReginoRamirez,
        [EnumMember(Value = "ReidHopeKing")]
        [Description("Reid Hope King")]
        ReidHopeKing,
        [EnumMember(Value = "Reklaw")]
        [Description("Reklaw")]
        Reklaw,
        [EnumMember(Value = "Relampago")]
        [Description("Relampago")]
        Relampago,
        [EnumMember(Value = "Rendon")]
        [Description("Rendon")]
        Rendon,
        [EnumMember(Value = "Reno")]
        [Description("Reno")]
        Reno,
        [EnumMember(Value = "Retreat")]
        [Description("Retreat")]
        Retreat,
        [EnumMember(Value = "Rhome")]
        [Description("Rhome")]
        Rhome,
        [EnumMember(Value = "Ricardo")]
        [Description("Ricardo")]
        Ricardo,
        [EnumMember(Value = "Rice")]
        [Description("Rice")]
        Rice,
        [EnumMember(Value = "Richardson")]
        [Description("Richardson")]
        Richardson,
        [EnumMember(Value = "Richland")]
        [Description("Richland")]
        Richland,
        [EnumMember(Value = "RichlandHills")]
        [Description("Richland Hills")]
        RichlandHills,
        [EnumMember(Value = "RichlandSprings")]
        [Description("Richland Springs")]
        RichlandSprings,
        [EnumMember(Value = "Richmond")]
        [Description("Richmond")]
        Richmond,
        [EnumMember(Value = "Richwood")]
        [Description("Richwood")]
        Richwood,
        [EnumMember(Value = "Riesel")]
        [Description("Riesel")]
        Riesel,
        [EnumMember(Value = "RioBravo")]
        [Description("Rio Bravo")]
        RioBravo,
        [EnumMember(Value = "RioGrandeCity")]
        [Description("Rio Grande City")]
        RioGrandeCity,
        [EnumMember(Value = "RioHondo")]
        [Description("Rio Hondo")]
        RioHondo,
        [EnumMember(Value = "RioVista")]
        [Description("Rio Vista")]
        RioVista,
        [EnumMember(Value = "RisingStar")]
        [Description("Rising Star")]
        RisingStar,
        [EnumMember(Value = "RiverOaks")]
        [Description("River Oaks")]
        RiverOaks,
        [EnumMember(Value = "Rivereno")]
        [Description("Rivereno")]
        Rivereno,
        [EnumMember(Value = "Riverside")]
        [Description("Riverside")]
        Riverside,
        [EnumMember(Value = "Riviera")]
        [Description("Riviera")]
        Riviera,
        [EnumMember(Value = "Roanoke")]
        [Description("Roanoke")]
        Roanoke,
        [EnumMember(Value = "RoaringSprings")]
        [Description("Roaring Springs")]
        RoaringSprings,
        [EnumMember(Value = "RobertLee")]
        [Description("Robert Lee")]
        RobertLee,
        [EnumMember(Value = "Robinson")]
        [Description("Robinson")]
        Robinson,
        [EnumMember(Value = "Robstown")]
        [Description("Robstown")]
        Robstown,
        [EnumMember(Value = "Roby")]
        [Description("Roby")]
        Roby,
        [EnumMember(Value = "Rochester")]
        [Description("Rochester")]
        Rochester,
        [EnumMember(Value = "Rockdale")]
        [Description("Rockdale")]
        Rockdale,
        [EnumMember(Value = "Rockport")]
        [Description("Rockport")]
        Rockport,
        [EnumMember(Value = "ROCKS")]
        [Description("Rocksprings")]
        Rocksprings,
        [EnumMember(Value = "Rockwall")]
        [Description("Rockwall")]
        Rockwall,
        [EnumMember(Value = "RockyMound")]
        [Description("Rocky Mound")]
        RockyMound,
        [EnumMember(Value = "Rogers")]
        [Description("Rogers")]
        Rogers,
        [EnumMember(Value = "Rollingwood")]
        [Description("Rollingwood")]
        Rollingwood,
        [EnumMember(Value = "Roma")]
        [Description("Roma")]
        Roma,
        [EnumMember(Value = "RomaCreek")]
        [Description("Roma Creek")]
        RomaCreek,
        [EnumMember(Value = "RomanForest")]
        [Description("Roman Forest")]
        RomanForest,
        [EnumMember(Value = "Ropesville")]
        [Description("Ropesville")]
        Ropesville,
        [EnumMember(Value = "ROSAN")]
        [Description("Rosanky")]
        Rosanky,
        [EnumMember(Value = "Roscoe")]
        [Description("Roscoe")]
        Roscoe,
        [EnumMember(Value = "RoseCity")]
        [Description("Rose City")]
        RoseCity,
        [EnumMember(Value = "RoseHillAcres")]
        [Description("Rose Hill Acres")]
        RoseHillAcres,
        [EnumMember(Value = "Rosebud")]
        [Description("Rosebud")]
        Rosebud,
        [EnumMember(Value = "Rosenberg")]
        [Description("Rosenberg")]
        Rosenberg,
        [EnumMember(Value = "Rosharon")]
        [Description("Rosharon")]
        Rosharon,
        [EnumMember(Value = "Rosita")]
        [Description("Rosita")]
        Rosita,
        [EnumMember(Value = "Ross")]
        [Description("Ross")]
        Ross,
        [EnumMember(Value = "Rosser")]
        [Description("Rosser")]
        Rosser,
        [EnumMember(Value = "Rotan")]
        [Description("Rotan")]
        Rotan,
        [EnumMember(Value = "RNDMO")]
        [Description("Round Mountain")]
        RoundMountain,
        [EnumMember(Value = "RNDRO")]
        [Description("Round Rock")]
        RoundRock,
        [EnumMember(Value = "RoundTop")]
        [Description("Round Top")]
        RoundTop,
        [EnumMember(Value = "Rowlett")]
        [Description("Rowlett")]
        Rowlett,
        [EnumMember(Value = "Roxton")]
        [Description("Roxton")]
        Roxton,
        [EnumMember(Value = "RoyseCity")]
        [Description("Royse City")]
        RoyseCity,
        [EnumMember(Value = "Rule")]
        [Description("Rule")]
        Rule,
        [EnumMember(Value = "RunawayBay")]
        [Description("Runaway Bay")]
        RunawayBay,
        [EnumMember(Value = "RUNGE")]
        [Description("Runge")]
        Runge,
        [EnumMember(Value = "Rusk")]
        [Description("Rusk")]
        Rusk,
        [EnumMember(Value = "Sabinal")]
        [Description("Sabinal")]
        Sabinal,
        [EnumMember(Value = "Sachse")]
        [Description("Sachse")]
        Sachse,
        [EnumMember(Value = "Sadler")]
        [Description("Sadler")]
        Sadler,
        [EnumMember(Value = "Saginaw")]
        [Description("Saginaw")]
        Saginaw,
        [EnumMember(Value = "SAINT")]
        [Description("Saint Hedwig")]
        SaintHedwig,
        [EnumMember(Value = "Salado")]
        [Description("Salado")]
        Salado,
        [EnumMember(Value = "Salineño")]
        [Description("Salineño")]
        Salineño,
        [EnumMember(Value = "SalineñoNorth")]
        [Description("Salineño North")]
        SalineñoNorth,
        [EnumMember(Value = "SamRayburn")]
        [Description("Sam Rayburn")]
        SamRayburn,
        [EnumMember(Value = "SammyMartinez")]
        [Description("Sammy Martinez")]
        SammyMartinez,
        [EnumMember(Value = "Samnorwood")]
        [Description("Samnorwood")]
        Samnorwood,
        [EnumMember(Value = "SanAngelo")]
        [Description("San Angelo")]
        SanAngelo,
        [EnumMember(Value = "SANAN")]
        [Description("San Antonio")]
        SanAntonio,
        [EnumMember(Value = "SanAugustine")]
        [Description("San Augustine")]
        SanAugustine,
        [EnumMember(Value = "SanBenito")]
        [Description("San Benito")]
        SanBenito,
        [EnumMember(Value = "SanCarlos")]
        [Description("San Carlos")]
        SanCarlos,
        [EnumMember(Value = "SanCarlosI")]
        [Description("San Carlos I")]
        SanCarlosI,
        [EnumMember(Value = "SanCarlosII")]
        [Description("San Carlos II")]
        SanCarlosII,
        [EnumMember(Value = "SanDiego")]
        [Description("San Diego")]
        SanDiego,
        [EnumMember(Value = "SanElizario")]
        [Description("San Elizario")]
        SanElizario,
        [EnumMember(Value = "SanFelipe")]
        [Description("San Felipe")]
        SanFelipe,
        [EnumMember(Value = "SanFernando")]
        [Description("San Fernando")]
        SanFernando,
        [EnumMember(Value = "SanIsidro")]
        [Description("San Isidro")]
        SanIsidro,
        [EnumMember(Value = "SanJuan")]
        [Description("San Juan")]
        SanJuan,
        [EnumMember(Value = "SanLeanna")]
        [Description("San Leanna")]
        SanLeanna,
        [EnumMember(Value = "SanLeon")]
        [Description("San Leon")]
        SanLeon,
        [EnumMember(Value = "SANMA")]
        [Description("San Marcos")]
        SanMarcos,
        [EnumMember(Value = "SanPatricio")]
        [Description("San Patricio")]
        SanPatricio,
        [EnumMember(Value = "SanPedro")]
        [Description("San Pedro")]
        SanPedro,
        [EnumMember(Value = "SanPerlita")]
        [Description("San Perlita")]
        SanPerlita,
        [EnumMember(Value = "SanSaba")]
        [Description("San Saba")]
        SanSaba,
        [EnumMember(Value = "SanYgnacio")]
        [Description("San Ygnacio")]
        SanYgnacio,
        [EnumMember(Value = "Sanctuary")]
        [Description("Sanctuary")]
        Sanctuary,
        [EnumMember(Value = "SandSprings")]
        [Description("Sand Springs")]
        SandSprings,
        [EnumMember(Value = "Sanderson")]
        [Description("Sanderson")]
        Sanderson,
        [EnumMember(Value = "Sandia")]
        [Description("Sandia")]
        Sandia,
        [EnumMember(Value = "Sandoval")]
        [Description("Sandoval")]
        Sandoval,
        [EnumMember(Value = "SandyHollowEscondidas")]
        [Description("Sandy Hollow-Escondidas")]
        SandyHollowEscondidas,
        [EnumMember(Value = "SandyPoint")]
        [Description("Sandy Point")]
        SandyPoint,
        [EnumMember(Value = "Sanford")]
        [Description("Sanford")]
        Sanford,
        [EnumMember(Value = "Sanger")]
        [Description("Sanger")]
        Sanger,
        [EnumMember(Value = "SansomPark")]
        [Description("Sansom Park")]
        SansomPark,
        [EnumMember(Value = "SantaAnna")]
        [Description("Santa Anna")]
        SantaAnna,
        [EnumMember(Value = "SANTA")]
        [Description("Santa Clara")]
        SantaClara,
        [EnumMember(Value = "SantaCruz")]
        [Description("Santa Cruz")]
        SantaCruz,
        [EnumMember(Value = "SantaFe")]
        [Description("Santa Fe")]
        SantaFe,
        [EnumMember(Value = "SantaMaria")]
        [Description("Santa Maria")]
        SantaMaria,
        [EnumMember(Value = "SantaMonica")]
        [Description("Santa Monica")]
        SantaMonica,
        [EnumMember(Value = "SantaRosa")]
        [Description("Santa Rosa")]
        SantaRosa,
        [EnumMember(Value = "Santel")]
        [Description("Santel")]
        Santel,
        [EnumMember(Value = "Sarita")]
        [Description("Sarita")]
        Sarita,
        [EnumMember(Value = "Savannah")]
        [Description("Savannah")]
        Savannah,
        [EnumMember(Value = "Savoy")]
        [Description("Savoy")]
        Savoy,
        [EnumMember(Value = "ScenicOaks")]
        [Description("Scenic Oaks")]
        ScenicOaks,
        [EnumMember(Value = "SCHER")]
        [Description("Schertz")]
        Schertz,
        [EnumMember(Value = "Schroeder")]
        [Description("Schroeder")]
        Schroeder,
        [EnumMember(Value = "SCHUL")]
        [Description("Schulenburg")]
        Schulenburg,
        [EnumMember(Value = "Schwertner")]
        [Description("Schwertner")]
        Schwertner,
        [EnumMember(Value = "Scissors")]
        [Description("Scissors")]
        Scissors,
        [EnumMember(Value = "Scotland")]
        [Description("Scotland")]
        Scotland,
        [EnumMember(Value = "Scottsville")]
        [Description("Scottsville")]
        Scottsville,
        [EnumMember(Value = "Scurry")]
        [Description("Scurry")]
        Scurry,
        [EnumMember(Value = "Seabrook")]
        [Description("Seabrook")]
        Seabrook,
        [EnumMember(Value = "SEADR")]
        [Description("Seadrift")]
        Seadrift,
        [EnumMember(Value = "Seagoville")]
        [Description("Seagoville")]
        Seagoville,
        [EnumMember(Value = "Seagraves")]
        [Description("Seagraves")]
        Seagraves,
        [EnumMember(Value = "Sealy")]
        [Description("Sealy")]
        Sealy,
        [EnumMember(Value = "Sebastian")]
        [Description("Sebastian")]
        Sebastian,
        [EnumMember(Value = "SecoMines")]
        [Description("Seco Mines")]
        SecoMines,
        [EnumMember(Value = "SEGUI")]
        [Description("Seguin")]
        Seguin,
        [EnumMember(Value = "SELMA")]
        [Description("Selma")]
        Selma,
        [EnumMember(Value = "Seminole")]
        [Description("Seminole")]
        Seminole,
        [EnumMember(Value = "Serenada")]
        [Description("Serenada")]
        Serenada,
        [EnumMember(Value = "SethWard")]
        [Description("Seth Ward")]
        SethWard,
        [EnumMember(Value = "SevenOaks")]
        [Description("Seven Oaks")]
        SevenOaks,
        [EnumMember(Value = "SevenPoints")]
        [Description("Seven Points")]
        SevenPoints,
        [EnumMember(Value = "Seymour")]
        [Description("Seymour")]
        Seymour,
        [EnumMember(Value = "ShadyHollow")]
        [Description("Shady Hollow")]
        ShadyHollow,
        [EnumMember(Value = "ShadyShores")]
        [Description("Shady Shores")]
        ShadyShores,
        [EnumMember(Value = "Shadybrook")]
        [Description("Shadybrook")]
        Shadybrook,
        [EnumMember(Value = "Shallowater")]
        [Description("Shallowater")]
        Shallowater,
        [EnumMember(Value = "Shamrock")]
        [Description("Shamrock")]
        Shamrock,
        [EnumMember(Value = "SHAVA")]
        [Description("Shavano Park")]
        ShavanoPark,
        [EnumMember(Value = "Sheldon")]
        [Description("Sheldon")]
        Sheldon,
        [EnumMember(Value = "Shenandoah")]
        [Description("Shenandoah")]
        Shenandoah,
        [EnumMember(Value = "Shepherd")]
        [Description("Shepherd")]
        Shepherd,
        [EnumMember(Value = "Sheridan")]
        [Description("Sheridan")]
        Sheridan,
        [EnumMember(Value = "Sherman")]
        [Description("Sherman")]
        Sherman,
        [EnumMember(Value = "SherwoodShores")]
        [Description("Sherwood Shores")]
        SherwoodShores,
        [EnumMember(Value = "SHINE")]
        [Description("Shiner")]
        Shiner,
        [EnumMember(Value = "Shoreacres")]
        [Description("Shoreacres")]
        Shoreacres,
        [EnumMember(Value = "SiennaPlantation")]
        [Description("Sienna Plantation")]
        SiennaPlantation,
        [EnumMember(Value = "SierraBlanca")]
        [Description("Sierra Blanca")]
        SierraBlanca,
        [EnumMember(Value = "SiestaAcres")]
        [Description("Siesta Acres")]
        SiestaAcres,
        [EnumMember(Value = "SiestaShores")]
        [Description("Siesta Shores")]
        SiestaShores,
        [EnumMember(Value = "Silsbee")]
        [Description("Silsbee")]
        Silsbee,
        [EnumMember(Value = "Silverton")]
        [Description("Silverton")]
        Silverton,
        [EnumMember(Value = "Simonton")]
        [Description("Simonton")]
        Simonton,
        [EnumMember(Value = "Sinton")]
        [Description("Sinton")]
        Sinton,
        [EnumMember(Value = "Skellytown")]
        [Description("Skellytown")]
        Skellytown,
        [EnumMember(Value = "Skidmore")]
        [Description("Skidmore")]
        Skidmore,
        [EnumMember(Value = "Slaton")]
        [Description("Slaton")]
        Slaton,
        [EnumMember(Value = "SMILE")]
        [Description("Smiley")]
        Smiley,
        [EnumMember(Value = "SMITH")]
        [Description("Smithville")]
        Smithville,
        [EnumMember(Value = "Smyer")]
        [Description("Smyer")]
        Smyer,
        [EnumMember(Value = "Snook")]
        [Description("Snook")]
        Snook,
        [EnumMember(Value = "Snyder")]
        [Description("Snyder")]
        Snyder,
        [EnumMember(Value = "Socorro")]
        [Description("Socorro")]
        Socorro,
        [EnumMember(Value = "Solis")]
        [Description("Solis")]
        Solis,
        [EnumMember(Value = "SOMER")]
        [Description("Somerset")]
        Somerset,
        [EnumMember(Value = "Somerville")]
        [Description("Somerville")]
        Somerville,
        [EnumMember(Value = "Sonora")]
        [Description("Sonora")]
        Sonora,
        [EnumMember(Value = "SourLake")]
        [Description("Sour Lake")]
        SourLake,
        [EnumMember(Value = "SouthAlamo")]
        [Description("South Alamo")]
        SouthAlamo,
        [EnumMember(Value = "SouthForkEstates")]
        [Description("South Fork Estates")]
        SouthForkEstates,
        [EnumMember(Value = "SouthHouston")]
        [Description("South Houston")]
        SouthHouston,
        [EnumMember(Value = "SouthLaPaloma")]
        [Description("South La Paloma")]
        SouthLaPaloma,
        [EnumMember(Value = "SouthMountain")]
        [Description("South Mountain")]
        SouthMountain,
        [EnumMember(Value = "SouthPadreIsland")]
        [Description("South Padre Island")]
        SouthPadreIsland,
        [EnumMember(Value = "SouthPoint")]
        [Description("South Point")]
        SouthPoint,
        [EnumMember(Value = "SouthToledoBend")]
        [Description("South Toledo Bend")]
        SouthToledoBend,
        [EnumMember(Value = "Southlake")]
        [Description("Southlake")]
        Southlake,
        [EnumMember(Value = "Southmayd")]
        [Description("Southmayd")]
        Southmayd,
        [EnumMember(Value = "SouthsidePlace")]
        [Description("Southside Place")]
        SouthsidePlace,
        [EnumMember(Value = "Spade")]
        [Description("Spade")]
        Spade,
        [EnumMember(Value = "Sparks")]
        [Description("Sparks")]
        Sparks,
        [EnumMember(Value = "Spearman")]
        [Description("Spearman")]
        Spearman,
        [EnumMember(Value = "SPICE")]
        [Description("Spicewood")]
        Spicewood,
        [EnumMember(Value = "Splendora")]
        [Description("Splendora")]
        Splendora,
        [EnumMember(Value = "Spofford")]
        [Description("Spofford")]
        Spofford,
        [EnumMember(Value = "Spring")]
        [Description("Spring")]
        Spring,
        [EnumMember(Value = "SPRIN")]
        [Description("Spring Branch")]
        SpringBranch,
        [EnumMember(Value = "SpringGardens")]
        [Description("Spring Gardens")]
        SpringGardens,
        [EnumMember(Value = "SpringValleyVillage")]
        [Description("Spring Valley Village")]
        SpringValleyVillage,
        [EnumMember(Value = "Springlake")]
        [Description("Springlake")]
        Springlake,
        [EnumMember(Value = "Springtown")]
        [Description("Springtown")]
        Springtown,
        [EnumMember(Value = "Spur")]
        [Description("Spur")]
        Spur,
        [EnumMember(Value = "StJo")]
        [Description("St. Jo")]
        StJo,
        [EnumMember(Value = "StPaul")]
        [Description("St. Paul")]
        StPaul,
        [EnumMember(Value = "Stafford")]
        [Description("Stafford")]
        Stafford,
        [EnumMember(Value = "Stagecoach")]
        [Description("Stagecoach")]
        Stagecoach,
        [EnumMember(Value = "STAIR")]
        [Description("Stairtown")]
        Stairtown,
        [EnumMember(Value = "Stamford")]
        [Description("Stamford")]
        Stamford,
        [EnumMember(Value = "Stanton")]
        [Description("Stanton")]
        Stanton,
        [EnumMember(Value = "STAPL")]
        [Description("Staples")]
        Staples,
        [EnumMember(Value = "StarHarbor")]
        [Description("Star Harbor")]
        StarHarbor,
        [EnumMember(Value = "Stephenville")]
        [Description("Stephenville")]
        Stephenville,
        [EnumMember(Value = "SterlingCity")]
        [Description("Sterling City")]
        SterlingCity,
        [EnumMember(Value = "Stinnett")]
        [Description("Stinnett")]
        Stinnett,
        [EnumMember(Value = "STOCK")]
        [Description("Stockdale")]
        Stockdale,
        [EnumMember(Value = "STONE")]
        [Description("Stonewall")]
        Stonewall,
        [EnumMember(Value = "Stowell")]
        [Description("Stowell")]
        Stowell,
        [EnumMember(Value = "Stratford")]
        [Description("Stratford")]
        Stratford,
        [EnumMember(Value = "Strawn")]
        [Description("Strawn")]
        Strawn,
        [EnumMember(Value = "Streetman")]
        [Description("Streetman")]
        Streetman,
        [EnumMember(Value = "StudyButte")]
        [Description("Study Butte")]
        StudyButte,
        [EnumMember(Value = "Sublime")]
        [Description("Sublime")]
        Sublime,
        [EnumMember(Value = "Sudan")]
        [Description("Sudan")]
        Sudan,
        [EnumMember(Value = "SugarLand")]
        [Description("Sugar Land")]
        SugarLand,
        [EnumMember(Value = "SullivanCity")]
        [Description("Sullivan City")]
        SullivanCity,
        [EnumMember(Value = "SulphurSprings")]
        [Description("Sulphur Springs")]
        SulphurSprings,
        [EnumMember(Value = "SunValley")]
        [Description("Sun Valley")]
        SunValley,
        [EnumMember(Value = "Sundown")]
        [Description("Sundown")]
        Sundown,
        [EnumMember(Value = "Sunnyvale")]
        [Description("Sunnyvale")]
        Sunnyvale,
        [EnumMember(Value = "Sunray")]
        [Description("Sunray")]
        Sunray,
        [EnumMember(Value = "SUNRI")]
        [Description("Sunrise Beach")]
        SunriseBeach,
        [EnumMember(Value = "SunriseBeachVillage")]
        [Description("Sunrise Beach Village")]
        SunriseBeachVillage,
        [EnumMember(Value = "Sunset")]
        [Description("Sunset")]
        Sunset,
        [EnumMember(Value = "SunsetAcres")]
        [Description("Sunset Acres")]
        SunsetAcres,
        [EnumMember(Value = "SunsetValley")]
        [Description("Sunset Valley")]
        SunsetValley,
        [EnumMember(Value = "SurfsideBeach")]
        [Description("Surfside Beach")]
        SurfsideBeach,
        [EnumMember(Value = "SUTHE")]
        [Description("Sutherland Springs")]
        SutherlandSprings,
        [EnumMember(Value = "Sweeny")]
        [Description("Sweeny")]
        Sweeny,
        [EnumMember(Value = "SweetHome")]
        [Description("Sweet Home")]
        SweetHome,
        [EnumMember(Value = "Sweetwater")]
        [Description("Sweetwater")]
        Sweetwater,
        [EnumMember(Value = "Taft")]
        [Description("Taft")]
        Taft,
        [EnumMember(Value = "TaftSouthwest")]
        [Description("Taft Southwest")]
        TaftSouthwest,
        [EnumMember(Value = "Tahoka")]
        [Description("Tahoka")]
        Tahoka,
        [EnumMember(Value = "Talco")]
        [Description("Talco")]
        Talco,
        [EnumMember(Value = "Talty")]
        [Description("Talty")]
        Talty,
        [EnumMember(Value = "TanquecitosSouthAcres")]
        [Description("Tanquecitos South Acres")]
        TanquecitosSouthAcres,
        [EnumMember(Value = "TanquecitosSouthAcresII")]
        [Description("Tanquecitos South Acres II")]
        TanquecitosSouthAcresII,
        [EnumMember(Value = "Tatum")]
        [Description("Tatum")]
        Tatum,
        [EnumMember(Value = "Taylor")]
        [Description("Taylor")]
        Taylor,
        [EnumMember(Value = "TaylorLakeVillage")]
        [Description("Taylor Lake Village")]
        TaylorLakeVillage,
        [EnumMember(Value = "TaylorLanding")]
        [Description("Taylor Landing")]
        TaylorLanding,
        [EnumMember(Value = "Teague")]
        [Description("Teague")]
        Teague,
        [EnumMember(Value = "Tehuacana")]
        [Description("Tehuacana")]
        Tehuacana,
        [EnumMember(Value = "Telferner")]
        [Description("Telferner")]
        Telferner,
        [EnumMember(Value = "TEMPL")]
        [Description("Temple")]
        Temple,
        [EnumMember(Value = "Tenaha")]
        [Description("Tenaha")]
        Tenaha,
        [EnumMember(Value = "TennesseeColony")]
        [Description("Tennessee Colony")]
        TennesseeColony,
        [EnumMember(Value = "Terlingua")]
        [Description("Terlingua")]
        Terlingua,
        [EnumMember(Value = "Terrell")]
        [Description("Terrell")]
        Terrell,
        [EnumMember(Value = "TerrellHills")]
        [Description("Terrell Hills")]
        TerrellHills,
        [EnumMember(Value = "Texarkana")]
        [Description("Texarkana")]
        Texarkana,
        [EnumMember(Value = "TexasCity")]
        [Description("Texas City")]
        TexasCity,
        [EnumMember(Value = "Texhoma")]
        [Description("Texhoma")]
        Texhoma,
        [EnumMember(Value = "Texline")]
        [Description("Texline")]
        Texline,
        [EnumMember(Value = "TheColony")]
        [Description("The Colony")]
        TheColony,
        [EnumMember(Value = "TheHills")]
        [Description("The Hills")]
        TheHills,
        [EnumMember(Value = "TheWoodlands")]
        [Description("The Woodlands")]
        TheWoodlands,
        [EnumMember(Value = "Thomaston")]
        [Description("Thomaston")]
        Thomaston,
        [EnumMember(Value = "Thompsons")]
        [Description("Thompsons")]
        Thompsons,
        [EnumMember(Value = "Thompsonville")]
        [Description("Thompsonville")]
        Thompsonville,
        [EnumMember(Value = "Thorndale")]
        [Description("Thorndale")]
        Thorndale,
        [EnumMember(Value = "Thornton")]
        [Description("Thornton")]
        Thornton,
        [EnumMember(Value = "Thorntonville")]
        [Description("Thorntonville")]
        Thorntonville,
        [EnumMember(Value = "Thrall")]
        [Description("Thrall")]
        Thrall,
        [EnumMember(Value = "ThreeRivers")]
        [Description("Three Rivers")]
        ThreeRivers,
        [EnumMember(Value = "Throckmorton")]
        [Description("Throckmorton")]
        Throckmorton,
        [EnumMember(Value = "ThunderbirdBay")]
        [Description("Thunderbird Bay")]
        ThunderbirdBay,
        [EnumMember(Value = "TierraBonita")]
        [Description("Tierra Bonita")]
        TierraBonita,
        [EnumMember(Value = "TierraDorada")]
        [Description("Tierra Dorada")]
        TierraDorada,
        [EnumMember(Value = "TierraGrande")]
        [Description("Tierra Grande")]
        TierraGrande,
        [EnumMember(Value = "TierraVerde")]
        [Description("Tierra Verde")]
        TierraVerde,
        [EnumMember(Value = "TikiIsland")]
        [Description("Tiki Island")]
        TikiIsland,
        [EnumMember(Value = "Tilden")]
        [Description("Tilden")]
        Tilden,
        [EnumMember(Value = "TimbercreekCanyon")]
        [Description("Timbercreek Canyon")]
        TimbercreekCanyon,
        [EnumMember(Value = "TimberwoodPark")]
        [Description("Timberwood Park")]
        TimberwoodPark,
        [EnumMember(Value = "Timpson")]
        [Description("Timpson")]
        Timpson,
        [EnumMember(Value = "Tioga")]
        [Description("Tioga")]
        Tioga,
        [EnumMember(Value = "Tira")]
        [Description("Tira")]
        Tira,
        [EnumMember(Value = "Tivoli")]
        [Description("Tivoli")]
        Tivoli,
        [EnumMember(Value = "Toco")]
        [Description("Toco")]
        Toco,
        [EnumMember(Value = "ToddMission")]
        [Description("Todd Mission")]
        ToddMission,
        [EnumMember(Value = "Tolar")]
        [Description("Tolar")]
        Tolar,
        [EnumMember(Value = "TomBean")]
        [Description("Tom Bean")]
        TomBean,
        [EnumMember(Value = "Tomball")]
        [Description("Tomball")]
        Tomball,
        [EnumMember(Value = "Tool")]
        [Description("Tool")]
        Tool,
        [EnumMember(Value = "Topsey")]
        [Description("Topsey")]
        Topsey,
        [EnumMember(Value = "Tornillo")]
        [Description("Tornillo")]
        Tornillo,
        [EnumMember(Value = "Tow")]
        [Description("Tow")]
        Tow,
        [EnumMember(Value = "Toyah")]
        [Description("Toyah")]
        Toyah,
        [EnumMember(Value = "Tradewinds")]
        [Description("Tradewinds")]
        Tradewinds,
        [EnumMember(Value = "TravisRanch")]
        [Description("Travis Ranch")]
        TravisRanch,
        [EnumMember(Value = "Trent")]
        [Description("Trent")]
        Trent,
        [EnumMember(Value = "Trenton")]
        [Description("Trenton")]
        Trenton,
        [EnumMember(Value = "Trinidad")]
        [Description("Trinidad")]
        Trinidad,
        [EnumMember(Value = "Trinity")]
        [Description("Trinity")]
        Trinity,
        [EnumMember(Value = "TrophyClub")]
        [Description("Trophy Club")]
        TrophyClub,
        [EnumMember(Value = "Troup")]
        [Description("Troup")]
        Troup,
        [EnumMember(Value = "Troy")]
        [Description("Troy")]
        Troy,
        [EnumMember(Value = "Tuleta")]
        [Description("Tuleta")]
        Tuleta,
        [EnumMember(Value = "Tulia")]
        [Description("Tulia")]
        Tulia,
        [EnumMember(Value = "Tulsita")]
        [Description("Tulsita")]
        Tulsita,
        [EnumMember(Value = "Turkey")]
        [Description("Turkey")]
        Turkey,
        [EnumMember(Value = "Tuscola")]
        [Description("Tuscola")]
        Tuscola,
        [EnumMember(Value = "Tye")]
        [Description("Tye")]
        Tye,
        [EnumMember(Value = "Tyler")]
        [Description("Tyler")]
        Tyler,
        [EnumMember(Value = "Tynan")]
        [Description("Tynan")]
        Tynan,
        [EnumMember(Value = "UHLAN")]
        [Description("Uhland")]
        Uhland,
        [EnumMember(Value = "UnionGrove")]
        [Description("Union Grove")]
        UnionGrove,
        [EnumMember(Value = "UnionValley")]
        [Description("Union Valley")]
        UnionValley,
        [EnumMember(Value = "UNIVE")]
        [Description("Universal City")]
        UniversalCity,
        [EnumMember(Value = "UniversityPark")]
        [Description("University Park")]
        UniversityPark,
        [EnumMember(Value = "Utopia")]
        [Description("Utopia")]
        Utopia,
        [EnumMember(Value = "Uvalde")]
        [Description("Uvalde")]
        Uvalde,
        [EnumMember(Value = "UvaldeEstates")]
        [Description("Uvalde Estates")]
        UvaldeEstates,
        [EnumMember(Value = "ValVerdePark")]
        [Description("Val Verde Park")]
        ValVerdePark,
        [EnumMember(Value = "Valentine")]
        [Description("Valentine")]
        Valentine,
        [EnumMember(Value = "ValleHermoso")]
        [Description("Valle Hermoso")]
        ValleHermoso,
        [EnumMember(Value = "ValleVerde")]
        [Description("Valle Verde")]
        ValleVerde,
        [EnumMember(Value = "ValleVista")]
        [Description("Valle Vista")]
        ValleVista,
        [EnumMember(Value = "ValleyMills")]
        [Description("Valley Mills")]
        ValleyMills,
        [EnumMember(Value = "ValleyView")]
        [Description("Valley View")]
        ValleyView,
        [EnumMember(Value = "Van")]
        [Description("Van")]
        Van,
        [EnumMember(Value = "VanAlstyne")]
        [Description("Van Alstyne")]
        VanAlstyne,
        [EnumMember(Value = "VanHorn")]
        [Description("Van Horn")]
        VanHorn,
        [EnumMember(Value = "VANVL")]
        [Description("Van Vleck")]
        VanVleck,
        [EnumMember(Value = "Vanderbilt")]
        [Description("Vanderbilt")]
        Vanderbilt,
        [EnumMember(Value = "Vanderpool")]
        [Description("Vanderpool")]
        Vanderpool,
        [EnumMember(Value = "Vega")]
        [Description("Vega")]
        Vega,
        [EnumMember(Value = "VENTU")]
        [Description("Venture")]
        Venture,
        [EnumMember(Value = "Venus")]
        [Description("Venus")]
        Venus,
        [EnumMember(Value = "Vernon")]
        [Description("Vernon")]
        Vernon,
        [EnumMember(Value = "VICTO")]
        [Description("Victoria")]
        Victoria,
        [EnumMember(Value = "VictoriaVera")]
        [Description("Victoria Vera")]
        VictoriaVera,
        [EnumMember(Value = "Vidor")]
        [Description("Vidor")]
        Vidor,
        [EnumMember(Value = "VilladelSol")]
        [Description("Villa del Sol")]
        VillaDelSol,
        [EnumMember(Value = "VillaPancho")]
        [Description("Villa Pancho")]
        VillaPancho,
        [EnumMember(Value = "VillaVerde")]
        [Description("Villa Verde")]
        VillaVerde,
        [EnumMember(Value = "Villarreal")]
        [Description("Villarreal")]
        Villarreal,
        [EnumMember(Value = "Vinton")]
        [Description("Vinton")]
        Vinton,
        [EnumMember(Value = "Volente")]
        [Description("Volente")]
        Volente,
        [EnumMember(Value = "VONOR")]
        [Description("Von Ormy")]
        VonOrmy,
        [EnumMember(Value = "Waco")]
        [Description("Waco")]
        Waco,
        [EnumMember(Value = "WAELD")]
        [Description("Waelder")]
        Waelder,
        [EnumMember(Value = "WakeVillage")]
        [Description("Wake Village")]
        WakeVillage,
        [EnumMember(Value = "Waller")]
        [Description("Waller")]
        Waller,
        [EnumMember(Value = "Wallis")]
        [Description("Wallis")]
        Wallis,
        [EnumMember(Value = "WalnutSprings")]
        [Description("Walnut Springs")]
        WalnutSprings,
        [EnumMember(Value = "Warren")]
        [Description("Warren")]
        Warren,
        [EnumMember(Value = "WarrenCity")]
        [Description("Warren City")]
        WarrenCity,
        [EnumMember(Value = "Warrenton")]
        [Description("Warrenton")]
        Warrenton,
        [EnumMember(Value = "Washington")]
        [Description("Washington")]
        Washington,
        [EnumMember(Value = "Waskom")]
        [Description("Waskom")]
        Waskom,
        [EnumMember(Value = "Watauga")]
        [Description("Watauga")]
        Watauga,
        [EnumMember(Value = "Waxahachie")]
        [Description("Waxahachie")]
        Waxahachie,
        [EnumMember(Value = "Weatherford")]
        [Description("Weatherford")]
        Weatherford,
        [EnumMember(Value = "Webberville")]
        [Description("Webberville")]
        Webberville,
        [EnumMember(Value = "Webster")]
        [Description("Webster")]
        Webster,
        [EnumMember(Value = "Weesatche")]
        [Description("Weesatche")]
        Weesatche,
        [EnumMember(Value = "Weimar")]
        [Description("Weimar")]
        Weimar,
        [EnumMember(Value = "Weinert")]
        [Description("Weinert")]
        Weinert,
        [EnumMember(Value = "Weir")]
        [Description("Weir")]
        Weir,
        [EnumMember(Value = "Welch")]
        [Description("Welch")]
        Welch,
        [EnumMember(Value = "Wellington")]
        [Description("Wellington")]
        Wellington,
        [EnumMember(Value = "Wellman")]
        [Description("Wellman")]
        Wellman,
        [EnumMember(Value = "Wells")]
        [Description("Wells")]
        Wells,
        [EnumMember(Value = "WellsBranch")]
        [Description("Wells Branch")]
        WellsBranch,
        [EnumMember(Value = "Weslaco")]
        [Description("Weslaco")]
        Weslaco,
        [EnumMember(Value = "West")]
        [Description("West")]
        West,
        [EnumMember(Value = "WestAltoBonito")]
        [Description("West Alto Bonito")]
        WestAltoBonito,
        [EnumMember(Value = "WestColumbia")]
        [Description("West Columbia")]
        WestColumbia,
        [EnumMember(Value = "WestLakeHills")]
        [Description("West Lake Hills")]
        WestLakeHills,
        [EnumMember(Value = "WestLivingston")]
        [Description("West Livingston")]
        WestLivingston,
        [EnumMember(Value = "WestOdessa")]
        [Description("West Odessa")]
        WestOdessa,
        [EnumMember(Value = "WestOrange")]
        [Description("West Orange")]
        WestOrange,
        [EnumMember(Value = "WestSharyland")]
        [Description("West Sharyland")]
        WestSharyland,
        [EnumMember(Value = "WestTawakoni")]
        [Description("West Tawakoni")]
        WestTawakoni,
        [EnumMember(Value = "WestUniversityPlace")]
        [Description("West University Place")]
        WestUniversityPlace,
        [EnumMember(Value = "Westbrook")]
        [Description("Westbrook")]
        Westbrook,
        [EnumMember(Value = "Westdale")]
        [Description("Westdale")]
        Westdale,
        [EnumMember(Value = "WesternLake")]
        [Description("Western Lake")]
        WesternLake,
        [EnumMember(Value = "WESTH")]
        [Description("Westhoff")]
        Westhoff,
        [EnumMember(Value = "Westlake")]
        [Description("Westlake")]
        Westlake,
        [EnumMember(Value = "Westminster")]
        [Description("Westminster")]
        Westminster,
        [EnumMember(Value = "Weston")]
        [Description("Weston")]
        Weston,
        [EnumMember(Value = "WestonLakes")]
        [Description("Weston Lakes")]
        WestonLakes,
        [EnumMember(Value = "WestoverHills")]
        [Description("Westover Hills")]
        WestoverHills,
        [EnumMember(Value = "Westphalia")]
        [Description("Westphalia")]
        Westphalia,
        [EnumMember(Value = "Westway")]
        [Description("Westway")]
        Westway,
        [EnumMember(Value = "WestwoodShores")]
        [Description("Westwood Shores")]
        WestwoodShores,
        [EnumMember(Value = "WestworthVillage")]
        [Description("Westworth Village")]
        WestworthVillage,
        [EnumMember(Value = "Wharton")]
        [Description("Wharton")]
        Wharton,
        [EnumMember(Value = "Wheeler")]
        [Description("Wheeler")]
        Wheeler,
        [EnumMember(Value = "WhiteDeer")]
        [Description("White Deer")]
        WhiteDeer,
        [EnumMember(Value = "WhiteOak")]
        [Description("White Oak")]
        WhiteOak,
        [EnumMember(Value = "WhiteSettlement")]
        [Description("White Settlement")]
        WhiteSettlement,
        [EnumMember(Value = "Whiteface")]
        [Description("Whiteface")]
        Whiteface,
        [EnumMember(Value = "Whitehouse")]
        [Description("Whitehouse")]
        Whitehouse,
        [EnumMember(Value = "Whitesboro")]
        [Description("Whitesboro")]
        Whitesboro,
        [EnumMember(Value = "Whitewright")]
        [Description("Whitewright")]
        Whitewright,
        [EnumMember(Value = "Whitney")]
        [Description("Whitney")]
        Whitney,
        [EnumMember(Value = "Whitsett")]
        [Description("Whitsett")]
        Whitsett,
        [EnumMember(Value = "WichitaFalls")]
        [Description("Wichita Falls")]
        WichitaFalls,
        [EnumMember(Value = "Wickett")]
        [Description("Wickett")]
        Wickett,
        [EnumMember(Value = "WildPeachVillage")]
        [Description("Wild Peach Village")]
        WildPeachVillage,
        [EnumMember(Value = "Wildwood")]
        [Description("Wildwood")]
        Wildwood,
        [EnumMember(Value = "Willis")]
        [Description("Willis")]
        Willis,
        [EnumMember(Value = "WillowPark")]
        [Description("Willow Park")]
        WillowPark,
        [EnumMember(Value = "WillsPoint")]
        [Description("Wills Point")]
        WillsPoint,
        [EnumMember(Value = "Wilmer")]
        [Description("Wilmer")]
        Wilmer,
        [EnumMember(Value = "Wilson")]
        [Description("Wilson")]
        Wilson,
        [EnumMember(Value = "WIMBE")]
        [Description("Wimberley")]
        Wimberley,
        [EnumMember(Value = "WINDC")]
        [Description("Windcrest")]
        Windcrest,
        [EnumMember(Value = "Windemere")]
        [Description("Windemere")]
        Windemere,
        [EnumMember(Value = "Windom")]
        [Description("Windom")]
        Windom,
        [EnumMember(Value = "Windthorst")]
        [Description("Windthorst")]
        Windthorst,
        [EnumMember(Value = "Winfield")]
        [Description("Winfield")]
        Winfield,
        [EnumMember(Value = "Wink")]
        [Description("Wink")]
        Wink,
        [EnumMember(Value = "Winnie")]
        [Description("Winnie")]
        Winnie,
        [EnumMember(Value = "Winnsboro")]
        [Description("Winnsboro")]
        Winnsboro,
        [EnumMember(Value = "Winona")]
        [Description("Winona")]
        Winona,
        [EnumMember(Value = "Winters")]
        [Description("Winters")]
        Winters,
        [EnumMember(Value = "WixonValley")]
        [Description("Wixon Valley")]
        WixonValley,
        [EnumMember(Value = "WolfeCity")]
        [Description("Wolfe City")]
        WolfeCity,
        [EnumMember(Value = "Wolfforth")]
        [Description("Wolfforth")]
        Wolfforth,
        [EnumMember(Value = "Woodhi")]
        [Description("Wood Hi")]
        WoodHi,
        [EnumMember(Value = "Woodbranch")]
        [Description("Woodbranch")]
        Woodbranch,
        [EnumMember(Value = "WOODC")]
        [Description("Woodcreek")]
        Woodcreek,
        [EnumMember(Value = "Woodloch")]
        [Description("Woodloch")]
        Woodloch,
        [EnumMember(Value = "Woodsboro")]
        [Description("Woodsboro")]
        Woodsboro,
        [EnumMember(Value = "Woodson")]
        [Description("Woodson")]
        Woodson,
        [EnumMember(Value = "Woodville")]
        [Description("Woodville")]
        Woodville,
        [EnumMember(Value = "Woodway")]
        [Description("Woodway")]
        Woodway,
        [EnumMember(Value = "Wortham")]
        [Description("Wortham")]
        Wortham,
        [EnumMember(Value = "WRIGH")]
        [Description("Wrightsboro")]
        Wrightsboro,
        [EnumMember(Value = "Wyldwood")]
        [Description("Wyldwood")]
        Wyldwood,
        [EnumMember(Value = "Wylie")]
        [Description("Wylie")]
        Wylie,
        [EnumMember(Value = "Yantis")]
        [Description("Yantis")]
        Yantis,
        [EnumMember(Value = "YOAKU")]
        [Description("Yoakum")]
        Yoakum,
        [EnumMember(Value = "YORKT")]
        [Description("Yorktown")]
        Yorktown,
        [EnumMember(Value = "Youngsport")]
        [Description("Youngsport")]
        Youngsport,
        [EnumMember(Value = "Yznaga")]
        [Description("Yznaga")]
        Yznaga,
        [EnumMember(Value = "Zapata")]
        [Description("Zapata")]
        Zapata,
        [EnumMember(Value = "ZapataRanch")]
        [Description("Zapata Ranch")]
        ZapataRanch,
        [EnumMember(Value = "Zarate")]
        [Description("Zarate")]
        Zarate,
        [EnumMember(Value = "Zavalla")]
        [Description("Zavalla")]
        Zavalla,
        [EnumMember(Value = "Zuehl")]
        [Description("Zuehl")]
        Zuehl,
    }
}
