using Audit.WebApi;
using Dalmarkit.AspNetCore.Controllers;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.AuditTrail;
using Dalmarkit.Common.Dtos.InputDtos;
using Dalmarkit.Common.Errors;
using Dalmarkit.Common.Identity;
using Dalmarkit.Common.Validation;
using Dalmarkit.Sample.Application.Services.ApplicationServices;
using Dalmarkit.Sample.Application.Options;
using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.Core.Dtos.Outputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Dalmarkit.Sample.WebApi.Controllers.V1
{
    [Authorize]
    public class DalmarkitSampleUploadController : RestApiUploadControllerBase
    {
        public const long UploadImageMultipartBodyLengthLimitBytes = 1024 * 1024 * 5;
        // public const int UploadImageHeadersCountLimit = 16;
        // public const int UploadImageHeadersLengthLimitBytes = 1024 * 16;
        // public const int UploadImageKeyLengthLimitBytes = 1024 * 2;
        // public const int UploadImageMultipartBoundaryLengthLimitBytes = 128;
        // public const int UploadImageValueCountLimit = 1024;
        // public const int UploadImageValueLengthLimitBytes = 1024 * 1024 * 4;

        private readonly IDalmarkitSampleUploadCommandService _dalmarkitSampleUploadCommandService;
        private readonly IImageValidatorService _imageValidatorService;
        private readonly EntityOptions _entityOptions;

        public DalmarkitSampleUploadController(
            IDalmarkitSampleUploadCommandService dalmarkitSampleUploadCommandService,
            IImageValidatorService imageValidatorService,
            IOptions<EntityOptions> entityOptions)
        {
            _dalmarkitSampleUploadCommandService = Guard.NotNull(dalmarkitSampleUploadCommandService, nameof(dalmarkitSampleUploadCommandService));
            _imageValidatorService = Guard.NotNull(imageValidatorService, nameof(imageValidatorService));

            _entityOptions = Guard.NotNull(entityOptions, nameof(entityOptions)).Value;
            _ = Guard.NotNullOrWhiteSpace(_entityOptions.ImageS3BucketName, nameof(_entityOptions.ImageS3BucketName));
            _ = Guard.NotNullOrWhiteSpace(_entityOptions.ImageRootFolderName, nameof(_entityOptions.ImageRootFolderName));
            _ = Guard.NotNullOrWhiteSpace(_entityOptions.ImageCloudFrontDistributionId, nameof(_entityOptions.ImageCloudFrontDistributionId));
            _ = Guard.NotNull(_entityOptions.SupportedImageContentTypes, nameof(_entityOptions.SupportedImageContentTypes));
            _ = Guard.NotNull(_entityOptions.SupportedImageFileExtensions, nameof(_entityOptions.SupportedImageFileExtensions));
            if (_entityOptions.SupportedImageContentTypes!.Count < 1)
            {
                throw new ArgumentException("No supported image types", nameof(entityOptions));
            }
            if (_entityOptions.SupportedImageFileExtensions!.Count < 1)
            {
                throw new ArgumentException("No supported image extensions", nameof(entityOptions));
            }
        }

        #region Entity Image
        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
        [HttpDelete]
        public async Task<ActionResult> DeleteEntityImageAsync([FromBody] DeleteEntityImageInputDto inputDto)
        {
            AuditDetail auditDetail = CreateAuditDetail();
            Result<Guid, ErrorDetail> result = await _dalmarkitSampleUploadCommandService.DeleteEntityImageAsync(inputDto, auditDetail);

            return ApiResponse(result);
        }

        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = UploadImageMultipartBodyLengthLimitBytes)]
        public async Task<ActionResult> UploadEntityImageAsync([FromForm][Required] string createRequestId,
            [FromForm][Required][NotDefault] Guid entityId,
            [FromForm][Required][StringLength(64, ErrorMessage = ErrorMessages.ModelStateErrors.LengthExceeded)] string imageName,
            [AuditIgnore][Required] IFormFile fileContent)
        {
            AuditDetail auditDetail = CreateAuditDetail();

            (UploadObjectInputDto? uploadObjectInputDto, string? fileExtensionWithoutPeriod, ActionResult? errorResponse) = GetUploadObjectInputDto(
                createRequestId,
                entityId,
                imageName,
                UploadImageMultipartBodyLengthLimitBytes,
                [.. _entityOptions.SupportedImageContentTypes!],
                [.. _entityOptions.SupportedImageFileExtensions!],
                fileContent);
            if (errorResponse != null)
            {
                return errorResponse;
            }

            await using Stream imageStream = fileContent.OpenReadStream();
            if (!_imageValidatorService.IsValid(imageStream, fileContent.ContentType, fileExtensionWithoutPeriod!))
            {
                return ApiResponse(Result.Error<bool, ErrorDetail>(ErrorTypes.BadRequestDetails
                    .WithArgs(ErrorMessages.ObjectInvalid)));
            }

            Result<EntityImageOutputDto, ErrorDetail> result = await _dalmarkitSampleUploadCommandService.UploadEntityImageAsync(
                uploadObjectInputDto!,
                imageStream,
                auditDetail);

            return ApiResponse(result);
        }
        #endregion Entity Image
    }
}
