
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class MotoDto
    {
        [Required]
       public string Identificador { get; set; }
        [Required]
        public string Modelo { get; set; }
        [Required]
        public  int Ano { get; set; }
        [Required]
        public string Placa { get; set; }
    }
}
