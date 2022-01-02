[xml]$projectFile = Get-Content Nate\Nate.csproj
$versionSuffix = $projectFile.Project.PropertyGroup.Version

pushd .\Nate
Write-Output "Building release $versionSuffix nuget packages..."
dotnet pack --configuration Release --include-symbols --version-suffix $versionSuffix
Write-Output "Moving $versionSuffix nuget packages to releases folder..."
If(!(test-path ..\releases))
{
      New-Item -ItemType Directory -Force -Path ..\..\releases
}
Move-Item .\bin\Release\*.nupkg ..\..\releases -Force
Write-Output "Done."
popd