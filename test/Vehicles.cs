using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public class Vehicle
    {
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; }
        public int VehicleTypeId { get; set; }
        public string GroupNumber { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public bool ConnectionState { get; set; }
        public int? StatusId { get; set; }
        public int Tacho { get; set; }
        public bool Visible { get; set; }
        public int TotalTacho { get; set; }
        public DateTime LastMsgDate { get; set; }
        public bool IsOff { get; set; }
        public string VehicleDescription { get; set; }
        public decimal? PrevLongitude { get; set; }
        public decimal? PrevLatitude { get; set; }
        public string Direction { get; set; }
        public string TrainNumber { get; set; }
        public string Relation { get; set; }
        public string LayoutBackgroundTop { get; set; }
        public string LayoutBackgroundSide { get; set; }
        public string EVNNo { get; set; }
        public bool EnableDSU { get; set; }
        public int? MaxErrorFramesPerHourCount { get; set; }
        public int? VehicleGroupId { get; set; }
        public int? OrganisationUnitId { get; set; }
        public string VehicleDescriptionOriginal { get; set; }
    }

}
