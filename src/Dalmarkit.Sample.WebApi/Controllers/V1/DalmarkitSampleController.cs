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
    public class DalmarkitSampleController : RestApiUploadControllerBase
    {
        public const long UploadImageMultipartBodyLengthLimitBytes = 1024 * 1024 * 5;
        // public const int UploadImageHeadersCountLimit = 16;
        // public const int UploadImageHeadersLengthLimitBytes = 1024 * 16;
        // public const int UploadImageKeyLengthLimitBytes = 1024 * 2;
        // public const int UploadImageMultipartBoundaryLengthLimitBytes = 128;
        // public const int UploadImageValueCountLimit = 1024;
        // public const int UploadImageValueLengthLimitBytes = 1024 * 1024 * 4;

        private readonly IDalmarkitSampleQueryService _dalmarkitSampleQueryService;
        private readonly IDalmarkitSampleCommandService _dalmarkitSampleCommandService;
        private readonly IImageValidatorService _imageValidatorService;
        private readonly EntityOptions _entityOptions;

        public DalmarkitSampleController(IDalmarkitSampleQueryService dalmarkitSampleQueryService,
            IDalmarkitSampleCommandService dalmarkitSampleCommandService,
            IImageValidatorService imageValidatorService,
            IOptions<EntityOptions> entityOptions)
        {
            _dalmarkitSampleQueryService = Guard.NotNull(dalmarkitSampleQueryService, nameof(dalmarkitSampleQueryService));
            _dalmarkitSampleCommandService = Guard.NotNull(dalmarkitSampleCommandService, nameof(dalmarkitSampleCommandService));
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

        #region Entity
        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
        [HttpPost]
        public async Task<ActionResult> CreateEntityAsync([FromBody] CreateEntityInputDto inputDto)
        {
            AuditDetail auditDetail = CreateAuditDetail();
            Result<Guid, ErrorDetail> result = await _dalmarkitSampleCommandService.CreateEntityAsync(inputDto, auditDetail);

            return ApiResponse(result);
        }

        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
        [HttpDelete]
        public async Task<ActionResult> DeleteEntityAsync([FromBody] DeleteEntityInputDto inputDto)
        {
            AuditDetail auditDetail = CreateAuditDetail();
            Result<Guid, ErrorDetail> result = await _dalmarkitSampleCommandService.DeleteEntityAsync(inputDto, auditDetail);

            return ApiResponse(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetEntityDetailAsync([FromQuery] GetEntityDetailInputDto inputDto)
        {
            Result<EntityDetailOutputDto, ErrorDetail> result = await _dalmarkitSampleQueryService.GetEntityDetailAsync(inputDto);

            return ApiResponse(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetEntitiesAsync([FromQuery] GetEntitiesInputDto inputDto)
        {
            Result<ResponsePagination<EntityOutputDto>, ErrorDetail> result = await _dalmarkitSampleQueryService.GetEntitiesAsync(inputDto);

            return ApiResponse(result);
        }

        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.TenantAdminScopes))]
        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.TenantAdminGroups))]
        [HttpGet]
        public async Task<ActionResult> GetEntityListAsync([FromQuery] GetEntityListInputDto inputDto)
        {
            Result<ResponsePagination<EntityOutputDto>, ErrorDetail> result = await _dalmarkitSampleQueryService.GetEntityListAsync(inputDto);

            return ApiResponse(result);
        }

        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.TenantAdminScopes))]
        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.TenantAdminGroups))]
        [HttpPut]
        public async Task<ActionResult> UpdateEntityAsync([FromBody] UpdateEntityInputDto inputDto)
        {
            AuditDetail auditDetail = CreateAuditDetail();
            Result<Guid, ErrorDetail> result = await _dalmarkitSampleCommandService.UpdateEntityAsync(inputDto, auditDetail);

            return ApiResponse(result);
        }
        #endregion Entity

        #region Entity Image
        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
        [HttpDelete]
        public async Task<ActionResult> DeleteEntityImageAsync([FromBody] DeleteEntityImageInputDto inputDto)
        {
            AuditDetail auditDetail = CreateAuditDetail();
            Result<Guid, ErrorDetail> result = await _dalmarkitSampleCommandService.DeleteEntityImageAsync(inputDto, auditDetail);

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

            Result<EntityImageOutputDto, ErrorDetail> result = await _dalmarkitSampleCommandService.UploadEntityImageAsync(
                uploadObjectInputDto!,
                imageStream,
                auditDetail);

            return ApiResponse(result);
        }
        #endregion Entity Image

        #region Dependent Entity
        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
        [HttpPost]
        public async Task<ActionResult> CreateDependentEntitiesAsync([FromBody] CreateDependentEntitiesInputDto inputDto)
        {
            AuditDetail auditDetail = CreateAuditDetail();
            Result<IEnumerable<Guid>, ErrorDetail> result = await _dalmarkitSampleCommandService.CreateDependentEntitiesAsync(inputDto, auditDetail);

            return ApiResponse(result);
        }

        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
        [HttpDelete]
        public async Task<ActionResult> DeleteDependentEntitiesAsync([FromBody] DeleteDependentEntitiesInputDto inputDto)
        {
            AuditDetail auditDetail = CreateAuditDetail();
            Result<IEnumerable<Guid>, ErrorDetail> result = await _dalmarkitSampleCommandService.DeleteDependentEntitiesAsync(inputDto, auditDetail);

            return ApiResponse(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetDependentEntityDetailAsync([FromQuery] GetDependentEntityDetailInputDto inputDto)
        {
            Result<DependentEntityDetailOutputDto, ErrorDetail> result = await _dalmarkitSampleQueryService.GetDependentEntityDetailAsync(inputDto);

            return ApiResponse(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetDependentEntitiesAsync([FromQuery] GetDependentEntitiesInputDto inputDto)
        {
            Result<IEnumerable<DependentEntityOutputDto>, ErrorDetail> result = await _dalmarkitSampleQueryService.GetDependentEntitiesAsync(inputDto);

            return ApiResponse(result);
        }

        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.TenantAdminScopes))]
        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.TenantAdminGroups))]
        [HttpPut]
        public async Task<ActionResult> UpdateDependentEntitiesAsync([FromBody] UpdateDependentEntitiesInputDto inputDto)
        {
            AuditDetail auditDetail = CreateAuditDetail();
            Result<IEnumerable<Guid>, ErrorDetail> result = await _dalmarkitSampleCommandService.UpdateDependentEntitiesAsync(inputDto, auditDetail);

            return ApiResponse(result);
        }
        #endregion Dependent Entity

        #region Blockchain
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetEvmEventInfoAsync([FromQuery] GetEvmEventInfoInputDto inputDto)
        {
            Result<EvmEventInfoOutputDto, ErrorDetail> result = await _dalmarkitSampleQueryService.GetEvmEventInfoAsync(inputDto);

            return ApiResponse(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetEvmEventsAsync([FromQuery] GetEvmEventsInputDto inputDto)
        {
            Result<ResponsePagination<EvmEventOutputDto>, ErrorDetail> result = await _dalmarkitSampleQueryService.GetEvmEventsAsync(inputDto);

            return ApiResponse(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetNonFungiblePositionManagerPositionsAsync([FromQuery] GetNonFungiblePositionManagerPositionsInputDto inputDto)
        {
            Result<GetNonFungiblePositionManagerPositionsOutputDto, ErrorDetail> result = await _dalmarkitSampleQueryService.GetNonFungiblePositionManagerPositionsAsync(inputDto);

            return ApiResponse(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetLooksRareExchangeRoyaltyEventAsync([FromQuery] GetLooksRareExchangeRoyaltyPaymentEventInputDto inputDto)
        {
            Result<List<GetLooksRareExchangeRoyaltyPaymentEventOutputDto>, ErrorDetail> result = await _dalmarkitSampleQueryService.GetLooksRareExchangeRoyaltyPaymentEventAsync(inputDto);

            return ApiResponse(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetLooksRareExchangeRoyaltyEventByNameAsync([FromQuery] GetLooksRareExchangeRoyaltyPaymentEventInputDto inputDto)
        {
            Result<string?, ErrorDetail> result = await _dalmarkitSampleQueryService.GetLooksRareExchangeRoyaltyPaymentEventByNameAsync(inputDto);

            return ApiResponse(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetLooksRareExchangeRoyaltyEventBySha3SignatureAsync([FromQuery] GetLooksRareExchangeRoyaltyPaymentEventInputDto inputDto)
        {
            Result<string?, ErrorDetail> result = await _dalmarkitSampleQueryService.GetLooksRareExchangeRoyaltyPaymentEventBySha3SignatureAsync(inputDto);

            return ApiResponse(result);
        }

        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes))]
        [Authorize(Policy = nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups))]
        [HttpPost]
        public async Task<ActionResult> PutEvmEventByNameAsync([FromBody] PutEvmEventByNameInputDto inputDto)
        {
            AuditDetail auditDetail = CreateAuditDetail();
            Result<Guid, ErrorDetail> result = await _dalmarkitSampleCommandService.PutEvmEventByNameAsync(inputDto, auditDetail);

            return ApiResponse(result);
        }
        #endregion Blockchain

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetSupportedBlockchainNetworksAsync()
        {
            Result<string[], ErrorDetail> result = await _dalmarkitSampleQueryService.GetSupportedBlockchainNetworksAsync();

            return ApiResponse(result);
        }
    }
}
