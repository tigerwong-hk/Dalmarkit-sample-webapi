using AutoMapper.Configuration.Annotations;
using Dalmarkit.Blockchain.Constants;
using Dalmarkit.Common.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dalmarkit.Sample.EntityFrameworkCore.Entities;

public class EvmEvent : ReadOnlyEntityBase
{
    [Key]
    [Required]
    [Ignore]
    public Guid EvmEventId { get; set; }

    [Required]
    public string EventName { get; set; } = null!;

    [Required]
    public string ContractAddress { get; set; } = null!;

    [Required]
    public string TransactionHash { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public BlockchainNetwork BlockchainNetwork { get; set; }

    [Required]
    [Column(TypeName = "jsonb")]
    public string EventDetail { get; set; } = null!;
}
