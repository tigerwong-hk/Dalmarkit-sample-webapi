using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace Dalmarkit.Sample.Application.Services.ExternalServices.Contracts;

public class RoyaltyPaymentEventDTO : RoyaltyPaymentEventDTOBase;

[Event("RoyaltyPayment")]
public class RoyaltyPaymentEventDTOBase : IEventDTO
{
    [Parameter("address", "collection", 1, true)]
    public virtual string Collection { get; set; } = null!;
    [Parameter("uint256", "tokenId", 2, true)]
    public virtual BigInteger TokenId { get; set; }
    [Parameter("address", "royaltyRecipient", 3, true)]
    public virtual string RoyaltyRecipient { get; set; } = null!;
    [Parameter("address", "currency", 4, false)]
    public virtual string Currency { get; set; } = null!;
    [Parameter("uint256", "amount", 5, false)]
    public virtual BigInteger Amount { get; set; }
}
