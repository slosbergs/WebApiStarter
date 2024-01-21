FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /source
COPY WebApiStarter/WebApiStarter.csproj WebApiStarter/WebApiStarter.csproj 
RUN dotnet restore WebApiStarter/WebApiStarter.csproj
COPY . .
RUN dotnet build WebApiStarter -c Release --no-restore

FROM build as test
RUN dotnet test WebApiStarterTests -c Release --logger html --collect:"XPlat Code Coverage"; exit 0
RUN dotnet tool install -g dotnet-reportgenerator-globaltool
RUN COVERAGEFILE=$(find WebApiStarterTests/TestResults -name "coverage.cobertura.xml" -type f) && \
		/root/.dotnet/tools/reportgenerator \
			-reports:"$COVERAGEFILE" \
			-targetdir:WebApiStarterTests/TestResults/CoverageReport \
			-reporttypes:Html

FROM scratch as export-test-results
#COPY --from=test /source/WebApiStarterTests/TestResults/CoverageReport ./CoverageReport
#COPY --from=test /source/WebApiStarterTests/TestResults/*.html .
COPY --from=test /source/WebApiStarterTests/TestResults .

FROM build AS publish
RUN dotnet publish WebApiStarter -c Release --no-build -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebApiStarter.dll"]