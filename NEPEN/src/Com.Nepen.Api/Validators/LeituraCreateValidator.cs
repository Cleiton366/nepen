using Desafio_NEPEN.Com.Nepen.Api.Dtos.Leitura;
using FluentValidation;

namespace Desafio_NEPEN.Com.Nepen.Api.Validators;

public class LeituraCreateValidator : AbstractValidator<LeituraCreateDto>
{
    public LeituraCreateValidator()
    {
        RuleFor(x => x.Timestamp)
            .Must(timestamp => timestamp <= DateTime.UtcNow.AddMinutes(5))
            .WithMessage("Timestamp não pode ser futuro.");

        RuleFor(x => x.Tensao)
            .InclusiveBetween(0, 1000)
            .WithMessage("Tensão deve estar entre 0 e 1000V");

        RuleFor(x => x.Corrente)
            .InclusiveBetween(0, 1000)
            .WithMessage("Corrente deve estar entre 0 e 1000A");

        RuleFor(x => x.PotenciaAtiva)
            .InclusiveBetween(0, 100_000)
            .WithMessage("Potência Ativa deve estar entre 0 e 100kW");

        RuleFor(x => x.PotenciaReativa)
            .InclusiveBetween(0, 50_000)
            .WithMessage("Potência Reativa deve estar entre 0 e 50kVAr");

        RuleFor(x => x.EnergiaAtivaDireta)
            .InclusiveBetween(0, 999_999_999.99m)
            .WithMessage("Energia Ativa Direta fora do limite");

        RuleFor(x => x.EnergiaAtivaReversa)
            .InclusiveBetween(0, 999_999_999.99m)
            .WithMessage("Energia Ativa Reversa fora do limite");

        RuleFor(x => x.FatorPotencia)
            .InclusiveBetween(0, 1)
            .WithMessage("Fator de Potência deve estar entre 0 e 1");

        RuleFor(x => x.Frequencia)
            .InclusiveBetween(58, 62)
            .WithMessage("Frequência deve estar entre 58 e 62Hz");
    }
}