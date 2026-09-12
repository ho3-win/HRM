# HRM

dotnet tool install --global dotnet-ef
export PATH="$PATH:$HOME/.dotnet/tools"
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet ef migrations list
