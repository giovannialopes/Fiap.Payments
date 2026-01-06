using System.Net;
using Microsoft.Extensions.Configuration;
using Payments.Domain.Shared.Http;
using Payments.Domain.Shared.Http.Dto;
using Payments.Domain.Shared.Http.Interface;

namespace Payments.Domain.Shared.Http.Class;

public class UsersHttpClient : IUsersHttpClient
{
    private readonly IConfiguration _configuration;
    private readonly string _baseUrl;

    public UsersHttpClient(IConfiguration configuration)
    {
        _configuration = configuration;
        _baseUrl = _configuration["ExternalServices:Users"] ?? "http://users-api:8080";
    }

    public CarteiraDto.CarteiraDtoResponse PegaSaldo(Guid perfilId)
    {
        var headers = new WebHeaderCollection { { "UsuarioId", perfilId.ToString() } };
        var url = $"{_baseUrl}/api/v1/consulta/saldos";
        return Request.Get<CarteiraDto.CarteiraDtoResponse>(url, headers: headers);
    }

    public CarteiraDto.CarteiraDtoResponse RemoverSaldo(CarteiraDto.CarteiraDtoRequest request)
    {
        var url = $"{_baseUrl}/api/v1/remover/saldos";
        return Request.Delete<CarteiraDto.CarteiraDtoResponse>(url, request);
    }
}

