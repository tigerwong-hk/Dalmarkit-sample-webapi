using AutoMapper;
using Dalmarkit.Common.Dtos.InputDtos;
using Dalmarkit.Common.Services;
using Dalmarkit.EntityFrameworkCore.Mappers;
using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.Core.Dtos.Outputs;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;

namespace Dalmarkit.Sample.Application.Mapping;

public class MapperConfigurations : MapperConfigurationBase
{
    protected override void DtoToDtoMappingConfigure(IMapperConfigurationExpression config)
    {
        _ = config.CreateMap<GetEntitiesInputDto, GetEntityListInputDto>();
        _ = config.CreateMap<UpdateEntityInputDto, Entity>();
    }

    protected override void DtoToEntityMappingConfigure(IMapperConfigurationExpression config)
    {
        _ = config.CreateMap<CreateEntityInputDto, Entity>();
        _ = config.CreateMap<UpdateEntityInputDto, Entity>();

        _ = config.CreateMap<DependentEntityInputDto, DependentEntity>();
        _ = config.CreateMap<UpdateDependentEntitiesInputDto, DependentEntity>();

        _ = config.CreateMap<UploadObjectInputDto, EntityImage>();
    }

    protected override void EntityToDtoMappingConfigure(IMapperConfigurationExpression config)
    {
        _ = config.CreateMap<Entity, EntityOutputDto>();
        _ = config.CreateMap<Entity, EntityDetailOutputDto>();

        _ = config.CreateMap<DependentEntity, DependentEntityOutputDto>();
        _ = config.CreateMap<DependentEntity, DependentEntityDetailOutputDto>();

        _ = config.CreateMap<EntityImage, EntityImageOutputDto>()
            .ForMember(d => d.ImageUrl, opt => opt.MapFrom((src, _, _, context) =>
                ObjectStorageAccess.GetPublicStorageObjectUrl((string)context.Items[MapperItemKeys.BucketName],
                    (string)context.Items[MapperItemKeys.RootFolderName],
                    src.EntityId,
                    src.EntityImageId,
                    src.ObjectExtension)));
    }
}
