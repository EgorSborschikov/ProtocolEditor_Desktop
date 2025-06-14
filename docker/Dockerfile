FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /ProtocolEditor

COPY ./*.sln ./
COPY ./ProtocolEditor/*.csproj ./ProtocolEditor
RUN dotnet restore

COPY . .
RUN dotnet publish "./ProtocolEditor/ProtocolEditor.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

COPY --from=build /app/publish .

RUN apt-get update && \
    apt-get install -y \
        libx11-dev \
        libice-dev \
        libsm-dev \
        libfontconfig1 \
        libxext6 \
        libgl1-mesa-glx \
        libc6-dev 

ENTRYPOINT ["dotnet", "ProtocolEditor.dll"]