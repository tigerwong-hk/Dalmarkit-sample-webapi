using System.Numerics;
using Dalmarkit.Blockchain.Constants;
using Dalmarkit.Sample.Application.Services.ExternalServices.Contracts;

namespace Dalmarkit.Sample.Application.Services.ExternalServices;

public interface IEvmBlockchainService
{
    Task<PositionsOutputDTO?> CallNonFungiblePositionManagerPositionsFunctionAsync(BigInteger tokenId, BlockchainNetwork blockchainNetwork);
    (string, string?) GetContractInfo(string contractName, BlockchainNetwork blockchainNetwork);
    Task<string?> GetEvmEventByNameAsync(string eventName, string contractName, string transactionHash, BlockchainNetwork blockchainNetwork);
    Task<List<RoyaltyPaymentEventDTO>?> GetLooksRareExchangeRoyaltyPaymentEventAsync(string transactionHash, BlockchainNetwork blockchainNetwork);
    Task<string?> GetLooksRareExchangeRoyaltyPaymentEventByNameAsync(string transactionHash, BlockchainNetwork blockchainNetwork);
    Task<string?> GetLooksRareExchangeRoyaltyPaymentEventBySha3SignatureAsync(string transactionHash, BlockchainNetwork blockchainNetwork);
}
