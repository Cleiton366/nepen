using AutoMapper;
using Desafio_NEPEN.Com.Nepen.Api.Dtos.Leitura;
using Desafio_NEPEN.Com.Nepen.Core.Exceptions;
using Desafio_NEPEN.Com.Nepen.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Desafio_NEPEN.Com.Nepen.Api.Controllers;

[ApiController]
[Route("api/medidores/{medidorId}/[controller]")]
public class LeiturasController : ControllerBase
{
    private readonly ILeituraService _leituraService;
    private readonly IMapper _mapper;
    private readonly IValidator<LeituraCreateDto> _validator;

    public LeiturasController(
        ILeituraService leituraService,
        IMapper mapper,
        IValidator<LeituraCreateDto> validator)
    {
        _leituraService = leituraService;
        _mapper = mapper;
        _validator = validator;
    }

    [HttpGet]
    public async Task<IActionResult> ObterLeituras(
        string medidorId,
        [FromQuery] DateTime dataInicio,
        [FromQuery] DateTime dataFim,
        [FromQuery] int limite = 100)
    {
        
        limite = Math.Clamp(limite, 1, 1000);
        
        var medidor = await _leituraService.ObterPorIdAsync(medidorId);
        if (medidor == null) throw new NotFoundException($"Medidor '{medidorId}' não encontrado");

        var medidorDto = await _leituraService.ObterLeiturasAsync(medidorId, dataInicio, dataFim, limite);
        
        var correlationId = Request.Headers["X-Request-ID"].FirstOrDefault() ?? HttpContext.TraceIdentifier;
        Response.Headers["X-Request-ID"] = correlationId;
        Response.Headers["X-Total-Count"] = medidorDto.TotalRegistros.ToString();
        Response.Headers["X-Page-Size"] = Math.Clamp(limite, 1, 1000).ToString();
        Response.Headers["X-Current-Page"] = "1";

        return Ok(medidorDto);
    }

    [HttpPost]
    public async Task<IActionResult> CriarLeitura(string medidorId, [FromBody] LeituraCreateDto leituraDto)
    {
        var correlationId = Request.Headers["X-Request-ID"].FirstOrDefault() ?? HttpContext.TraceIdentifier;
        Response.Headers["X-Request-ID"] = correlationId;
        
        var validationResult = await _validator.ValidateAsync(leituraDto);
        if (!validationResult.IsValid)
            throw new UnprocessableEntityException(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        
        var leituraExistente = await _leituraService.ObterPorMedidorETimestampAsync(medidorId, leituraDto.Timestamp);
        if (leituraExistente != null)
            throw new ConflictException($"Já existe uma leitura para o medidor '{medidorId}' no timestamp '{leituraDto.Timestamp:O}'.");
        
        var leituraCriada = await _leituraService.CriarLeituraAsync(medidorId, leituraDto, correlationId);

        return CreatedAtAction(nameof(ObterLeituras), new { medidorId }, _mapper.Map<LeituraDto>(leituraCriada));
    }
}
