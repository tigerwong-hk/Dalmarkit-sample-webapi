using Dalmarkit.Common.Api.Responses;
using Dalmarkit.Common.Errors;
using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.Core.Dtos.Outputs;

namespace Dalmarkit.Sample.Application.Services.ApplicationServices;

public interface IDalmarkitSampleQueryService
{
    Task<Result<EntityDetailOutputDto, ErrorDetail>> GetEntityDetailAsync(GetEntityDetailInputDto inputDto, CancellationToken cancellationToken = default);

    Task<Result<ResponsePagination<EntityOutputDto>, ErrorDetail>> GetEntitiesAsync(GetEntitiesInputDto inputDto, CancellationToken cancellationToken = default);

    Task<Result<ResponsePagination<EntityOutputDto>, ErrorDetail>> GetEntityListAsync(GetEntityListInputDto inputDto, CancellationToken cancellationToken = default);

    Task<Result<DependentEntityDetailOutputDto, ErrorDetail>> GetDependentEntityDetailAsync(GetDependentEntityDetailInputDto inputDto, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<DependentEntityOutputDto>, ErrorDetail>> GetDependentEntitiesAsync(GetDependentEntitiesInputDto inputDto, CancellationToken cancellationToken = default);

    Task<Result<string[], ErrorDetail>> GetSupportedBlockchainNetworksAsync(CancellationToken cancellationToken = default);
}
