using System.Collections.Generic;
using System.Threading.Tasks;
using teste_api.DTO;
using teste_api.Models;

namespace teste_api.Infraestructure.Interfaces
{
    public interface IAeroportosRepository
    {
        Task<List<Aeroportos>> BuscarAeroportos();
        Task<List<Planes>> Buscar99Planes();
        Task<List<UberAir>> BuscarUberAir();
        Task<List<Voo>> BuscarVoos(FiltroVoos filtro);
    }
}