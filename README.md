# Control Component Library 

# Usage
Compile the project by running `dotnet build`
Use the resulting .dll files in your project to access control component feature of this library

## ControlComponent

## OperationMode
Inheriting the OperationModeBase class allows you to override the execution state method to inject your process logic.

# Contribution
1. Prepare VSCode https://code.visualstudio.com/docs/languages/dotnet

## Used setup commands
[Project Structure](https://docs.microsoft.com/de-de/dotnet/core/porting/project-structure)  
[dotnet cli](https://docs.microsoft.com/de-de/dotnet/core/tools/dotnet-new)


## Testing
This project uses [NUnit](https://nunit.org/).

### Usage
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
reportgenerator -reports:.\tests\ControlComponent.Core.Tests\TestResults\efa01ed4-c5e1-4bf4-bda1-d18ec296017c\coverage.cobertura.xml -targetdir:coveragereport -reporttypes:Html
```

Open the index.html file in your browser to access the results.

### Test dependency packages
1. Moq to provide Mock and Stub functionality
2. coverlet.collector to provide test coverage plugin
https://github.com/coverlet-coverage/coverlet/blob/master/README.md
https://docs.microsoft.com/de-de/dotnet/core/testing/unit-testing-code-coverage?tabs=windows

### Writing Tests
[assembly:InternalsVisibleTo("ControlComponent.Tests")]

### Logs in Unit Tests by configure logger in code
https://github.com/NLog/NLog/wiki/Configure-from-code

## Target Framework
Changed TargetFramework to netstandard2.0 to enable unity3d support
https://docs.unity3d.com/2019.3/Documentation/Manual/dotnetProfileSupport.html
https://docs.microsoft.com/de-de/dotnet/standard/frameworks
https://docs.microsoft.com/de-de/dotnet/standard/net-standard


