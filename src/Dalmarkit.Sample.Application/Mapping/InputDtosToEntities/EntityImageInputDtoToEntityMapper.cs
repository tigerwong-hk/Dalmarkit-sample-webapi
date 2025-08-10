using Dalmarkit.Common.Dtos.InputDtos;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;
using Riok.Mapperly.Abstractions;

namespace Dalmarkit.Sample.Application.Mapping.InputDtosToEntities;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
public static partial class EntityImageInputDtoToEntityMapper
{
    [MapperIgnoreSource(nameof(UploadObjectInputDto.ParentId))]
    public static partial EntityImage ToTarget(UploadObjectInputDto source);
}
