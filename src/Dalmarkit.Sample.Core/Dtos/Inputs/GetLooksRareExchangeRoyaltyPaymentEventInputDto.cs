using Dalmarkit.Blockchain.Constants;
using Dalmarkit.Common.Errors;
using System.ComponentModel.DataAnnotations;

namespace Dalmarkit.Sample.Core.Dtos.Inputs;

public class GetLooksRareExchangeRoyaltyPaymentEventInputDto
{
    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    public string TransactionHash { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    [EnumDataType(typeof(BlockchainNetwork))]
    public BlockchainNetwork BlockchainNetwork { get; set; }
}
