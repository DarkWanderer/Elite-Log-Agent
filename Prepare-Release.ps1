param (
    [Parameter(Mandatory=$true)][string]$versionString
)

$version = [version]$versionString
$appveyorYaml = Convert-Path ".\appveyor.yml"

if ($version.Major -eq -1) { throw "Major version must be provided"; }
if ($version.Major -eq -1) { throw "Minor version must be provided"; }
if ($version.Build -eq -1) { throw "Patch version must be provided"; }
    
$versionString = "{0}.{1}.{2}" -f $version.Major, $version.Minor, $version.Build

Write-Host "Preparing release $version"
Write-Host $appveyorYaml

if(![System.IO.File]::Exists($appveyorYaml)) { throw "appveyor.yml not found"; }

git checkout master
git reset

(Get-Content $appveyorYaml) `
    -replace "version: [\.0-9]+", "version: $versionString." |
   Out-File $appveyorYaml

git add $appveyorYaml