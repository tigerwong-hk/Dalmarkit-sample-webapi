using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;
using Riok.Mapperly.Abstractions;

namespace Dalmarkit.Sample.Application.Mapping.InputDtosToEntities;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
public static partial class EntityInputDtoToEntityMapper
{
    public static partial Entity ToTarget(CreateEntityInputDto source);

    [MapperIgnoreSource(nameof(UpdateEntityInputDto.EntityId))]
    public static partial void UpdateTarget(UpdateEntityInputDto source, Entity target);
}
