[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
dotnet new uninstall SLH.BurmeseConsole.Templates
if ($LASTEXITCODE -ne 0) { throw 'Template uninstall failed.' }
