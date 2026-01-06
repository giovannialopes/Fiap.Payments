using Payments.Domain.Shared.Http.Dto;

namespace Payments.Domain.Shared.Http.Interface;

public interface IGamesHttpClient
{
    LibraryDto.LibraryDtoResponse InserirJogo(LibraryDto.LibraryDtoRequest request);
}

