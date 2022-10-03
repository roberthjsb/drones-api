using drones_api.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace drones_api.Entities
{
    public class Dron
    {
     [Required]
        public string SerialNumber { get; set; }
        [ListStringValidator(new[] { "peso ligero", "peso medio", "peso crucero", "peso pesado"})]
        public string Model { get; set; }
        [Range(0,500)]
        public int LimitWeight { get; set; }
        [Range(0,100)]
        public int BateryLevel { get; set; }
        [ListStringValidator(new[] { "INACTIVO", "CARGANDO", "CARGADO", "ENTREGANDO CARGA", "CARGA ENTREGADA", "REGRESANDO" })]
        public string State { get; set; }

        public ICollection<Medicine> Medicines { get; set; }
    }
}
