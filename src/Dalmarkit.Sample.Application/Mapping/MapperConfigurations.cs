using AutoMapper;
using Dalmarkit.Blockchain.Evm.Services;
using Dalmarkit.Common.Dtos.InputDtos;
using Dalmarkit.Common.Services;
using Dalmarkit.EntityFrameworkCore.Mappers;
using Dalmarkit.Sample.Application.Services.ExternalServices.Contracts;
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
        _ = config.CreateMap<PositionsOutputDTO, GetNonFungiblePositionManagerPositionsOutputDto>();
        _ = config.CreateMap<RoyaltyPaymentEventDTO, GetLooksRareExchangeRoyaltyPaymentEventOutputDto>();
    }

    protected override void DtoToEntityMappingConfigure(IMapperConfigurationExpression config)
    {
        _ = config.CreateMap<CreateEntityInputDto, Entity>();
        _ = config.CreateMap<UpdateEntityInputDto, Entity>();

        _ = config.CreateMap<DependentEntityInputDto, DependentEntity>();
        _ = config.CreateMap<UpdateDependentEntitiesInputDto, DependentEntity>();

        _ = config.CreateMap<UploadObjectInputDto, EntityImage>();

        _ = config.CreateMap<PutEvmEventByNameInputDto, EvmEvent>()
            .ForMember(d => d.ContractAddress, opt => opt.MapFrom((_, _, _, context) =>
                (string)context.Items[MappingItemKeys.ContractAddress]
            ))
            .ForMember(d => d.EventDetail, opt => opt.MapFrom((_, _, _, context) =>
                (string)context.Items[MappingItemKeys.EventDetail]
            ));

        _ = config.CreateMap<EvmEventDto, EvmEvent>()
            .ForMember(d => d.EventDetail, opt => opt.MapFrom(s => s.EventJson))
            .ForMember(d => d.CreateRequestId, opt => opt.MapFrom((_, _, _, context) =>
                (string)context.Items[MappingItemKeys.CreateRequestId]
            ));
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

        _ = config.CreateMap<EvmEvent, EvmEventOutputDto>();
        _ = config.CreateMap<EvmEvent, EvmEventInfoOutputDto>();
    }
}
