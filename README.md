# Control Component Library 
This library contains the projects ControlComponents.Core, ControlComponents.ML and ControlComponent.Frame.

- ControlComponents.Core provides basic functionality to create ControlComponents as defined in [BaSyx](https://wiki.eclipse.org/BaSyx_/_Documentation_/_API_/_ControlComponent#Service_Interface).  

- ControlComponents.ML provides basic functionality to extend ControlComponents with Reinforcement Learning specific interfaces.  

- ControlComponents.Frame provides the FrameControlComponent, which can be used to extract operation modes and use those in a different component. The relevant states and calls are synchronized between those two components.

This library supports netstandard2.0 to be used in Unity3D.

# Usage
Compile the project by running `dotnet build`
Use the resulting .dll files in your project to access control component feature of this library.  

By extending the OperationMode class and overriding the state specific methods, you can inject your own logic and encapsulate it in a BaSys ControlComponent. Create or extend the ControlComponent class and pass OperationModes and OrderOutputs to it.

You can find an example [here](https://git.rwth-aachen.de/tobias.rink/pts) in the PTS.ControlComponents project.

# Contribution
1. Prepare VSCode https://code.visualstudio.com/docs/languages/dotnet
2. Follow this [Project Structure](https://docs.microsoft.com/de-de/dotnet/core/porting/project-structure)
3. You can use the [dotnet cli](https://docs.microsoft.com/de-de/dotnet/core/tools/dotnet-new) to create new projects
4. Try to implement specific, small and tested features. Create a pull request.

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

install reportgenerator to generate a test report. `dotnet tool install -g dotnet-reportgenerator-globaltool`.
Run the following command (with the correct result id) to create a "coveragereport" folder with an index.html file to present the coverage results:
```
reportgenerator -reports:.\tests\ControlComponents.Core.Tests\TestResults\58238ace-ffed-4f17-8afa-d9a60792f957\coverage.cobertura.xml -targetdir:coveragereport -reporttypes:Html
```

Open the index.html file in your browser to access the results.

### Test dependency packages
1. Moq to provide Mock and Stub functionality
2. coverlet.collector to provide test coverage plugin
https://github.com/coverlet-coverage/coverlet/blob/master/README.md
https://docs.microsoft.com/de-de/dotnet/core/testing/unit-testing-code-coverage?tabs=windows

### Logs in Unit Tests by configure logger in code
https://github.com/NLog/NLog/wiki/Configure-from-code


