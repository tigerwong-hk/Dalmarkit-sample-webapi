using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Numerics;

namespace Dalmarkit.Sample.Application.Services.ExternalServices.Contracts;

public class PositionsFunction : PositionsFunctionBase;

[Function("positions", typeof(PositionsOutputDTO))]
public class PositionsFunctionBase : FunctionMessage
{
    [Parameter("uint256", "tokenId", 1)]
    public virtual BigInteger TokenId { get; set; }
}

public class PositionsOutputDTO : PositionsOutputDTOBase;

[FunctionOutput]
public class PositionsOutputDTOBase : IFunctionOutputDTO
{
    [Parameter("uint96", "nonce", 1)]
    public virtual BigInteger Nonce { get; set; }
    [Parameter("address", "operator", 2)]
    public virtual string? Operative { get; set; }
    [Parameter("address", "token0", 3)]
    public virtual string? Token0 { get; set; }
    [Parameter("address", "token1", 4)]
    public virtual string? Token1 { get; set; }
    [Parameter("uint24", "fee", 5)]
    public virtual uint Fee { get; set; }
    [Parameter("int24", "tickLower", 6)]
    public virtual int TickLower { get; set; }
    [Parameter("int24", "tickUpper", 7)]
    public virtual int TickUpper { get; set; }
    [Parameter("uint128", "liquidity", 8)]
    public virtual BigInteger Liquidity { get; set; }
    [Parameter("uint256", "feeGrowthInside0LastX128", 9)]
    public virtual BigInteger FeeGrowthInside0LastX128 { get; set; }
    [Parameter("uint256", "feeGrowthInside1LastX128", 10)]
    public virtual BigInteger FeeGrowthInside1LastX128 { get; set; }
    [Parameter("uint128", "tokensOwed0", 11)]
    public virtual BigInteger TokensOwed0 { get; set; }
    [Parameter("uint128", "tokensOwed1", 12)]
    public virtual BigInteger TokensOwed1 { get; set; }
}
