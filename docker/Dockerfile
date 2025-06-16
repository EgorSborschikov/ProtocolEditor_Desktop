# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /ProtocolEditor

# Копирование csproj и восстановление зависимостей
COPY *.csproj ./
RUN dotnet restore

# Копирование остальных файлов и сборка приложения
COPY . ./
RUN dotnet publish -c Debug -o out

# Этап времени выполнения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /ProtocolEditor

# Копирование собранного приложения из этапа сборки
COPY --from=build /ProtocolEditor/out .

# Запуск приложения
ENTRYPOINT ["dotnet", "ProtocolEditor.dll"]
