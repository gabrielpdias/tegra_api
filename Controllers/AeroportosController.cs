using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using teste_api.DTO;
using teste_api.Infraestructure.Interfaces;
using teste_api.Models;

namespace teste_api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AeroportosController : ControllerBase
    {
        private IAeroportosRepository repository;

        public AeroportosController(IAeroportosRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> BuscarAeropostos()
        {
            try
            {
                var aeroportos = await repository.BuscarAeroportos();
                return Ok(aeroportos);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, "Erro ao listar aeroportos:\n" + e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> BuscarVoos([FromBody]FiltroVoos filtro)
        {
            try
            {
                var voos = await repository.BuscarVoos(filtro);

                return Ok(voos);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, "Erro ao buscar voos:\n" + e.Message);
            }
        }
    }
}