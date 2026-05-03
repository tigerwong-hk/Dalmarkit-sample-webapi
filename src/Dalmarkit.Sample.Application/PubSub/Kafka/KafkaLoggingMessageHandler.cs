using Confluent.Kafka;
using Dalmarkit.Messaging.Kafka.Consumers;
using Dalmarkit.Messaging.PubSub.Kafka;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Dalmarkit.Sample.Application.PubSub.Kafka;

public class KafkaLoggingMessageHandler<TValue> : IKafkaMessageHandler<TValue>
{
    private readonly ILogger<KafkaLoggingMessageHandler<TValue>> _logger;

    public KafkaLoggingMessageHandler(ILogger<KafkaLoggingMessageHandler<TValue>> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
    }

    public Task HandleAsync(ConsumeResult<string, TValue> consumeResult, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(consumeResult);

        int valueSize = GetValueSize(consumeResult.Message is null ? default : consumeResult.Message.Value);
        string? businessMessageId = ReadHeader(consumeResult, KafkaMessageHeaders.BusinessMessageId);
        string? method = ReadHeader(consumeResult, KafkaMessageHeaders.Method);
        string? payloadType = ReadHeader(consumeResult, KafkaMessageHeaders.PayloadType);
        string? publishTimestamp = ReadHeader(consumeResult, KafkaMessageHeaders.PublishTimestamp);

        if (consumeResult.Message != null && !EqualityComparer<TValue?>.Default.Equals(consumeResult.Message.Value, default) && consumeResult.Message.Value is byte[] messageBytes)
        {
            string messageJson = Encoding.UTF8.GetString(messageBytes);
            _logger.KafkaLoggingMessageHandlerHandleAsyncMessageInfo(
                consumeResult.Topic,
                consumeResult.Partition.Value,
                consumeResult.Offset.Value,
                consumeResult.Message?.Key,
                valueSize,
                method,
                payloadType,
                businessMessageId,
                messageJson,
                publishTimestamp);
        }
        else
        {
            _logger.KafkaLoggingMessageHandlerHandleAsyncNullMessageWarning(
                consumeResult.Topic,
                consumeResult.Partition.Value,
                consumeResult.Offset.Value,
                consumeResult.Message?.Key,
                valueSize,
                method,
                payloadType,
                businessMessageId,
                publishTimestamp);
        }

        return Task.CompletedTask;
    }

    private static int GetValueSize(TValue? value)
    {
        // Returns the wire byte size for the two TValue shapes the consumer service supports (byte[] / string)
        // -1 for any other deserialized type so logs surface the unknown branch
        return value switch
        {
            byte[] bytes => bytes.Length,
            string s => Encoding.UTF8.GetByteCount(s),
            null => 0,
            _ => -1,
        };
    }

    private static string? ReadHeader(ConsumeResult<string, TValue> consumeResult, string headerName)
    {
        if (consumeResult.Message?.Headers is null)
        {
            return null;
        }

#pragma warning disable IDE0046 // Convert to conditional expression
        if (!consumeResult.Message.Headers.TryGetLastBytes(headerName, out byte[] bytes))
        {
            return null;
        }
#pragma warning restore IDE0046 // Convert to conditional expression

        return bytes == null || bytes.Length == 0 ? null : Encoding.UTF8.GetString(bytes);
    }
}

public static partial class KafkaLoggingMessageHandlerLogs
{
    [LoggerMessage(
        EventId = 1010,
        Level = LogLevel.Information,
        Message = "KafkaLoggingMessageHandlerHandleAsync: handled topic `{Topic}` in partition `{Partition}` at offset `{Offset}` with key `{Key}`, valueSize `{ValueSize}`, method `{Method}`, payloadType `{PayloadType}`, businessMessageId `{BusinessMessageId}` and publish timestamp `{PublishTimestamp}`: {MessageJson}")]
    public static partial void KafkaLoggingMessageHandlerHandleAsyncMessageInfo(
        this ILogger logger,
        string topic,
        int partition,
        long offset,
        string? key,
        int valueSize,
        string? method,
        string? payloadType,
        string? businessMessageId,
        string? messageJson,
        string? publishTimestamp);

    [LoggerMessage(
        EventId = 1020,
        Level = LogLevel.Warning,
        Message = "KafkaLoggingMessageHandlerHandleAsync: handled topic `{Topic}` in partition `{Partition}` at offset `{Offset}` with key `{Key}`, valueSize `{ValueSize}`, method `{Method}`, payloadType `{PayloadType}`, businessMessageId `{BusinessMessageId}` and publish timestamp `{PublishTimestamp}`")]
    public static partial void KafkaLoggingMessageHandlerHandleAsyncNullMessageWarning(
        this ILogger logger,
        string topic,
        int partition,
        long offset,
        string? key,
        int valueSize,
        string? method,
        string? payloadType,
        string? businessMessageId,
        string? publishTimestamp);
}
