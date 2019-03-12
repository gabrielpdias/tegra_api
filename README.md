# API 

Para a API, foi utilizado o .NET Core e para rodar o repositório, é necessário:
- Ter o SDK do .NET Core 2.0+ instalado na máquina;
- Abrir a pasta do repositório via prompt de comando e rodar `dotnet restore`, `dotnet build` e `dotnet run`

- Para o primeiro objetivo, o endpoint será http://localhost:51335/api/aeroportos e basta apenas chamá-lo via GET
- Para o segundo objetivo, o endpoint será http://localhost:51335/api/aeroportos e basta apenas chamá-lo via POST passando como parâmetro `Aeroporto de Origem`, `Aeroporto de Destino` e `Data do voo`.
**Exemplo de corpo para request**
```json
{
	"AeroportoOrigem":"MCZ",
	"AeroportoDestino":"PLU",
	"DataVoo":"2019-02-11"
}
```
