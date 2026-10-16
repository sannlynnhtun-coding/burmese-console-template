[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot 'Build.ps1')
[xml]$project = Get-Content (Join-Path $repo 'SLH.BurmeseConsole.Templates.csproj') -Raw
$version = $project.Project.PropertyGroup.PackageVersion
$package = Join-Path $repo "artifacts/packages/SLH.BurmeseConsole.Templates.$version.nupkg"
Push-Location $repo
try {
    $installed = dotnet new uninstall | Out-String
    if ($installed.Contains('SLH.BurmeseConsole.Templates')) {
        dotnet new uninstall SLH.BurmeseConsole.Templates
        if ($LASTEXITCODE -ne 0) { throw 'Could not replace the previous template package.' }
    }
    dotnet new install $package
    if ($LASTEXITCODE -ne 0) { throw 'Template installation failed.' }
    Write-Host 'Installed: Burmese Console App (Pyidaungsu). Reopen the Visual Studio New Project dialog.'
} finally { Pop-Location }
