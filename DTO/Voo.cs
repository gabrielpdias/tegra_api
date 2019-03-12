using System;
using System.Collections.Generic;

namespace teste_api.DTO
{
    public class Voo
    {
        public string Origem { get; set; }
        public string Destino { get; set; }
        public DateTime Saida { get; set; }
        public DateTime Chegada { get; set; }
        public List<Trecho> Trechos { get; set; }
    }

    public class Trecho
    {
        public string Origem { get; set; }
        public string Destino { get; set; }
        public DateTime Saida { get; set; }
        public DateTime Chegada { get; set; }
        public string Operadora { get; set; }
        public decimal Preco { get; set; }
    }
}