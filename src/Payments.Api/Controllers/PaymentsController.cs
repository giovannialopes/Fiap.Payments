using Microsoft.AspNetCore.Mvc;
using Payments.Domain.Services.Interface;
using static Payments.Domain.DTO.ErrorDto;

namespace Payments.Api.Controllers;

[Route("api/v1")]
[ApiController]
public class PaymentsController([FromServices] IPaymentService services, ILoggerServices logger) : ControllerBase
{
    [HttpGet]
    [Route("consultar/processos/pagamentos/{perfilId}/{jogoId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ConsultarProcessos(Guid perfilId, Guid jogoId) {
        await logger.LogInformation("Iniciou ConsultarProcessos");
        var result = await services.ConsultarProcessos(perfilId,jogoId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
