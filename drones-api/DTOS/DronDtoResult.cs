using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace drones_api.DTOS
{
    public class DronDtoResult
    {
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public int LimitWeight { get; set; }
        public int BateryLevel { get; set; }
        public string State { get; set; }
    }
}
