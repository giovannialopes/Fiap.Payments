using Payments.Domain.Shared.Http.Dto;

namespace Payments.Domain.Shared.Http.Interface;

public interface IUsersHttpClient
{
    CarteiraDto.CarteiraDtoResponse PegaSaldo(Guid perfilId);
    CarteiraDto.CarteiraDtoResponse RemoverSaldo(CarteiraDto.CarteiraDtoRequest request);
}

