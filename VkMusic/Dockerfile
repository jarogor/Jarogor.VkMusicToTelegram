FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["VkMusic/VkMusic.csproj", "VkMusic/"]
RUN dotnet restore "VkMusic/VkMusic.csproj"
COPY . .
WORKDIR "/src/VkMusic"
RUN dotnet build "VkMusic.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VkMusic.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VkMusic.dll"]
