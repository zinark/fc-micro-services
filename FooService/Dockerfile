﻿FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FooService/FooService.csproj", "FooService/"]
RUN dotnet restore "FooService/FooService.csproj"
COPY . .
WORKDIR "/src/FooService"
RUN dotnet build "FooService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FooService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FooService.dll"]
