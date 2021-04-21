# ControlComponent

# Usage
1. If you override a operation mode state method (starting, suspending, etc...), you must consider the following:
-   Leaving the method without calling any state change or calling the base method (which changes the state or waits until external state change) will result in a direct recall of that method, which will lead to a performance issue or even crash the application.
=> Best practives: while loop with Delay and token to cancel OR one time run with state change at the end

# Contribution
1. Prepare VSCode https://code.visualstudio.com/docs/languages/dotnet

# Used setup commands
[Project Structure](https://docs.microsoft.com/de-de/dotnet/core/porting/project-structure)  
[dotnet cli](https://docs.microsoft.com/de-de/dotnet/core/tools/dotnet-new)
```
dotnet new sln

mkdir src/ControlComponent
cd src/ControlComponent
dotnet new classlib

cd ../..
mkdir tests/ControlComponent.Tests
dotnet new nunit
dotnet add reference ..\..\src\ControlComponent\
dotnet add package NLog --version 4.7.9
dotnet add package Moq --version 4.16.1
dotnet add package coverlet.collector --version 3.0.3
dotnet tool install -g dotnet-reportgenerator-globaltool

cd ../..
dotnet sln add .\src\ControlComponent\ControlComponent.csproj
dotnet sln add .\tests\ControlComponent.Tests\ControlComponent.Tests.csproj
```

# Testing
This project uses [NUnit](https://nunit.org/).

## Usage
Run the following command to execute tests ([dotnet test reference](https://docs.microsoft.com/de-de/dotnet/core/tools/dotnet-test)).
```
dotnet test
```

Using the following command creates a TestResult folder with the coverage results ([coverage reference](https://docs.microsoft.com/de-de/dotnet/core/testing/unit-testing-code-coverage?tabs=windows#integrate-with-net-test)):  
```
dotnet test --collect:"XPlat Code Coverage"
```

Run the following command (with the correct result id) to create a "coveragereport" folder with an index.html file to present the coverage results:
```
reportgenerator -reports:.\tests\ControlComponent.Tests\TestResults\efa01ed4-c5e1-4bf4-bda1-d18ec296017c\coverage.cobertura.xml -targetdir:coveragereport -reporttypes:Html
```

Open the index.html file in your browser to access the results.

## Test dependency packages
1. Moq to provide Mock and Stub functionality
2. coverlet.collector to provide test coverage plugin
https://github.com/coverlet-coverage/coverlet/blob/master/README.md
https://docs.microsoft.com/de-de/dotnet/core/testing/unit-testing-code-coverage?tabs=windows

## Writing Tests
[assembly:InternalsVisibleTo("ControlComponent.Tests")]

## Logs in Unit Tests by configure logger in code
https://github.com/NLog/NLog/wiki/Configure-from-code

# Target Framework
Changed TargetFramework to netstandard2.0 to enable unity3d support
https://docs.unity3d.com/2019.3/Documentation/Manual/dotnetProfileSupport.html
https://docs.microsoft.com/de-de/dotnet/standard/frameworks
https://docs.microsoft.com/de-de/dotnet/standard/net-standard


