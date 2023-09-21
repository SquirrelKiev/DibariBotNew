# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-env

WORKDIR /source

COPY DibariBot/*.csproj DibariBot/

RUN dotnet restore DibariBot/

COPY . .

RUN set -xe; \
dotnet publish -c Release -o /app; \
chmod +x /app/DibariBot

FROM mcr.microsoft.com/dotnet/runtime:7.0 as runtime

WORKDIR /app

COPY --from=build-env /app .

VOLUME [ "/data" ]

ENV DIBARI_CONFIG_LOCATION /data/botconfig.toml

CMD dotnet DibariBot.dll