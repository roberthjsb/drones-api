using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace drones_api.DTOS
{
    public class MedicineDto
    {
        public string Code { get; set; }
        public string Nombre { get; set; }
        public int Weigth { get; set; }
        public IFormFile Image { get; set; }
    }
}
