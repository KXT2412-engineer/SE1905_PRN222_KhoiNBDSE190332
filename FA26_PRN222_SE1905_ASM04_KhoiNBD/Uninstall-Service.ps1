#Requires -RunAsAdministrator
$ErrorActionPreference = 'Stop'
$serviceName = 'KhoiNBD_PRN222_ASM04'
$service = Get-Service -Name $serviceName -ErrorAction SilentlyContinue

if (-not $service) {
    Write-Host "Service $serviceName is not installed."
    return
}
if ($service.Status -ne 'Stopped') {
    Stop-Service -Name $serviceName
    $service.WaitForStatus('Stopped', [TimeSpan]::FromSeconds(20))
}

& sc.exe delete $serviceName
if ($LASTEXITCODE -ne 0) {
    throw 'Windows could not remove the service registration.'
}
Write-Host 'Service registration removed. Published files and text output are preserved.'
