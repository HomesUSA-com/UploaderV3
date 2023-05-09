namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SchoolDistrict
    {
        [EnumMember(Value = "ABBOTTISD")]
        [Description("Abbott ISD")]
        Abbott,
        [EnumMember(Value = "ABERNATHYISD")]
        [Description("Abernathy ISD")]
        Abernathy,
        [EnumMember(Value = "ABILENEISD")]
        [Description("Abilene ISD")]
        Abilene,
        [EnumMember(Value = "ACADEMYISD")]
        [Description("Academy ISD")]
        Academy,
        [EnumMember(Value = "ADRIANISD")]
        [Description("Adrian ISD")]
        Adrian,
        [EnumMember(Value = "AGUADULCEISD")]
        [Description("Agua Dulce ISD")]
        AguaDulce,
        [EnumMember(Value = "Alamo")]
        [Description("Alamo Heights ISD")]
        AlamoHeights,
        [EnumMember(Value = "ALBAGOLDENISD")]
        [Description("Alba-Golden ISD")]
        AlbaGolden,
        [EnumMember(Value = "ALBANYISD")]
        [Description("Albany ISD")]
        Albany,
        [EnumMember(Value = "ALDINEISD")]
        [Description("Aldine ISD")]
        Aldine,
        [EnumMember(Value = "ALEDOISD")]
        [Description("Aledo ISD")]
        Aledo,
        [EnumMember(Value = "ALICEISD")]
        [Description("Alice ISD")]
        Alice,
        [EnumMember(Value = "ALIEFISD")]
        [Description("Alief ISD")]
        Alief,
        [EnumMember(Value = "ALLENISD")]
        [Description("Allen ISD")]
        Allen,
        [EnumMember(Value = "ALPINEISD")]
        [Description("Alpine ISD")]
        Alpine,
        [EnumMember(Value = "ALTOISD")]
        [Description("Alto ISD")]
        Alto,
        [EnumMember(Value = "ALVARADOISD")]
        [Description("Alvarado ISD")]
        Alvarado,
        [EnumMember(Value = "ALVINISD")]
        [Description("Alvin ISD")]
        Alvin,
        [EnumMember(Value = "ALVORDISD")]
        [Description("Alvord ISD")]
        Alvord,
        [EnumMember(Value = "AMARILLOISD")]
        [Description("Amarillo ISD")]
        Amarillo,
        [EnumMember(Value = "AMHERSTISD")]
        [Description("Amherst ISD")]
        Amherst,
        [EnumMember(Value = "ANAHUACISD")]
        [Description("Anahuac ISD")]
        Anahuac,
        [EnumMember(Value = "ANDERSONSHIROCONSISD")]
        [Description("Anderson-Shiro Consolidated ISD")]
        AndersonShiroConsolidated,
        [EnumMember(Value = "ANDREWSISD")]
        [Description("Andrews ISD")]
        Andrews,
        [EnumMember(Value = "ANGLETONISD")]
        [Description("Angleton ISD")]
        Angleton,
        [EnumMember(Value = "ANNAISD")]
        [Description("Anna ISD")]
        Anna,
        [EnumMember(Value = "ANSONISD")]
        [Description("Anson ISD")]
        Anson,
        [EnumMember(Value = "ANTHONYISD")]
        [Description("Anthony ISD")]
        Anthony,
        [EnumMember(Value = "ANTONISD")]
        [Description("Anton ISD")]
        Anton,
        [EnumMember(Value = "APPLESPRINGSISD")]
        [Description("Apple Springs ISD")]
        AppleSprings,
        [EnumMember(Value = "AQUILLAISD")]
        [Description("Aquilla ISD")]
        Aquilla,
        [EnumMember(Value = "ARANSASCOUNTYISD")]
        [Description("Aransas County ISD")]
        AransasCounty,
        [EnumMember(Value = "ARANSASPASSISD")]
        [Description("Aransas Pass ISD")]
        AransasPass,
        [EnumMember(Value = "ARCHERCITYISD")]
        [Description("Archer City ISD")]
        ArcherCity,
        [EnumMember(Value = "ARGYLEISD")]
        [Description("Argyle ISD")]
        Argyle,
        [EnumMember(Value = "ARLINGTONISD")]
        [Description("Arlington ISD")]
        Arlington,
        [EnumMember(Value = "ARPISD")]
        [Description("Arp ISD")]
        Arp,
        [EnumMember(Value = "ASPERMONTISD")]
        [Description("Aspermont ISD")]
        Aspermont,
        [EnumMember(Value = "ATHENSISD")]
        [Description("Athens ISD")]
        Athens,
        [EnumMember(Value = "ATLANTAISD")]
        [Description("Atlanta ISD")]
        Atlanta,
        [EnumMember(Value = "AUBREYISD")]
        [Description("Aubrey ISD")]
        Aubrey,
        [EnumMember(Value = "Austin")]
        [Description("Austin ISD")]
        Austin,
        [EnumMember(Value = "AUSTWELLTIVOLIISD")]
        [Description("Austwell-Tivoli ISD")]
        AustwellTivoli,
        [EnumMember(Value = "AVALONISD")]
        [Description("Avalon ISD")]
        Avalon,
        [EnumMember(Value = "AVERYISD")]
        [Description("Avery ISD")]
        Avery,
        [EnumMember(Value = "AVINGERISD")]
        [Description("Avinger ISD")]
        Avinger,
        [EnumMember(Value = "AXTELLISD")]
        [Description("Axtell ISD")]
        Axtell,
        [EnumMember(Value = "AZLEISD")]
        [Description("Azle ISD")]
        Azle,
        [EnumMember(Value = "BAIRDISD")]
        [Description("Baird ISD")]
        Baird,
        [EnumMember(Value = "BALLINGERISD")]
        [Description("Ballinger ISD")]
        Ballinger,
        [EnumMember(Value = "BALMORHEAISD")]
        [Description("Balmorhea I.S.D.")]
        Balmorhea,
        [EnumMember(Value = "Bandera")]
        [Description("Bandera ISD")]
        Bandera,
        [EnumMember(Value = "BANGSISD")]
        [Description("Bangs ISD")]
        Bangs,
        [EnumMember(Value = "BANQUETEISD")]
        [Description("Banquete ISD")]
        Banquete,
        [EnumMember(Value = "BARBERSHILLISD")]
        [Description("Barbers Hill ISD")]
        BarbersHill,
        [EnumMember(Value = "BARTLETTISD")]
        [Description("Bartlett ISD")]
        Bartlett,
        [EnumMember(Value = "Bastrop")]
        [Description("Bastrop ISD")]
        Bastrop,
        [EnumMember(Value = "BAYCITYISD")]
        [Description("Bay City ISD")]
        BayCity,
        [EnumMember(Value = "BEAUMONTISD")]
        [Description("Beaumont ISD")]
        Beaumont,
        [EnumMember(Value = "BECKVILLEISD")]
        [Description("Beckville ISD")]
        Beckville,
        [EnumMember(Value = "BEEVILLEISD")]
        [Description("Beeville ISD")]
        Beeville,
        [EnumMember(Value = "BELLEVUEISD")]
        [Description("Bellevue ISD")]
        Bellevue,
        [EnumMember(Value = "BELLSISD")]
        [Description("Bells ISD")]
        Bells,
        [EnumMember(Value = "BELLVILLEISD")]
        [Description("Bellville ISD")]
        Bellville,
        [EnumMember(Value = "BELTONISD")]
        [Description("Belton ISD")]
        Belton,
        [EnumMember(Value = "BENBOLTPALITOBLANCOISD")]
        [Description("Ben Bolt-Palito Blanco ISD")]
        BenBoltPalitoBlanco,
        [EnumMember(Value = "BENAVIDESISD")]
        [Description("Benavides ISD")]
        Benavides,
        [EnumMember(Value = "BENJAMINISD")]
        [Description("Benjamin ISD")]
        Benjamin,
        [EnumMember(Value = "BIGSANDYISD")]
        [Description("Big Sandy ISD")]
        BigSandy,
        [EnumMember(Value = "BIGSPRINGISD")]
        [Description("Big Spring ISD")]
        BigSpring,
        [EnumMember(Value = "BIRDVILLEISD")]
        [Description("Birdville ISD")]
        Birdville,
        [EnumMember(Value = "BISHOPCONSISD")]
        [Description("Bishop Consolidated ISD")]
        BishopConsolidated,
        [EnumMember(Value = "BLACKWELLCONSISD")]
        [Description("Blackwell Consolidated ISD")]
        BlackwellConsolidated,
        [EnumMember(Value = "Blanco")]
        [Description("Blanco ISD")]
        Blanco,
        [EnumMember(Value = "BLANDISD")]
        [Description("Bland ISD")]
        Bland,
        [EnumMember(Value = "BLANKETISD")]
        [Description("Blanket ISD")]
        Blanket,
        [EnumMember(Value = "BLOOMBURGISD")]
        [Description("Bloomburg ISD")]
        Bloomburg,
        [EnumMember(Value = "BLOOMINGGROVEISD")]
        [Description("Blooming Grove ISD")]
        BloomingGrove,
        [EnumMember(Value = "BLOOMINGTONISD")]
        [Description("Bloomington ISD")]
        Bloomington,
        [EnumMember(Value = "BLUERIDGEISD")]
        [Description("Blue Ridge ISD")]
        BlueRidge,
        [EnumMember(Value = "BLUFFDALEISD")]
        [Description("Bluff Dale ISD")]
        BluffDale,
        [EnumMember(Value = "BLUMISD")]
        [Description("Blum ISD")]
        Blum,
        [EnumMember(Value = "Boerne")]
        [Description("Boerne ISD")]
        Boerne,
        [EnumMember(Value = "BOLESISD")]
        [Description("Boles ISD")]
        Boles,
        [EnumMember(Value = "BOLINGISD")]
        [Description("Boling ISD")]
        Boling,
        [EnumMember(Value = "BONHAMISD")]
        [Description("Bonham ISD")]
        Bonham,
        [EnumMember(Value = "BOOKERISD")]
        [Description("Booker ISD")]
        Booker,
        [EnumMember(Value = "BORDENCOUNTYISD")]
        [Description("Borden County ISD")]
        BordenCounty,
        [EnumMember(Value = "BORGERISD")]
        [Description("Borger ISD")]
        Borger,
        [EnumMember(Value = "BOSQUEVILLEISD")]
        [Description("Bosqueville ISD")]
        Bosqueville,
        [EnumMember(Value = "BOVINAISD")]
        [Description("Bovina ISD")]
        Bovina,
        [EnumMember(Value = "BOWIEISD")]
        [Description("Bowie ISD")]
        Bowie,
        [EnumMember(Value = "BOYDISD")]
        [Description("Boyd ISD")]
        Boyd,
        [EnumMember(Value = "BOYSRANCHISD")]
        [Description("Boys Ranch ISD")]
        BoysRanch,
        [EnumMember(Value = "BRACKETTISD")]
        [Description("Brackett ISD")]
        Brackett,
        [EnumMember(Value = "BRADYISD")]
        [Description("Brady ISD")]
        Brady,
        [EnumMember(Value = "BRAZOSISD")]
        [Description("Brazos ISD")]
        Brazos,
        [EnumMember(Value = "BRAZOSPORTISD")]
        [Description("Brazosport ISD")]
        Brazosport,
        [EnumMember(Value = "BRECKENRIDGEISD")]
        [Description("Breckenridge ISD")]
        Breckenridge,
        [EnumMember(Value = "BREMONDISD")]
        [Description("Bremond ISD")]
        Bremond,
        [EnumMember(Value = "BRENHAMISD")]
        [Description("Brenham ISD")]
        Brenham,
        [EnumMember(Value = "BRIDGECITYISD")]
        [Description("Bridge City ISD")]
        BridgeCity,
        [EnumMember(Value = "BRIDGEPORTISD")]
        [Description("Bridgeport ISD")]
        Bridgeport,
        [EnumMember(Value = "BROADDUSISD")]
        [Description("Broaddus ISD")]
        Broaddus,
        [EnumMember(Value = "BROCKISD")]
        [Description("Brock ISD")]
        Brock,
        [EnumMember(Value = "BRONTEISD")]
        [Description("Bronte ISD")]
        Bronte,
        [EnumMember(Value = "BROOKELANDISD")]
        [Description("Brookeland ISD")]
        Brookeland,
        [EnumMember(Value = "BROOKESMITHISD")]
        [Description("Brookesmith ISD")]
        Brookesmith,
        [EnumMember(Value = "BROOKSCOUNTYISD")]
        [Description("Brooks County ISD")]
        BrooksCounty,
        [EnumMember(Value = "BROWNFIELDISD")]
        [Description("Brownfield ISD")]
        Brownfield,
        [EnumMember(Value = "BROWNSBOROISD")]
        [Description("Brownsboro ISD")]
        Brownsboro,
        [EnumMember(Value = "BROWNSVILLEISD")]
        [Description("Brownsville ISD")]
        Brownsville,
        [EnumMember(Value = "BROWNWOODISD")]
        [Description("Brownwood ISD")]
        Brownwood,
        [EnumMember(Value = "BRUCEVILLEEDDYISD")]
        [Description("Bruceville-Eddy ISD")]
        BrucevilleEddy,
        [EnumMember(Value = "BRYANISD")]
        [Description("Bryan ISD")]
        Bryan,
        [EnumMember(Value = "BRYSONISD")]
        [Description("Bryson ISD")]
        Bryson,
        [EnumMember(Value = "BUCKHOLTSISD")]
        [Description("Buckholts ISD")]
        Buckholts,
        [EnumMember(Value = "BUENAVISTAISD")]
        [Description("Buena Vista ISD")]
        BuenaVista,
        [EnumMember(Value = "BUFFALOISD")]
        [Description("Buffalo ISD")]
        Buffalo,
        [EnumMember(Value = "BULLARDISD")]
        [Description("Bullard ISD")]
        Bullard,
        [EnumMember(Value = "BUNAISD")]
        [Description("Buna ISD")]
        Buna,
        [EnumMember(Value = "BURKBURNETTISD")]
        [Description("Burkburnett ISD")]
        Burkburnett,
        [EnumMember(Value = "BURKEVILLEISD")]
        [Description("Burkeville ISD")]
        Burkeville,
        [EnumMember(Value = "BURLESONISD")]
        [Description("Burleson ISD")]
        Burleson,
        [EnumMember(Value = "BURNETCONSISD")]
        [Description("Burnet Consolidated ISD")]
        BurnetConsolidated,
        [EnumMember(Value = "BURTONISD")]
        [Description("Burton ISD")]
        Burton,
        [EnumMember(Value = "BUSHLANDISD")]
        [Description("Bushland ISD")]
        Bushland,
        [EnumMember(Value = "BYNUMISD")]
        [Description("Bynum ISD")]
        Bynum,
        [EnumMember(Value = "CADDOMILLSISD")]
        [Description("Caddo Mills ISD")]
        CaddoMills,
        [EnumMember(Value = "CALALLENISD")]
        [Description("Calallen ISD")]
        Calallen,
        [EnumMember(Value = "CALDWELLISD")]
        [Description("Caldwell ISD")]
        Caldwell,
        [EnumMember(Value = "Calhoun")]
        [Description("Calhoun County ISD")]
        CalhounCounty,
        [EnumMember(Value = "CALLISBURGISD")]
        [Description("Callisburg ISD")]
        Callisburg,
        [EnumMember(Value = "CALVERTISD")]
        [Description("Calvert ISD")]
        Calvert,
        [EnumMember(Value = "CAMERONISD")]
        [Description("Cameron ISD")]
        Cameron,
        [EnumMember(Value = "CAMPBELLISD")]
        [Description("Campbell ISD")]
        Campbell,
        [EnumMember(Value = "CANADIANISD")]
        [Description("Canadian ISD")]
        Canadian,
        [EnumMember(Value = "CANTONISD")]
        [Description("Canton ISD")]
        Canton,
        [EnumMember(Value = "CANUTILLOISD")]
        [Description("Canutillo ISD")]
        Canutillo,
        [EnumMember(Value = "CANYONISD")]
        [Description("Canyon ISD")]
        Canyon,
        [EnumMember(Value = "CARLISLEISD")]
        [Description("Carlisle ISD")]
        Carlisle,
        [EnumMember(Value = "Carrizo")]
        [Description("Carrizo Springs Cisd")]
        CarrizoSpringsCisd,
        [EnumMember(Value = "CARROLLISD")]
        [Description("Carroll ISD")]
        Carroll,
        [EnumMember(Value = "CARROLLTONFARMERSBRANCHISD")]
        [Description("Carrollton-Farmers Branch ISD")]
        CarrolltonFarmersBranch,
        [EnumMember(Value = "CARTHAGEISD")]
        [Description("Carthage ISD")]
        Carthage,
        [EnumMember(Value = "CASTLEBERRYISD")]
        [Description("Castleberry ISD")]
        Castleberry,
        [EnumMember(Value = "CAYUGAISD")]
        [Description("Cayuga ISD")]
        Cayuga,
        [EnumMember(Value = "CEDARHILLISD")]
        [Description("Cedar Hill ISD")]
        CedarHill,
        [EnumMember(Value = "CELESTEISD")]
        [Description("Celeste ISD")]
        Celeste,
        [EnumMember(Value = "CELINAISD")]
        [Description("Celina ISD")]
        Celina,
        [EnumMember(Value = "CENTERISD")]
        [Description("Center ISD")]
        Center,
        [EnumMember(Value = "CENTERPOINTISD")]
        [Description("Center Point ISD")]
        CenterPoint,
        [EnumMember(Value = "CENTERVILLEISD")]
        [Description("Centerville ISD")]
        Centerville,
        [EnumMember(Value = "CENTRALHEIGHTSISD")]
        [Description("Central Heights ISD")]
        CentralHeights,
        [EnumMember(Value = "CENTRALISD")]
        [Description("Central ISD")]
        Central,
        [EnumMember(Value = "CHANNELVIEWISD")]
        [Description("Channelview ISD")]
        Channelview,
        [EnumMember(Value = "CHANNINGISD")]
        [Description("Channing ISD")]
        Channing,
        [EnumMember(Value = "CHAPELHILLISD")]
        [Description("Chapel Hill ISD")]
        ChapelHill,
        [EnumMember(Value = "Charlotte")]
        [Description("Charlotte ISD")]
        Charlotte,
        [EnumMember(Value = "CHEROKEEISD")]
        [Description("Cherokee ISD")]
        Cherokee,
        [EnumMember(Value = "CHESTERISD")]
        [Description("Chester ISD")]
        Chester,
        [EnumMember(Value = "CHICOISD")]
        [Description("Chico ISD")]
        Chico,
        [EnumMember(Value = "CHILDRESSISD")]
        [Description("Childress ISD")]
        Childress,
        [EnumMember(Value = "CHILLICOTHEISD")]
        [Description("Chillicothe ISD")]
        Chillicothe,
        [EnumMember(Value = "ChiltonISD")]
        [Description("Chilton Independent School District")]
        Chilton,
        [EnumMember(Value = "CHINASPRINGISD")]
        [Description("China Spring ISD")]
        ChinaSpring,
        [EnumMember(Value = "CHIRENOISD")]
        [Description("Chireno ISD")]
        Chireno,
        [EnumMember(Value = "CHISUMISD")]
        [Description("Chisum ISD")]
        Chisum,
        [EnumMember(Value = "CHRISTOVALISD")]
        [Description("Christoval ISD")]
        Christoval,
        [EnumMember(Value = "CISCOISD")]
        [Description("Cisco ISD")]
        Cisco,
        [EnumMember(Value = "CITYVIEWISD")]
        [Description("City View ISD")]
        CityView,
        [EnumMember(Value = "CLARENDONISD")]
        [Description("Clarendon ISD")]
        Clarendon,
        [EnumMember(Value = "CLARKSVILLEISD")]
        [Description("Clarksville ISD")]
        Clarksville,
        [EnumMember(Value = "CLAUDEISD")]
        [Description("Claude ISD")]
        Claude,
        [EnumMember(Value = "CLEARCREEKISD")]
        [Description("Clear Creek ISD")]
        ClearCreek,
        [EnumMember(Value = "CLEBURNEISD")]
        [Description("Cleburne ISD")]
        Cleburne,
        [EnumMember(Value = "CLEVELANDISD")]
        [Description("Cleveland ISD")]
        Cleveland,
        [EnumMember(Value = "CLIFTONISD")]
        [Description("Clifton ISD")]
        Clifton,
        [EnumMember(Value = "CLINTISD")]
        [Description("Clint ISD")]
        Clint,
        [EnumMember(Value = "CLYDECONSISD")]
        [Description("Clyde Consolidated ISD")]
        ClydeConsolidated,
        [EnumMember(Value = "COAHOMAISD")]
        [Description("Coahoma ISD")]
        Coahoma,
        [EnumMember(Value = "COLDSPRINGOAKHURSTCONSISD")]
        [Description("Coldspring-Oakhurst Consolidated ISD")]
        ColdspringOakhurstConsolidated,
        [EnumMember(Value = "COLEMANISD")]
        [Description("Coleman ISD")]
        Coleman,
        [EnumMember(Value = "COLLEGESTATIONISD")]
        [Description("College Station ISD")]
        CollegeStation,
        [EnumMember(Value = "COLLINSVILLEISD")]
        [Description("Collinsville ISD")]
        Collinsville,
        [EnumMember(Value = "COLMESNEILISD")]
        [Description("Colmesneil ISD")]
        Colmesneil,
        [EnumMember(Value = "COLORADOISD")]
        [Description("Colorado ISD")]
        Colorado,
        [EnumMember(Value = "COLUMBIABRAZORIAISD")]
        [Description("Columbia-Brazoria ISD")]
        ColumbiaBrazoria,
        [EnumMember(Value = "COLUMBUSISD")]
        [Description("Columbus ISD")]
        Columbus,
        [EnumMember(Value = "Comal")]
        [Description("Comal ISD")]
        Comal,
        [EnumMember(Value = "COMANCHEISD")]
        [Description("Comanche ISD")]
        Comanche,
        [EnumMember(Value = "Comfort")]
        [Description("Comfort ISD")]
        Comfort,
        [EnumMember(Value = "COMMERCEISD")]
        [Description("Commerce ISD")]
        Commerce,
        [EnumMember(Value = "COMMUNITYISD")]
        [Description("Community ISD")]
        Community,
        [EnumMember(Value = "COMOPICKTONCONSISD")]
        [Description("Como-Pickton Consolidated ISD")]
        ComoPicktonConsolidated,
        [EnumMember(Value = "COMSTOCKISD")]
        [Description("Comstock ISD")]
        Comstock,
        [EnumMember(Value = "CONNALLYISD")]
        [Description("Connally ISD")]
        Connally,
        [EnumMember(Value = "CONROEISD")]
        [Description("Conroe ISD")]
        Conroe,
        [EnumMember(Value = "COOLIDGEISD")]
        [Description("Coolidge ISD")]
        Coolidge,
        [EnumMember(Value = "COOPERISD")]
        [Description("Cooper ISD")]
        Cooper,
        [EnumMember(Value = "COPPELLISD")]
        [Description("Coppell ISD")]
        Coppell,
        [EnumMember(Value = "COPPERASCOVEISD")]
        [Description("Copperas Cove ISD")]
        CopperasCove,
        [EnumMember(Value = "CORPUSCHRISTIISD")]
        [Description("Corpus Christi ISD")]
        CorpusChristi,
        [EnumMember(Value = "CORRIGANCAMDENISD")]
        [Description("Corrigan-Camden ISD")]
        CorriganCamden,
        [EnumMember(Value = "CORSICANAISD")]
        [Description("Corsicana ISD")]
        Corsicana,
        [EnumMember(Value = "COTTONCENTERISD")]
        [Description("Cotton Center ISD")]
        CottonCenter,
        [EnumMember(Value = "COTULLAISD")]
        [Description("Cotulla ISD")]
        Cotulla,
        [EnumMember(Value = "COUPLANDISD")]
        [Description("Coupland ISD")]
        Coupland,
        [EnumMember(Value = "COVINGTONISD")]
        [Description("Covington ISD")]
        Covington,
        [EnumMember(Value = "CRANDALLISD")]
        [Description("Crandall ISD")]
        Crandall,
        [EnumMember(Value = "CRANEISD")]
        [Description("Crane ISD")]
        Crane,
        [EnumMember(Value = "CRANFILLSGAPISD")]
        [Description("Cranfills Gap ISD")]
        CranfillsGap,
        [EnumMember(Value = "CRAWFORDISD")]
        [Description("Crawford ISD")]
        Crawford,
        [EnumMember(Value = "CROCKETTCOUNTYCONSCOMSD")]
        [Description("Crockett County Consolidate Common School District")]
        CrockettCountyConsolidateCommonSchoolDistrict,
        [EnumMember(Value = "CROCKETTISD")]
        [Description("Crockett ISD")]
        Crockett,
        [EnumMember(Value = "CROSBYISD")]
        [Description("Crosby ISD")]
        Crosby,
        [EnumMember(Value = "CROSBYTONCONSISD")]
        [Description("Crosbyton Consolidated ISD")]
        CrosbytonConsolidated,
        [EnumMember(Value = "CROSSPLAINSISD")]
        [Description("Cross Plains ISD")]
        CrossPlains,
        [EnumMember(Value = "CROWELLISD")]
        [Description("Crowell ISD")]
        Crowell,
        [EnumMember(Value = "CROWLEYISD")]
        [Description("Crowley ISD")]
        Crowley,
        [EnumMember(Value = "CRYSTALCITYISD")]
        [Description("Crystal City ISD")]
        CrystalCity,
        [EnumMember(Value = "Cuero")]
        [Description("Cuero ISD")]
        Cuero,
        [EnumMember(Value = "CULBERSONCOUNTYALLAMOOREISD")]
        [Description("Culberson County-Allamoore ISD")]
        CulbersonCountyAllamoore,
        [EnumMember(Value = "CUMBYISD")]
        [Description("Cumby ISD")]
        Cumby,
        [EnumMember(Value = "CUSHINGISD")]
        [Description("Cushing ISD")]
        Cushing,
        [EnumMember(Value = "CYPRESSFAIRBANKSISD")]
        [Description("Cypress-Fairbanks ISD")]
        CypressFairbanks,
        [EnumMember(Value = "DHANISISD")]
        [Description("D'Hanis ISD")]
        DHanis,
        [EnumMember(Value = "DAINGERFIELDLONESTARISD")]
        [Description("Daingerfield-Lone Star ISD")]
        DaingerfieldLoneStar,
        [EnumMember(Value = "DALHARTISD")]
        [Description("Dalhart ISD")]
        Dalhart,
        [EnumMember(Value = "DALLASISD")]
        [Description("Dallas ISD")]
        Dallas,
        [EnumMember(Value = "DAMONISD")]
        [Description("Damon ISD")]
        Damon,
        [EnumMember(Value = "DANBURYISD")]
        [Description("Danbury ISD")]
        Danbury,
        [EnumMember(Value = "DARROUZETTISD")]
        [Description("Darrouzett ISD")]
        Darrouzett,
        [EnumMember(Value = "DAWSONISD")]
        [Description("Dawson ISD")]
        Dawson,
        [EnumMember(Value = "DAYTONISD")]
        [Description("Dayton ISD")]
        Dayton,
        [EnumMember(Value = "DELEONISD")]
        [Description("De Leon ISD")]
        DeLeon,
        [EnumMember(Value = "DECATURISD")]
        [Description("Decatur ISD")]
        Decatur,
        [EnumMember(Value = "DEERPARKISD")]
        [Description("Deer Park ISD")]
        DeerPark,
        [EnumMember(Value = "DEKALBISD")]
        [Description("Dekalb ISD")]
        Dekalb,
        [EnumMember(Value = "DelValle")]
        [Description("Del Valle ISD")]
        DelValle,
        [EnumMember(Value = "DELLCITYISD")]
        [Description("Dell City ISD")]
        DellCity,
        [EnumMember(Value = "DENISONISD")]
        [Description("Denison ISD")]
        Denison,
        [EnumMember(Value = "DENTONISD")]
        [Description("Denton ISD")]
        Denton,
        [EnumMember(Value = "DENVERCITYISD")]
        [Description("Denver City ISD")]
        DenverCity,
        [EnumMember(Value = "DESOTOISD")]
        [Description("Desoto ISD")]
        Desoto,
        [EnumMember(Value = "DETROITISD")]
        [Description("Detroit ISD")]
        Detroit,
        [EnumMember(Value = "DEVERSISD")]
        [Description("Devers ISD")]
        Devers,
        [EnumMember(Value = "DEVINEISD")]
        [Description("Devine ISD")]
        Devine,
        [EnumMember(Value = "DEWISD")]
        [Description("Dew ISD")]
        Dew,
        [EnumMember(Value = "DEWEYVILLEISD")]
        [Description("Deweyville ISD")]
        Deweyville,
        [EnumMember(Value = "DIBOLLISD")]
        [Description("Diboll ISD")]
        Diboll,
        [EnumMember(Value = "DICKINSONISD")]
        [Description("Dickinson ISD")]
        Dickinson,
        [EnumMember(Value = "DILLEYISD")]
        [Description("Dilley ISD")]
        Dilley,
        [EnumMember(Value = "DIMEBOXISD")]
        [Description("Dime Box ISD")]
        DimeBox,
        [EnumMember(Value = "DIMMITTISD")]
        [Description("Dimmitt ISD")]
        Dimmitt,
        [EnumMember(Value = "DIVIDEISD")]
        [Description("Divide ISD")]
        Divide,
        [EnumMember(Value = "DODDCITYISD")]
        [Description("Dodd City ISD")]
        DoddCity,
        [EnumMember(Value = "DONNAISD")]
        [Description("Donna ISD")]
        Donna,
        [EnumMember(Value = "DOSSCONSCOMSCHOOLDISTRICT")]
        [Description("Doss Consolidate Common School District")]
        DossConsolidateCommonSchoolDistrict,
        [EnumMember(Value = "DOUGLASSISD")]
        [Description("Douglass ISD")]
        Douglass,
        [EnumMember(Value = "Dripping")]
        [Description("Dripping Springs ISD")]
        DrippingSprings,
        [EnumMember(Value = "DRISCOLLISD")]
        [Description("Driscoll ISD")]
        Driscoll,
        [EnumMember(Value = "DUBLINISD")]
        [Description("Dublin ISD")]
        Dublin,
        [EnumMember(Value = "DUMASISD")]
        [Description("Dumas ISD")]
        Dumas,
        [EnumMember(Value = "DUNCANVILLEISD")]
        [Description("Duncanville ISD")]
        Duncanville,
        [EnumMember(Value = "EAGLEMTSAGINAWISD")]
        [Description("Eagle Mt-Saginaw ISD")]
        EagleMtSaginaw,
        [EnumMember(Value = "EAGLEPASSISD")]
        [Description("Eagle Pass ISD")]
        EaglePass,
        [EnumMember(Value = "Eanes")]
        [Description("Eanes ISD")]
        Eanes,
        [EnumMember(Value = "EARLYISD")]
        [Description("Early ISD")]
        Early,
        [EnumMember(Value = "EASTBERNARDISD")]
        [Description("East Bernard ISD")]
        EastBernard,
        [EnumMember(Value = "EastCent")]
        [Description("East Central ISD")]
        EastCentral,
        [EnumMember(Value = "EASTCHAMBERSISD")]
        [Description("East Chambers ISD")]
        EastChambers,
        [EnumMember(Value = "EASTLANDISD")]
        [Description("Eastland ISD")]
        Eastland,
        [EnumMember(Value = "ECTORCOUNTYISD")]
        [Description("Ector County ISD")]
        EctorCounty,
        [EnumMember(Value = "EDCOUCHELSAISD")]
        [Description("Edcouch-Elsa ISD")]
        EdcouchElsa,
        [EnumMember(Value = "EDENCONSISD")]
        [Description("Eden Consolidated ISD")]
        EdenConsolidated,
        [EnumMember(Value = "Edgew")]
        [Description("Edgewood ISD")]
        Edgewood,
        [EnumMember(Value = "EDINBURGCONSISD")]
        [Description("Edinburg Consolidated ISD")]
        EdinburgConsolidated,
        [EnumMember(Value = "EDNAISD")]
        [Description("Edna ISD")]
        Edna,
        [EnumMember(Value = "ELCAMPOISD")]
        [Description("El Campo ISD")]
        ElCampo,
        [EnumMember(Value = "ELPASOISD")]
        [Description("El Paso ISD")]
        ElPaso,
        [EnumMember(Value = "ELECTRAISD")]
        [Description("Electra ISD")]
        Electra,
        [EnumMember(Value = "Elgin")]
        [Description("Elgin ISD")]
        Elgin,
        [EnumMember(Value = "ELKHARTISD")]
        [Description("Elkhart ISD")]
        Elkhart,
        [EnumMember(Value = "ELYSIANFIELDSISD")]
        [Description("Elysian Fields ISD")]
        ElysianFields,
        [EnumMember(Value = "ENNISISD")]
        [Description("Ennis ISD")]
        Ennis,
        [EnumMember(Value = "ERAISD")]
        [Description("Era ISD")]
        Era,
        [EnumMember(Value = "ETOILEISD")]
        [Description("Etoile ISD")]
        Etoile,
        [EnumMember(Value = "EULAISD")]
        [Description("Eula ISD")]
        Eula,
        [EnumMember(Value = "EUSTACEISD")]
        [Description("Eustace ISD")]
        Eustace,
        [EnumMember(Value = "EVADALEISD")]
        [Description("Evadale ISD")]
        Evadale,
        [EnumMember(Value = "EVANTISD")]
        [Description("Evant ISD")]
        Evant,
        [EnumMember(Value = "EVERMANISD")]
        [Description("Everman ISD")]
        Everman,
        [EnumMember(Value = "EXCELSIORISD")]
        [Description("Excelsior ISD")]
        Excelsior,
        [EnumMember(Value = "EZZELLISD")]
        [Description("Ezzell ISD")]
        Ezzell,
        [EnumMember(Value = "FABENSISD")]
        [Description("Fabens ISD")]
        Fabens,
        [EnumMember(Value = "FAIRFIELDISD")]
        [Description("Fairfield ISD")]
        Fairfield,
        [EnumMember(Value = "FALLSCITYISD")]
        [Description("Falls City ISD")]
        FallsCity,
        [EnumMember(Value = "FANNINDELISD")]
        [Description("Fannindel ISD")]
        Fannindel,
        [EnumMember(Value = "FARMERSVILLEISD")]
        [Description("Farmersville ISD")]
        Farmersville,
        [EnumMember(Value = "FARWELLISD")]
        [Description("Farwell ISD")]
        Farwell,
        [EnumMember(Value = "FAYETTEVILLEISD")]
        [Description("Fayetteville ISD")]
        Fayetteville,
        [EnumMember(Value = "FERRISISD")]
        [Description("Ferris ISD")]
        Ferris,
        [EnumMember(Value = "FLATONIAISD")]
        [Description("Flatonia ISD")]
        Flatonia,
        [EnumMember(Value = "FLORENCEISD")]
        [Description("Florence ISD")]
        Florence,
        [EnumMember(Value = "Flores")]
        [Description("Floresville ISD")]
        Floresville,
        [EnumMember(Value = "FLOURBLUFFISD")]
        [Description("Flour Bluff ISD")]
        FlourBluff,
        [EnumMember(Value = "FLOYDADAISD")]
        [Description("Floydada ISD")]
        Floydada,
        [EnumMember(Value = "FOLLETTISD")]
        [Description("Follett ISD")]
        Follett,
        [EnumMember(Value = "FORESTBURGISD")]
        [Description("Forestburg ISD")]
        Forestburg,
        [EnumMember(Value = "FORNEYISD")]
        [Description("Forney ISD")]
        Forney,
        [EnumMember(Value = "FORSANISD")]
        [Description("Forsan ISD")]
        Forsan,
        [EnumMember(Value = "FORTBENDISD")]
        [Description("Fort Bend ISD")]
        FortBend,
        [EnumMember(Value = "FORTELLIOTTCONSISD")]
        [Description("Fort Elliott Consolidated ISD")]
        FortElliottConsolidated,
        [EnumMember(Value = "FORTSTOCKTONISD")]
        [Description("Fort Stockton ISD")]
        FortStockton,
        [EnumMember(Value = "FORTWORTHISD")]
        [Description("Fort Worth ISD")]
        FortWorth,
        [EnumMember(Value = "FRANKLINISD")]
        [Description("Franklin ISD")]
        Franklin,
        [EnumMember(Value = "Freder")]
        [Description("Fredericksburg ISD")]
        Fredericksburg,
        [EnumMember(Value = "FREERISD")]
        [Description("Freer ISD")]
        Freer,
        [EnumMember(Value = "FRENSHIPISD")]
        [Description("Frenship ISD")]
        Frenship,
        [EnumMember(Value = "FRIENDSWOODISD")]
        [Description("Friendswood ISD")]
        Friendswood,
        [EnumMember(Value = "FRIONAISD")]
        [Description("Friona ISD")]
        Friona,
        [EnumMember(Value = "FRISCOISD")]
        [Description("Frisco ISD")]
        Frisco,
        [EnumMember(Value = "FROSTISD")]
        [Description("Frost ISD")]
        Frost,
        [EnumMember(Value = "FRUITVALEISD")]
        [Description("Fruitvale ISD")]
        Fruitvale,
        [EnumMember(Value = "FTDAVISISD")]
        [Description("Ft Davis ISD")]
        FtDavis,
        [EnumMember(Value = "FTHANCOCKISD")]
        [Description("Ft Hancock ISD")]
        FtHancock,
        [EnumMember(Value = "FTSAMHOUSTONISD")]
        [Description("Ft Sam Houston ISD")]
        FtSamHouston,
        [EnumMember(Value = "GAINESVILLEISD")]
        [Description("Gainesville ISD")]
        Gainesville,
        [EnumMember(Value = "GALENAPARKISD")]
        [Description("Galena Park ISD")]
        GalenaPark,
        [EnumMember(Value = "GALVESTONISD")]
        [Description("Galveston ISD")]
        Galveston,
        [EnumMember(Value = "GANADOISD")]
        [Description("Ganado ISD")]
        Ganado,
        [EnumMember(Value = "GARLANDISD")]
        [Description("Garland ISD")]
        Garland,
        [EnumMember(Value = "GARRISONISD")]
        [Description("Garrison ISD")]
        Garrison,
        [EnumMember(Value = "GARYISD")]
        [Description("Gary ISD")]
        Gary,
        [EnumMember(Value = "GATESVILLEISD")]
        [Description("Gatesville ISD")]
        Gatesville,
        [EnumMember(Value = "GAUSEISD")]
        [Description("Gause ISD")]
        Gause,
        [EnumMember(Value = "GEORGEWESTISD")]
        [Description("George West ISD")]
        GeorgeWest,
        [EnumMember(Value = "GEORGETOWNISD")]
        [Description("Georgetown ISD")]
        Georgetown,
        [EnumMember(Value = "GHOLSONISD")]
        [Description("Gholson ISD")]
        Gholson,
        [EnumMember(Value = "GIDDINGSISD")]
        [Description("Giddings ISD")]
        Giddings,
        [EnumMember(Value = "GILMERISD")]
        [Description("Gilmer ISD")]
        Gilmer,
        [EnumMember(Value = "GLADEWATERISD")]
        [Description("Gladewater ISD")]
        Gladewater,
        [EnumMember(Value = "GLASSCOCKCOUNTYISD")]
        [Description("Glasscock County ISD")]
        GlasscockCounty,
        [EnumMember(Value = "GLENROSEISD")]
        [Description("Glen Rose ISD")]
        GlenRose,
        [EnumMember(Value = "GODLEYISD")]
        [Description("Godley ISD")]
        Godley,
        [EnumMember(Value = "GOLDBURGISD")]
        [Description("Gold Burg ISD")]
        GoldBurg,
        [EnumMember(Value = "GOLDTHWAITEISD")]
        [Description("Goldthwaite ISD")]
        Goldthwaite,
        [EnumMember(Value = "GOLIADISD")]
        [Description("Goliad ISD")]
        Goliad,
        [EnumMember(Value = "Gonzal")]
        [Description("Gonzales ISD")]
        Gonzales,
        [EnumMember(Value = "GOODRICHISD")]
        [Description("Goodrich ISD")]
        Goodrich,
        [EnumMember(Value = "GOOSECREEKCISD")]
        [Description("Goose Creek Cisd")]
        GooseCreekCisd,
        [EnumMember(Value = "GORDONISD")]
        [Description("Gordon ISD")]
        Gordon,
        [EnumMember(Value = "GORMANISD")]
        [Description("Gorman ISD")]
        Gorman,
        [EnumMember(Value = "GRADYISD")]
        [Description("Grady ISD")]
        Grady,
        [EnumMember(Value = "GRAFORDISD")]
        [Description("Graford ISD")]
        Graford,
        [EnumMember(Value = "GRAHAMISD")]
        [Description("Graham ISD")]
        Graham,
        [EnumMember(Value = "GRANBURYISD")]
        [Description("Granbury ISD")]
        Granbury,
        [EnumMember(Value = "GRANDPRAIRIEISD")]
        [Description("Grand Prairie ISD")]
        GrandPrairie,
        [EnumMember(Value = "GRANDSALINEISD")]
        [Description("Grand Saline ISD")]
        GrandSaline,
        [EnumMember(Value = "GRANDFALLSROYALTYISD")]
        [Description("Grandfalls-Royalty ISD")]
        GrandfallsRoyalty,
        [EnumMember(Value = "GRANDVIEWISD")]
        [Description("Grandview ISD")]
        Grandview,
        [EnumMember(Value = "GRANDVIEWHOPKINSISD")]
        [Description("Grandview-Hopkins ISD")]
        GrandviewHopkins,
        [EnumMember(Value = "GRANGERISD")]
        [Description("Granger ISD")]
        Granger,
        [EnumMember(Value = "GRAPECREEKISD")]
        [Description("Grape Creek ISD")]
        GrapeCreek,
        [EnumMember(Value = "GRAPELANDISD")]
        [Description("Grapeland ISD")]
        Grapeland,
        [EnumMember(Value = "GRAPEVINECOLLEYVILLEISD")]
        [Description("Grapevine-Colleyville ISD")]
        GrapevineColleyville,
        [EnumMember(Value = "GREENVILLEISD")]
        [Description("Greenville ISD")]
        Greenville,
        [EnumMember(Value = "GREENWOODISD")]
        [Description("Greenwood ISD")]
        Greenwood,
        [EnumMember(Value = "GREGORYPORTLANDISD")]
        [Description("Gregory-Portland ISD")]
        GregoryPortland,
        [EnumMember(Value = "GROESBECKISD")]
        [Description("Groesbeck ISD")]
        Groesbeck,
        [EnumMember(Value = "GROOMISD")]
        [Description("Groom ISD")]
        Groom,
        [EnumMember(Value = "GROVETONISD")]
        [Description("Groveton ISD")]
        Groveton,
        [EnumMember(Value = "GRUVERISD")]
        [Description("Gruver ISD")]
        Gruver,
        [EnumMember(Value = "GUNTERISD")]
        [Description("Gunter ISD")]
        Gunter,
        [EnumMember(Value = "GUSTINEISD")]
        [Description("Gustine ISD")]
        Gustine,
        [EnumMember(Value = "GUTHRICOMMONSCHOOLDISTRICT")]
        [Description("Guthri Common School District")]
        GuthriCommonSchoolDistrict,
        [EnumMember(Value = "HALECENTERISD")]
        [Description("Hale Center ISD")]
        HaleCenter,
        [EnumMember(Value = "HALLETSVILLEISD912INVYSEHRAD")]
        [Description("Halletsville Isd (9-12) In Vysehrad")]
        HalletsvilleInVysehrad,
        [EnumMember(Value = "Hallet")]
        [Description("Hallettsville ISD")]
        Hallettsville,
        [EnumMember(Value = "HALLSBURGISD")]
        [Description("Hallsburg ISD")]
        Hallsburg,
        [EnumMember(Value = "HALLSVILLEISD")]
        [Description("Hallsville ISD")]
        Hallsville,
        [EnumMember(Value = "HAMILTONISD")]
        [Description("Hamilton ISD")]
        Hamilton,
        [EnumMember(Value = "HAMLINISD")]
        [Description("Hamlin ISD")]
        Hamlin,
        [EnumMember(Value = "HAMSHIREFANNETTISD")]
        [Description("Hamshire-Fannett ISD")]
        HamshireFannett,
        [EnumMember(Value = "HAPPYISD")]
        [Description("Happy ISD")]
        Happy,
        [EnumMember(Value = "HARDINISD")]
        [Description("Hardin ISD")]
        Hardin,
        [EnumMember(Value = "HARDINJEFFERSONISD")]
        [Description("Hardin-Jefferson ISD")]
        HardinJefferson,
        [EnumMember(Value = "Harlan")]
        [Description("Harlandale ISD")]
        Harlandale,
        [EnumMember(Value = "HARLETONISD")]
        [Description("Harleton ISD")]
        Harleton,
        [EnumMember(Value = "HARLINGENCONSISD")]
        [Description("Harlingen Consolidated ISD")]
        HarlingenConsolidated,
        [EnumMember(Value = "HARMONYISD")]
        [Description("Harmony ISD")]
        Harmony,
        [EnumMember(Value = "HARPERISD")]
        [Description("Harper ISD")]
        Harper,
        [EnumMember(Value = "HARROLDISD")]
        [Description("Harrold ISD")]
        Harrold,
        [EnumMember(Value = "HARTISD")]
        [Description("Hart ISD")]
        Hart,
        [EnumMember(Value = "HARTLEYISD")]
        [Description("Hartley ISD")]
        Hartley,
        [EnumMember(Value = "HARTSBLUFFISD")]
        [Description("Harts Bluff ISD")]
        HartsBluff,
        [EnumMember(Value = "HASKELLCONSISD")]
        [Description("Haskell Consolidated ISD")]
        HaskellConsolidated,
        [EnumMember(Value = "HAWKINSISD")]
        [Description("Hawkins ISD")]
        Hawkins,
        [EnumMember(Value = "HAWLEYISD")]
        [Description("Hawley ISD")]
        Hawley,
        [EnumMember(Value = "Hays")]
        [Description("Hays Cisd")]
        HaysCisd,
        [EnumMember(Value = "HEARNEISD")]
        [Description("Hearne ISD")]
        Hearne,
        [EnumMember(Value = "HEDLEYISD")]
        [Description("Hedley ISD")]
        Hedley,
        [EnumMember(Value = "HEMPHILLISD")]
        [Description("Hemphill ISD")]
        Hemphill,
        [EnumMember(Value = "HEMPSTEADISD")]
        [Description("Hempstead ISD")]
        Hempstead,
        [EnumMember(Value = "HENDERSONISD")]
        [Description("Henderson ISD")]
        Henderson,
        [EnumMember(Value = "HENRIETTAISD")]
        [Description("Henrietta ISD")]
        Henrietta,
        [EnumMember(Value = "HEREFORDISD")]
        [Description("Hereford ISD")]
        Hereford,
        [EnumMember(Value = "HERMLEIGHISD")]
        [Description("Hermleigh ISD")]
        Hermleigh,
        [EnumMember(Value = "HICOISD")]
        [Description("Hico ISD")]
        Hico,
        [EnumMember(Value = "HIDALGOISD")]
        [Description("Hidalgo ISD")]
        Hidalgo,
        [EnumMember(Value = "HIGGINSISD")]
        [Description("Higgins ISD")]
        Higgins,
        [EnumMember(Value = "HIGHISLANDISD")]
        [Description("High Island ISD")]
        HighIsland,
        [EnumMember(Value = "HIGHLANDISD")]
        [Description("Highland ISD")]
        Highland,
        [EnumMember(Value = "HIGHLANDPARKISD")]
        [Description("Highland Park ISD")]
        HighlandPark,
        [EnumMember(Value = "HILLSBOROISD")]
        [Description("Hillsboro ISD")]
        Hillsboro,
        [EnumMember(Value = "HITCHCOCKISD")]
        [Description("Hitchcock ISD")]
        Hitchcock,
        [EnumMember(Value = "HOLLANDISD")]
        [Description("Holland ISD")]
        Holland,
        [EnumMember(Value = "HOLLIDAYISD")]
        [Description("Holliday ISD")]
        Holliday,
        [EnumMember(Value = "HONDOISD")]
        [Description("Hondo ISD")]
        Hondo,
        [EnumMember(Value = "HONEYGROVEISD")]
        [Description("Honey Grove ISD")]
        HoneyGrove,
        [EnumMember(Value = "HOOKSISD")]
        [Description("Hooks ISD")]
        Hooks,
        [EnumMember(Value = "HOUSTONISD")]
        [Description("Houston ISD")]
        Houston,
        [EnumMember(Value = "HOWEISD")]
        [Description("Howe ISD")]
        Howe,
        [EnumMember(Value = "HUBBARDISD")]
        [Description("Hubbard ISD")]
        Hubbard,
        [EnumMember(Value = "HUCKABAYISD")]
        [Description("Huckabay ISD")]
        Huckabay,
        [EnumMember(Value = "HUDSONISD")]
        [Description("Hudson ISD")]
        Hudson,
        [EnumMember(Value = "HUFFMANISD")]
        [Description("Huffman ISD")]
        Huffman,
        [EnumMember(Value = "HUGHESSPRINGSISD")]
        [Description("Hughes Springs ISD")]
        HughesSprings,
        [EnumMember(Value = "HULLDAISETTAISD")]
        [Description("Hull-Daisetta ISD")]
        HullDaisetta,
        [EnumMember(Value = "HUMBLEISD")]
        [Description("Humble ISD")]
        Humble,
        [EnumMember(Value = "HUNTISD")]
        [Description("Hunt ISD")]
        Hunt,
        [EnumMember(Value = "HUNTINGTONISD")]
        [Description("Huntington ISD")]
        Huntington,
        [EnumMember(Value = "HUNTSVILLEISD")]
        [Description("Huntsville ISD")]
        Huntsville,
        [EnumMember(Value = "HURSTEULESSBEDFORDISD")]
        [Description("Hurst-Euless-Bedford ISD")]
        HurstEulessBedford,
        [EnumMember(Value = "HUTTOISD")]
        [Description("Hutto ISD")]
        Hutto,
        [EnumMember(Value = "IDALOUISD")]
        [Description("Idalou ISD")]
        Idalou,
        [EnumMember(Value = "INDUSTRIALISD")]
        [Description("Industrial ISD")]
        Industrial,
        [EnumMember(Value = "INGLESIDEISD")]
        [Description("Ingleside ISD")]
        Ingleside,
        [EnumMember(Value = "INGRAMISD")]
        [Description("Ingram ISD")]
        Ingram,
        [EnumMember(Value = "IOLAISD")]
        [Description("Iola ISD")]
        Iola,
        [EnumMember(Value = "IOWAPARKCONSISD")]
        [Description("Iowa Park Consolidated ISD")]
        IowaParkConsolidated,
        [EnumMember(Value = "IRAISD")]
        [Description("Ira ISD")]
        Ira,
        [EnumMember(Value = "IRAANSHEFFIELDISD")]
        [Description("Iraan-Sheffield ISD")]
        IraanSheffield,
        [EnumMember(Value = "IREDELLISD")]
        [Description("Iredell ISD")]
        Iredell,
        [EnumMember(Value = "IRIONCOUNTYISD")]
        [Description("Irion County ISD")]
        IrionCounty,
        [EnumMember(Value = "IRVINGISD")]
        [Description("Irving ISD")]
        Irving,
        [EnumMember(Value = "ITALYISD")]
        [Description("Italy ISD")]
        Italy,
        [EnumMember(Value = "ITASCAISD")]
        [Description("Itasca ISD")]
        Itasca,
        [EnumMember(Value = "JACKSBOROISD")]
        [Description("Jacksboro ISD")]
        Jacksboro,
        [EnumMember(Value = "JACKSONVILLEISD")]
        [Description("Jacksonville ISD")]
        Jacksonville,
        [EnumMember(Value = "JARRELLISD")]
        [Description("Jarrell ISD")]
        Jarrell,
        [EnumMember(Value = "JASPERISD")]
        [Description("Jasper ISD")]
        Jasper,
        [EnumMember(Value = "JAYTONGIRARDISD")]
        [Description("Jayton-Girard ISD")]
        JaytonGirard,
        [EnumMember(Value = "JEFFERSONISD")]
        [Description("Jefferson ISD")]
        Jefferson,
        [EnumMember(Value = "JIMHOGGCOUNTYISD")]
        [Description("Jim Hogg County ISD")]
        JimHoggCounty,
        [EnumMember(Value = "JIMNEDCONSISD")]
        [Description("Jim Ned Consolidated ISD")]
        JimNedConsolidated,
        [EnumMember(Value = "JOAQUINISD")]
        [Description("Joaquin ISD")]
        Joaquin,
        [EnumMember(Value = "Johnson")]
        [Description("Johnson City ISD")]
        JohnsonCity,
        [EnumMember(Value = "JONESBOROISD")]
        [Description("Jonesboro ISD")]
        Jonesboro,
        [EnumMember(Value = "JOSHUAISD")]
        [Description("Joshua ISD")]
        Joshua,
        [EnumMember(Value = "JOURDANTONISD")]
        [Description("Jourdanton ISD")]
        Jourdanton,
        [EnumMember(Value = "Judson")]
        [Description("Judson ISD")]
        Judson,
        [EnumMember(Value = "JUNCTIONISD")]
        [Description("Junction ISD")]
        Junction,
        [EnumMember(Value = "KARNACKISD")]
        [Description("Karnack ISD")]
        Karnack,
        [EnumMember(Value = "Karnes")]
        [Description("Karnes City")]
        KarnesCity,
        [EnumMember(Value = "KATYISD")]
        [Description("Katy ISD")]
        Katy,
        [EnumMember(Value = "KAUFMANISD")]
        [Description("Kaufman ISD")]
        Kaufman,
        [EnumMember(Value = "KEENEISD")]
        [Description("Keene ISD")]
        Keene,
        [EnumMember(Value = "KELLERISD")]
        [Description("Keller ISD")]
        Keller,
        [EnumMember(Value = "KELTONISD")]
        [Description("Kelton ISD")]
        Kelton,
        [EnumMember(Value = "KEMPISD")]
        [Description("Kemp ISD")]
        Kemp,
        [EnumMember(Value = "KENEDYISD")]
        [Description("Kenedy ISD")]
        Kenedy,
        [EnumMember(Value = "KENNARDISD")]
        [Description("Kennard ISD")]
        Kennard,
        [EnumMember(Value = "KENNEDALEISD")]
        [Description("Kennedale ISD")]
        Kennedale,
        [EnumMember(Value = "KERENSISD")]
        [Description("Kerens ISD")]
        Kerens,
        [EnumMember(Value = "KERMITISD")]
        [Description("Kermit ISD")]
        Kermit,
        [EnumMember(Value = "KERRVILLEISD")]
        [Description("Kerrville ISD")]
        Kerrville,
        [EnumMember(Value = "KILGOREISD")]
        [Description("Kilgore ISD")]
        Kilgore,
        [EnumMember(Value = "KILLEENISD")]
        [Description("Killeen ISD")]
        Killeen,
        [EnumMember(Value = "KINGSVILLEISD")]
        [Description("Kingsville ISD")]
        Kingsville,
        [EnumMember(Value = "KIRBYVILLECONSISD")]
        [Description("Kirbyville Consolidated ISD")]
        KirbyvilleConsolidated,
        [EnumMember(Value = "KLEINISD")]
        [Description("Klein ISD")]
        Klein,
        [EnumMember(Value = "KLONDIKEISD")]
        [Description("Klondike ISD")]
        Klondike,
        [EnumMember(Value = "KNIPPAISD")]
        [Description("Knippa ISD")]
        Knippa,
        [EnumMember(Value = "KNOXCITYOBRIENCONSISD")]
        [Description("Knox City-O'Brien Consolidated ISD")]
        KnoxCityOBrienConsolidated,
        [EnumMember(Value = "KOPPERLISD")]
        [Description("Kopperl ISD")]
        Kopperl,
        [EnumMember(Value = "KOUNTZEISD")]
        [Description("Kountze ISD")]
        Kountze,
        [EnumMember(Value = "KRESSISD")]
        [Description("Kress ISD")]
        Kress,
        [EnumMember(Value = "KRUMISD")]
        [Description("Krum ISD")]
        Krum,
        [EnumMember(Value = "LAFERIAISD")]
        [Description("La Feria ISD")]
        LaFeria,
        [EnumMember(Value = "LAGLORIAISD")]
        [Description("La Gloria ISD")]
        LaGloria,
        [EnumMember(Value = "LAGRANGEISD")]
        [Description("La Grange ISD")]
        LaGrange,
        [EnumMember(Value = "LAJOYAISD")]
        [Description("La Joya ISD")]
        LaJoya,
        [EnumMember(Value = "LAMARQUEISD")]
        [Description("La Marque ISD")]
        LaMarque,
        [EnumMember(Value = "LAPORTEISD")]
        [Description("La Porte ISD")]
        LaPorte,
        [EnumMember(Value = "LAPRYORISD")]
        [Description("La Pryor ISD")]
        LaPryor,
        [EnumMember(Value = "LAVEGAISD")]
        [Description("La Vega ISD")]
        LaVega,
        [EnumMember(Value = "LaVerni")]
        [Description("La Vernia ISD")]
        LaVernia,
        [EnumMember(Value = "LAVILLAISD")]
        [Description("La Villa ISD")]
        LaVilla,
        [EnumMember(Value = "LACKLANDISD")]
        [Description("Lackland ISD")]
        Lackland,
        [EnumMember(Value = "LagoVist")]
        [Description("Lago Vista ISD")]
        LagoVista,
        [EnumMember(Value = "LAKEDALLASISD")]
        [Description("Lake Dallas ISD")]
        LakeDallas,
        [EnumMember(Value = "LakeTra")]
        [Description("Lake Travis ISD")]
        LakeTravis,
        [EnumMember(Value = "LAKEWORTHISD")]
        [Description("Lake Worth ISD")]
        LakeWorth,
        [EnumMember(Value = "LAMARCONSISD")]
        [Description("Lamar Consolidated ISD")]
        LamarConsolidated,
        [EnumMember(Value = "LAMESAISD")]
        [Description("Lamesa ISD")]
        Lamesa,
        [EnumMember(Value = "LAMPASASISD")]
        [Description("Lampasas ISD")]
        Lampasas,
        [EnumMember(Value = "LANCASTERISD")]
        [Description("Lancaster ISD")]
        Lancaster,
        [EnumMember(Value = "LANEVILLEISD")]
        [Description("Laneville ISD")]
        Laneville,
        [EnumMember(Value = "LAPOYNORISD")]
        [Description("Lapoynor ISD")]
        Lapoynor,
        [EnumMember(Value = "LAREDOISD")]
        [Description("Laredo ISD")]
        Laredo,
        [EnumMember(Value = "LASARAISD")]
        [Description("Lasara ISD")]
        Lasara,
        [EnumMember(Value = "LATEXOISD")]
        [Description("Latexo ISD")]
        Latexo,
        [EnumMember(Value = "LAZBUDDIEISD")]
        [Description("Lazbuddie ISD")]
        Lazbuddie,
        [EnumMember(Value = "Leakey")]
        [Description("Leakey ISD")]
        Leakey,
        [EnumMember(Value = "Leand")]
        [Description("Leander ISD")]
        Leander,
        [EnumMember(Value = "LEARYISD")]
        [Description("Leary ISD")]
        Leary,
        [EnumMember(Value = "LEFORSISD")]
        [Description("Lefors ISD")]
        Lefors,
        [EnumMember(Value = "LEGGETTISD")]
        [Description("Leggett ISD")]
        Leggett,
        [EnumMember(Value = "LEONISD")]
        [Description("Leon ISD")]
        Leon,
        [EnumMember(Value = "LEONARDISD")]
        [Description("Leonard ISD")]
        Leonard,
        [EnumMember(Value = "LEVELLANDISD")]
        [Description("Levelland ISD")]
        Levelland,
        [EnumMember(Value = "LEVERETTSCHAPELISD")]
        [Description("Leveretts Chapel ISD")]
        LeverettsChapel,
        [EnumMember(Value = "LEWISVILLEISD")]
        [Description("Lewisville ISD")]
        Lewisville,
        [EnumMember(Value = "LEXINGTONISD")]
        [Description("Lexington ISD")]
        Lexington,
        [EnumMember(Value = "LIBERTYHILLISD")]
        [Description("Liberty Hill ISD")]
        LibertyHill,
        [EnumMember(Value = "LIBERTYISD")]
        [Description("Liberty ISD")]
        Liberty,
        [EnumMember(Value = "LIBERTYEYLAUISD")]
        [Description("Liberty-Eylau ISD")]
        LibertyEylau,
        [EnumMember(Value = "LINDALEISD")]
        [Description("Lindale ISD")]
        Lindale,
        [EnumMember(Value = "LINDENKILDARECONSISD")]
        [Description("Linden-Kildare Consolidated ISD")]
        LindenKildareConsolidated,
        [EnumMember(Value = "LINDSAYISD")]
        [Description("Lindsay ISD")]
        Lindsay,
        [EnumMember(Value = "LINGLEVILLEISD")]
        [Description("Lingleville ISD")]
        Lingleville,
        [EnumMember(Value = "LIPANISD")]
        [Description("Lipan ISD")]
        Lipan,
        [EnumMember(Value = "LITTLECYPRESSMAURICEVILLECONSISD")]
        [Description("Little Cypress-Mauriceville Consolidated Independent Sd")]
        LittleCypressMauricevilleConsolidated,
        [EnumMember(Value = "LITTLEELMISD")]
        [Description("Little Elm ISD")]
        LittleElm,
        [EnumMember(Value = "LITTLEFIELDISD")]
        [Description("Littlefield ISD")]
        Littlefield,
        [EnumMember(Value = "LIVINGSTONISD")]
        [Description("Livingston ISD")]
        Livingston,
        [EnumMember(Value = "Llano")]
        [Description("Llano ISD")]
        Llano,
        [EnumMember(Value = "Lockh")]
        [Description("Lockhart ISD")]
        Lockhart,
        [EnumMember(Value = "LOCKNEYISD")]
        [Description("Lockney ISD")]
        Lockney,
        [EnumMember(Value = "LOHNISD")]
        [Description("Lohn ISD")]
        Lohn,
        [EnumMember(Value = "LOMETAISD")]
        [Description("Lometa ISD")]
        Lometa,
        [EnumMember(Value = "LONDONISD")]
        [Description("London ISD")]
        London,
        [EnumMember(Value = "LONEOAKISD")]
        [Description("Lone Oak ISD")]
        LoneOak,
        [EnumMember(Value = "LONGVIEWISD")]
        [Description("Longview ISD")]
        Longview,
        [EnumMember(Value = "LOOPISD")]
        [Description("Loop ISD")]
        Loop,
        [EnumMember(Value = "LORAINEISD")]
        [Description("Loraine ISD")]
        Loraine,
        [EnumMember(Value = "LORENAISD")]
        [Description("Lorena ISD")]
        Lorena,
        [EnumMember(Value = "LORENZOISD")]
        [Description("Lorenzo ISD")]
        Lorenzo,
        [EnumMember(Value = "LOSFRESNOSCONSISD")]
        [Description("Los Fresnos Consolidated ISD")]
        LosFresnosConsolidated,
        [EnumMember(Value = "LOUISEISD")]
        [Description("Louise ISD")]
        Louise,
        [EnumMember(Value = "LOVEJOYISD")]
        [Description("Lovejoy ISD")]
        Lovejoy,
        [EnumMember(Value = "LOVELADYISD")]
        [Description("Lovelady ISD")]
        Lovelady,
        [EnumMember(Value = "LUBBOCKISD")]
        [Description("Lubbock ISD")]
        Lubbock,
        [EnumMember(Value = "LUBBOCKCOOPERISD")]
        [Description("Lubbock-Cooper ISD")]
        LubbockCooper,
        [EnumMember(Value = "LUEDERSAVOCAISD")]
        [Description("Lueders-Avoca ISD")]
        LuedersAvoca,
        [EnumMember(Value = "LUFKINISD")]
        [Description("Lufkin ISD")]
        Lufkin,
        [EnumMember(Value = "Luling")]
        [Description("Luling ISD")]
        Luling,
        [EnumMember(Value = "LUMBERTONISD")]
        [Description("Lumberton ISD")]
        Lumberton,
        [EnumMember(Value = "LYFORDCONSISD")]
        [Description("Lyford Consolidated ISD")]
        LyfordConsolidated,
        [EnumMember(Value = "LYTLEISD")]
        [Description("Lytle ISD")]
        Lytle,
        [EnumMember(Value = "MABANKISD")]
        [Description("Mabank ISD")]
        Mabank,
        [EnumMember(Value = "MADISONVILLECONSISD")]
        [Description("Madisonville Consolidated ISD")]
        MadisonvilleConsolidated,
        [EnumMember(Value = "MAGNOLIAISD")]
        [Description("Magnolia ISD")]
        Magnolia,
        [EnumMember(Value = "MALAKOFFISD")]
        [Description("Malakoff ISD")]
        Malakoff,
        [EnumMember(Value = "MALONEISD")]
        [Description("Malone ISD")]
        Malone,
        [EnumMember(Value = "MALTAISD")]
        [Description("Malta ISD")]
        Malta,
        [EnumMember(Value = "Manor")]
        [Description("Manor ISD")]
        Manor,
        [EnumMember(Value = "MANSFIELDISD")]
        [Description("Mansfield ISD")]
        Mansfield,
        [EnumMember(Value = "MARATHONISD")]
        [Description("Marathon ISD")]
        Marathon,
        [EnumMember(Value = "Marble")]
        [Description("Marble Falls ISD")]
        MarbleFalls,
        [EnumMember(Value = "MARFAISD")]
        [Description("Marfa ISD")]
        Marfa,
        [EnumMember(Value = "Marion")]
        [Description("Marion ISD")]
        Marion,
        [EnumMember(Value = "MARLINISD")]
        [Description("Marlin ISD")]
        Marlin,
        [EnumMember(Value = "MARSHALLISD")]
        [Description("Marshall ISD")]
        Marshall,
        [EnumMember(Value = "MARTISD")]
        [Description("Mart ISD")]
        Mart,
        [EnumMember(Value = "MARTINSMILLISD")]
        [Description("Martins Mill ISD")]
        MartinsMill,
        [EnumMember(Value = "MARTINSVILLEISD")]
        [Description("Martinsville ISD")]
        Martinsville,
        [EnumMember(Value = "MASONISD")]
        [Description("Mason ISD")]
        Mason,
        [EnumMember(Value = "MATAGORDAISD")]
        [Description("Matagorda ISD")]
        Matagorda,
        [EnumMember(Value = "MATHISISD")]
        [Description("Mathis ISD")]
        Mathis,
        [EnumMember(Value = "MAUDISD")]
        [Description("Maud ISD")]
        Maud,
        [EnumMember(Value = "MAYISD")]
        [Description("May ISD")]
        May,
        [EnumMember(Value = "MAYPEARLISD")]
        [Description("Maypearl ISD")]
        Maypearl,
        [EnumMember(Value = "MCALLENISD")]
        [Description("Mcallen ISD")]
        Mcallen,
        [EnumMember(Value = "MCCAMEYISD")]
        [Description("Mccamey ISD")]
        Mccamey,
        [EnumMember(Value = "MCDADEISD")]
        [Description("Mcdade ISD")]
        Mcdade,
        [EnumMember(Value = "MCGREGORISD")]
        [Description("Mcgregor ISD")]
        Mcgregor,
        [EnumMember(Value = "MCKINNEYISD")]
        [Description("Mckinney ISD")]
        Mckinney,
        [EnumMember(Value = "MCLEANISD")]
        [Description("Mclean ISD")]
        Mclean,
        [EnumMember(Value = "MCLEODISD")]
        [Description("Mcleod ISD")]
        Mcleod,
        [EnumMember(Value = "MCMULLENCOUNTYISD")]
        [Description("Mcmullen County ISD")]
        McmullenCounty,
        [EnumMember(Value = "MEADOWISD")]
        [Description("Meadow ISD")]
        Meadow,
        [EnumMember(Value = "MEDINAISD")]
        [Description("Medina ISD")]
        Medina,
        [EnumMember(Value = "Medina")]
        [Description("Medina Valley ISD")]
        MedinaValley,
        [EnumMember(Value = "MELISSAISD")]
        [Description("Melissa ISD")]
        Melissa,
        [EnumMember(Value = "MEMPHISISD")]
        [Description("Memphis ISD")]
        Memphis,
        [EnumMember(Value = "MENARDISD")]
        [Description("Menard ISD")]
        Menard,
        [EnumMember(Value = "MERCEDESISD")]
        [Description("Mercedes ISD")]
        Mercedes,
        [EnumMember(Value = "MERIDIANISD")]
        [Description("Meridian ISD")]
        Meridian,
        [EnumMember(Value = "MERKELISD")]
        [Description("Merkel ISD")]
        Merkel,
        [EnumMember(Value = "MESQUITEISD")]
        [Description("Mesquite ISD")]
        Mesquite,
        [EnumMember(Value = "MEXIAISD")]
        [Description("Mexia ISD")]
        Mexia,
        [EnumMember(Value = "MEYERSVILLEISD")]
        [Description("Meyersville ISD")]
        Meyersville,
        [EnumMember(Value = "MIAMIISD")]
        [Description("Miami ISD")]
        Miami,
        [EnumMember(Value = "MIDLANDISD")]
        [Description("Midland ISD")]
        Midland,
        [EnumMember(Value = "MIDLOTHIANISD")]
        [Description("Midlothian ISD")]
        Midlothian,
        [EnumMember(Value = "MIDWAYISD")]
        [Description("Midway ISD")]
        Midway,
        [EnumMember(Value = "MILANOISD")]
        [Description("Milano ISD")]
        Milano,
        [EnumMember(Value = "MILDREDISD")]
        [Description("Mildred ISD")]
        Mildred,
        [EnumMember(Value = "MILESISD")]
        [Description("Miles ISD")]
        Miles,
        [EnumMember(Value = "MILFORDISD")]
        [Description("Milford ISD")]
        Milford,
        [EnumMember(Value = "MILLERGROVEISD")]
        [Description("Miller Grove ISD")]
        MillerGrove,
        [EnumMember(Value = "MILLSAPISD")]
        [Description("Millsap ISD")]
        Millsap,
        [EnumMember(Value = "MINEOLAISD")]
        [Description("Mineola ISD")]
        Mineola,
        [EnumMember(Value = "MINERALWELLSISD")]
        [Description("Mineral Wells ISD")]
        MineralWells,
        [EnumMember(Value = "MISSIONCONSISD")]
        [Description("Mission Consolidated ISD")]
        MissionConsolidated,
        [EnumMember(Value = "MONAHANSWICKETTPYOTEISD")]
        [Description("Monahans-Wickett-Pyote ISD")]
        MonahansWickettPyote,
        [EnumMember(Value = "MONTAGUEISD")]
        [Description("Montague ISD")]
        Montague,
        [EnumMember(Value = "MONTEALTOISD")]
        [Description("Monte Alto ISD")]
        MonteAlto,
        [EnumMember(Value = "MONTGOMERYISD")]
        [Description("Montgomery ISD")]
        Montgomery,
        [EnumMember(Value = "MOODYISD")]
        [Description("Moody ISD")]
        Moody,
        [EnumMember(Value = "MORANISD")]
        [Description("Moran ISD")]
        Moran,
        [EnumMember(Value = "MORGANISD")]
        [Description("Morgan ISD")]
        Morgan,
        [EnumMember(Value = "MORGANMILLISD")]
        [Description("Morgan Mill ISD")]
        MorganMill,
        [EnumMember(Value = "MORTONISD")]
        [Description("Morton ISD")]
        Morton,
        [EnumMember(Value = "MOTLEYCOUNTYISD")]
        [Description("Motley County ISD")]
        MotleyCounty,
        [EnumMember(Value = "Moulton")]
        [Description("Moulton ISD")]
        Moulton,
        [EnumMember(Value = "MOUNTCALMISD")]
        [Description("Mount Calm ISD")]
        MountCalm,
        [EnumMember(Value = "MOUNTENTERPRISEISD")]
        [Description("Mount Enterprise ISD")]
        MountEnterprise,
        [EnumMember(Value = "MOUNTPLEASANTISD")]
        [Description("Mount Pleasant ISD")]
        MountPleasant,
        [EnumMember(Value = "MOUNTVERNONISD")]
        [Description("Mount Vernon ISD")]
        MountVernon,
        [EnumMember(Value = "MUENSTERISD")]
        [Description("Muenster ISD")]
        Muenster,
        [EnumMember(Value = "MULESHOEISD")]
        [Description("Muleshoe ISD")]
        Muleshoe,
        [EnumMember(Value = "MULLINISD")]
        [Description("Mullin ISD")]
        Mullin,
        [EnumMember(Value = "MUMFORDISD")]
        [Description("Mumford ISD")]
        Mumford,
        [EnumMember(Value = "MUNDAYCONSISD")]
        [Description("Munday Consolidated ISD")]
        MundayConsolidated,
        [EnumMember(Value = "MURCHISONISD")]
        [Description("Murchison ISD")]
        Murchison,
        [EnumMember(Value = "NACOGDOCHESISD")]
        [Description("Nacogdoches ISD")]
        Nacogdoches,
        [EnumMember(Value = "NATALIAISD")]
        [Description("Natalia ISD")]
        Natalia,
        [EnumMember(Value = "Navarro")]
        [Description("Navarro ISD")]
        Navarro,
        [EnumMember(Value = "NAVASOTAISD")]
        [Description("Navasota ISD")]
        Navasota,
        [EnumMember(Value = "NAZARETHISD")]
        [Description("Nazareth ISD")]
        Nazareth,
        [EnumMember(Value = "NECHESISD")]
        [Description("Neches ISD")]
        Neches,
        [EnumMember(Value = "NEDERLANDISD")]
        [Description("Nederland ISD")]
        Nederland,
        [EnumMember(Value = "NEEDVILLEISD")]
        [Description("Needville ISD")]
        Needville,
        [EnumMember(Value = "NEWBOSTONISD")]
        [Description("New Boston ISD")]
        NewBoston,
        [EnumMember(Value = "NewBrau")]
        [Description("New Braunfels ISD")]
        NewBraunfels,
        [EnumMember(Value = "NEWCANEYISD")]
        [Description("New Caney ISD")]
        NewCaney,
        [EnumMember(Value = "NEWDEALISD")]
        [Description("New Deal ISD")]
        NewDeal,
        [EnumMember(Value = "NEWDIANAISD")]
        [Description("New Diana ISD")]
        NewDiana,
        [EnumMember(Value = "NEWHOMEISD")]
        [Description("New Home ISD")]
        NewHome,
        [EnumMember(Value = "NEWSUMMERFIELDISD")]
        [Description("New Summerfield ISD")]
        NewSummerfield,
        [EnumMember(Value = "NEWWAVERLYISD")]
        [Description("New Waverly ISD")]
        NewWaverly,
        [EnumMember(Value = "NEWCASTLEISD")]
        [Description("Newcastle ISD")]
        Newcastle,
        [EnumMember(Value = "NEWTONISD")]
        [Description("Newton ISD")]
        Newton,
        [EnumMember(Value = "Nixon")]
        [Description("Nixon-Smiley Cisd")]
        NixonSmileyCisd,
        [EnumMember(Value = "NOCONAISD")]
        [Description("Nocona ISD")]
        Nocona,
        [EnumMember(Value = "NORDHEIMISD")]
        [Description("Nordheim ISD")]
        Nordheim,
        [EnumMember(Value = "NORMANGEEISD")]
        [Description("Normangee ISD")]
        Normangee,
        [EnumMember(Value = "NorthE")]
        [Description("North East ISD")]
        NorthEast,
        [EnumMember(Value = "NORTHHOPKINSISD")]
        [Description("North Hopkins ISD")]
        NorthHopkins,
        [EnumMember(Value = "NORTHLAMARISD")]
        [Description("North Lamar ISD")]
        NorthLamar,
        [EnumMember(Value = "NORTHZULCHISD")]
        [Description("North Zulch ISD")]
        NorthZulch,
        [EnumMember(Value = "Norths")]
        [Description("Northside ISD")]
        Northside,
        [EnumMember(Value = "NORTHWESTISD")]
        [Description("Northwest ISD")]
        Northwest,
        [EnumMember(Value = "NUECESCANYONCONSISD")]
        [Description("Nueces Canyon Consolidated ISD")]
        NuecesCanyonConsolidated,
        [EnumMember(Value = "NURSERYISD")]
        [Description("Nursery ISD")]
        Nursery,
        [EnumMember(Value = "ODONNELLISD")]
        [Description("O'Donnell ISD")]
        ODonnell,
        [EnumMember(Value = "OAKWOODISD")]
        [Description("Oakwood ISD")]
        Oakwood,
        [EnumMember(Value = "ODEMEDROYISD")]
        [Description("Odem-Edroy ISD")]
        OdemEdroy,
        [EnumMember(Value = "OGLESBYISD")]
        [Description("Oglesby ISD")]
        Oglesby,
        [EnumMember(Value = "OLFENISD")]
        [Description("Olfen ISD")]
        Olfen,
        [EnumMember(Value = "OLNEYISD")]
        [Description("Olney ISD")]
        Olney,
        [EnumMember(Value = "OLTONISD")]
        [Description("Olton ISD")]
        Olton,
        [EnumMember(Value = "ONALASKAISD")]
        [Description("Onalaska ISD")]
        Onalaska,
        [EnumMember(Value = "ORANGEGROVEISD")]
        [Description("Orange Grove ISD")]
        OrangeGrove,
        [EnumMember(Value = "ORANGEFIELDISD")]
        [Description("Orangefield ISD")]
        Orangefield,
        [EnumMember(Value = "ORECITYISD")]
        [Description("Ore City ISD")]
        OreCity,
        [EnumMember(Value = "OVERTONISD")]
        [Description("Overton ISD")]
        Overton,
        [EnumMember(Value = "PADUCAHISD")]
        [Description("Paducah ISD")]
        Paducah,
        [EnumMember(Value = "PAINTCREEKISD")]
        [Description("Paint Creek ISD")]
        PaintCreek,
        [EnumMember(Value = "PAINTROCKISD")]
        [Description("Paint Rock ISD")]
        PaintRock,
        [EnumMember(Value = "PALACIOSISD")]
        [Description("Palacios ISD")]
        Palacios,
        [EnumMember(Value = "PALESTINEISD")]
        [Description("Palestine ISD")]
        Palestine,
        [EnumMember(Value = "PALMERISD")]
        [Description("Palmer ISD")]
        Palmer,
        [EnumMember(Value = "PALOPINTOISD")]
        [Description("Palo Pinto ISD")]
        PaloPinto,
        [EnumMember(Value = "PAMPAISD")]
        [Description("Pampa ISD")]
        Pampa,
        [EnumMember(Value = "PANHANDLEISD")]
        [Description("Panhandle ISD")]
        Panhandle,
        [EnumMember(Value = "PANTHERCREEKCONSISD")]
        [Description("Panther Creek Consolidated ISD")]
        PantherCreekConsolidated,
        [EnumMember(Value = "PARADISEISD")]
        [Description("Paradise ISD")]
        Paradise,
        [EnumMember(Value = "PARISISD")]
        [Description("Paris ISD")]
        Paris,
        [EnumMember(Value = "PASADENAISD")]
        [Description("Pasadena ISD")]
        Pasadena,
        [EnumMember(Value = "PATTONSPRINGSISD")]
        [Description("Patton Springs ISD")]
        PattonSprings,
        [EnumMember(Value = "PAWNEEISD")]
        [Description("Pawnee ISD")]
        Pawnee,
        [EnumMember(Value = "PEARLANDISD")]
        [Description("Pearland ISD")]
        Pearland,
        [EnumMember(Value = "PEARSALLISD")]
        [Description("Pearsall ISD")]
        Pearsall,
        [EnumMember(Value = "PEASTERISD")]
        [Description("Peaster ISD")]
        Peaster,
        [EnumMember(Value = "PECOSBARSTOWTOYAHISD")]
        [Description("Pecos-Barstow-Toyah ISD")]
        PecosBarstowToyah,
        [EnumMember(Value = "PENELOPEISD")]
        [Description("Penelope ISD")]
        Penelope,
        [EnumMember(Value = "PERRINWHITTCONSISD")]
        [Description("Perrin-Whitt Consolidated ISD")]
        PerrinWhittConsolidated,
        [EnumMember(Value = "PERRYTONISD")]
        [Description("Perryton ISD")]
        Perryton,
        [EnumMember(Value = "PETERSBURGISD")]
        [Description("Petersburg ISD")]
        Petersburg,
        [EnumMember(Value = "PETROLIAISD")]
        [Description("Petrolia ISD")]
        Petrolia,
        [EnumMember(Value = "PETTUSISD")]
        [Description("Pettus ISD")]
        Pettus,
        [EnumMember(Value = "PEWITTCONSISD")]
        [Description("Pewitt Consolidated ISD")]
        PewittConsolidated,
        [EnumMember(Value = "Pfluger")]
        [Description("Pflugerville ISD")]
        Pflugerville,
        [EnumMember(Value = "PHARRSANJUANALAMOISD")]
        [Description("Pharr-San Juan-Alamo ISD")]
        PharrSanJuanAlamo,
        [EnumMember(Value = "PILOTPOINTISD")]
        [Description("Pilot Point ISD")]
        PilotPoint,
        [EnumMember(Value = "PINETREEISD")]
        [Description("Pine Tree ISD")]
        PineTree,
        [EnumMember(Value = "PITTSBURGISD")]
        [Description("Pittsburg ISD")]
        Pittsburg,
        [EnumMember(Value = "PLAINSISD")]
        [Description("Plains ISD")]
        Plains,
        [EnumMember(Value = "PLAINVIEWISD")]
        [Description("Plainview ISD")]
        Plainview,
        [EnumMember(Value = "PLANOISD")]
        [Description("Plano ISD")]
        Plano,
        [EnumMember(Value = "PLEASANTGROVEISD")]
        [Description("Pleasant Grove ISD")]
        PleasantGrove,
        [EnumMember(Value = "PLEASANTONISD")]
        [Description("Pleasanton ISD")]
        Pleasanton,
        [EnumMember(Value = "PLEMONSSTINNETTPHILLIPSCONSISD")]
        [Description("Plemons-Stinnett-Phillips Consolidated Independent Sd")]
        PlemonsStinnettPhillipsConsolidated,
        [EnumMember(Value = "POINTISABELISD")]
        [Description("Point Isabel ISD")]
        PointIsabel,
        [EnumMember(Value = "PONDERISD")]
        [Description("Ponder ISD")]
        Ponder,
        [EnumMember(Value = "POOLVILLEISD")]
        [Description("Poolville ISD")]
        Poolville,
        [EnumMember(Value = "PortAra")]
        [Description("Port Aransas ISD")]
        PortAransas,
        [EnumMember(Value = "PORTARTHURISD")]
        [Description("Port Arthur ISD")]
        PortArthur,
        [EnumMember(Value = "PORTNECHESGROVESISD")]
        [Description("Port Neches-Groves ISD")]
        PortNechesGroves,
        [EnumMember(Value = "POSTISD")]
        [Description("Post ISD")]
        Post,
        [EnumMember(Value = "POTEETISD")]
        [Description("Poteet ISD")]
        Poteet,
        [EnumMember(Value = "Poth")]
        [Description("Poth ISD")]
        Poth,
        [EnumMember(Value = "POTTSBOROISD")]
        [Description("Pottsboro ISD")]
        Pottsboro,
        [EnumMember(Value = "Prairie")]
        [Description("Prairie Lea ISD")]
        PrairieLea,
        [EnumMember(Value = "PRAIRIEVALLEYISD")]
        [Description("Prairie Valley ISD")]
        PrairieValley,
        [EnumMember(Value = "PRAIRILANDISD")]
        [Description("Prairiland ISD")]
        Prairiland,
        [EnumMember(Value = "PREMONTISD")]
        [Description("Premont ISD")]
        Premont,
        [EnumMember(Value = "PRESIDIOISD")]
        [Description("Presidio ISD")]
        Presidio,
        [EnumMember(Value = "PRIDDYISD")]
        [Description("Priddy ISD")]
        Priddy,
        [EnumMember(Value = "PRINCETONISD")]
        [Description("Princeton ISD")]
        Princeton,
        [EnumMember(Value = "PRINGLEMORSECONSISD")]
        [Description("Pringle-Morse Consolidated ISD")]
        PringleMorseConsolidated,
        [EnumMember(Value = "PROGRESOISD")]
        [Description("Progreso ISD")]
        Progreso,
        [EnumMember(Value = "PROSPERISD")]
        [Description("Prosper ISD")]
        Prosper,
        [EnumMember(Value = "QUANAHISD")]
        [Description("Quanah ISD")]
        Quanah,
        [EnumMember(Value = "QUEENCITYISD")]
        [Description("Queen City ISD")]
        QueenCity,
        [EnumMember(Value = "QUINLANISD")]
        [Description("Quinlan ISD")]
        Quinlan,
        [EnumMember(Value = "QUITMANISD")]
        [Description("Quitman ISD")]
        Quitman,
        [EnumMember(Value = "RAINSISD")]
        [Description("Rains ISD")]
        Rains,
        [EnumMember(Value = "RALLSISD")]
        [Description("Ralls ISD")]
        Ralls,
        [EnumMember(Value = "RAMIRECOMMONSCHOOLDISTRICT")]
        [Description("Ramire Common School District")]
        RamireCommonSchoolDistrict,
        [EnumMember(Value = "RANDOLPHFIELDISD")]
        [Description("Randolph Field ISD")]
        RandolphField,
        [EnumMember(Value = "RANGERISD")]
        [Description("Ranger ISD")]
        Ranger,
        [EnumMember(Value = "RANKINISD")]
        [Description("Rankin ISD")]
        Rankin,
        [EnumMember(Value = "RAYMONDVILLEISD")]
        [Description("Raymondville ISD")]
        Raymondville,
        [EnumMember(Value = "REAGANCOUNTYISD")]
        [Description("Reagan County ISD")]
        ReaganCounty,
        [EnumMember(Value = "REDLICKISD")]
        [Description("Red Lick ISD")]
        RedLick,
        [EnumMember(Value = "REDOAKISD")]
        [Description("Red Oak ISD")]
        RedOak,
        [EnumMember(Value = "REDWATERISD")]
        [Description("Redwater ISD")]
        Redwater,
        [EnumMember(Value = "REFUGIOISD")]
        [Description("Refugio ISD")]
        Refugio,
        [EnumMember(Value = "RICARDOISD")]
        [Description("Ricardo ISD")]
        Ricardo,
        [EnumMember(Value = "RICECONSISD")]
        [Description("Rice Consolidated ISD")]
        RiceConsolidated,
        [EnumMember(Value = "RICEISD")]
        [Description("Rice ISD")]
        Rice,
        [EnumMember(Value = "RICHARDSISD")]
        [Description("Richards ISD")]
        Richards,
        [EnumMember(Value = "RICHARDSONISD")]
        [Description("Richardson ISD")]
        Richardson,
        [EnumMember(Value = "RICHLANDSPRINGSISD")]
        [Description("Richland Springs ISD")]
        RichlandSprings,
        [EnumMember(Value = "RIESELISD")]
        [Description("Riesel ISD")]
        Riesel,
        [EnumMember(Value = "RIOHONDOISD")]
        [Description("Rio Hondo ISD")]
        RioHondo,
        [EnumMember(Value = "RIOVISTAISD")]
        [Description("Rio Vista ISD")]
        RioVista,
        [EnumMember(Value = "RISINGSTARISD")]
        [Description("Rising Star ISD")]
        RisingStar,
        [EnumMember(Value = "RIVERROADISD")]
        [Description("River Road ISD")]
        RiverRoad,
        [EnumMember(Value = "RIVERCRESTISD")]
        [Description("Rivercrest ISD")]
        Rivercrest,
        [EnumMember(Value = "RIVIERAISD")]
        [Description("Riviera ISD")]
        Riviera,
        [EnumMember(Value = "ROBERTLEEISD")]
        [Description("Robert Lee ISD")]
        RobertLee,
        [EnumMember(Value = "ROBINSONISD")]
        [Description("Robinson ISD")]
        Robinson,
        [EnumMember(Value = "ROBSTOWNISD")]
        [Description("Robstown ISD")]
        Robstown,
        [EnumMember(Value = "ROBYCONSISD")]
        [Description("Roby Consolidated ISD")]
        RobyConsolidated,
        [EnumMember(Value = "ROCHELLEISD")]
        [Description("Rochelle ISD")]
        Rochelle,
        [EnumMember(Value = "ROCKDALEISD")]
        [Description("Rockdale ISD")]
        Rockdale,
        [EnumMember(Value = "ROCKSPRINGSISD")]
        [Description("Rocksprings ISD")]
        Rocksprings,
        [EnumMember(Value = "ROCKWALLISD")]
        [Description("Rockwall ISD")]
        Rockwall,
        [EnumMember(Value = "ROGERSISD")]
        [Description("Rogers ISD")]
        Rogers,
        [EnumMember(Value = "ROMAISD")]
        [Description("Roma ISD")]
        Roma,
        [EnumMember(Value = "ROOSEVELTISD")]
        [Description("Roosevelt ISD")]
        Roosevelt,
        [EnumMember(Value = "ROPESISD")]
        [Description("Ropes ISD")]
        Ropes,
        [EnumMember(Value = "ROSCOEISD")]
        [Description("Roscoe ISD")]
        Roscoe,
        [EnumMember(Value = "ROSEBUDLOTTISD")]
        [Description("Rosebud-Lott ISD")]
        RosebudLott,
        [EnumMember(Value = "ROTANISD")]
        [Description("Rotan ISD")]
        Rotan,
        [EnumMember(Value = "Round")]
        [Description("Round Rock ISD")]
        RoundRock,
        [EnumMember(Value = "ROUNDTOPCARMINEISD")]
        [Description("Round Top-Carmine ISD")]
        RoundTopCarmine,
        [EnumMember(Value = "ROXTONISD")]
        [Description("Roxton ISD")]
        Roxton,
        [EnumMember(Value = "ROYALISD")]
        [Description("Royal ISD")]
        Royal,
        [EnumMember(Value = "ROYSECITYISD")]
        [Description("Royse City ISD")]
        RoyseCity,
        [EnumMember(Value = "RULEISD")]
        [Description("Rule ISD")]
        Rule,
        [EnumMember(Value = "Runge")]
        [Description("Runge ISD")]
        Runge,
        [EnumMember(Value = "RUSKISD")]
        [Description("Rusk ISD")]
        Rusk,
        [EnumMember(Value = "SANDSCONSISD")]
        [Description("S And S Consolidated ISD")]
        SAndSConsolidated,
        [EnumMember(Value = "SABINALISD")]
        [Description("Sabinal ISD")]
        Sabinal,
        [EnumMember(Value = "SABINEISD")]
        [Description("Sabine ISD")]
        Sabine,
        [EnumMember(Value = "SABINEPASSISD")]
        [Description("Sabine Pass ISD")]
        SabinePass,
        [EnumMember(Value = "SAINTJOISD")]
        [Description("Saint Jo ISD")]
        SaintJo,
        [EnumMember(Value = "SALADOISD")]
        [Description("Salado ISD")]
        Salado,
        [EnumMember(Value = "SALTILLOISD")]
        [Description("Saltillo ISD")]
        Saltillo,
        [EnumMember(Value = "SAMRAYBURNISD")]
        [Description("Sam Rayburn ISD")]
        SamRayburn,
        [EnumMember(Value = "SANANGELOISD")]
        [Description("San Angelo ISD")]
        SanAngelo,
        [EnumMember(Value = "SanAnt")]
        [Description("San Antonio ISD")]
        SanAntonio,
        [EnumMember(Value = "SANAUGUSTINEISD")]
        [Description("San Augustine ISD")]
        SanAugustine,
        [EnumMember(Value = "SANBENITOCONSISD")]
        [Description("San Benito Consolidated ISD")]
        SanBenitoConsolidated,
        [EnumMember(Value = "SANDIEGOISD")]
        [Description("San Diego ISD")]
        SanDiego,
        [EnumMember(Value = "SANELIZARIOISD")]
        [Description("San Elizario ISD")]
        SanElizario,
        [EnumMember(Value = "SANFELIPEDELRIOCONSISD")]
        [Description("San Felipe-Del Rio Consolidated ISD")]
        SanFelipeDelRioConsolidated,
        [EnumMember(Value = "SANISIDROISD")]
        [Description("San Isidro ISD")]
        SanIsidro,
        [EnumMember(Value = "SanMar")]
        [Description("San Marcos Cisd")]
        SanMarcosCisd,
        [EnumMember(Value = "SANPERLITAISD")]
        [Description("San Perlita ISD")]
        SanPerlita,
        [EnumMember(Value = "SANSABAISD")]
        [Description("San Saba ISD")]
        SanSaba,
        [EnumMember(Value = "SANVICENTEISD")]
        [Description("San Vicente ISD")]
        SanVicente,
        [EnumMember(Value = "SANFORDFRITCHISD")]
        [Description("Sanford-Fritch ISD")]
        SanfordFritch,
        [EnumMember(Value = "SANGERISD")]
        [Description("Sanger ISD")]
        Sanger,
        [EnumMember(Value = "SANTAANNAISD")]
        [Description("Santa Anna ISD")]
        SantaAnna,
        [EnumMember(Value = "SANTAFEISD")]
        [Description("Santa Fe ISD")]
        SantaFe,
        [EnumMember(Value = "SANTAGERTRUDISISD")]
        [Description("Santa Gertrudis ISD")]
        SantaGertrudis,
        [EnumMember(Value = "SANTAMARIAISD")]
        [Description("Santa Maria ISD")]
        SantaMaria,
        [EnumMember(Value = "SANTAROSAISD")]
        [Description("Santa Rosa ISD")]
        SantaRosa,
        [EnumMember(Value = "SANTOISD")]
        [Description("Santo ISD")]
        Santo,
        [EnumMember(Value = "SAVOYISD")]
        [Description("Savoy ISD")]
        Savoy,
        [EnumMember(Value = "Schert")]
        [Description("Schertz-Cibolo Universal City ISD")]
        SchertzCiboloUniversalCity,
        [EnumMember(Value = "SCHLEICHERISD")]
        [Description("Schleicher ISD")]
        Schleicher,
        [EnumMember(Value = "SCHULENBURGISD")]
        [Description("Schulenburg ISD")]
        Schulenburg,
        [EnumMember(Value = "SCURRYROSSERISD")]
        [Description("Scurry-Rosser ISD")]
        ScurryRosser,
        [EnumMember(Value = "SEAGRAVESISD")]
        [Description("Seagraves ISD")]
        Seagraves,
        [EnumMember(Value = "SEALYISD")]
        [Description("Sealy ISD")]
        Sealy,
        [EnumMember(Value = "Seguin")]
        [Description("Seguin ISD")]
        Seguin,
        [EnumMember(Value = "SEMINOLEISD")]
        [Description("Seminole ISD")]
        Seminole,
        [EnumMember(Value = "SEYMOURISD")]
        [Description("Seymour ISD")]
        Seymour,
        [EnumMember(Value = "SHALLOWATERISD")]
        [Description("Shallowater ISD")]
        Shallowater,
        [EnumMember(Value = "SHAMROCKISD")]
        [Description("Shamrock ISD")]
        Shamrock,
        [EnumMember(Value = "SHARYLANDISD")]
        [Description("Sharyland ISD")]
        Sharyland,
        [EnumMember(Value = "SHELBYVILLEISD")]
        [Description("Shelbyville ISD")]
        Shelbyville,
        [EnumMember(Value = "SHELDONISD")]
        [Description("Sheldon ISD")]
        Sheldon,
        [EnumMember(Value = "SHEPHERDISD")]
        [Description("Shepherd ISD")]
        Shepherd,
        [EnumMember(Value = "SHERMANISD")]
        [Description("Sherman ISD")]
        Sherman,
        [EnumMember(Value = "Shiner")]
        [Description("Shiner ISD")]
        Shiner,
        [EnumMember(Value = "SIDNEYISD")]
        [Description("Sidney ISD")]
        Sidney,
        [EnumMember(Value = "SIERRABLANCAISD")]
        [Description("Sierra Blanca ISD")]
        SierraBlanca,
        [EnumMember(Value = "SILSBEEISD")]
        [Description("Silsbee ISD")]
        Silsbee,
        [EnumMember(Value = "SILVERTONISD")]
        [Description("Silverton ISD")]
        Silverton,
        [EnumMember(Value = "SIMMSISD")]
        [Description("Simms ISD")]
        Simms,
        [EnumMember(Value = "SINTONISD")]
        [Description("Sinton ISD")]
        Sinton,
        [EnumMember(Value = "SIVELLSBENDISD")]
        [Description("Sivells Bend ISD")]
        SivellsBend,
        [EnumMember(Value = "SKIDMORETYNANISD")]
        [Description("Skidmore-Tynan ISD")]
        SkidmoreTynan,
        [EnumMember(Value = "SLATONISD")]
        [Description("Slaton ISD")]
        Slaton,
        [EnumMember(Value = "SLIDELLISD")]
        [Description("Slidell ISD")]
        Slidell,
        [EnumMember(Value = "SLOCUMISD")]
        [Description("Slocum ISD")]
        Slocum,
        [EnumMember(Value = "Smith")]
        [Description("Smithville")]
        Smithville,
        [EnumMember(Value = "SMYERISD")]
        [Description("Smyer ISD")]
        Smyer,
        [EnumMember(Value = "SNOOKISD")]
        [Description("Snook ISD")]
        Snook,
        [EnumMember(Value = "SNYDERISD")]
        [Description("Snyder ISD")]
        Snyder,
        [EnumMember(Value = "SOCORROISD")]
        [Description("Socorro ISD")]
        Socorro,
        [EnumMember(Value = "Somer")]
        [Description("Somerset ISD")]
        Somerset,
        [EnumMember(Value = "SOMERVILLEISD")]
        [Description("Somerville ISD")]
        Somerville,
        [EnumMember(Value = "SONORAISD")]
        [Description("Sonora ISD")]
        Sonora,
        [EnumMember(Value = "SouSan")]
        [Description("South San Antonio ISD")]
        SouthSanAntonio,
        [EnumMember(Value = "SOUTHLANDISD")]
        [Description("Southland ISD")]
        Southland,
        [EnumMember(Value = "Souside")]
        [Description("Southside ISD")]
        Southside,
        [EnumMember(Value = "Souwes")]
        [Description("Southwest ISD")]
        Southwest,
        [EnumMember(Value = "SPEARMANISD")]
        [Description("Spearman ISD")]
        Spearman,
        [EnumMember(Value = "SPLENDORAISD")]
        [Description("Splendora ISD")]
        Splendora,
        [EnumMember(Value = "SPRINGBRANCHISD")]
        [Description("Spring Branch ISD")]
        SpringBranch,
        [EnumMember(Value = "SPRINGCREEKISD")]
        [Description("Spring Creek ISD")]
        SpringCreek,
        [EnumMember(Value = "SPRINGHILLISD")]
        [Description("Spring Hill ISD")]
        SpringHill,
        [EnumMember(Value = "SPRINGISD")]
        [Description("Spring ISD")]
        Spring,
        [EnumMember(Value = "SPRINGLAKEEARTHISD")]
        [Description("Springlake-Earth ISD")]
        SpringlakeEarth,
        [EnumMember(Value = "SPRINGTOWNISD")]
        [Description("Springtown ISD")]
        Springtown,
        [EnumMember(Value = "SPURISD")]
        [Description("Spur ISD")]
        Spur,
        [EnumMember(Value = "SPURGERISD")]
        [Description("Spurger ISD")]
        Spurger,
        [EnumMember(Value = "STAFFORDMUNICIPALSCHOOLDISTRICT")]
        [Description("Stafford Municipal School District")]
        StaffordMunicipalSchoolDistrict,
        [EnumMember(Value = "STAMFORDISD")]
        [Description("Stamford ISD")]
        Stamford,
        [EnumMember(Value = "STANTONISD")]
        [Description("Stanton ISD")]
        Stanton,
        [EnumMember(Value = "STARISD")]
        [Description("Star ISD")]
        Star,
        [EnumMember(Value = "STEPHENVILLE")]
        [Description("Stephenville")]
        Stephenville,
        [EnumMember(Value = "STERLINGCITYISD")]
        [Description("Sterling City ISD")]
        SterlingCity,
        [EnumMember(Value = "Stockd")]
        [Description("Stockdale ISD")]
        Stockdale,
        [EnumMember(Value = "STRATFORDISD")]
        [Description("Stratford ISD")]
        Stratford,
        [EnumMember(Value = "STRAWNISD")]
        [Description("Strawn ISD")]
        Strawn,
        [EnumMember(Value = "SUDANISD")]
        [Description("Sudan ISD")]
        Sudan,
        [EnumMember(Value = "SULPHURBLUFFISD")]
        [Description("Sulphur Bluff ISD")]
        SulphurBluff,
        [EnumMember(Value = "SULPHURSPRINGSISD")]
        [Description("Sulphur Springs ISD")]
        SulphurSprings,
        [EnumMember(Value = "SUNDOWNISD")]
        [Description("Sundown ISD")]
        Sundown,
        [EnumMember(Value = "SUNNYVALEISD")]
        [Description("Sunnyvale ISD")]
        Sunnyvale,
        [EnumMember(Value = "SUNRAYISD")]
        [Description("Sunray ISD")]
        Sunray,
        [EnumMember(Value = "SWEENYISD")]
        [Description("Sweeny ISD")]
        Sweeny,
        [EnumMember(Value = "SWEETHOMEISD")]
        [Description("Sweet Home ISD")]
        SweetHome,
        [EnumMember(Value = "SWEETWATERISD")]
        [Description("Sweetwater ISD")]
        Sweetwater,
        [EnumMember(Value = "TAFTISD")]
        [Description("Taft ISD")]
        Taft,
        [EnumMember(Value = "TAHOKAISD")]
        [Description("Tahoka ISD")]
        Tahoka,
        [EnumMember(Value = "TARKINGTONISD")]
        [Description("Tarkington ISD")]
        Tarkington,
        [EnumMember(Value = "TATUMISD")]
        [Description("Tatum ISD")]
        Tatum,
        [EnumMember(Value = "TAYLORISD")]
        [Description("Taylor ISD")]
        Taylor,
        [EnumMember(Value = "TEAGUEISD")]
        [Description("Teague ISD")]
        Teague,
        [EnumMember(Value = "TEMPLEISD")]
        [Description("Temple ISD")]
        Temple,
        [EnumMember(Value = "TENAHAISD")]
        [Description("Tenaha ISD")]
        Tenaha,
        [EnumMember(Value = "TERLINGUCOMMONSCHOOLDISTRICT")]
        [Description("Terlingu Common School District")]
        TerlinguCommonSchoolDistrict,
        [EnumMember(Value = "TERRELLCOUNTYISD")]
        [Description("Terrell County ISD")]
        TerrellCounty,
        [EnumMember(Value = "TERRELLISD")]
        [Description("Terrell ISD")]
        Terrell,
        [EnumMember(Value = "TEXARKANAISD")]
        [Description("Texarkana ISD")]
        Texarkana,
        [EnumMember(Value = "TEXASCITYISD")]
        [Description("Texas City ISD")]
        TexasCity,
        [EnumMember(Value = "TEXHOMAISD")]
        [Description("Texhoma ISD")]
        Texhoma,
        [EnumMember(Value = "TEXLINEISD")]
        [Description("Texline ISD")]
        Texline,
        [EnumMember(Value = "THORNDALEISD")]
        [Description("Thorndale ISD")]
        Thorndale,
        [EnumMember(Value = "THRALLISD")]
        [Description("Thrall ISD")]
        Thrall,
        [EnumMember(Value = "THREERIVERSISD")]
        [Description("Three Rivers ISD")]
        ThreeRivers,
        [EnumMember(Value = "THREEWAYISD")]
        [Description("Three Way ISD")]
        ThreeWay,
        [EnumMember(Value = "THROCKMORTONISD")]
        [Description("Throckmorton ISD")]
        Throckmorton,
        [EnumMember(Value = "TIDEHAVENISD")]
        [Description("Tidehaven ISD")]
        Tidehaven,
        [EnumMember(Value = "TIMPSONISD")]
        [Description("Timpson ISD")]
        Timpson,
        [EnumMember(Value = "TIOGAISD")]
        [Description("Tioga ISD")]
        Tioga,
        [EnumMember(Value = "TOLARISD")]
        [Description("Tolar ISD")]
        Tolar,
        [EnumMember(Value = "TOMBEANISD")]
        [Description("Tom Bean ISD")]
        TomBean,
        [EnumMember(Value = "TOMBALLISD")]
        [Description("Tomball ISD")]
        Tomball,
        [EnumMember(Value = "TORNILLOISD")]
        [Description("Tornillo ISD")]
        Tornillo,
        [EnumMember(Value = "TRENTISD")]
        [Description("Trent ISD")]
        Trent,
        [EnumMember(Value = "TRENTONISD")]
        [Description("Trenton ISD")]
        Trenton,
        [EnumMember(Value = "TRINIDADISD")]
        [Description("Trinidad ISD")]
        Trinidad,
        [EnumMember(Value = "TRINITYISD")]
        [Description("Trinity ISD")]
        Trinity,
        [EnumMember(Value = "TROUPISD")]
        [Description("Troup ISD")]
        Troup,
        [EnumMember(Value = "TROYISD")]
        [Description("Troy ISD")]
        Troy,
        [EnumMember(Value = "TULIAISD")]
        [Description("Tulia ISD")]
        Tulia,
        [EnumMember(Value = "TULOSOMIDWAYISD")]
        [Description("Tuloso-Midway ISD")]
        TulosoMidway,
        [EnumMember(Value = "TURKEYQUITAQUEISD")]
        [Description("Turkey-Quitaque ISD")]
        TurkeyQuitaque,
        [EnumMember(Value = "TYLERISD")]
        [Description("Tyler ISD")]
        Tyler,
        [EnumMember(Value = "UNIONGROVEISD")]
        [Description("Union Grove ISD")]
        UnionGrove,
        [EnumMember(Value = "UNITEDISD")]
        [Description("United ISD")]
        United,
        [EnumMember(Value = "UTOPIAISD")]
        [Description("Utopia ISD")]
        Utopia,
        [EnumMember(Value = "UVALDECONSISD")]
        [Description("Uvalde Consolidated ISD")]
        UvaldeConsolidated,
        [EnumMember(Value = "VALENTINEISD")]
        [Description("Valentine ISD")]
        Valentine,
        [EnumMember(Value = "VALLEYMILLSISD")]
        [Description("Valley Mills ISD")]
        ValleyMills,
        [EnumMember(Value = "VALLEYVIEWISD")]
        [Description("Valley View ISD")]
        ValleyView,
        [EnumMember(Value = "VANALSTYNEISD")]
        [Description("Van Alstyne ISD")]
        VanAlstyne,
        [EnumMember(Value = "VANISD")]
        [Description("Van ISD")]
        Van,
        [EnumMember(Value = "VANVLECKISD")]
        [Description("Van Vleck ISD")]
        VanVleck,
        [EnumMember(Value = "VEGAISD")]
        [Description("Vega ISD")]
        Vega,
        [EnumMember(Value = "VENUSISD")]
        [Description("Venus ISD")]
        Venus,
        [EnumMember(Value = "VERIBESTISD")]
        [Description("Veribest ISD")]
        Veribest,
        [EnumMember(Value = "VERNONISD")]
        [Description("Vernon ISD")]
        Vernon,
        [EnumMember(Value = "VICTORIAISD")]
        [Description("Victoria ISD")]
        Victoria,
        [EnumMember(Value = "VIDORISD")]
        [Description("Vidor ISD")]
        Vidor,
        [EnumMember(Value = "VYSEHRADISD")]
        [Description("Vysehrad ISD")]
        Vysehrad,
        [EnumMember(Value = "WACOISD")]
        [Description("Waco ISD")]
        Waco,
        [EnumMember(Value = "Waelde")]
        [Description("Waelder ISD")]
        Waelder,
        [EnumMember(Value = "WALCOTTISD")]
        [Description("Walcott ISD")]
        Walcott,
        [EnumMember(Value = "WALLISD")]
        [Description("Wall ISD")]
        Wall,
        [EnumMember(Value = "WALLERISD")]
        [Description("Waller ISD")]
        Waller,
        [EnumMember(Value = "WALNUTBENDISD")]
        [Description("Walnut Bend ISD")]
        WalnutBend,
        [EnumMember(Value = "WALNUTSPRINGSISD")]
        [Description("Walnut Springs ISD")]
        WalnutSprings,
        [EnumMember(Value = "WARRENISD")]
        [Description("Warren ISD")]
        Warren,
        [EnumMember(Value = "WASKOMISD")]
        [Description("Waskom ISD")]
        Waskom,
        [EnumMember(Value = "WATERVALLEYISD")]
        [Description("Water Valley ISD")]
        WaterValley,
        [EnumMember(Value = "WAXAHACHIEISD")]
        [Description("Waxahachie ISD")]
        Waxahachie,
        [EnumMember(Value = "WEATHERFORDISD")]
        [Description("Weatherford ISD")]
        Weatherford,
        [EnumMember(Value = "WEBBCONSISD")]
        [Description("Webb Consolidated ISD")]
        WebbConsolidated,
        [EnumMember(Value = "WEIMARISD")]
        [Description("Weimar ISD")]
        Weimar,
        [EnumMember(Value = "WELLINGTONISD")]
        [Description("Wellington ISD")]
        Wellington,
        [EnumMember(Value = "WELLMANUNIONCONSISD")]
        [Description("Wellman-Union Consolidated ISD")]
        WellmanUnionConsolidated,
        [EnumMember(Value = "WELLSISD")]
        [Description("Wells ISD")]
        Wells,
        [EnumMember(Value = "WESLACOISD")]
        [Description("Weslaco ISD")]
        Weslaco,
        [EnumMember(Value = "WESTHARDINCOUNTYCONSISD")]
        [Description("West Hardin County Consolidated ISD")]
        WestHardinCountyConsolidated,
        [EnumMember(Value = "WESTISD")]
        [Description("West ISD")]
        West,
        [EnumMember(Value = "WESTORANGECOVECONSISD")]
        [Description("West Orange-Cove Consolidated ISD")]
        WestOrangeCoveConsolidated,
        [EnumMember(Value = "WESTOSOISD")]
        [Description("West Oso ISD")]
        WestOso,
        [EnumMember(Value = "WESTRUSKISD")]
        [Description("West Rusk ISD")]
        WestRusk,
        [EnumMember(Value = "WESTSABINEISD")]
        [Description("West Sabine ISD")]
        WestSabine,
        [EnumMember(Value = "WESTBROOKISD")]
        [Description("Westbrook ISD")]
        Westbrook,
        [EnumMember(Value = "WESTHOFFISD")]
        [Description("Westhoff ISD")]
        Westhoff,
        [EnumMember(Value = "WESTPHALIAISD")]
        [Description("Westphalia ISD")]
        Westphalia,
        [EnumMember(Value = "WESTWOODISD")]
        [Description("Westwood ISD")]
        Westwood,
        [EnumMember(Value = "WHARTONISD")]
        [Description("Wharton ISD")]
        Wharton,
        [EnumMember(Value = "WHEELERISD")]
        [Description("Wheeler ISD")]
        Wheeler,
        [EnumMember(Value = "WHITEDEERISD")]
        [Description("White Deer ISD")]
        WhiteDeer,
        [EnumMember(Value = "WHITEOAKISD")]
        [Description("White Oak ISD")]
        WhiteOak,
        [EnumMember(Value = "WHITESETTLEMENTISD")]
        [Description("White Settlement ISD")]
        WhiteSettlement,
        [EnumMember(Value = "WHITEFACECONSISD")]
        [Description("Whiteface Consolidated ISD")]
        WhitefaceConsolidated,
        [EnumMember(Value = "WHITEHOUSEISD")]
        [Description("Whitehouse ISD")]
        Whitehouse,
        [EnumMember(Value = "WHITESBOROISD")]
        [Description("Whitesboro ISD")]
        Whitesboro,
        [EnumMember(Value = "WHITEWRIGHTISD")]
        [Description("Whitewright ISD")]
        Whitewright,
        [EnumMember(Value = "WHITHARRALISD")]
        [Description("Whitharral ISD")]
        Whitharral,
        [EnumMember(Value = "WHITNEYISD")]
        [Description("Whitney ISD")]
        Whitney,
        [EnumMember(Value = "WICHITAFALLSISD")]
        [Description("Wichita Falls ISD")]
        WichitaFalls,
        [EnumMember(Value = "WILDORADOISD")]
        [Description("Wildorado ISD")]
        Wildorado,
        [EnumMember(Value = "WILLISISD")]
        [Description("Willis ISD")]
        Willis,
        [EnumMember(Value = "WILLSPOINTISD")]
        [Description("Wills Point ISD")]
        WillsPoint,
        [EnumMember(Value = "WILSONISD")]
        [Description("Wilson ISD")]
        Wilson,
        [EnumMember(Value = "Wimber")]
        [Description("Wimberley ISD")]
        Wimberley,
        [EnumMember(Value = "WINDTHORSTISD")]
        [Description("Windthorst ISD")]
        Windthorst,
        [EnumMember(Value = "WINFIELDISD")]
        [Description("Winfield ISD")]
        Winfield,
        [EnumMember(Value = "WINKLOVINGISD")]
        [Description("Wink-Loving ISD")]
        WinkLoving,
        [EnumMember(Value = "WINNSBOROISD")]
        [Description("Winnsboro ISD")]
        Winnsboro,
        [EnumMember(Value = "WINONAISD")]
        [Description("Winona ISD")]
        Winona,
        [EnumMember(Value = "WINTERSISD")]
        [Description("Winters ISD")]
        Winters,
        [EnumMember(Value = "WODENISD")]
        [Description("Woden ISD")]
        Woden,
        [EnumMember(Value = "WOLFECITYISD")]
        [Description("Wolfe City ISD")]
        WolfeCity,
        [EnumMember(Value = "WOODSBOROISD")]
        [Description("Woodsboro ISD")]
        Woodsboro,
        [EnumMember(Value = "WOODSONISD")]
        [Description("Woodson ISD")]
        Woodson,
        [EnumMember(Value = "WOODVILLEISD")]
        [Description("Woodville ISD")]
        Woodville,
        [EnumMember(Value = "WORTHAMISD")]
        [Description("Wortham ISD")]
        Wortham,
        [EnumMember(Value = "WYLIEISD")]
        [Description("Wylie ISD")]
        Wylie,
        [EnumMember(Value = "YANTISISD")]
        [Description("Yantis ISD")]
        Yantis,
        [EnumMember(Value = "YOAKUMISD")]
        [Description("Yoakum ISD")]
        Yoakum,
        [EnumMember(Value = "YORKTOWNISD")]
        [Description("Yorktown ISD")]
        Yorktown,
        [EnumMember(Value = "YSLETAISD")]
        [Description("Ysleta ISD")]
        Ysleta,
        [EnumMember(Value = "ZAPATACOUNTYISD")]
        [Description("Zapata County ISD")]
        ZapataCounty,
        [EnumMember(Value = "ZAVALLAISD")]
        [Description("Zavalla ISD")]
        Zavalla,
        [EnumMember(Value = "ZEPHYRISD")]
        [Description("Zephyr ISD")]
        Zephyr,
    }
}
