using IntegrationPlatform.Application.Interfaces;
using IntegrationPlatform.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;
namespace IntegrationPlatform.Api.Controllers;
[ApiController]
[Route("api/operations")]
public class OperationsController : ControllerBase
{
    private readonly IOperationService _operationService;
    private readonly ILogger<OperationsController> _logger;
    public OperationsController(IOperationService operationService, ILogger<OperationsController> logger)
    {
        _operationService = operationService;
        _logger = logger;
    }
    /// <summary>
    /// Execute an operation synchronously.
    /// </summary>
    [HttpPost("sync")]
    public async Task<IActionResult> ExecuteSync([FromBody] CreateOperationRequest request, CancellationToken cancellationToken)
    {
        if (request is null) { return BadRequest("Request body is required."); }
        var result = await _operationService.ExecuteSyncAsync(request, cancellationToken);
        return Ok(result);
    }
    /// <summary>
    /// Enqueue an operation for asynchronous processing.
    /// </summary>
    [HttpPost("async")]
    public async Task<IActionResult> EnqueueAsync([FromBody] CreateOperationRequest request, CancellationToken cancellationToken)
    {
        if (request is null) { return BadRequest("Request body is required."); }
        var result = await _operationService.EnqueueAsync(request, cancellationToken);
        return Accepted(result);
    }
    /// <summary>
    /// Get an operation by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _operationService.GetByIdAsync(id, cancellationToken);
        if (result is null) { return NotFound($"Operation {id} not found."); }
        return Ok(result);
    }
    /// <summary>
    /// Retry a failed or timed-out operation.
    /// </summary>
    [HttpPost("{id:guid}/retry")]
    public async Task<IActionResult> Retry(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _operationService.RetryAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }
    /// <summary>
    /// Cancel a pending or running operation.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _operationService.CancelAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }
}