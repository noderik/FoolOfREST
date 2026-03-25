FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
ENV PYTHONUNBUFFERED=1
RUN apt-get update \
    && apt-get install -y --no-install-recommends python3 python3-venv \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
COPY TelegramLogger/ ./TelegramLogger/
RUN python3 -m venv TelegramLogger/.venv \
    && TelegramLogger/.venv/bin/pip install --no-cache-dir -r TelegramLogger/requirements.txt
EXPOSE 5001
ENTRYPOINT ["dotnet", "FoolOfRESTAPI.dll"]
