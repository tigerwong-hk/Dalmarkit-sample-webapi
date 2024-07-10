using Dalmarkit.Common.Converters;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Dalmarkit.Sample.Core.Dtos.Outputs;

public class GetNonFungiblePositionManagerPositionsOutputDto
{
    [JsonConverter(typeof(BigIntegerJsonConverter))]
    public BigInteger Nonce { get; set; }

    public string? Operative { get; set; }
    public string? Token0 { get; set; }
    public string? Token1 { get; set; }
    public uint Fee { get; set; }
    public int TickLower { get; set; }
    public int TickUpper { get; set; }

    [JsonConverter(typeof(BigIntegerJsonConverter))]
    public BigInteger Liquidity { get; set; }

    [JsonConverter(typeof(BigIntegerJsonConverter))]
    public BigInteger FeeGrowthInside0LastX128 { get; set; }

    [JsonConverter(typeof(BigIntegerJsonConverter))]
    public BigInteger FeeGrowthInside1LastX128 { get; set; }

    [JsonConverter(typeof(BigIntegerJsonConverter))]
    public BigInteger TokensOwed0 { get; set; }

    [JsonConverter(typeof(BigIntegerJsonConverter))]
    public BigInteger TokensOwed1 { get; set; }
}
