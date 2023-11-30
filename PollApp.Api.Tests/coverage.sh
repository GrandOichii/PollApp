RESULT_PATH="$(dotnet test --collect:"XPlat Code Coverage" | grep 'coverage.cobertura.xml')"
# reportgenerator doens't like the RESULT_PATH as a report for some reason, copy coverage file contents to a different file
cat $RESULT_PATH > coverage.xml
reportgenerator -reports:coverage.xml -targetdir:"coveragereport" -reporttypes:Html
start "coveragereport\index.html"