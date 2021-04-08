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
dotnet add package NLog --version 4.7.9
dotnet add package Moq --version 4.16.1


Changed TargetFramework to netstandard2.0 to enable unity3d support
https://docs.unity3d.com/2019.3/Documentation/Manual/dotnetProfileSupport.html
https://docs.microsoft.com/de-de/dotnet/standard/frameworks
https://docs.microsoft.com/de-de/dotnet/standard/net-standard


# TODO
[assembly:InternalsVisibleTo("ControlComponent.Tests")]

## Logs in Unit Tests by configure logger in code
https://github.com/NLog/NLog/wiki/Configure-from-code