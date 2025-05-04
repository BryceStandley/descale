FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Receiving/WMS.Receiving.API/WMS.Receiving.API.csproj", "src/Services/Receiving/WMS.Receiving.API/"]
COPY ["src/Services/Receiving/WMS.Receiving.Application/WMS.Receiving.Application.csproj", "src/Services/Receiving/WMS.Receiving.Application/"]
COPY ["src/Services/Receiving/WMS.Receiving.Domain/WMS.Receiving.Domain.csproj", "src/Services/Receiving/WMS.Receiving.Domain/"]
COPY ["src/Services/Receiving/WMS.Receiving.Infrastructure/WMS.Receiving.Infrastructure.csproj", "src/Services/Receiving/WMS.Receiving.Infrastructure/"]
COPY ["src/BuildingBlocks/WMS.SharedKernel/WMS.SharedKernel.csproj", "src/BuildingBlocks/WMS.SharedKernel/"]
COPY ["src/BuildingBlocks/WMS.EventBus/WMS.EventBus.csproj", "src/BuildingBlocks/WMS.EventBus/"]

RUN dotnet restore "src/Services/Receiving/WMS.Receiving.API/WMS.Receiving.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Receiving/WMS.Receiving.API"
RUN dotnet build "WMS.Receiving.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WMS.Receiving.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WMS.Receiving.API.dll"]