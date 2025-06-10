using System.Numerics;
using Dalmarkit.Blockchain.Constants;
using Dalmarkit.Blockchain.Evm.Services;
using Dalmarkit.Common.Validation;
using Dalmarkit.Sample.Application.Options;
using Dalmarkit.Sample.Application.Services.ExternalServices.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dalmarkit.Sample.Application.Services.ExternalServices;

public class EvmBlockchainService : EvmBlockchainServiceBase, IEvmBlockchainService
{
    private readonly ContractOptions _contractOptions;
    private readonly ILogger _logger;

    public EvmBlockchainService(
        IOptions<ContractOptions> contractOptions,
        IOptions<EvmBlockchainOptions> blockchainOptions,
        IOptions<EvmWalletOptions>? walletOptions,
        ILogger<EvmBlockchainServiceBase> logger) : base(blockchainOptions, walletOptions, logger)
    {
        _contractOptions = Guard.NotNull(contractOptions, nameof(contractOptions)).Value;
        _ = Guard.NotNull(_contractOptions.ContractInfo, nameof(_contractOptions.ContractInfo));

        _logger = Guard.NotNull(logger, nameof(logger));
    }

    public async Task<PositionsOutputDTO?> CallNonFungiblePositionManagerPositionsFunctionAsync(BigInteger tokenId, BlockchainNetwork blockchainNetwork)
    {
        (string contractAddress, _) = GetContractInfo("NonFungiblePositionManager", blockchainNetwork);
        if (string.IsNullOrWhiteSpace(contractAddress))
        {
            _logger.ContractAddressNullOrWhitespaceForError("NonFungiblePositionManager", blockchainNetwork);
            return default;
        }

        PositionsFunction positionsFunction = new()
        {
            TokenId = tokenId,
        };

        return await CallReadContractFunctionAsync<PositionsFunction, PositionsOutputDTO>(positionsFunction, contractAddress, blockchainNetwork);
    }

    public (string, string?) GetContractInfo(string contractName, BlockchainNetwork blockchainNetwork)
    {
        _ = Guard.NotNullOrWhiteSpace(contractName, nameof(contractName));

        if (!_contractOptions.ContractInfo!.TryGetValue(contractName, out ContractInfo? contractInfo))
        {
            _logger.ContractInfoNotFoundForError(contractName);
            return default;
        }

        if (contractInfo == null)
        {
            _logger.ContractInfoNullForError(contractName);
            return default;
        }

        if (contractInfo.ContractAddresses == null)
        {
            _logger.ContractAddressesNullForError(contractName);
            return default;
        }

        string? contractAddress = GetPropertyForBlockchainNetwork(contractInfo.ContractAddresses!, blockchainNetwork);
        if (string.IsNullOrWhiteSpace(contractAddress))
        {
            _logger.ContractAddressNullOrWhitespaceForError(contractName, blockchainNetwork);
            return default;
        }

        return (contractAddress, contractInfo.ContractJsonAbiFile);
    }

    public async Task<string?> GetEvmEventByNameAsync(string eventName, string contractName, string transactionHash, BlockchainNetwork blockchainNetwork)
    {
        _ = Guard.NotNullOrWhiteSpace(eventName, nameof(eventName));
        _ = Guard.NotNullOrWhiteSpace(contractName, nameof(contractName));
        _ = Guard.NotNullOrWhiteSpace(transactionHash, nameof(transactionHash));

        (string contractAddress, string? jsonAbiFile) = GetContractInfo(contractName, blockchainNetwork);

        if (string.IsNullOrWhiteSpace(contractAddress))
        {
            _logger.ContractAddressNullOrWhitespaceForError(contractName, blockchainNetwork);
            return default;
        }

        if (string.IsNullOrWhiteSpace(jsonAbiFile))
        {
            _logger.JsonAbiFileNullOrWhitespaceForError(contractName, blockchainNetwork);
            return default;
        }

        string jsonAbi = await File.ReadAllTextAsync(jsonAbiFile);

        return await GetEventByNameAsync(contractAddress, transactionHash, blockchainNetwork, eventName, jsonAbi);
    }

    public async Task<List<EvmEventDto>?> GetEvmEventsByNameAsync(string eventName, string contractName, string transactionHash, BlockchainNetwork blockchainNetwork)
    {
        _ = Guard.NotNullOrWhiteSpace(eventName, nameof(eventName));
        _ = Guard.NotNullOrWhiteSpace(contractName, nameof(contractName));
        _ = Guard.NotNullOrWhiteSpace(transactionHash, nameof(transactionHash));

        (string contractAddress, string? jsonAbiFile) = GetContractInfo(contractName, blockchainNetwork);

        if (string.IsNullOrWhiteSpace(contractAddress))
        {
            _logger.ContractAddressNullOrWhitespaceForError(contractName, blockchainNetwork);
            return default;
        }

        if (string.IsNullOrWhiteSpace(jsonAbiFile))
        {
            _logger.JsonAbiFileNullOrWhitespaceForError(contractName, blockchainNetwork);
            return default;
        }

        string jsonAbi = await File.ReadAllTextAsync(jsonAbiFile);

        return await GetEventsByNameAsync(contractAddress, transactionHash, blockchainNetwork, eventName, jsonAbi);
    }

