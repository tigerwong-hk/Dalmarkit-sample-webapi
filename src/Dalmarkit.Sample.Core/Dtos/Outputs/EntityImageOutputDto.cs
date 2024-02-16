namespace Dalmarkit.Sample.Core.Dtos.Outputs;

public class EntityImageOutputDto
{
    public Guid EntityImageId { get; set; }

    public string ObjectName { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;
}
