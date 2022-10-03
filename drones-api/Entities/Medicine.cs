using System.ComponentModel.DataAnnotations;

namespace drones_api.Entities
{
    public class Medicine
    {
        public string Code { get; set; }
        public string Nombre { get; set; }
        public int Weigth { get; set; }
        public byte[] Image { get; set; }

    }
}
