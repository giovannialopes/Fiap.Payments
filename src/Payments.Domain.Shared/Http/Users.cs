using Payments.Domain.Shared.Http.Dto;
using System.Net;
using System.Reflection;

namespace Payments.Domain.Shared.Http;

public static class Users
{
    private const string _HOST = "https://users-api-fiap-f5eraydvfqatejb8.brazilsouth-01.azurewebsites.net/api/v1";


    public static CarteiraDto.CarteiraDtoResponse PegaSaldo(Guid perfilId) {
        var headers = new WebHeaderCollection { { "UsuarioId", perfilId.ToString() } };
        return Request.Get<CarteiraDto.CarteiraDtoResponse>($"{_HOST}/consulta/saldos", headers: headers);
    }

    public static CarteiraDto.CarteiraDtoResponse RemoverSaldo(CarteiraDto.CarteiraDtoRequest request)
    => Request.Delete<CarteiraDto.CarteiraDtoResponse>($"{_HOST}/remover/saldos", request);
}

