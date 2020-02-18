# Data

This library can be used as an enhancement to a project's Data Access layer

## Setup

This library is implemented in ASP.NET Core 3.1. Therefore, ensure that you have the [latest version](https://dotnet.microsoft.com/download/dotnet-core) of ASP.NET Core installed

###### NuGet
Setup a local NuGet source on your PC/Workstation. To do so, perform the following steps:

In Visual Studios 2019,
1. Open `Tools > NuGet Package Manager > Package Manager Settings`
2. From the side menu, select `NuGet Package Manager > Package Sources`
3. Click add (the big green `+` symbol, top right)
4. Enter a name for the source (preferably your computer name)
5. For the source, point to a local folder (e.g. `D:\NuGet`)

## Commands

To use this library in any of your applications, you need to build and publish a nuget package file.
Once you are ready to export the source, run the following command,

> dotnet pack -c Release -o [your local nuget source folder]

Example,
> dotnet pack -c Release -o D:\NuGet

This command will build a nuget package built in Release configuration and output it directly to your local nuget source.