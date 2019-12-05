

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["WePrint/WePrint.csproj", "WePrint/"]
RUN dotnet restore "WePrint/WePrint.csproj"
COPY . .
WORKDIR "/src/WePrint"
ARG app_version=1.0.0.0
ARG configuration=Debug
RUN dotnet build "WePrint.csproj" -c $configuration -o /app/build -p:Version=$app_version

FROM build AS publish
ARG app_version=1.0.0.0
ARG configuration=Debug
RUN dotnet publish "WePrint.csproj" -c $configuration -o /app/publish -p:Version=$app_version

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WePrint.dll"]