ARG APP_NAME="dalmarkit-sample-webapi"
ARG RELEASE_PROJECT_NAME="Dalmarkit.Sample.WebApi"
ARG RELEASE_PROJECT_RELATIVE_PATH="src/${RELEASE_PROJECT_NAME}"
ARG WORK_DIR="/opt/${APP_NAME}"

#
# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG RELEASE_PROJECT_RELATIVE_PATH
ARG WORK_DIR
ARG NUGET_PRIVATE_USERNAME
ARG NUGET_PRIVATE_PASSWORD

# https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet#environment-variables
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true \
    DOTNET_NOLOGO=true \
    DOTNET_CLI_TELEMETRY_OPTOUT=true \
    NUGET_PRIVATE_USERNAME="${NUGET_PRIVATE_USERNAME}" \
    NUGET_PRIVATE_PASSWORD="${NUGET_PRIVATE_PASSWORD}"

WORKDIR "${WORK_DIR}"

COPY . .

RUN : \
    # Setup the local nuget package source
    && mkdir ~/.nuget_local_packages \
    # Pre-check & Unit testing
    && dotnet restore \
    && dotnet clean -c Release \
    && dotnet build -c Release --no-restore /WarnAsError \
    && dotnet test -c Release \
    # Release
    && rm -rf "${RELEASE_PROJECT_RELATIVE_PATH}/bin" "${RELEASE_PROJECT_RELATIVE_PATH}/obj" \
    && dotnet clean -c Release "${RELEASE_PROJECT_RELATIVE_PATH}" \
    && dotnet publish -c Release "${RELEASE_PROJECT_RELATIVE_PATH}"

#
# Deploy
FROM mcr.microsoft.com/dotnet/aspnet:8.0

ARG RELEASE_PROJECT_NAME
ARG RELEASE_PROJECT_RELATIVE_PATH
ARG WORK_DIR

RUN groupadd -r default && useradd -r -g default default
COPY --chown=default:default --from=build "${WORK_DIR}/${RELEASE_PROJECT_RELATIVE_PATH}/bin/Release/net8.0/publish/" "${WORK_DIR}"

WORKDIR "${WORK_DIR}"

RUN ln -s "${WORK_DIR}/${RELEASE_PROJECT_NAME}.dll" "${WORK_DIR}/application.dll"

ENV ASPNETCORE_URLS=http://+:5000
#ENV ASPNETCORE_URLS=http://+:5001

EXPOSE 5000
#EXPOSE 5001

USER default

CMD ["dotnet", "application.dll"]
