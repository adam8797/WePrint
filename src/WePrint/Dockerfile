

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["WePrint/WePrint.csproj", "WePrint/"]
RUN dotnet restore "WePrint/WePrint.csproj"
COPY . .
WORKDIR "/src/WePrint"
ARG app_version
RUN dotnet build "WePrint.csproj" -c Release -o /app/build -p:Version=$app_version

FROM build AS publish
ARG app_version
RUN dotnet publish "WePrint.csproj" -c Release -o /app/publish -p:Version=$app_version

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WePrint.dll"]