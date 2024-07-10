using Dalmarkit.Blockchain.Constants;

namespace Dalmarkit.Sample.Core.Dtos.Outputs;

public class EvmEventOutputDto
{
    public Guid EvmEventId { get; set; }

    public string EventName { get; set; } = null!;

    public string ContractAddress { get; set; } = null!;

    public string TransactionHash { get; set; } = null!;

    public BlockchainNetwork BlockchainNetwork { get; set; }

    public string EventDetail { get; set; } = null!;
}
