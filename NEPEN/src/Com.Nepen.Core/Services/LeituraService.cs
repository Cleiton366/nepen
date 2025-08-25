using AutoMapper;
using Desafio_NEPEN.Com.Nepen.Api.Dtos.Leitura;
using Desafio_NEPEN.Com.Nepen.Api.Dtos.Medidor;
using Desafio_NEPEN.Com.Nepen.Core.Entities;
using Desafio_NEPEN.Com.Nepen.Core.Interfaces;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;

namespace Desafio_NEPEN.Com.Nepen.Core.Services;

public class LeituraService : ILeituraService
{
    private readonly ILeituraRepository _leituraRepository;
    private readonly IMedidorRepository _medidorRepository;
    private readonly IDatabase _redis;
    private readonly IMapper _mapper;

    public LeituraService(
        ILeituraRepository leituraRepository,
        IMedidorRepository medidorRepository,
        IMapper mapper,
        IConnectionMultiplexer redis)
    {
        _leituraRepository = leituraRepository;
        _medidorRepository = medidorRepository;
        _mapper = mapper;
        _redis = redis.GetDatabase();
    }

    public async Task<MedidorDto> ObterLeiturasAsync(string medidorId, DateTime dataInicio, DateTime dataFim, int limite)
    {
        limite = Math.Clamp(limite, 1, 1000);
        
        var ultimaLeitura = await _leituraRepository.ObterUltimaLeituraAsync(medidorId, dataInicio, dataFim);
        string ultimaTimestamp = ultimaLeitura?.Timestamp.ToString("yyyyMMddHHmmss") ?? "none";

        string cacheKey = $"leituras:{medidorId}:{dataInicio:yyyyMMddHHmmss}:{dataFim:yyyyMMddHHmmss}:{limite}:last:{ultimaTimestamp}";
        
        var cached = await _redis.StringGetAsync(cacheKey);
        if (cached.HasValue) return JsonConvert.DeserializeObject<MedidorDto>(cached)!;
        
        var leituras = await _leituraRepository.ObterPorMedidorAsync(medidorId, dataInicio, dataFim, limite);

        var res = new MedidorDto
        {
            MedidorId = medidorId,
            TotalRegistros = leituras.Count,
            Periodo = leituras.Count != 0
                ? new PeriodoDto { Inicio = leituras.Min(l => l.Timestamp), 
                    Fim = leituras.Max(l => l.Timestamp) }
                : new PeriodoDto { Inicio = DateTime.MinValue, 
                    Fim = DateTime.MinValue },
            Leituras = _mapper.Map<List<LeituraDto>>(leituras)
        };
        
        await _redis.StringSetAsync(cacheKey, JsonConvert.SerializeObject(res), TimeSpan.FromMinutes(10));

        return res;
    }

    public async Task<LeituraDto> CriarLeituraAsync(string medidorId, LeituraCreateDto leituraDto, string correlationId)
    {
        var medidor = await _medidorRepository.ObterPorIdAsync(medidorId);
        var leitura = _mapper.Map<Leitura>(leituraDto);
        leitura.MedidorId = medidorId;
        leitura.Medidor = medidor;
        leitura.DataCriacao = DateTime.UtcNow;

        if (medidor == null)
        {
            medidor = new Medidor
            {
                MedidorId = medidorId,
                Leituras = []
            };
            await _medidorRepository.CriarAsync(medidor);
            leitura.Medidor = medidor;
        }
 
        var criada = await _leituraRepository.CriarAsync(leitura);
        
        if (leitura.Tensao > 950)
            Log.Warning("Tensão próxima do limite | Medidor: {MedidorId} | Timestamp: {Timestamp} | Valor: {Tensao} | CorrelationId: {CorrelationId}",
                leitura.MedidorId, leitura.Timestamp, leitura.Tensao, correlationId);

        if (leitura.EnergiaAtivaReversa > 0)
            Log.Information("Prosumidor detectado | Medidor: {MedidorId} | Timestamp: {Timestamp} | EnergiaReversa: {EnergiaAtivaReversa} | CorrelationId: {CorrelationId}",
                leitura.MedidorId, leitura.Timestamp, leitura.EnergiaAtivaReversa, correlationId);

        if (leitura.Frequencia < 59.9m || leitura.Frequencia > 60.1m)
            Log.Warning("Qualidade de energia fora do padrão | Medidor: {MedidorId} | Timestamp: {Timestamp} | Frequencia: {Frequencia} | CorrelationId: {CorrelationId}",
                leitura.MedidorId, leitura.Timestamp, leitura.Frequencia, correlationId);

        return _mapper.Map<LeituraDto>(criada);
    }

    public async Task<Medidor?> ObterPorIdAsync(string medidorId)
    {
        string cacheKey = $"medidor:{medidorId}";
        var cached = await _redis.StringGetAsync(cacheKey);
        if (cached.HasValue)
            return JsonConvert.DeserializeObject<Medidor>(cached);

        var medidor = await _medidorRepository.ObterPorIdAsync(medidorId);
        if (medidor != null)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            await _redis.StringSetAsync(
                cacheKey,
                JsonConvert.SerializeObject(medidor, settings),
                TimeSpan.FromMinutes(10)
            );
        }

        return medidor;
    }
    public async Task<Leitura?> ObterPorMedidorETimestampAsync(string medidorId, DateTime timestamp)
    {
        string cacheKey = $"leitura:{medidorId}:{timestamp:yyyyMMddHHmmss}";
        var cached = await _redis.StringGetAsync(cacheKey);
        if (cached.HasValue) return JsonConvert.DeserializeObject<Leitura>(cached);
        
        var leitura = await _leituraRepository.ObterPorMedidorETimestampAsync(medidorId, timestamp);
        if (leitura != null)
            await _redis.StringSetAsync(cacheKey, 
                JsonConvert.SerializeObject(leitura), 
                TimeSpan.FromMinutes(5));

        return leitura;
    }
}
