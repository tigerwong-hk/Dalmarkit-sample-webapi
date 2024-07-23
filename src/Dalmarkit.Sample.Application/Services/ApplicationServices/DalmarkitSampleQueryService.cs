using AutoMapper;
using Dalmarkit.Blockchain.Constants;
using Dalmarkit.Blockchain.Evm.Services;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.Errors;
using Dalmarkit.Common.Validation;
using Dalmarkit.EntityFrameworkCore.Mappers;
using Dalmarkit.EntityFrameworkCore.Services.ApplicationServices;
using Dalmarkit.Sample.Application.Options;
using Dalmarkit.Sample.Application.Services.DataServices;
using Dalmarkit.Sample.Application.Services.ExternalServices;
using Dalmarkit.Sample.Application.Services.ExternalServices.Contracts;
using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.Core.Dtos.Outputs;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;
using Microsoft.Extensions.Options;

namespace Dalmarkit.Sample.Application.Services.ApplicationServices;

public class DalmarkitSampleQueryService : ApplicationQueryServiceBase, IDalmarkitSampleQueryService
{
    private readonly IMapper _mapper;
    private readonly EntityOptions _entityOptions;
    private readonly IEntityDataService _entityDataService;
    private readonly IDependentEntityDataService _dependentEntityDataService;
    private readonly IEvmEventDataService _evmEventDataService;
    private readonly IEvmBlockchainService _evmBlockchainService;

    public DalmarkitSampleQueryService(IMapper mapper,
        IOptions<EntityOptions> entityOptions,
        IEntityDataService entityDataService,
        IDependentEntityDataService dependentEntityDataService,
        IEvmEventDataService evmEventDataService,
        IEvmBlockchainService evmBlockchainService) : base(mapper)
    {
        _mapper = Guard.NotNull(mapper, nameof(mapper));

        _entityOptions = Guard.NotNull(entityOptions, nameof(entityOptions)).Value;
        _ = Guard.NotNullOrWhiteSpace(_entityOptions.ImageS3BucketName, nameof(_entityOptions.ImageS3BucketName));
        _ = Guard.NotNullOrWhiteSpace(_entityOptions.ImageRootFolderName, nameof(_entityOptions.ImageRootFolderName));

        _entityDataService = Guard.NotNull(entityDataService, nameof(entityDataService));
        _dependentEntityDataService = Guard.NotNull(dependentEntityDataService, nameof(dependentEntityDataService));
        _evmEventDataService = Guard.NotNull(evmEventDataService, nameof(evmEventDataService));
        _evmBlockchainService = Guard.NotNull(evmBlockchainService, nameof(evmBlockchainService));
    }

    #region Entity
    public async Task<Result<EntityDetailOutputDto, ErrorDetail>> GetEntityDetailAsync(GetEntityDetailInputDto inputDto, CancellationToken cancellationToken = default)
    {
        Entity? entityDetail = await _entityDataService.GetEntityDetailAsync(inputDto.EntityId, cancellationToken);
        if (entityDetail == null)
        {
            return Error<EntityDetailOutputDto>(ErrorTypes.ResourceNotFound, "Entity", inputDto.EntityId);
        }

        EntityDetailOutputDto? output = _mapper.Map<EntityDetailOutputDto>(entityDetail, opt =>
        {
            opt.Items[MapperItemKeys.BucketName] = _entityOptions.ImageS3BucketName;
            opt.Items[MapperItemKeys.RootFolderName] = _entityOptions.ImageRootFolderName;
        });

        return Ok(output);
    }

    public async Task<Result<ResponsePagination<EntityOutputDto>, ErrorDetail>> GetEntitiesAsync(GetEntitiesInputDto inputDto, CancellationToken cancellationToken = default)
    {
        GetEntityListInputDto entityListInputDto = _mapper.Map<GetEntityListInputDto>(inputDto);

        return await GetEntityListAsync(entityListInputDto, cancellationToken);
    }

    public async Task<Result<ResponsePagination<EntityOutputDto>, ErrorDetail>> GetEntityListAsync(GetEntityListInputDto inputDto, CancellationToken cancellationToken = default)
    {
        ResponsePagination<Entity> entityList = await _entityDataService.GetEntityListAsync(inputDto, cancellationToken);
        IEnumerable<EntityOutputDto> output = _mapper.Map<IEnumerable<EntityOutputDto>>(entityList.Data, opt =>
        {
            opt.Items[MapperItemKeys.BucketName] = _entityOptions.ImageS3BucketName;
            opt.Items[MapperItemKeys.RootFolderName] = _entityOptions.ImageRootFolderName;
        });

        return Ok(new ResponsePagination<EntityOutputDto>(output, entityList.FilteredCount, entityList.PageNumber, entityList.PageSize));
    }
    #endregion Entity

    #region Dependent Entity
    public async Task<Result<DependentEntityDetailOutputDto, ErrorDetail>> GetDependentEntityDetailAsync(GetDependentEntityDetailInputDto inputDto, CancellationToken cancellationToken = default)
    {
        DependentEntity? dependentEntity = await _dependentEntityDataService.GetDependentEntityDetailAsync(inputDto.DependentEntityId);
        if (dependentEntity == null)
        {
            return Error<DependentEntityDetailOutputDto>(ErrorTypes.ResourceNotFound, "DependentEntity", inputDto.DependentEntityId);
        }

        DependentEntityDetailOutputDto output = _mapper.Map<DependentEntityDetailOutputDto>(dependentEntity);

        return Ok(output);
    }

