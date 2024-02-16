using Dalmarkit.Common.Api.Responses;
using Dalmarkit.EntityFrameworkCore.Services.DataServices;
using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;

namespace Dalmarkit.Sample.Application.Services.DataServices;

public interface IEntityDataService : IReadWriteDataServiceBase<Entity>
{
    Task<Entity?> GetEntityDetailAsync(Guid entityId, CancellationToken cancellationToken = default);

    Task<ResponsePagination<Entity>> GetEntityListAsync(GetEntityListInputDto inputDto, CancellationToken cancellationToken = default);
}
