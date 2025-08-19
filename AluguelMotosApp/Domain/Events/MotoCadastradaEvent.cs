using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public class MotoCadastradaEvent
    {
        public string Id { get; set; }
        public string Placa { get; set; }
        public int Ano { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    }
}