    public async Task<List<RoyaltyPaymentEventDTO>?> GetLooksRareExchangeRoyaltyPaymentEventAsync(string transactionHash, BlockchainNetwork blockchainNetwork)
    {
        _ = Guard.NotNullOrWhiteSpace(transactionHash, nameof(transactionHash));

        (string contractAddress, _) = GetContractInfo("LooksRareExchange", blockchainNetwork);
        if (string.IsNullOrWhiteSpace(contractAddress))
        {
            _logger.ContractAddressNullOrWhitespaceForError("LooksRareExchange", blockchainNetwork);
            return default;
        }

        return await GetEventAsync<RoyaltyPaymentEventDTO>(contractAddress, transactionHash, blockchainNetwork);
    }

    public async Task<string?> GetLooksRareExchangeRoyaltyPaymentEventByNameAsync(string transactionHash, BlockchainNetwork blockchainNetwork)
    {
        _ = Guard.NotNullOrWhiteSpace(transactionHash, nameof(transactionHash));

        (string contractAddress, string? jsonAbiFile) = GetContractInfo("LooksRareExchange", blockchainNetwork);

        if (string.IsNullOrWhiteSpace(contractAddress))
        {
            _logger.ContractAddressNullOrWhitespaceForError("LooksRareExchange", blockchainNetwork);
            return default;
        }

        if (string.IsNullOrWhiteSpace(jsonAbiFile))
        {
            _logger.JsonAbiFileNullOrWhitespaceForError("LooksRareExchange", blockchainNetwork);
            return default;
        }

        string jsonAbi = await File.ReadAllTextAsync(jsonAbiFile);

        return await GetEventByNameAsync(contractAddress, transactionHash, blockchainNetwork, "RoyaltyPayment", jsonAbi);
    }

    public async Task<List<EvmEventDto>?> GetLooksRareExchangeRoyaltyPaymentEventsByNameAsync(string transactionHash, BlockchainNetwork blockchainNetwork)
    {
        _ = Guard.NotNullOrWhiteSpace(transactionHash, nameof(transactionHash));

        (string contractAddress, string? jsonAbiFile) = GetContractInfo("LooksRareExchange", blockchainNetwork);

        if (string.IsNullOrWhiteSpace(contractAddress))
        {
            _logger.ContractAddressNullOrWhitespaceForError("LooksRareExchange", blockchainNetwork);
            return default;
        }

        if (string.IsNullOrWhiteSpace(jsonAbiFile))
        {
            _logger.JsonAbiFileNullOrWhitespaceForError("LooksRareExchange", blockchainNetwork);
            return default;
        }

        string jsonAbi = await File.ReadAllTextAsync(jsonAbiFile);

        return await GetEventsByNameAsync(contractAddress, transactionHash, blockchainNetwork, "RoyaltyPayment", jsonAbi);
    }

    public async Task<string?> GetLooksRareExchangeRoyaltyPaymentEventBySha3SignatureAsync(string transactionHash, BlockchainNetwork blockchainNetwork)
    {
        _ = Guard.NotNullOrWhiteSpace(transactionHash, nameof(transactionHash));

        (string contractAddress, string? jsonAbiFile) = GetContractInfo("LooksRareExchange", blockchainNetwork);

        if (string.IsNullOrWhiteSpace(contractAddress))
        {
            _logger.ContractAddressNullOrWhitespaceForError("LooksRareExchange", blockchainNetwork);
            return default;
        }

        if (string.IsNullOrWhiteSpace(jsonAbiFile))
        {
            _logger.JsonAbiFileNullOrWhitespaceForError("LooksRareExchange", blockchainNetwork);
            return default;
        }

        string jsonAbi = await File.ReadAllTextAsync(jsonAbiFile);

        return await GetEventBySha3SignatureAsync(contractAddress, transactionHash, blockchainNetwork, "0x27c4f0403323142b599832f26acd21c74a9e5b809f2215726e244a4ac588cd7d", jsonAbi);
    }
}

public static partial class EvmBlockchainServiceLogs
{
    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Error,
        Message = "Contract addresses null for `{ContractName}`")]
    public static partial void ContractAddressesNullForError(
        this ILogger logger, string contractName);

    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Error,
        Message = "`{ContractName}` contract address null or whitespace for blockchain network `{BlockchainNetwork}`")]
    public static partial void ContractAddressNullOrWhitespaceForError(
        this ILogger logger, string contractName, BlockchainNetwork blockchainNetwork);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Contract info not found for `{ContractName}`")]
    public static partial void ContractInfoNotFoundForError(
        this ILogger logger, string contractName);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Error,
        Message = "Contract info null for `{ContractName}`")]
    public static partial void ContractInfoNullForError(
        this ILogger logger, string contractName);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Error,
        Message = "`{ContractName}` JSON ABI file null or whitespace for blockchain network `{BlockchainNetwork}`")]
    public static partial void JsonAbiFileNullOrWhitespaceForError(
        this ILogger logger, string contractName, BlockchainNetwork blockchainNetwork);
}
