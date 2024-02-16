using Dalmarkit.EntityFrameworkCore.Services.DataServices;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;

namespace Dalmarkit.Sample.Application.Services.DataServices;

public interface IEntityImageDataService : IReadWriteDataServiceBase<EntityImage>, IStorageObjectDataService<EntityImage>
{
    Task<EntityImage?> GetByEntityImageIdAsync(Guid entityImageId);
}
