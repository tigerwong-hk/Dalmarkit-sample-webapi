using Dalmarkit.EntityFrameworkCore.Services.DataServices;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;

namespace Dalmarkit.Sample.Application.Services.DataServices;

public interface IDependentEntityDataService : IMultipleReadWriteDataServiceBase<DependentEntity>
{
    Task<DependentEntity?> GetDependentEntityDetailAsync(Guid dependentEntityId);
    Task<IEnumerable<DependentEntity>> GetDependentEntitiesAsync(Guid entityId, CancellationToken cancellationToken = default);
}
