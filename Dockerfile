# ----------- Stage 1: Base runtime image -----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

# ----------- Stage 2: Build -----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all project files for restore (good caching)
COPY ["BankRUs.Api/BankRUs.Api.csproj", "BankRUs.Api/"]
COPY ["BankRUs.Application/BankRUs.Application.csproj", "BankRUs.Application/"]
COPY ["BankRUs.Domain/BankRUs.Domain.csproj", "BankRUs.Domain/"]
COPY ["BankRUs.Intrastructure/BankRUs.Intrastructure.csproj", "BankRUs.Intrastructure/"]

# Restore NuGet packages
RUN dotnet restore "BankRUs.Api/BankRUs.Api.csproj"

# Copy everything else
COPY . .

# Build the project (for caching & error checking)
WORKDIR "/src/BankRUs.Api"
RUN dotnet build "BankRUs.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ----------- Stage 3: Publish -----------
FROM build AS publish
RUN dotnet publish "BankRUs.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ----------- Stage 4: Final runtime image -----------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankRUs.Api.dll"]