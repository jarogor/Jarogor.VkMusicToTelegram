FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["VkMusicToTelegram/VkMusicToTelegram.csproj", "VkMusicToTelegram/"]
RUN dotnet restore "VkMusicToTelegram/VkMusicToTelegram.csproj"
COPY . .
WORKDIR "/src/VkMusicToTelegram"
RUN dotnet build "VkMusicToTelegram.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VkMusicToTelegram.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VkMusicToTelegram.dll"]
