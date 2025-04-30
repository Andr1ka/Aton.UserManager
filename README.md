create and appy migrations
dotnet ef migrations add InitialCreate --project Persistence --startup-project WebApi
dotnet ef database update --project Persistence --startup-project WebApi
