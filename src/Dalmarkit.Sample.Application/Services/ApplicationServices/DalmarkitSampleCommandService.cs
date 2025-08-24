using Dalmarkit.Blockchain.Evm.Services;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.AuditTrail;
using Dalmarkit.Common.Errors;
using Dalmarkit.Common.Validation;
using Dalmarkit.EntityFrameworkCore.Services.ApplicationServices;
using Dalmarkit.Sample.Application.Mapping.InputDtosToEntities;
using Dalmarkit.Sample.Application.Options;
using Dalmarkit.Sample.Application.Services.DataServices;
using Dalmarkit.Sample.Application.Services.ExternalServices;
using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;
using Microsoft.Extensions.Options;

namespace Dalmarkit.Sample.Application.Services.ApplicationServices;

public class DalmarkitSampleCommandService : ApplicationCommandReadWriteDependentsServiceBase, IDalmarkitSampleCommandService
{
    private readonly EntityOptions _entityOptions;
    private readonly IEntityDataService _entityDataService;
    private readonly IDependentEntityDataService _dependentEntityDataService;
    private readonly IEvmEventDataService _evmEventDataService;
    private readonly IEvmBlockchainService _evmBlockchainService;

    public DalmarkitSampleCommandService(
        IOptions<EntityOptions> entityOptions,
        IEntityDataService entityDataService,
        IDependentEntityDataService dependentEntityDataService,
        IEvmEventDataService evmEventDataService,
        IEvmBlockchainService evmBlockchainService)
    {
        _entityOptions = Guard.NotNull(entityOptions, nameof(entityOptions)).Value;
        _ = Guard.NotNullOrWhiteSpace(_entityOptions.ImageS3BucketName, nameof(_entityOptions.ImageS3BucketName));
        _ = Guard.NotNullOrWhiteSpace(_entityOptions.ImageRootFolderName, nameof(_entityOptions.ImageRootFolderName));
        _ = Guard.NotNullOrWhiteSpace(_entityOptions.ImageCloudFrontDistributionId, nameof(_entityOptions.ImageCloudFrontDistributionId));

        _entityDataService = Guard.NotNull(entityDataService, nameof(entityDataService));
        _dependentEntityDataService = Guard.NotNull(dependentEntityDataService, nameof(dependentEntityDataService));
        _evmEventDataService = Guard.NotNull(evmEventDataService, nameof(evmEventDataService));
        _evmBlockchainService = Guard.NotNull(evmBlockchainService, nameof(evmBlockchainService));
    }

    #region Entity
    public async Task<Result<Guid, ErrorDetail>> CreateEntityAsync(CreateEntityInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        Entity entity = EntityInputDtoToEntityMapper.ToTarget(inputDto);
        _ = await _entityDataService.CreateAsync(entity, auditDetail, cancellationToken);
        _ = await _entityDataService.SaveChangesAsync(cancellationToken);

        return Ok(entity.EntityId);
    }

    public async Task<Result<Guid, ErrorDetail>> DeleteEntityAsync(DeleteEntityInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        Entity? entity = await _entityDataService.FindEntityIdAsync(inputDto.EntityId, false, cancellationToken);
        if (entity == null)
        {
            return Error<Guid>(ErrorTypes.ResourceNotFound, "Entity", inputDto.EntityId);
        }

        entity.IsDeleted = true;
        _ = _entityDataService.Update(entity, auditDetail);
        _ = await _entityDataService.SaveChangesAsync(cancellationToken);

        return Ok(entity.EntityId);
    }

    public async Task<Result<Guid, ErrorDetail>> UpdateEntityAsync(UpdateEntityInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        Entity? entity = await _entityDataService.FindEntityIdAsync(inputDto.EntityId, false, cancellationToken);
        if (entity == null)
        {
            return Error<Guid>(ErrorTypes.ResourceNotFound, "Entity", inputDto.EntityId);
        }

        EntityInputDtoToEntityMapper.UpdateTarget(inputDto, entity);
        _ = _entityDataService.Update(entity, auditDetail);
        _ = await _entityDataService.SaveChangesAsync(cancellationToken);

        return Ok(entity.EntityId);
    }
    #endregion Entity

