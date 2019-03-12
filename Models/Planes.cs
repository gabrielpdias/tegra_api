using System;
using System.ComponentModel;

namespace teste_api.Models
{
    public class Planes
    {
        public string Voo { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public DateTime Data_Saida { get; set; }
        public DateTime Saida { get; set; }
        public DateTime Chegada { get; set; }
        public decimal Valor { get; set; }
    }
}