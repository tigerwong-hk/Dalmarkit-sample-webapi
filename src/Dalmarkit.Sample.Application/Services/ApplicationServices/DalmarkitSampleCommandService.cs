using AutoMapper;
using Dalmarkit.Cloud.Aws.Services;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.AuditTrail;
using Dalmarkit.Common.Dtos.InputDtos;
using Dalmarkit.Common.Errors;
using Dalmarkit.Common.Validation;
using Dalmarkit.EntityFrameworkCore.Services.ApplicationServices;
using Dalmarkit.Sample.Application.Options;
using Dalmarkit.Sample.Application.Services.DataServices;
using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.Core.Dtos.Outputs;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;
using Microsoft.Extensions.Options;

namespace Dalmarkit.Sample.Application.Services.ApplicationServices;

public class DalmarkitSampleCommandService : ApplicationUploadCommandServiceBase, IDalmarkitSampleCommandService
{
    private readonly IMapper _mapper;
    private readonly EntityOptions _entityOptions;
    private readonly IEntityDataService _entityDataService;
    private readonly IEntityImageDataService _entityImageDataService;
    private readonly IDependentEntityDataService _dependentEntityDataService;
    private readonly IAwsS3Service _awsS3Service;

    public DalmarkitSampleCommandService(IMapper mapper,
        IOptions<EntityOptions> entityOptions,
        IEntityDataService entityDataService,
        IEntityImageDataService entityImageDataService,
        IDependentEntityDataService dependentEntityDataService,
        IAwsS3Service awsS3Service) : base(mapper)
    {
        _mapper = Guard.NotNull(mapper, nameof(mapper));

        _entityOptions = Guard.NotNull(entityOptions, nameof(entityOptions)).Value;
        _ = Guard.NotNullOrWhiteSpace(_entityOptions.ImageS3BucketName, nameof(_entityOptions.ImageS3BucketName));
        _ = Guard.NotNullOrWhiteSpace(_entityOptions.ImageRootFolderName, nameof(_entityOptions.ImageRootFolderName));
        _ = Guard.NotNullOrWhiteSpace(_entityOptions.ImageCloudFrontDistributionId, nameof(_entityOptions.ImageCloudFrontDistributionId));

        _entityDataService = Guard.NotNull(entityDataService, nameof(entityDataService));
        _entityImageDataService = Guard.NotNull(entityImageDataService, nameof(entityImageDataService));
        _dependentEntityDataService = Guard.NotNull(dependentEntityDataService, nameof(dependentEntityDataService));
        _awsS3Service = Guard.NotNull(awsS3Service, nameof(awsS3Service));
    }

    #region Entity
    public async Task<Result<Guid, ErrorDetail>> CreateEntityAsync(CreateEntityInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        Entity entity = _mapper.Map<Entity>(inputDto);
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

        entity = _mapper.Map(inputDto, entity);
        _ = _entityDataService.Update(entity, auditDetail);
        _ = await _entityDataService.SaveChangesAsync(cancellationToken);

        return Ok(entity.EntityId);
    }
    #endregion Entity

    #region Entity Image
    public async Task<Result<Guid, ErrorDetail>> DeleteEntityImageAsync(DeleteEntityImageInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        EntityImage? entityImage = await _entityImageDataService.FindEntityIdAsync(inputDto.EntityImageId, false, cancellationToken);
        if (entityImage == null)
        {
            return Error<Guid>(ErrorTypes.ResourceNotFound, "EntityImage", inputDto.EntityImageId);
        }

        entityImage.IsDeleted = true;
        _ = _entityImageDataService.Update(entityImage, auditDetail);
        _ = await _entityImageDataService.SaveChangesAsync(cancellationToken);

        return Ok(entityImage.EntityImageId);
    }

    public async Task<Result<EntityImageOutputDto, ErrorDetail>> UploadEntityImageAsync(UploadObjectInputDto inputDto,
        Stream stream,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        return await UploadObjectAsync<IEntityDataService, IEntityImageDataService, IAwsS3Service, EntityImageOutputDto, Entity, EntityImage>(
            _entityDataService,
            _entityImageDataService,
            _awsS3Service,
            inputDto,
            stream,
            _entityOptions.ImageS3BucketName!,
            _entityOptions.ImageRootFolderName!,
            _entityOptions.ImageCloudFrontDistributionId!,
            auditDetail,
            cancellationToken);
    }
    #endregion Entity Image

    #region Dependent Entities
    public async Task<Result<IEnumerable<Guid>, ErrorDetail>> CreateDependentEntitiesAsync(CreateDependentEntitiesInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default)
    {
        return await CreateEntityDependentsAsync<IEntityDataService, IDependentEntityDataService, CreateDependentEntitiesInputDto, DependentEntityInputDto, Entity, DependentEntity>(
            _entityDataService,
            _dependentEntityDataService,
            inputDto,
            auditDetail,
            cancellationToken
        );
    }

    public async Task<Result<IEnumerable<Guid>, ErrorDetail>> DeleteDependentEntitiesAsync(DeleteDependentEntitiesInputDto inputDto,
       AuditDetail auditDetail,
       CancellationToken cancellationToken = default)
    {
        return await DeleteEntityDependentsAsync<IEntityDataService, IDependentEntityDataService, DeleteDependentEntitiesInputDto, DeleteDependentEntityInputDto, Entity, DependentEntity>(
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
        return await UpdateEntityDependentsAsync<IEntityDataService, IDependentEntityDataService, UpdateDependentEntitiesInputDto, UpdateDependentEntityInputDto, Entity, DependentEntity>(
            _entityDataService,
            _dependentEntityDataService,
            inputDto,
            auditDetail,
            cancellationToken
        );
    }
    #endregion Dependent Entities
}
