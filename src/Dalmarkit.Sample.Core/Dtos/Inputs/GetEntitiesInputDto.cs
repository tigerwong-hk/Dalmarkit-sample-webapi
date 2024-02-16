using Dalmarkit.Common.Dtos.Components;
using Dalmarkit.Common.Errors;
using System.ComponentModel.DataAnnotations;

namespace Dalmarkit.Sample.Core.Dtos.Inputs;

public class GetEntitiesInputDto : PaginationContext
{
    [StringLength(255, ErrorMessage = ErrorMessages.ModelStateErrors.LengthExceeded)]
    public string? EntityName { get; set; }
}
