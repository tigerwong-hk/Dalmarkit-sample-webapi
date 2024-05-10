using AutoMapper.Configuration.Annotations;
using Dalmarkit.Common.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dalmarkit.Sample.EntityFrameworkCore.Entities;

public class DependentEntity : DependentReadWriteEntityBase
{
    [Key]
    [Required]
    [Ignore]
    public Guid DependentEntityId { get; set; }

    [Required]
    public string DependentEntityName { get; set; } = null!;

    #region Foreign Key
    [Required]
    [Ignore]
    public Guid EntityId { get; set; }
    #endregion Foreign Key

    #region Inheritance
    public override Guid PrincipalId
    {
        get => EntityId;
        set => EntityId = value;
    }

    public override Guid SelfId
    {
        get => DependentEntityId;
        set => DependentEntityId = value;
    }
    #endregion Inheritance

    #region Navigation
    [Required]
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public Entity Entity { get; set; } = null!;
    #endregion Navigation
}
