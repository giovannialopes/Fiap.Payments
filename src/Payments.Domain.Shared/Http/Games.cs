using Payments.Domain.Shared.Http.Dto;

namespace Payments.Domain.Shared.Http;

public static class Games
{
    private const string _HOST = "https://localhost:7245/api/v1";

    public static LibraryDto.LibraryDtoResponse InserirJogo(LibraryDto.LibraryDtoRequest request)
    => Request.Post<LibraryDto.LibraryDtoResponse>($"{_HOST}/adicionar/jogo/biblioteca", request);
}
