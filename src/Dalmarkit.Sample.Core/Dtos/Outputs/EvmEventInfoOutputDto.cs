namespace Dalmarkit.Sample.Core.Dtos.Outputs;

public class EvmEventInfoOutputDto : EvmEventOutputDto
{
    public DateTime CreatedOn { get; set; }
    public string CreatorId { get; set; } = null!;
}
