FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TinyProxy.csproj", "."]
RUN dotnet restore "TinyProxy.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "TinyProxy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TinyProxy.csproj" -c Release -o /app/publish --self-contained true --runtime linux-x64 -p:PublishTrimmed=true -p:PublishSingleFile=true

FROM base AS final
USER 100000
WORKDIR /app
COPY --chown=100000:100000 --from=publish /app/publish .
ENTRYPOINT ["./TinyProxy"]
