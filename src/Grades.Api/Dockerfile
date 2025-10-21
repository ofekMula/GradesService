FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish src/Grades.Api/Grades.Api.csproj -c Release -o /out /p:UseAppHost=false

FROM base
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "Grades.Api.dll"]
