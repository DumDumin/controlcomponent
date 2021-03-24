# ControlComponent
https://docs.microsoft.com/de-de/dotnet/core/porting/project-structure

# Contribution
1. Prepare VSCode https://code.visualstudio.com/docs/languages/dotnet

# Used Commands
https://docs.microsoft.com/de-de/dotnet/core/tools/dotnet-new
dotnet new sln

dotnet new classlib --name ControlComponent
dotnet new nunit --name ControlComponent.Tests

dotnet sln add .\src\ControlComponent\ControlComponent.csproj
dotnet sln add .\tests\ControlComponent.Tests\ControlComponent.Tests.csproj

dotnet add reference ..\..\src\ControlComponent\