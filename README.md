# Dalmarkit-sample-webapi

Sample implementation of a ASP.NET Core web API using [Dalmarkit][dalmarkit]

## Prerequisites

1. AWS IAM user with sufficient permission to:

    1. create user pool in Cognito
    2. create S3 bucket
    3. create CloudFront distribution with S3 bucket as origin
    4. create encrypted parameters in Systems Manager Parameter Store

2. Runtime AWS IAM credentials (AWS Access Key ID with AWS Secret Access Key) with sufficient permission to:

    1. write logs to CloudWatch Logs (_logs:CreateLogGroup_, _logs:CreateLogStream_, _logs:DescribeLogGroups_, _logs:DescribeLogStreams_ and _logs:PutLogEvents_)
    2. store objects inside S3 bucket (_s3:PutObject_)
    3. create invalidations for CloudFront distribution (_cloudfront:CreateInvalidation_)
    4. read encrypted parameters from Systems Manager Parameter Store (_ssm:GetParametersByPath_)

## Deployment

1. Create user pool in Cognito with the following:

    1. 2 groups with names _bo.adm_ and _tn.adm_ for backoffice administrators and tenant administrators respectively
    2. 2 or 3 users with 1 user assigned to _bo.adm_ group and 1 or 2 users assigned _tn.adm_ group
    3. 1 resource server with name _dalmarkit_ that has 3 scopes with scope names _bo.adm_, _tn.adm_ and _cmty.usr_ for backoffice administrators, tenant administrators and community users respectively
    4. 3 app clients

        - public backoffice client with _dalmarkit/bo.adm_ scope
        - public tenant client with _dalmarkit/tn.adm_ scope
        - confidential server-to-server client with _dalmarkit/bo.adm_ scope

2. Create bucket in S3 to store uploaded images
3. Create CloudFront distribution with above S3 bucket as origin
4. Create the following Systems Manager Parameter Store encrypted parameters:

    1. _/DalmarkitSample/Api/Development/AwsCognitoAuthenticationOptions/UserPoolId_ - Cognito user pool ID
    2. _/DalmarkitSample/Api/Development/AwsCognitoAuthenticationOptions/ValidClientIds_ - space delimited list of client IDs allowed to access the web API (e.g. public backoffice client ID, public tenant client ID and confidential server-to-server client ID)
    3. _/DalmarkitSample/Api/Development/AwsCognitoAuthorizationOptions/BackofficeAdminClientIds_ - confidential server to server client ID
    4. _/DalmarkitSample/Api/Development/ConnectionStrings/DefaultConnection_ - database connection string
    5. _/DalmarkitSample/Api/Development/EntityOptions/ImageS3BucketName_ - name of S3 bucket that will store uploaded images
    6. _/DalmarkitSample/Api/Development/EntityOptions/ImageCloudFrontDistributionId_ - ID of CloudFront distribution used to deliver images stored in S3 bucket

5. Setup PostgreSQL database by running SQL scripts inside [Migration folder][migration] in order

6. Deploy with Docker

```sh
docker run -d -it -p 5500:5000 -e ASPNETCORE_ENVIRONMENT=Development -e AWS_ACCESS_KEY_ID=<AwsAccessKeyId> -e AWS_SECRET_ACCESS_KEY=+<AwsSecretAccessKey> tigerwonghk/dalmarkit-sample-webapi
```

7. Access Swagger UI web page at http://localhost:5500/swagger to browse the web API documentation

## Debug in Visual Studio Code

1. Clone [GitHub repository][repository]

```sh
git clone https://github.com/tigerwong-hk/Dalmarkit-sample-webapi.git
```

2. Install the [Remote Development extension][remotedev] in Visual Studio Code
3. Copy [.env.example][envexample] to .env and replace the values in .env as follows:

-   AWS_ACCESS_KEY_ID - AWS access key ID of runtime AWS IAM credentials under [Prerequisites][prerequisites]
-   AWS_SECRET_ACCESS_KEY - AWS secret access key of runtime AWS IAM credentials under [Prerequisites][prerequisites]
-   BATCH_CLIENT_SECRET - client secret of confidential server-to-server client

4. Update _.vscode/settings_ according to Cognito configuration

5. Bring up the Visual Studio Code _Command Palette..._, choose _Dev Containers: Open Folder in Container..._, navigate to the root directory of the cloned repository and click _Open_
6. Click on _Start Debugging_ under _Run_ menu to start debugging

## Feedback

Bug reports and contributions are welcome at our [GitHub repository][repository].

<!-- LINKS -->

[dalmarkit]: https://github.com/tigerwong-hk/Dalmarkit
[envexample]: .env.example
[migration]: src/Dalmarkit.Sample.WebApi/Migrations
[prerequisites]: #prerequisites
[remotedev]: https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.vscode-remote-extensionpack
[repository]: https://github.com/tigerwong-hk/Dalmarkit-sample-webapi
