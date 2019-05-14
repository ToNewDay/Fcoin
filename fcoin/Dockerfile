FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY fcoin/fcoin.csproj fcoin/
COPY DataAccess/DataAccess.csproj DataAccess/
COPY FcoinUtil/FcoinUtil.csproj FcoinUtil/
RUN dotnet restore fcoin/fcoin.csproj
COPY . .
WORKDIR /src/fcoin
RUN dotnet build fcoin.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish fcoin.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "fcoin.dll"]
