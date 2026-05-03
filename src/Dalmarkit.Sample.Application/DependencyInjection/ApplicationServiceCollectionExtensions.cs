using Amazon.CloudFront;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Kafka;
using Amazon.Route53;
using Amazon.S3;
using Amazon.S3.Transfer;
using Dalmarkit.Cloud.Aws.Services;
using Dalmarkit.Common.Validation;
using Dalmarkit.Messaging.Kafka.Config;
using Dalmarkit.Messaging.Kafka.Consumers;
using Dalmarkit.Messaging.Kafka.Handlers;
using Dalmarkit.Sample.Application.PubSub.Kafka;
using Dalmarkit.Sample.Application.Services.ApplicationServices;
using Dalmarkit.Sample.Application.Services.DataServices;
using Dalmarkit.Sample.Application.Services.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dalmarkit.Sample.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
    {
        _ = services.Configure<AwsKafkaOptions>(config.GetSection(AwsKafkaOptions.SectionName));
        _ = services.AddOptions<KafkaConsumerOptions>()
            .Bind(config.GetSection(KafkaConsumerOptions.SectionName))
            .ValidateOnStart();

        _ = services.AddSingleton<IBootstrapServersProvider, AwsSdkSaslIamBootstrapServersProvider>();
        _ = services.AddSingleton<IOauthHandlers, AwsMskIamOauthHandlers>();
        _ = services.AddSingleton<IKafkaMessageHandler<byte[]>, KafkaLoggingMessageHandler<byte[]>>();

        // Single Byte-oriented Kafka consumer shared across every TPayload
        // Envelope-aware deserialization happens in the handler
        _ = services.AddSingleton<IKafkaConsumerService<byte[]>, KafkaConsumerService<byte[]>>();
        _ = services.AddHostedService<KafkaConsumerBackgroundService<byte[]>>();

        _ = services.AddBlockchainServices();
        _ = services.AddCloudServices(config);
        _ = services.AddUtilityServices();
        _ = services.AddValidators();

        _ = services.AddScoped<IEntityDataService, EntityDataService>();
        _ = services.AddScoped<IEntityImageDataService, EntityImageDataService>();
        _ = services.AddScoped<IDependentEntityDataService, DependentEntityDataService>();
        _ = services.AddScoped<IEvmEventDataService, EvmEventDataService>();

        _ = services.AddScoped<IDalmarkitSampleQueryService, DalmarkitSampleQueryService>();
        _ = services.AddScoped<IDalmarkitSampleCommandService, DalmarkitSampleCommandService>();
        _ = services.AddScoped<IDalmarkitSampleUploadCommandService, DalmarkitSampleUploadCommandService>();

        return services;
    }

    public static IServiceCollection AddBlockchainServices(this IServiceCollection services)
    {
        _ = services.AddSingleton<IEvmBlockchainService, EvmBlockchainService>();

        return services;
    }

    public static IServiceCollection AddCloudServices(this IServiceCollection services, IConfiguration config)
    {
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        AWSOptions awsOptions = config.GetAWSOptions();

        _ = services.AddSingleton(_ => new AmazonCloudFrontClient(awsOptions.Region));

        _ = services.AddSingleton<IAmazonCognitoIdentityProvider>(_ => new AmazonCognitoIdentityProviderClient(awsOptions.Region));

        _ = services.AddSingleton(_ => new AmazonKafkaClient(awsOptions.Region));
        _ = services.AddSingleton<IAmazonKafka>(sp => sp.GetRequiredService<AmazonKafkaClient>());

        _ = services.AddSingleton(_ => new AmazonRoute53Client(awsOptions.Region));

        _ = services.AddSingleton(_ => new AmazonS3Client(awsOptions.Region));
        _ = services.AddSingleton(provider =>
        {
            AmazonS3Client amazonS3Client = provider.GetRequiredService<AmazonS3Client>();
            return new TransferUtility(amazonS3Client);
        });

        _ = services.AddScoped<IAwsCloudFrontService, AwsCloudFrontService>();
        _ = services.AddScoped<IAwsCognitoService, AwsCognitoService>();
        _ = services.AddScoped<IAwsRoute53Service, AwsRoute53Service>();
        _ = services.AddScoped<IAwsS3Service, AwsS3Service>();

        return services;
    }

    public static IServiceCollection AddUtilityServices(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        _ = services.AddSingleton<IImageValidatorService, ImageValidatorService>();
        _ = services.AddSingleton<IDocumentValidatorService, DocumentValidatorService>();

        return services;
    }
}
