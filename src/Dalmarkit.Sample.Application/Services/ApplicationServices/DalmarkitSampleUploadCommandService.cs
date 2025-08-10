using Dalmarkit.Cloud.Aws.Services;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.AuditTrail;
using Dalmarkit.Common.Dtos.InputDtos;
using Dalmarkit.Common.Errors;
using Dalmarkit.Common.Validation;
using Dalmarkit.EntityFrameworkCore.Services.ApplicationServices;
using Dalmarkit.Sample.Application.Mapping.EntitiesToOutputDtos;
using Dalmarkit.Sample.Application.Mapping.InputDtosToEntities;
using Dalmarkit.Sample.Application.Options;
using Dalmarkit.Sample.Application.Services.DataServices;
using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.Core.Dtos.Outputs;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;
using Microsoft.Extensions.Options;

namespace Dalmarkit.Sample.Application.Services.ApplicationServices;

public class DalmarkitSampleUploadCommandService : ApplicationCommandUploadServiceBase, IDalmarkitSampleUploadCommandService
{
    private readonly EntityOptions _entityOptions;
    private readonly IEntityDataService _entityDataService;
    private readonly IEntityImageDataService _entityImageDataService;
    private readonly IAwsS3Service _awsS3Service;

    public DalmarkitSampleUploadCommandService(
        IOptions<EntityOptions> entityOptions,
        IEntityDataService entityDataService,
        IEntityImageDataService entityImageDataService,
        IAwsS3Service awsS3Service)
    {
        _entityOptions = Guard.NotNull(entityOptions, nameof(entityOptions)).Value;
        _ = Guard.NotNullOrWhiteSpace(_entityOptions.ImageS3BucketName, nameof(_entityOptions.ImageS3BucketName));
        _ = Guard.NotNullOrWhiteSpace(_entityOptions.ImageRootFolderName, nameof(_entityOptions.ImageRootFolderName));
        _ = Guard.NotNullOrWhiteSpace(_entityOptions.ImageCloudFrontDistributionId, nameof(_entityOptions.ImageCloudFrontDistributionId));

        _entityDataService = Guard.NotNull(entityDataService, nameof(entityDataService));
        _entityImageDataService = Guard.NotNull(entityImageDataService, nameof(entityImageDataService));
        _awsS3Service = Guard.NotNull(awsS3Service, nameof(awsS3Service));
    }

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
        EntityEntityToOutputDtoMapper entityEntityToOutputDtoMapper = new(_entityOptions.ImageS3BucketName!, _entityOptions.ImageRootFolderName!);

        return await UploadObjectAsync<Entity, EntityImage, IEntityDataService, IEntityImageDataService, IAwsS3Service, EntityImageOutputDto>(
            _entityDataService,
            _entityImageDataService,
            _awsS3Service,
            EntityImageInputDtoToEntityMapper.ToTarget,
            entityEntityToOutputDtoMapper.ToTarget,
            inputDto,
            stream,
            _entityOptions.ImageS3BucketName!,
            _entityOptions.ImageRootFolderName!,
            auditDetail,
            cancellationToken);
    }
    #endregion Entity Image
}
