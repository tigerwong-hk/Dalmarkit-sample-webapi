using Dalmarkit.Common.Converters;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Dalmarkit.Sample.Core.Dtos.Outputs;

public class GetLooksRareExchangeRoyaltyPaymentEventOutputDto
{
    public string Collection { get; set; } = null!;

    [JsonConverter(typeof(BigIntegerJsonConverter))]
    public BigInteger TokenId { get; set; }

    public string RoyaltyRecipient { get; set; } = null!;

    public string Currency { get; set; } = null!;

    [JsonConverter(typeof(BigIntegerJsonConverter))]
    public BigInteger Amount { get; set; }
}
