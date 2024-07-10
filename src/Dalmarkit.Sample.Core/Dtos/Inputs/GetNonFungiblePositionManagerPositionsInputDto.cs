using Dalmarkit.Blockchain.Constants;
using Dalmarkit.Common.Converters;
using Dalmarkit.Common.Errors;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Dalmarkit.Sample.Core.Dtos.Inputs;

public class GetNonFungiblePositionManagerPositionsInputDto
{
    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    [JsonConverter(typeof(BigIntegerJsonConverter))]
    public BigInteger TokenId { get; set; }

    [Required(ErrorMessage = ErrorMessages.ModelStateErrors.FieldRequired)]
    [EnumDataType(typeof(BlockchainNetwork))]
    public BlockchainNetwork BlockchainNetwork { get; set; }
}
