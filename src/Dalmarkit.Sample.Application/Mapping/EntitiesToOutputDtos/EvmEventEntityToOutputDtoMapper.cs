using Dalmarkit.Sample.Core.Dtos.Outputs;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;
using Riok.Mapperly.Abstractions;

namespace Dalmarkit.Sample.Application.Mapping.EntitiesToOutputDtos;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class EvmEventEntityToOutputDtoMapper
{
    public static partial EvmEventInfoOutputDto ToTarget(EvmEvent source);
    public static partial IEnumerable<EvmEventOutputDto> ToTarget(IEnumerable<EvmEvent> source);
}
