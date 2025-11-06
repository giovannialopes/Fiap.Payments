using Payments.Domain.Shared.Http.Dto;

namespace Payments.Domain.Shared.Http;

public static class Games
{
    private const string _HOST = "https://games-api-fiap-asgcdca8hrg9b5gv.brazilsouth-01.azurewebsites.net/api/v1";

    public static LibraryDto.LibraryDtoResponse InserirJogo(LibraryDto.LibraryDtoRequest request)
    => Request.Post<LibraryDto.LibraryDtoResponse>($"{_HOST}/adicionar/jogo/biblioteca", request);
}
