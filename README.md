create and appy migrations
dotnet ef migrations add InitialCreate -s WebApi -p Persistence
dotnet ef database update --project WebApi --startup-project Persistanse
