FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY StackOverflow.Web.slnx .
COPY BuildingBlocks.MediatR/BuildingBlocks.MediatR.csproj BuildingBlocks.MediatR/
COPY StackOverflow.Domain/StackOverflow.Domain.csproj StackOverflow.Domain/
COPY StackOverflow.Application/StackOverflow.Application.csproj StackOverflow.Application/
COPY StackOverflow.Infrastructure/StackOverflow.Infrastructure.csproj StackOverflow.Infrastructure/
COPY StackOverflow.WebAPI/StackOverflow.WebAPI.csproj StackOverflow.WebAPI/
COPY StackOverflow.UnitTests/StackOverflow.UnitTests.csproj StackOverflow.UnitTests/
COPY StackOverflow.IntegrationTests/StackOverflow.IntegrationTests.csproj StackOverflow.IntegrationTests/
RUN dotnet restore StackOverflow.Web.slnx

COPY . .
RUN dotnet publish StackOverflow.WebAPI/StackOverflow.WebAPI.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 5150
ENV ASPNETCORE_URLS=http://+:5150
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StackOverflow.WebAPI.dll"]
