FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем только файл проекта
COPY SportCoursework.csproj .
RUN dotnet restore SportCoursework.csproj

# Копируем всё остальное
COPY . .
RUN dotnet publish SportCoursework.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
# Убедитесь, что здесь имя совпадает с проектом
ENTRYPOINT ["dotnet", "SportCoursework.dll"]