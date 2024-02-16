dotnet tool update dotnet-reportgenerator-globaltool --tool-path ../tools --version 5.2.1

dotnet test ./tests/PlanningPoker/UnitTests /p:CollectCoverage=true /p:CoverletOutput=test-reports/coverage-info/ /p:CoverletOutputFormat=cobertura --no-build --verbosity normal --logger trx --results-directory "test-reports"

"../tools/reportgenerator.exe" -reports:./tests/PlanningPoker/UnitTests/test-reports/coverage-info/coverage.cobertura.xml -targetdir:./test-reports/coverage-report