    public async Task<Result<IEnumerable<DependentEntityOutputDto>, ErrorDetail>> GetDependentEntitiesAsync(GetDependentEntitiesInputDto inputDto, CancellationToken cancellationToken = default)
    {
        IEnumerable<DependentEntity> dependentEntities = await _dependentEntityDataService.GetDependentEntitiesAsync(inputDto.EntityId, cancellationToken);
        IEnumerable<DependentEntityOutputDto> output = _mapper.Map<IEnumerable<DependentEntityOutputDto>>(dependentEntities);

        return Ok(output);
    }
    #endregion Dependent Entity

    #region Blockchain
    public async Task<Result<EvmEventInfoOutputDto, ErrorDetail>> GetEvmEventInfoAsync(GetEvmEventInfoInputDto inputDto, CancellationToken cancellationToken = default)
    {
        EvmEvent? evmEventInfo = await _evmEventDataService.GetEvmEventInfoAsync(inputDto.EvmEventId, cancellationToken);
        if (evmEventInfo == null)
        {
            return Error<EvmEventInfoOutputDto>(ErrorTypes.ResourceNotFound, "EvmEvent", inputDto.EvmEventId);
        }

        EvmEventInfoOutputDto? output = _mapper.Map<EvmEventInfoOutputDto>(evmEventInfo);

        return Ok(output);
    }

    public async Task<Result<ResponsePagination<EvmEventOutputDto>, ErrorDetail>> GetEvmEventsAsync(GetEvmEventsInputDto inputDto, CancellationToken cancellationToken = default)
    {
        ResponsePagination<EvmEvent> evmEvents = await _evmEventDataService.GetEvmEventsAsync(inputDto, cancellationToken);
        IEnumerable<EvmEventOutputDto> output = _mapper.Map<IEnumerable<EvmEventOutputDto>>(evmEvents.Data);

        return Ok(new ResponsePagination<EvmEventOutputDto>(output, evmEvents.FilteredCount, evmEvents.PageNumber, evmEvents.PageSize));
    }

    public async Task<Result<GetNonFungiblePositionManagerPositionsOutputDto, ErrorDetail>> GetNonFungiblePositionManagerPositionsAsync(GetNonFungiblePositionManagerPositionsInputDto inputDto, CancellationToken cancellationToken = default)
    {
        PositionsOutputDTO? positionsOutput = await _evmBlockchainService.CallNonFungiblePositionManagerPositionsFunctionAsync(inputDto.TokenId, inputDto.BlockchainNetwork);
        GetNonFungiblePositionManagerPositionsOutputDto output = _mapper.Map<GetNonFungiblePositionManagerPositionsOutputDto>(positionsOutput);

        return Ok(output);
    }

    public async Task<Result<List<GetLooksRareExchangeRoyaltyPaymentEventOutputDto>, ErrorDetail>> GetLooksRareExchangeRoyaltyPaymentEventAsync(GetLooksRareExchangeRoyaltyPaymentEventInputDto inputDto, CancellationToken cancellationToken = default)
    {
        List<RoyaltyPaymentEventDTO>? royaltyPaymentEvents = await _evmBlockchainService.GetLooksRareExchangeRoyaltyPaymentEventAsync(inputDto.TransactionHash, inputDto.BlockchainNetwork);
        List<GetLooksRareExchangeRoyaltyPaymentEventOutputDto> output = _mapper.Map<List<GetLooksRareExchangeRoyaltyPaymentEventOutputDto>>(royaltyPaymentEvents);

        return Ok(output);
    }

    public async Task<Result<string?, ErrorDetail>> GetLooksRareExchangeRoyaltyPaymentEventByNameAsync(GetLooksRareExchangeRoyaltyPaymentEventInputDto inputDto, CancellationToken cancellationToken = default)
    {
        string? royaltyPaymentEvents = await _evmBlockchainService.GetLooksRareExchangeRoyaltyPaymentEventByNameAsync(inputDto.TransactionHash, inputDto.BlockchainNetwork);

        return Ok(royaltyPaymentEvents);
    }

    public async Task<Result<List<EvmEventDto>?, ErrorDetail>> GetLooksRareExchangeRoyaltyPaymentEventsByNameAsync(GetLooksRareExchangeRoyaltyPaymentEventInputDto inputDto, CancellationToken cancellationToken = default)
    {
        List<EvmEventDto>? royaltyPaymentEvents = await _evmBlockchainService.GetLooksRareExchangeRoyaltyPaymentEventsByNameAsync(inputDto.TransactionHash, inputDto.BlockchainNetwork);

        return Ok(royaltyPaymentEvents);
    }

    public async Task<Result<string?, ErrorDetail>> GetLooksRareExchangeRoyaltyPaymentEventBySha3SignatureAsync(GetLooksRareExchangeRoyaltyPaymentEventInputDto inputDto, CancellationToken cancellationToken = default)
    {
        string? royaltyPaymentEvents = await _evmBlockchainService.GetLooksRareExchangeRoyaltyPaymentEventBySha3SignatureAsync(inputDto.TransactionHash, inputDto.BlockchainNetwork);

        return Ok(royaltyPaymentEvents);
    }
    #endregion Blockchain

    #region Enum
    public async Task<Result<string[], ErrorDetail>> GetSupportedBlockchainNetworksAsync(CancellationToken cancellationToken = default)
    {
        string[] blockchainNetworks = await GetEnumNamesAsync(typeof(BlockchainNetwork));

        return Ok(blockchainNetworks);
    }
    #endregion Enum
}
