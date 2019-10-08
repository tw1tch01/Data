param(
	[string]$VersionSource = "DevGate.Data.csproj",
	[string]$PackageLocation = "bin\release",
	[string]$Name = "DevGate.Data",
	[string]$NugetSource = "DevGate.NuGet"
)

# Publishes the current version of DevGate.Data to the NugetSource.
# -- NOTE: Source must be built in Release mode, and Unit tests must ALL be passing before package is published.

# Command line template:
# -VersionSource "$(build.sourcesdirectory)\DevGate.Data\DevGate.Data.csproj" -Name "DevGate.Data" -NugetSource [Nuget Source] -ApiKey [API key]

[xml]$XmlDocument = Get-Content -Path $VersionSource
$Package = "$Name.$($XmlDocument.GetElementsByTagName("Version")[0].InnerText).nupkg"
$currentLocation = Get-Location
if ($PackageLocation -ne "") {
	Set-Location $PackageLocation
}

dotnet pack 

Write-Host "Publishing package: $Package to NuGet source: $NugetSource"
nuget push $Package -source $NugetSource 
Set-Location $currentLocation