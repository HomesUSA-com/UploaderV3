using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.ViewModels.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MlsArea
    {
        [EnumMember(Value = "0100")]
        [Description("0100")]
        Hundred = 100,
        [EnumMember(Value = "0101")]
        [Description("0101")]
        HundredOne = 101,
        [EnumMember(Value = "0102")]
        [Description("0102")]
        HundredTwo = 102,
        [EnumMember(Value = "0103")]
        [Description("0103")]
        HundredThree = 103,
        [EnumMember(Value = "0104")]
        [Description("0104")]
        HundredFour = 104,
        [EnumMember(Value = "0105")]
        [Description("0105")]
        HundredFive = 105,
        [EnumMember(Value = "0200")]
        [Description("0200")]
        TwoHundred = 200,
        [EnumMember(Value = "0300")]
        [Description("0300")]
        ThreeHundred = 300,
        [EnumMember(Value = "0400")]
        [Description("0400")]
        FourHundred = 400,
        [EnumMember(Value = "0500")]
        [Description("0500")]
        FiveHundred = 500,
        [EnumMember(Value = "0600")]
        [Description("0600")]
        SixHundred = 600,
        [EnumMember(Value = "0700")]
        [Description("0700")]
        SevenHundred = 700,
        [EnumMember(Value = "0800")]
        [Description("0800")]
        EightHundred = 800,
        [EnumMember(Value = "0900")]
        [Description("0900")]
        NineHundred = 900,
        [EnumMember(Value = "1000")]
        [Description("1000")]
        Thousand = 1000,
        [EnumMember(Value = "1001")]
        [Description("1001")]
        ThousandOne = 1001,
        [EnumMember(Value = "1002")]
        [Description("1002")]
        ThousandTwo = 1002,
        [EnumMember(Value = "1003")]
        [Description("1003")]
        ThousandThree = 1003,
        [EnumMember(Value = "1004")]
        [Description("1004")]
        ThousandFour = 1004,
        [EnumMember(Value = "1005")]
        [Description("1005")]
        ThousandFive = 1005,
        [EnumMember(Value = "1006")]
        [Description("1006")]
        ThousandSix = 1006,
        [EnumMember(Value = "1100")]
        [Description("1100")]
        ThousandOneHundred = 1100,
        [EnumMember(Value = "1200")]
        [Description("1200")]
        ThousandTwoHundred = 1200,
        [EnumMember(Value = "1300")]
        [Description("1300")]
        ThousandThreeHundred = 1300,
        [EnumMember(Value = "1400")]
        [Description("1400")]
        ThousandFourHundred = 1400,
        [EnumMember(Value = "1500")]
        [Description("1500")]
        ThousandFiveHundred = 1500,
        [EnumMember(Value = "1600")]
        [Description("1600")]
        ThousandSixHundred = 1600,
        [EnumMember(Value = "1700")]
        [Description("1700")]
        ThousandSevenHundred = 1700,
        [EnumMember(Value = "1800")]
        [Description("1800")]
        ThousandEightHundred = 1800,
        [EnumMember(Value = "1801")]
        [Description("1801")]
        ThousandEightHundredOne = 1801,
        [EnumMember(Value = "1802")]
        [Description("1802")]
        ThousandEightHundredTwo = 1802,
        [EnumMember(Value = "1803")]
        [Description("1803")]
        ThousandEightHundredThree = 1803,
        [EnumMember(Value = "1804")]
        [Description("1804")]
        ThousandEightHundredFour = 1804,
        [EnumMember(Value = "1900")]
        [Description("1900")]
        ThousandNineHundred = 1900,
        [EnumMember(Value = "2000")]
        [Description("2000")]
        TwoThousand = 2000,
        [EnumMember(Value = "2001")]
        [Description("2001")]
        TwoThousandOne = 2001,
        [EnumMember(Value = "2002")]
        [Description("2002")]
        TwoThousandTwo = 2002,
        [EnumMember(Value = "2003")]
        [Description("2003")]
        TwoThousandThree = 2003,
        [EnumMember(Value = "2004")]
        [Description("2004")]
        TwoThousandFour = 2004,
        [EnumMember(Value = "2100")]
        [Description("2100")]
        TwoThousandOneHundred = 2100,
        [EnumMember(Value = "2200")]
        [Description("2200")]
        TwoThousandTwoHundred = 2200,
        [EnumMember(Value = "2300")]
        [Description("2300")]
        TwoThousandThreeHundred = 2300,
        [EnumMember(Value = "2301")]
        [Description("2301")]
        TwoThousandThreeHundredOne = 2301,
        [EnumMember(Value = "2302")]
        [Description("2302")]
        TwoThousandThreeHundredTwo = 2302,
        [EnumMember(Value = "2303")]
        [Description("2303")]
        TwoThousandThreeHundredThree = 2303,
        [EnumMember(Value = "2304")]
        [Description("2304")]
        TwoThousandThreeHundredFour = 2304,
        [EnumMember(Value = "2400")]
        [Description("2400")]
        TwoThousandFourHundred = 2400,
        [EnumMember(Value = "2500")]
        [Description("2500")]
        TwoThousandFiveHundred = 2500,
        [EnumMember(Value = "2501")]
        [Description("2501")]
        TwoThousandFiveHundredOne = 2501,
        [EnumMember(Value = "2502")]
        [Description("2502")]
        TwoThousandFiveHundredTwo = 2502,
        [EnumMember(Value = "2503")]
        [Description("2503")]
        TwoThousandFiveHundredThree = 2503,
        [EnumMember(Value = "2504")]
        [Description("2504")]
        TwoThousandFiveHundredFour = 2504,
        [EnumMember(Value = "2505")]
        [Description("2505")]
        TwoThousandFiveHundredFive = 2505,
        [EnumMember(Value = "2506")]
        [Description("2506")]
        TwoThousandFiveHundredSix = 2506,
        [EnumMember(Value = "2507")]
        [Description("2507")]
        TwoThousandFiveHundredSeven = 2507,
        [EnumMember(Value = "2508")]
        [Description("2508")]
        TwoThousandFiveHundredEight = 2508,
        [EnumMember(Value = "2509")]
        [Description("2509")]
        TwoThousandFiveHundredNine = 2509,
        [EnumMember(Value = "2510")]
        [Description("2510")]
        TwoThousandFiveHundredTen = 2510,
        [EnumMember(Value = "2600")]
        [Description("2600")]
        TwoThousandSixHundred = 2600,
        [EnumMember(Value = "2601")]
        [Description("2601")]
        TwoThousandSixHundredOne = 2601,
        [EnumMember(Value = "2602")]
        [Description("2602")]
        TwoThousandSixHundredTwo = 2602,
        [EnumMember(Value = "2603")]
        [Description("2603")]
        TwoThousandSixHundredThree = 2603,
        [EnumMember(Value = "2604")]
        [Description("2604")]
        TwoThousandSixHundredFour = 2604,
        [EnumMember(Value = "2605")]
        [Description("2605")]
        TwoThousandSixHundredFive = 2605,
        [EnumMember(Value = "2606")]
        [Description("2606")]
        TwoThousandSixHundredSix = 2606,
        [EnumMember(Value = "2607")]
        [Description("2607")]
        TwoThousandSixHundredSeven = 2607,
        [EnumMember(Value = "2608")]
        [Description("2608")]
        TwoThousandSixHundredEight = 2608,
        [EnumMember(Value = "2609")]
        [Description("2609")]
        TwoThousandSixHundredNine = 2609,
        [EnumMember(Value = "2610")]
        [Description("2610")]
        TwoThousandSixHundredTen = 2610,
        [EnumMember(Value = "2611")]
        [Description("2611")]
        TwoThousandSixHundredEleven = 2611,
        [EnumMember(Value = "2612")]
        [Description("2612")]
        TwoThousandSixHundredTwelve = 2612,
        [EnumMember(Value = "2613")]
        [Description("2613")]
        TwoThousandSixHundredThirteen = 2613,
        [EnumMember(Value = "2614")]
        [Description("2614")]
        TwoThousandSixHundredFourteen = 2614,
        [EnumMember(Value = "2615")]
        [Description("2615")]
        TwoThousandSixHundredFifteen = 2615,
        [EnumMember(Value = "2616")]
        [Description("2616")]
        TwoThousandSixHundredSixteen = 2616,
        [EnumMember(Value = "2617")]
        [Description("2617")]
        TwoThousandSixHundredSevenTeen = 2617,
        [EnumMember(Value = "2618")]
        [Description("2618")]
        TwoThousandSixHundredEighteen = 2618,
        [EnumMember(Value = "2619")]
        [Description("2619")]
        TwoThousandSixHundredNineteen = 2619,
        [EnumMember(Value = "2620")]
        [Description("2620")]
        TwoThousandSixHundredTwenty = 2620,
        [EnumMember(Value = "2621")]
        [Description("2621")]
        TwoThousandSixHundredTwentyOne = 2621,
        [EnumMember(Value = "2622")]
        [Description("2622")]
        TwoThousandSixHundredTwentyTwo = 2622,
        [EnumMember(Value = "2623")]
        [Description("2623")]
        TwoThousandSixHundredTwentyThree = 2623,
        [EnumMember(Value = "2624")]
        [Description("2624")]
        TwoThousandSixHundredTwentyFour = 2624,
        [EnumMember(Value = "2700")]
        [Description("2700")]
        TwoThousandSevenHundred = 2700,
        [EnumMember(Value = "2701")]
        [Description("2701")]
        TwoThousandSevenHundredOne = 2701,
        [EnumMember(Value = "2702")]
        [Description("2702")]
        TwoThousandSevenHundredTwo = 2702,
        [EnumMember(Value = "2703")]
        [Description("2703")]
        TwoThousandSevenHundredThree = 2703,
        [EnumMember(Value = "2704")]
        [Description("2704")]
        TwoThousandSevenHundredFour = 2704,
        [EnumMember(Value = "2705")]
        [Description("2705")]
        TwoThousandSevenHundredFive = 2705,
        [EnumMember(Value = "2706")]
        [Description("2706")]
        TwoThousandSevenHundredSix = 2706,
        [EnumMember(Value = "2707")]
        [Description("2707")]
        TwoThousandSevenHundredSeven = 2707,
        [EnumMember(Value = "2708")]
        [Description("2708")]
        TwoThousandSevenHundredEight = 2708,
        [EnumMember(Value = "2709")]
        [Description("2709")]
        TwoThousandSevenHundredNine = 2709,
        [EnumMember(Value = "2800")]
        [Description("2800")]
        TwoThousandEightHundred = 2800,
        [EnumMember(Value = "2900")]
        [Description("2900")]
        TwoThousandNineHundred = 2900,
        [EnumMember(Value = "3000")]
        [Description("3000")]
        ThreeThousand = 3000,
        [EnumMember(Value = "3100")]
        [Description("3100")]
        ThreeThousandOneHundred = 3100,
    }
}
