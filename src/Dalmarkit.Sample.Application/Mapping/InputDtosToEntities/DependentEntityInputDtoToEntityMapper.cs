using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;
using Riok.Mapperly.Abstractions;

namespace Dalmarkit.Sample.Application.Mapping.InputDtosToEntities;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
public static partial class DependentEntityInputDtoToEntityMapper
{
    public static partial DependentEntity ToTarget(DependentEntityInputDto source);

    [MapperIgnoreSource(nameof(UpdateDependentEntityInputDto.DependentId))]
    public static partial void UpdateTarget(UpdateDependentEntityInputDto source, DependentEntity target);
}
