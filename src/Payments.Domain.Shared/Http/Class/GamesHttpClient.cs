using Microsoft.Extensions.Configuration;
using Payments.Domain.Shared.Http;
using Payments.Domain.Shared.Http.Dto;
using Payments.Domain.Shared.Http.Interface;

namespace Payments.Domain.Shared.Http.Class;

public class GamesHttpClient : IGamesHttpClient
{
    private readonly IConfiguration _configuration;
    private readonly string _baseUrl;

    public GamesHttpClient(IConfiguration configuration)
    {
        _configuration = configuration;
        
        // Tenta ler de diferentes formas de configuração (docker-compose usa __ que vira :)
        _baseUrl = _configuration["ExternalServices:Games"] 
                   ?? _configuration["ExternalServices__Games"]
                   ?? "http://games-api:8080";
        
        // Remove trailing slash se existir e garante que é HTTP (não HTTPS) para Docker
        _baseUrl = _baseUrl.TrimEnd('/').Replace("https://", "http://");
        
        // Garante que usa o nome do serviço Docker, não localhost
        if (_baseUrl.Contains("localhost") || _baseUrl.Contains("127.0.0.1"))
        {
            _baseUrl = "http://games-api:8080";
        }
    }

    public LibraryDto.LibraryDtoResponse InserirJogo(LibraryDto.LibraryDtoRequest request)
    {
        var url = $"{_baseUrl}/api/v1/adicionar/jogo/biblioteca";
        return Request.Post<LibraryDto.LibraryDtoResponse>(url, request);
    }
}