    #region Dependent Entities
    public async Task<Result<IEnumerable<Guid>, ErrorDetail>> CreateDependentEntitiesAsync(CreateDependentEntitiesInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        return await CreateEntityDependentsAsync<Entity, DependentEntity, IEntityDataService, IDependentEntityDataService, DependentEntityInputDto, CreateDependentEntitiesInputDto>(
            _entityDataService,
            _dependentEntityDataService,
            DependentEntityInputDtoToEntityMapper.ToTarget,
            inputDto,
            auditDetail,
            cancellationToken
        );
    }

    public async Task<Result<IEnumerable<Guid>, ErrorDetail>> DeleteDependentEntitiesAsync(DeleteDependentEntitiesInputDto inputDto,
       AuditDetail auditDetail,
       CancellationToken cancellationToken = default)
    {
        return await DeleteEntityDependentsAsync<Entity, DependentEntity, IEntityDataService, IDependentEntityDataService, DeleteDependentEntityInputDto, DeleteDependentEntitiesInputDto>(
            _entityDataService,
            _dependentEntityDataService,
            inputDto,
            auditDetail,
            cancellationToken
        );
    }

    public async Task<Result<IEnumerable<Guid>, ErrorDetail>> UpdateDependentEntitiesAsync(UpdateDependentEntitiesInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        return await UpdateEntityDependentsAsync<Entity, DependentEntity, IEntityDataService, IDependentEntityDataService, UpdateDependentEntityInputDto, UpdateDependentEntitiesInputDto>(
            _entityDataService,
            _dependentEntityDataService,
            DependentEntityInputDtoToEntityMapper.UpdateTarget,
            inputDto,
            auditDetail,
            cancellationToken
        );
    }
    #endregion Dependent Entities

    #region Blockchain
    public async Task<Result<Guid, ErrorDetail>> PutEvmEventByNameAsync(PutEvmEventByNameInputDto inputDto, AuditDetail auditDetail, CancellationToken cancellationToken = default)
    {
        _ = Guard.NotNullOrWhiteSpace(auditDetail.ModifierId, nameof(auditDetail.ModifierId));
        (string contractAddress, _) = _evmBlockchainService.GetContractInfo(inputDto.ContractName, inputDto.BlockchainNetwork);

        string? evmEvents = await _evmBlockchainService.GetEvmEventByNameAsync(inputDto.EventName, inputDto.ContractName, inputDto.TransactionHash, inputDto.BlockchainNetwork);
        if (evmEvents == null)
        {
            return Error<Guid>(ErrorTypes.ResourceNotFound, inputDto.EventName, inputDto.TransactionHash);
        }

        EvmEvent entity = EvmEventInputDtoToEntityMapper.ToTarget(inputDto, contractAddress, evmEvents);
        _ = await _evmEventDataService.CreateAsync(entity, auditDetail, cancellationToken);
        _ = await _evmEventDataService.SaveChangesAsync(cancellationToken);

        return Ok(entity.EvmEventId);
    }

    public async Task<Result<List<Guid>, ErrorDetail>> PutEvmEventsByNameAsync(PutEvmEventByNameInputDto inputDto, AuditDetail auditDetail, CancellationToken cancellationToken = default)
    {
        _ = Guard.NotNullOrWhiteSpace(auditDetail.ModifierId, nameof(auditDetail.ModifierId));

        (string contractAddress, _) = _evmBlockchainService.GetContractInfo(inputDto.ContractName, inputDto.BlockchainNetwork);

        List<EvmEventDto>? evmEventDtos = await _evmBlockchainService.GetEvmEventsByNameAsync(inputDto.EventName, inputDto.ContractName, inputDto.TransactionHash, inputDto.BlockchainNetwork);
        if (evmEventDtos == null || evmEventDtos.Count == 0)
        {
            return Error<List<Guid>>(ErrorTypes.ResourceNotFound, inputDto.EventName, inputDto.TransactionHash);
        }

        List<EvmEvent> evmEventEntities = [];
        foreach (EvmEventDto evmEventDto in evmEventDtos)
        {
            EvmEvent entity = EvmEventInputDtoToEntityMapper.ToTarget(evmEventDto, inputDto.CreateRequestId);
            _ = await _evmEventDataService.CreateAsync(entity, auditDetail, cancellationToken);
            evmEventEntities.Add(entity);
        }

        _ = await _evmEventDataService.SaveChangesAsync(cancellationToken);

        return Ok(evmEventEntities.ConvertAll(e => e.EvmEventId));
    }
    #endregion Blockchain
}
