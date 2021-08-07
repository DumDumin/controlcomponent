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

cd ../..
dotnet sln add .\src\ControlComponent\ControlComponent.csproj
dotnet sln add .\tests\ControlComponent.Tests\ControlComponent.Tests.csproj
```