using System;

namespace teste_api.Models
{
    public class UberAir
    {
        public string NumeroVoo { get; set; }
        public string AeroportoOrigem { get; set; }
        public string AeroportoDestino { get; set; }
        public DateTime Data { get; set; }
        public DateTime HorarioSaida { get; set; }
        public DateTime HorarioChegada { get; set; }
        public decimal Preco { get; set; }
    }
}