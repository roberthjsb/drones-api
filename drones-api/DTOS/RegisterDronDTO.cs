using System.ComponentModel.DataAnnotations;

namespace drones_api.DTOS
{
    public class RegisterDronDTO
    {
        [Required]
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        [Required]
        public int LimitWeight { get; set; }
    }
}
