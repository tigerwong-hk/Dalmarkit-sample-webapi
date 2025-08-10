using Dalmarkit.Sample.Core.Dtos.Outputs;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;
using Riok.Mapperly.Abstractions;

namespace Dalmarkit.Sample.Application.Mapping.EntitiesToOutputDtos;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class DependentEntityEntityToOutputDtoMapper
{
    public static partial DependentEntityDetailOutputDto ToTarget(DependentEntity source);
    public static partial IEnumerable<DependentEntityDetailOutputDto> ToTarget(IEnumerable<DependentEntity> source);
}
