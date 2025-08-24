using Dalmarkit.Sample.Core.Dtos.Inputs;
using Riok.Mapperly.Abstractions;

namespace Dalmarkit.Sample.Application.Mapping.InputDtosToInputDtos;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
public static partial class EntityInputDtoToInputDtoMapper
{
    public static partial GetEntityListInputDto ToTarget(GetEntitiesInputDto source);
}
