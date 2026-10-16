[CmdletBinding()]
param([string]$OutputDirectory)
$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent $PSScriptRoot
if (!$OutputDirectory) { $OutputDirectory = Join-Path $repo 'artifacts/packages' }
Push-Location $repo
try {
    dotnet pack SLH.BurmeseConsole.Templates.csproj -c Release -o $OutputDirectory --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Template package build failed.' }
} finally { Pop-Location }
