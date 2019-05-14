FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Trade/Trade.csproj Trade/
COPY DataAccess/DataAccess.csproj DataAccess/
COPY FcoinUtil/FcoinUtil.csproj FcoinUtil/
RUN dotnet restore Trade/Trade.csproj
COPY . .
WORKDIR /src/Trade
RUN dotnet build Trade.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Trade.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Trade.dll"]
