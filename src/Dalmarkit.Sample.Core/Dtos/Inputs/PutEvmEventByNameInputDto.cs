using System.ComponentModel.DataAnnotations;
using Dalmarkit.Blockchain.Constants;
using Dalmarkit.Common.Errors;

namespace Dalmarkit.Sample.Core.Dtos.Inputs;

public class PutEvmEventByNameInputDto
{
    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    [StringLength(64, ErrorMessage = ErrorMessages.ModelStateErrors.LengthExceeded)]
    public string CreateRequestId { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    [StringLength(255, ErrorMessage = ErrorMessages.ModelStateErrors.LengthExceeded)]
    public string EventName { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    [StringLength(255, ErrorMessage = ErrorMessages.ModelStateErrors.LengthExceeded)]
    public string ContractName { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    [StringLength(88, ErrorMessage = ErrorMessages.ModelStateErrors.LengthExceeded)]
    public string TransactionHash { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    [EnumDataType(typeof(BlockchainNetwork))]
    public BlockchainNetwork BlockchainNetwork { get; set; }

    public string[]? ContractCallerAddresses { get; set; }
}
