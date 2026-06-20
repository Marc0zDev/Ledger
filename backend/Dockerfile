# ── Build stage ────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Ledger.Domain/Ledger.Domain.csproj Ledger.Domain/
COPY Ledger.Application/Ledger.Application.csproj Ledger.Application/
COPY Ledger.Infrastructure/Ledger.Infrastructure.csproj Ledger.Infrastructure/
COPY Ledger.Api/Ledger.Api.csproj Ledger.Api/
RUN dotnet restore Ledger.Api/Ledger.Api.csproj

COPY . .
RUN dotnet publish Ledger.Api/Ledger.Api.csproj -c Release -o /app/publish --no-restore

# ── Runtime stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Chromium for PuppeteerSharp PDF generation
RUN apt-get update && apt-get install -y --no-install-recommends \
    chromium \
    fonts-liberation \
    libgbm1 \
    libasound2 \
    libatk-bridge2.0-0 \
    libgtk-3-0 \
    libnss3 \
    libx11-xcb1 \
    libxcomposite1 \
    libxrandr2 \
    xdg-utils \
  && rm -rf /var/lib/apt/lists/*

ENV PUPPETEER_EXECUTABLE_PATH=/usr/bin/chromium

WORKDIR /app
COPY --from=build /app/publish .

RUN mkdir -p wwwroot/boletos

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Ledger.Api.dll"]
