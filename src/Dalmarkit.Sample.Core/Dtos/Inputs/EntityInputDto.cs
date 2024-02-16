using Dalmarkit.Common.Errors;
using System.ComponentModel.DataAnnotations;

namespace Dalmarkit.Sample.Core.Dtos.Inputs;

public class EntityInputDto
{
    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    [StringLength(255, ErrorMessage = ErrorMessages.ModelStateErrors.LengthExceeded)]
    public string EntityName { get; set; } = null!;
}
