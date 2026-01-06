using Microsoft.AspNetCore.Mvc;
using Payments.Domain.DTO;
using Payments.Domain.Services.Interface;
using static Payments.Domain.DTO.ErrorDto;

namespace Payments.Api.Controllers;

/// <summary>
/// Controller para operações de pagamentos
/// </summary>
[Route("api/v1")]
[ApiController]
public class PaymentsController(
    [FromServices] IPaymentService services, 
    ILoggerServices logger,
    [FromServices] IMetricsService metricsService) : ControllerBase
{
    /// <summary>
    /// Consulta o status de um processo de pagamento
    /// </summary>
    /// <param name="perfilId">ID do perfil do usuário</param>
    /// <param name="jogoId">ID do jogo</param>
    /// <returns>Status do pagamento</returns>
    /// <response code="200">Pagamento encontrado</response>
    /// <response code="400">Erro na validação ou pagamento não encontrado</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet]
    [Route("consultar/processos/pagamentos/{perfilId}/{jogoId}")]
    [ProducesResponseType(typeof(PaymentsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ConsultarProcessos(Guid perfilId, Guid jogoId) {
        
        if (perfilId == Guid.Empty || jogoId == Guid.Empty)
        {
            return BadRequest(new ErrorResponse { Message = "PerfilId e JogoId são obrigatórios", Code = "400" });
        }
        
        await logger.LogInformation("Iniciou ConsultarProcessos");
        metricsService.IncrementPaymentsQueried();
        
        var result = await services.ConsultarProcessos(perfilId, jogoId);
        
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
