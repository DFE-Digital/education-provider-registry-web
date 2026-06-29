# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
COPY ["DfE.EducationProviderRegistry.Web.Mvc/DfE.EducationProviderRegistry.Web.Mvc.csproj", "DfE.EducationProviderRegistry.Web.Mvc/"]
RUN --mount=type=secret,id=nuget_config dotnet restore "DfE.EducationProviderRegistry.Web.Mvc/DfE.EducationProviderRegistry.Web.Mvc.csproj" --configfile /run/secrets/nuget_config

COPY . .
RUN dotnet build "DfE.EducationProviderRegistry.Web.Mvc/DfE.EducationProviderRegistry.Web.Mvc.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DfE.EducationProviderRegistry.Web.Mvc/DfE.EducationProviderRegistry.Web.Mvc.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DfE.EducationProviderRegistry.Web.Mvc.dll"]