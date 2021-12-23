using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Husa.Cargador.ViewModels.ListingRequestSaleSubClass
{
    public class SpaceAndDimensionsTab
    {
        public int? CarpotSpaces { get; set; }

        public int? FullBaths { get; set; }

        public string GarageDescription { get; set; }

        public int? GarageLength { get; set; }

        public int? GarageSpaces { get; set; }

        public int? GarageWidth { get; set; }

        public string HousingType { get; set; }

        public int? HalfBaths { get; set; }

        public string PropertyType { get; set; }

        public int? Stories { get; set; }

        public decimal? SquareFeets { get; set; }

        public string SquareFeetSource { get; set; }

        public string MasterBedroom { get; set; }

        public string UtilityRoom { get; set; }

        public IEnumerable<Rooms> Rooms { get; set; }
    }
}
