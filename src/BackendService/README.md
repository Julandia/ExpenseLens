# Run Docker container for Expense Lens Service
- Create certificate as instructed in https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-8.0
docker run -p 7000:8080 -p 7001:8081 -e ASPNETCORE_URLS="https://+:8081;http://+:8080" --env-file ./env.list -v $env:USERPROFILE\.aspnet\https:/https/ expense-lens-service
