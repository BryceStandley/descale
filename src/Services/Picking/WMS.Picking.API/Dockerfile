FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Picking/WMS.Picking.API/WMS.Picking.API.csproj", "src/Services/Picking/WMS.Picking.API/"]
COPY ["src/Services/Picking/WMS.Picking.Application/WMS.Picking.Application.csproj", "src/Services/Picking/WMS.Picking.Application/"]
COPY ["src/Services/Picking/WMS.Picking.Domain/WMS.Picking.Domain.csproj", "src/Services/Picking/WMS.Picking.Domain/"]
COPY ["src/Services/Picking/WMS.Picking.Infrastructure/WMS.Picking.Infrastructure.csproj", "src/Services/Picking/WMS.Picking.Infrastructure/"]
COPY ["src/BuildingBlocks/WMS.SharedKernel/WMS.SharedKernel.csproj", "src/BuildingBlocks/WMS.SharedKernel/"]
COPY ["src/BuildingBlocks/WMS.EventBus/WMS.EventBus.csproj", "src/BuildingBlocks/WMS.EventBus/"]

RUN dotnet restore "src/Services/Picking/WMS.Picking.API/WMS.Picking.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Picking/WMS.Picking.API"
RUN dotnet build "WMS.Picking.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WMS.Picking.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WMS.Picking.API.dll"]