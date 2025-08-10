using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.AuditTrail;
using Dalmarkit.Common.Dtos.InputDtos;
using Dalmarkit.Common.Errors;
using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.Core.Dtos.Outputs;

namespace Dalmarkit.Sample.Application.Services.ApplicationServices;

public interface IDalmarkitSampleUploadCommandService
{
    Task<Result<Guid, ErrorDetail>> DeleteEntityImageAsync(DeleteEntityImageInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default);

    Task<Result<EntityImageOutputDto, ErrorDetail>> UploadEntityImageAsync(UploadObjectInputDto inputDto,
        Stream stream,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default);
}
