using Dalmarkit.Common.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dalmarkit.Sample.EntityFrameworkCore.Entities;

public class EntityImage : StorageObjectEntityBase
{
    [Key]
    [Required]
    public Guid EntityImageId { get; set; }

    #region Foreign Key
    [Required]
    public Guid EntityId { get; set; }
    #endregion Foreign Key

    #region Inheritance
    public override Guid SelfId
    {
        get => EntityImageId;
        set => EntityImageId = value;
    }

    public override Guid PrincipalId
    {
        get => EntityId;
        set => EntityId = value;
    }
    #endregion Inheritance

    #region Navigation
    [Required]
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public Entity Entity { get; set; } = null!;
    #endregion Navigation
}
