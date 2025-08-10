using Dalmarkit.Sample.Application.Services.ExternalServices.Contracts;
using Dalmarkit.Sample.Core.Dtos.Outputs;
using Riok.Mapperly.Abstractions;

namespace Dalmarkit.Sample.Application.Mapping.InputDtosToInputDtos;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
public static partial class RoyaltyPaymentEventOutputDtoToOutputDtoMapper
{
    public static partial List<GetLooksRareExchangeRoyaltyPaymentEventOutputDto> ToTarget(List<RoyaltyPaymentEventDTO> source);
}
