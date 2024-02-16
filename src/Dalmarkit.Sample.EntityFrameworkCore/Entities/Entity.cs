using AutoMapper.Configuration.Annotations;
using Dalmarkit.Common.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace Dalmarkit.Sample.EntityFrameworkCore.Entities;

public class Entity : PrincipalEntityBase
{
    [Key]
    [Required]
    [Ignore]
    public Guid EntityId { get; set; }

    [Required]
    public string EntityName { get; set; } = null!;

    #region Inheritance
    public override Guid SelfId
    {
        get => EntityId;
        set => EntityId = value;
    }
    #endregion Inheritance

    #region Navigation
    public ICollection<EntityImage>? EntityImages { get; set; }
    #endregion Navigation
}
