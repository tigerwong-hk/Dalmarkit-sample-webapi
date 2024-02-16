using Dalmarkit.AspNetCore.AuditTrail;
using Dalmarkit.AspNetCore.Authorization;
using Dalmarkit.AspNetCore.Logging;
using Dalmarkit.AspNetCore.Exceptions;
using Dalmarkit.Blockchain.Evm.Services;
using Dalmarkit.Cloud.Aws.Services;
using Dalmarkit.Common.Identity;
using Dalmarkit.Common.Net;
using Dalmarkit.Common.Validation;
using Dalmarkit.Sample.Application.DependencyInjection;
using Dalmarkit.Sample.Application.Options;
using Dalmarkit.Sample.EntityFrameworkCore.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Globalization;
using System.Net;
using System.Security.Claims;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up!");

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    Log.Information($"ApplicationName: {builder.Environment.ApplicationName}");
    Log.Information($"EnvironmentName: {builder.Environment.EnvironmentName}");
    Log.Information($"ContentRootPath: {builder.Environment.ContentRootPath}");
    Log.Information($"WebRootPath: {builder.Environment.WebRootPath}");

    // Add services to the container.
    _ = builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    _ = builder.WebHost.UseKestrel(options =>
    {
        options.AddServerHeader = false; // Don't include Server header in each response
    });

    _ = builder.Configuration.AddSystemsManager($"/DalmarkitSample/Api/{builder.Environment.EnvironmentName}");

    _ = builder.Services.Configure<AwsCognitoAuthenticationOptions>(builder.Configuration.GetSection("AwsCognitoAuthenticationOptions"));
    _ = builder.Services.Configure<AwsCloudFrontOptions>(builder.Configuration.GetSection("AwsCloudFrontOptions"));
    _ = builder.Services.Configure<EvmBlockchainOptions>(builder.Configuration.GetSection("EvmBlockchainOptions"));
    _ = builder.Services.Configure<EntityOptions>(builder.Configuration.GetSection("EntityOptions"));

    string? databaseConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    _ = Guard.NotNullOrWhiteSpace(databaseConnectionString, nameof(databaseConnectionString));

    _ = builder.Services.AddDbContext<DalmarkitSampleDbContext>(options =>
        _ = options.UseNpgsql(databaseConnectionString, options => options.MigrationsAssembly(typeof(Program).Assembly.FullName)));

    _ = builder.Services.AddAuditTrail<DalmarkitSampleDbContext>(databaseConnectionString!);

    _ = builder.Services.AddHttpContextAccessor();

    AwsCognitoAuthenticationOptions? authenticationOptions = builder.Configuration.GetSection("AwsCognitoAuthenticationOptions").Get<AwsCognitoAuthenticationOptions>();
    _ = Guard.NotNull(authenticationOptions, nameof(authenticationOptions));

    _ = Guard.NotNullOrWhiteSpace(authenticationOptions!.ValidClientIds, nameof(authenticationOptions.ValidClientIds));
    string[] validClientIds = authenticationOptions.ValidClientIds!.Trim().Split(' ');
    if (validClientIds.Length < 1)
    {
        throw new InvalidOperationException("No valid client IDs.");
    }

    _ = builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.Authority = authenticationOptions.IssuerBaseUrl + authenticationOptions.UserPoolId;
            options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidAudiences = validClientIds,
                ValidateAudience = true,
                ValidIssuer = authenticationOptions.IssuerBaseUrl + authenticationOptions.UserPoolId,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidateTokenReplay = true,
                AudienceValidator = (_, securityToken, validationParameters) =>
                {
                    if (securityToken is not JsonWebToken jwtToken)
                    {
                        Log.Error("AudienceValidator: Not JSON web token");
                        return false;
                    }

                    Claim? tokenUseClaim = jwtToken.Claims.SingleOrDefault(c => c.Type == AwsCognitoJwtClaims.TokenUse);
                    if (tokenUseClaim == null)
                    {
                        Log.Error("AudienceValidator: No token use claim found");
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(tokenUseClaim.Value))
                    {
                        Log.Error("AudienceValidator: Missing token use claim value");
                        return false;
                    }

                    bool isValidTokenUseClaim = string.Equals(tokenUseClaim.Value, "access", StringComparison.Ordinal);
                    if (!isValidTokenUseClaim)
                    {
                        Log.Error($"AudienceValidator: Invalid token use claim value '{tokenUseClaim.Value}'");
                        return false;
                    }

                    Claim? clientIdClaim = jwtToken.Claims.SingleOrDefault(c => c.Type == AwsCognitoJwtClaims.ClientId);
                    if (clientIdClaim == null)
                    {
                        Log.Error("AudienceValidator: No client ID claim found");
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(clientIdClaim.Value))
                    {
                        Log.Error("AudienceValidator: Missing client ID claim value");
                        return false;
                    }

                    bool isValidClientIdClaim = validationParameters.ValidAudiences.Contains(clientIdClaim.Value);
                    if (!isValidClientIdClaim)
                    {
                        Log.Error($"AudienceValidator: Invalid client ID claim value '{clientIdClaim.Value}'");
                        return false;
                    }

                    return isValidTokenUseClaim && isValidClientIdClaim;
                }
            };
        }
    );

    AwsCognitoAuthorizationOptions? authorizationOptions = builder.Configuration.GetSection("AwsCognitoAuthorizationOptions").Get<AwsCognitoAuthorizationOptions>();
    _ = Guard.NotNull(authorizationOptions, nameof(authorizationOptions));

    _ = Guard.NotNull(authorizationOptions!.CommunityUserScopes, nameof(authorizationOptions.CommunityUserScopes));
    string[] allowedCommunityUserScopes = string.IsNullOrWhiteSpace(authorizationOptions.CommunityUserScopes) ?
        [] : authorizationOptions.CommunityUserScopes.Trim().Split(' ');

    _ = Guard.NotNull(authorizationOptions.BackofficeAdminClientIds, nameof(authorizationOptions.BackofficeAdminClientIds));
    string[] allowedBackofficeAdminClientIds = string.IsNullOrWhiteSpace(authorizationOptions.BackofficeAdminClientIds) ?
        [] : authorizationOptions.BackofficeAdminClientIds.Trim().Split(' ');

    _ = Guard.NotNull(authorizationOptions.BackofficeAdminGroups, nameof(authorizationOptions.BackofficeAdminGroups));
    string[] allowedBackofficeAdminGroups = string.IsNullOrWhiteSpace(authorizationOptions.BackofficeAdminGroups) ?
        [] : authorizationOptions.BackofficeAdminGroups.Trim().Split(' ');

    _ = Guard.NotNull(authorizationOptions.BackofficeAdminScopes, nameof(authorizationOptions.BackofficeAdminScopes));
    string[] allowedBackofficeAdminScopes = string.IsNullOrWhiteSpace(authorizationOptions.BackofficeAdminScopes) ?
        [] : authorizationOptions.BackofficeAdminScopes.Trim().Split(' ');

    _ = Guard.NotNull(authorizationOptions.TenantAdminGroups, nameof(authorizationOptions.TenantAdminGroups));
    string[] allowedTenantAdminGroups = string.IsNullOrWhiteSpace(authorizationOptions.TenantAdminGroups) ?
        [] : authorizationOptions.TenantAdminGroups.Trim().Split(' ');

    _ = Guard.NotNull(authorizationOptions.TenantAdminScopes, nameof(authorizationOptions.TenantAdminScopes));
    string[] allowedTenantAdminScopes = string.IsNullOrWhiteSpace(authorizationOptions.TenantAdminScopes) ?
        [] : authorizationOptions.TenantAdminScopes.Trim().Split(' ');

    _ = builder.Services.AddAuthorizationBuilder()
        .AddPolicy(nameof(AwsCognitoAuthorizationOptions.CommunityUserScopes), policyBuilder =>
            _ = policyBuilder.RequireScope(AwsCognitoJwtClaims.Scope, allowedCommunityUserScopes))
        .AddPolicy(nameof(AwsCognitoAuthorizationOptions.BackofficeAdminClientIds), policyBuilder =>
            _ = policyBuilder.RequireClaim(AwsCognitoJwtClaims.ClientId, allowedBackofficeAdminClientIds))
        .AddPolicy(nameof(AwsCognitoAuthorizationOptions.BackofficeAdminGroups), policyBuilder =>
            _ = policyBuilder.RequireClaim(AwsCognitoJwtClaims.CognitoGroups, allowedBackofficeAdminGroups))
        .AddPolicy(nameof(AwsCognitoAuthorizationOptions.BackofficeAdminScopes), policyBuilder =>
            _ = policyBuilder.RequireScope(AwsCognitoJwtClaims.Scope, allowedBackofficeAdminScopes))
        .AddPolicy(nameof(AwsCognitoAuthorizationOptions.TenantAdminGroups), policyBuilder =>
            _ = policyBuilder.RequireClaim(AwsCognitoJwtClaims.CognitoGroups, allowedTenantAdminGroups))
        .AddPolicy(nameof(AwsCognitoAuthorizationOptions.TenantAdminScopes), policyBuilder =>
            _ = policyBuilder.RequireScope(AwsCognitoJwtClaims.Scope, allowedTenantAdminScopes));

    builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IAuthorizationHandler, ScopeAuthorizationHandler>());

    _ = builder.Services.AddControllers(options => _ = options.AddGlobalAuditFilter([]))
    .AddJsonOptions(options =>
    {
        JsonStringEnumConverter enumConverter = new();
        options.JsonSerializerOptions.Converters.Add(enumConverter);
    });

    _ = builder.Services.AddApplication(builder.Configuration);

    _ = builder.Services.AddCors(options =>
    {
        string[] allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
        options.AddDefaultPolicy(builder =>
            builder.WithOrigins(allowedOrigins)
                .WithHeaders(HeaderNames.Authorization, HeaderNames.ContentType)
                .WithMethods("GET", "OPTIONS", "POST", "PUT", "DELETE")
    );
    });

    NetworkOptions? networkOptions = builder.Configuration.GetSection("NetworkOptions").Get<NetworkOptions>();
    _ = builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        if (networkOptions != null)
        {
            if (networkOptions.KnownNetworks?.Count > 0)
            {
                options.ForwardLimit = null;
                foreach (string network in networkOptions.KnownNetworks)
                {
                    string[] args = network.Split('/', 2);
                    if (args.Length == 2)
                    {
                        options.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Parse(args[0]), int.Parse(args[1], NumberStyles.None, CultureInfo.InvariantCulture)));
                    }
                }
            }
            if (networkOptions.KnownProxies?.Count > 0)
            {
                options.ForwardLimit = null;
                foreach (string proxy in networkOptions.KnownProxies)
                {
                    options.KnownProxies.Add(IPAddress.Parse(proxy));
                }
            }
            if (!string.IsNullOrWhiteSpace(networkOptions.ClientIpHeader))
            {
                options.ForwardedForHeaderName = networkOptions.ClientIpHeader;
            }
        }
    });

    _ = builder.Services.PostConfigure<ApiBehaviorOptions>(options =>
    {
        Func<ActionContext, IActionResult> builtInFactory = options.InvalidModelStateResponseFactory;

        options.InvalidModelStateResponseFactory = context =>
        {
            // Log Automatic HTTP 400 responses triggered by model validation errors in ApiControllers
            // See https://github.com/dotnet/AspNetCore.Docs/issues/12157
            if (!context.ModelState.IsValid)
            {
                ApiModelValidationErrorLogger.LogInformation(context);
            }

            return builtInFactory(context);
        };
    });

    _ = builder.Services.AddHealthChecks();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    _ = builder.Services.AddEndpointsApiExplorer();
    _ = builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Cognito Secured API", Version = "v1" });
        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme }
                },
                Array.Empty<string>()
            }
        });
    });

    WebApplication app = builder.Build();

    _ = app.UseForwardedHeaders();

    if (!app.Environment.IsDevelopment())
    {
        _ = app.UseExceptionsMiddleware();
    }

    _ = app.Use(async (context, next) =>
    {
        context.Request.EnableBuffering();
        await next();
    });

    _ = app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Qat"))
    {
        _ = app.UseSwagger();
        _ = app.UseSwaggerUI();
    }

    // app.UseHttpsRedirection();

    _ = app.UseRouting();

    _ = app.UseCors();

    _ = app.UseAuthentication();

    _ = app.UseAuthorization();

    _ = app.MapControllers();

    _ = app.MapHealthChecks("/health");

    app.AddAuditTrailCustomActions(app.Services.GetRequiredService<IHttpContextAccessor>());

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
