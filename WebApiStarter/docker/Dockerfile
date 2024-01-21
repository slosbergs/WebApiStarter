FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /source
COPY WebApiStarter/WebApiStarter.csproj ./
RUN dotnet restore WebApiStarter/WebApiStarter.csproj
COPY WebApiStarter ./
RUN dotnet build WebApiStarter -c Release --no-restore

FROM build as test
RUN dotnet test WebApiStarterTests -c Release --no-build --logger html --collect:"XPlat Code Coverage"
RUN dotnet tool install dotnet-reportgenerator-globaltool
RUN reportgenerator \
	-reports:"WebApiStarterTests/TestResults/**/coverage.cobertura.xml" \
	-targetdir:"WebApiStarterTests/TestResults/CoverageReport" \
	-reporttypes:Html

FROM scratch as export-test-results
COPY --from=test /source/TestResults/CoverageReport .
COPY --from=test /source/TestResults/*.html .

FROM build AS publish
RUN dotnet publish WebApiStarter -c Release --no-build -o /app

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebApiStarter.dll"]