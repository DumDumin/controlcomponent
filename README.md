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

# Packages
dotnet add package DotNet.NLog.NetCore --version 5.0.0
dotnet add package Moq --version 4.16.1



# TODO
[assembly:InternalsVisibleTo("ControlComponent.Tests")]

## Logs in Unit Tests by configure logger in code
https://github.com/NLog/NLog/wiki/Configure-from-code