using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using teste_api.DTO;
using teste_api.Infraestructure.Interfaces;
using teste_api.Models;

namespace teste_api.Infraestructure.Repositories
{
    public class AeroportosRepository : IAeroportosRepository
    {
        private readonly string contentRootPath;
        CultureInfo provider = CultureInfo.InvariantCulture;

        public AeroportosRepository(IHostingEnvironment hostingEnvironment)
        {
            this.contentRootPath = hostingEnvironment.ContentRootPath;
        }

        public async Task<List<Aeroportos>> BuscarAeroportos()
        {
            var arquivo = await File.ReadAllTextAsync(contentRootPath + "/aeroportos.json");
            var aeroportos = JsonConvert.DeserializeObject<List<Aeroportos>>(arquivo);
            return aeroportos;
        }

        public async Task<List<Planes>> Buscar99Planes()
        {
            var arquivo = await File.ReadAllTextAsync(contentRootPath + "/99planes.json");
            var planes = JsonConvert.DeserializeObject<List<Planes>>(arquivo);
            //para trabalhar poder trabalhar com DateTime nas propriedades chegada e saída
            foreach (var plane in planes)
            {
                plane.Chegada = plane.Data_Saida.AddHours(plane.Chegada.Hour).AddMinutes(plane.Chegada.Minute);
                plane.Saida = plane.Data_Saida.AddHours(plane.Saida.Hour).AddMinutes(plane.Saida.Minute);
            }
            return planes;
        }

        public async Task<List<UberAir>> BuscarUberAir()
        {
            var arquivo = await File.ReadAllLinesAsync(contentRootPath + "/uberair.csv");
            
            //quebrar arquivo em linhas e dividi-los pela vírgula (split)
            var linhas = from line in arquivo
                      select (line.Split(',')).ToArray();
            var listaRetorno = new List<UberAir>();
            foreach (var linha in linhas)
            {
                var uberAir = new UberAir();
                
                //pular cabeçalho
                if (linha[0] != "numero_voo")
                {
                    uberAir.NumeroVoo = linha[0];
                    uberAir.AeroportoOrigem = linha[1];
                    uberAir.AeroportoDestino = linha[2];
                    uberAir.Data = DateTime.ParseExact(linha[3], "yyyy-MM-dd", provider);
                    //Today para não pegar a hora atual e somar o timespan com 0
                    uberAir.HorarioSaida = uberAir.Data.Add(TimeSpan.Parse(linha[4]));
                    uberAir.HorarioChegada = uberAir.Data.Add(TimeSpan.Parse(linha[5]));
                    uberAir.Preco = Convert.ToDecimal(linha[6]);
                    listaRetorno.Add(uberAir);
                }
            }

            return listaRetorno;
        }

        public async Task<List<Voo>> BuscarVoos(FiltroVoos filtro)
        {
            //buscar todos os voos
            var voosUberAir = await BuscarUberAir();
            var voos99Planes = await Buscar99Planes();

            //inicializar lista de retorno
            var voos = new List<Voo>();
            var dataVoo = DateTime.ParseExact(filtro.DataVoo, "yyyy-MM-dd", provider);

            //novo voo para todos os voos da UberAir
            var vooUberAir = new Voo();
            vooUberAir.Trechos = new List<Trecho>();
            
            //seleciona todos os voos com mesma data e origem
            var voosOrigem = voosUberAir.Where(x => x.Data == dataVoo && x.AeroportoOrigem == filtro.AeroportoOrigem).ToList();
            foreach (var uberAir in voosOrigem)
            {
                //seleciona todas possíveis escalas (como fica a meu critério, foi utilizado no máximo 2 escalas)
                var possiveisEscalas = voosUberAir.Where(x => x.AeroportoOrigem == uberAir.AeroportoDestino &&
                                                    x.HorarioSaida >= uberAir.HorarioChegada &&
                                                    x.AeroportoDestino == filtro.AeroportoDestino).ToList();
                if (possiveisEscalas.Count > 0)
                {
                    //se houver escala, cujo destino seja equivalente ao filtro passado via POST
                    // adiciona os trechos para cada escala
                    AdicionaTrecho(vooUberAir, uberAir);
                    AdicionaTrecho(vooUberAir, possiveisEscalas.First());
                }
            }
            //prepara para retornar no formato desejado
            vooUberAir.Chegada = vooUberAir.Trechos.LastOrDefault() == null ? new DateTime() : vooUberAir.Trechos.LastOrDefault().Chegada;
            vooUberAir.Saida = vooUberAir.Trechos.FirstOrDefault() == null ? new DateTime() : vooUberAir.Trechos.FirstOrDefault().Saida;
            vooUberAir.Origem = filtro.AeroportoOrigem;
            vooUberAir.Destino = filtro.AeroportoDestino;

            voos.Add(vooUberAir);

            //mesma lógica para o 99Planes
            var voo99Plane = new Voo();
            voo99Plane.Trechos = new List<Trecho>();
            var voosOrigem99 = voos99Planes.Where(x => x.Data_Saida == dataVoo && x.Origem == filtro.AeroportoOrigem).ToList();
            foreach (var vooPlane in voosOrigem99)
            {
                var filhos = voos99Planes.Where(x => x.Origem == vooPlane.Destino &&
                                                    x.Saida >= vooPlane.Chegada &&
                                                    x.Destino == filtro.AeroportoDestino).ToList();
                if (filhos.Count > 0)
                {
                    AdicionaTrecho(voo99Plane, null, vooPlane);
                    AdicionaTrecho(voo99Plane, null, filhos.First());
                }
            }
            voo99Plane.Chegada = voo99Plane.Trechos.LastOrDefault() == null ? new DateTime() : voo99Plane.Trechos.LastOrDefault().Chegada;
            voo99Plane.Saida = voo99Plane.Trechos.FirstOrDefault() == null ? new DateTime() : voo99Plane.Trechos.FirstOrDefault().Saida;
            voo99Plane.Origem = filtro.AeroportoOrigem;
            voo99Plane.Destino = filtro.AeroportoDestino;

            voos.Add(voo99Plane);
            return voos.OrderBy(x=>x.Saida).ToList();
        }
        
        private void AdicionaTrecho(Voo voo, UberAir air = null, Planes plane = null)
        {
            if (plane != null)
            {
                var trecho = new Trecho();
                trecho.Origem = plane.Origem;
                trecho.Destino = plane.Destino;
                trecho.Saida = plane.Data_Saida.AddHours(plane.Saida.Hour).AddMinutes(plane.Saida.Minute);
                trecho.Chegada = plane.Data_Saida.AddHours(plane.Chegada.Hour).AddMinutes(plane.Chegada.Minute);
                trecho.Operadora = "99Planes";
                trecho.Preco = plane.Valor;
                voo.Trechos.Add(trecho);
            }

            if (air != null)
            {
                var trecho = new Trecho();
                trecho.Origem = air.AeroportoOrigem;
                trecho.Destino = air.AeroportoDestino;
                trecho.Saida = air.Data.AddHours(air.HorarioSaida.Hour).AddMinutes(air.HorarioSaida.Minute);
                trecho.Chegada = air.Data.AddHours(air.HorarioChegada.Hour).AddMinutes(air.HorarioChegada.Minute);
                trecho.Operadora = "UberAir";
                trecho.Preco = air.Preco;
                voo.Trechos.Add(trecho);
            }
        }
    }
}