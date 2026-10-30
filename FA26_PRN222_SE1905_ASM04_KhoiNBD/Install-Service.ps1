#Requires -RunAsAdministrator
$ErrorActionPreference = 'Stop'
$serviceName = 'KhoiNBD_PRN222_ASM04'
$executable = Join-Path $PSScriptRoot 'publish\zSubscription.WorkerService.KhoiNBD.exe'

if (-not (Test-Path -LiteralPath $executable)) {
    throw 'Publish the Worker project into the publish folder before installing the service.'
}
if (Get-Service -Name $serviceName -ErrorAction SilentlyContinue) {
    throw "Service $serviceName already exists. This script will not overwrite it."
}

if (-not [System.Diagnostics.EventLog]::SourceExists($serviceName)) {
    New-EventLog -LogName Application -Source $serviceName
}

New-Service -Name $serviceName -DisplayName $serviceName `
    -BinaryPathName ('"' + $executable + '"') -StartupType Manual `
    -Description 'PRN222 ASM04 KhoiNBD - append Membership Fee snapshots every 5 seconds.' | Out-Null

Start-Service -Name $serviceName
$service = Get-Service -Name $serviceName
$service.WaitForStatus('Running', [TimeSpan]::FromSeconds(20))
Get-Service -Name $serviceName | Select-Object Name, Status, StartType
Write-Host ('Text file: ' + (Join-Path $PSScriptRoot 'publish\Logs\KhoiNBD_SE1905_WS.txt'))
