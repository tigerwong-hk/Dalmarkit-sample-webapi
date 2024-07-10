using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.AuditTrail;
using Dalmarkit.Common.Dtos.InputDtos;
using Dalmarkit.Common.Errors;
using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.Core.Dtos.Outputs;

namespace Dalmarkit.Sample.Application.Services.ApplicationServices;

public interface IDalmarkitSampleCommandService
{
    Task<Result<Guid, ErrorDetail>> CreateEntityAsync(CreateEntityInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default);

    Task<Result<Guid, ErrorDetail>> DeleteEntityAsync(DeleteEntityInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default);

    Task<Result<Guid, ErrorDetail>> UpdateEntityAsync(UpdateEntityInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default);

    Task<Result<Guid, ErrorDetail>> DeleteEntityImageAsync(DeleteEntityImageInputDto inputDto,
                AuditDetail auditDetail,
                CancellationToken cancellationToken = default);

    Task<Result<EntityImageOutputDto, ErrorDetail>> UploadEntityImageAsync(UploadObjectInputDto inputDto,
        Stream stream,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Guid>, ErrorDetail>> CreateDependentEntitiesAsync(CreateDependentEntitiesInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Guid>, ErrorDetail>> DeleteDependentEntitiesAsync(DeleteDependentEntitiesInputDto inputDto,
       AuditDetail auditDetail,
       CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Guid>, ErrorDetail>> UpdateDependentEntitiesAsync(UpdateDependentEntitiesInputDto inputDto,
        AuditDetail auditDetail,
        CancellationToken cancellationToken = default);

    Task<Result<Guid, ErrorDetail>> PutEvmEventByNameAsync(PutEvmEventByNameInputDto inputDto, AuditDetail auditDetail, CancellationToken cancellationToken = default);
}
