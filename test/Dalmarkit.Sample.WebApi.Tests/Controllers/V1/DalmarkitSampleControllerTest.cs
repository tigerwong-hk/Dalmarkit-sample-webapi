using Moq;
using Dalmarkit.Sample.WebApi.Controllers.V1;
using Dalmarkit.Sample.Application.Services.ApplicationServices;
using Dalmarkit.Sample.Application.Options;
using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.Errors;
using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.Core.Dtos.Outputs;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

namespace Dalmarkit.Sample.WebApi.Tests.Controllers.V1;

#pragma warning disable CA1707 // Identifiers should not contain underscores

public class DalmarkitSampleControllerTest
{
    private readonly Mock<IDalmarkitSampleQueryService> _mockQueryService;
    private readonly Mock<IDalmarkitSampleCommandService> _mockCommandService;
    private readonly IOptions<EntityOptions> _entityOptions;
    private readonly DalmarkitSampleController _controller;

    public DalmarkitSampleControllerTest()
    {
        _mockQueryService = new Mock<IDalmarkitSampleQueryService>();
        _mockCommandService = new Mock<IDalmarkitSampleCommandService>();
        _entityOptions = Options.Create(new EntityOptions
        {
            ImageS3BucketName = "bucket",
            ImageRootFolderName = "root",
            ImageCloudFrontDistributionId = "dist",
            SupportedImageContentTypes = ["image/png"],
            SupportedImageFileExtensions = [".png"]
        });

        _controller = new DalmarkitSampleController(
            _mockQueryService.Object,
            _mockCommandService.Object,
            _entityOptions
        );
    }

    [Fact]
    public async Task GetEntitiesAsync_ReturnsOkResult_WhenServiceReturnsSuccess()
    {
        // Arrange
        GetEntitiesInputDto inputDto = new();
        ResponsePagination<EntityOutputDto> expectedData = new(
            [new EntityOutputDto()],
            1,
            1,
            10
        );
        CommonApiResponse<ResponsePagination<EntityOutputDto>> expectedResponse = new()
        {
            Success = true,
            Data = expectedData
        };
        Result<ResponsePagination<EntityOutputDto>, ErrorDetail> result = Result.Ok<ResponsePagination<EntityOutputDto>, ErrorDetail>(expectedData);

        _ = _mockQueryService
            .Setup(s => s.GetEntitiesAsync(inputDto, default))
            .ReturnsAsync(result);

        // Act
        ActionResult actionResult = await _controller.GetEntitiesAsync(inputDto);

        // Assert
        JsonResult objectResult = Assert.IsType<JsonResult>(actionResult);
        Assert.Null(objectResult.StatusCode);
        Assert.NotNull(objectResult.Value);
        Assert.Equivalent(expectedResponse, objectResult.Value, true);
    }

    [Fact]
    public async Task GetEntitiesAsync_ReturnsErrorResult_WhenServiceReturnsError()
    {
        // Arrange
        GetEntitiesInputDto inputDto = new();
        ErrorDetail errorDetail = ErrorTypes.BadRequestDetails.WithArgs("Some error");
        DefaultResponse expectedResponse = new()
        {
            ErrorCode = errorDetail.Code,
            ErrorMessage = errorDetail.Message
        };
        Result<ResponsePagination<EntityOutputDto>, ErrorDetail> result = Result.Error<ResponsePagination<EntityOutputDto>, ErrorDetail>(errorDetail);

        _ = _mockQueryService
            .Setup(s => s.GetEntitiesAsync(inputDto, default))
            .ReturnsAsync(result);

        // Act
        ActionResult actionResult = await _controller.GetEntitiesAsync(inputDto);

        // Assert
        JsonResult objectResult = Assert.IsType<JsonResult>(actionResult);
        Assert.Null(objectResult.StatusCode);
        Assert.NotNull(objectResult.Value);
        Assert.Equivalent(expectedResponse, objectResult.Value, true);
    }
}

#pragma warning restore CA1707 // Identifiers should not contain underscores
