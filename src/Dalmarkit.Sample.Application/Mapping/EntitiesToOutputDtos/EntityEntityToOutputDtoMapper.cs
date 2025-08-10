using Dalmarkit.Common.Services;
using Dalmarkit.Common.Validation;
using Dalmarkit.Sample.Core.Dtos.Outputs;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;
using Riok.Mapperly.Abstractions;

namespace Dalmarkit.Sample.Application.Mapping.EntitiesToOutputDtos;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class EntityEntityToOutputDtoMapper(string bucketName, string rootFolderName)
{
    private readonly string _bucketName = Guard.NotNullOrWhiteSpace(bucketName, nameof(bucketName));
    private readonly string _rootFolderName = Guard.NotNullOrWhiteSpace(rootFolderName, nameof(rootFolderName));

    public partial IEnumerable<EntityOutputDto> ToTarget(IEnumerable<Entity> source);

    public partial EntityDetailOutputDto ToTarget(Entity source);

    [MapPropertyFromSource(nameof(EntityImageOutputDto.ImageUrl), Use = nameof(MapImageUrl))]
    public partial EntityImageOutputDto ToTarget(EntityImage source);

    private string MapImageUrl(EntityImage source)
    {
        return ObjectStorageAccess.GetPublicStorageObjectUrl(_bucketName,
                _rootFolderName,
                source.EntityId,
                source.EntityImageId,
                source.ObjectExtension);
    }
}
