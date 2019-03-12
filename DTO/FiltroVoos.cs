using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace teste_api.DTO
{
    public class FiltroVoos
    {
        public string AeroportoOrigem { get; set; }
        public string AeroportoDestino { get; set; }
        public string DataVoo { get; set; }
    }
}
