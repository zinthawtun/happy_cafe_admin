FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["backend/Api/Api.csproj", "backend/Api/"]
COPY ["backend/Business/Business.csproj", "backend/Business/"]
COPY ["backend/DataAccess/DataAccess.csproj", "backend/DataAccess/"]
COPY ["backend/Database/Database.csproj", "backend/Database/"]
COPY ["backend/Infrastructure/Infrastructure.csproj", "backend/Infrastructure/"]
COPY ["backend/Service/Service.csproj", "backend/Service/"]
COPY ["backend/Utilities/Utilities.csproj", "backend/Utilities/"]
RUN dotnet restore "backend/Api/Api.csproj"
COPY . .
WORKDIR "/src/backend/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"] 