namespace Dalmarkit.Sample.Core.Dtos.Outputs;

public class EntityDetailOutputDto : EntityOutputDto
{
    public IEnumerable<EntityImageOutputDto>? EntityImages { get; set; }
}
