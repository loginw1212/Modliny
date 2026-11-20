$ErrorActionPreference = "Stop"
try {
  Add-Type -AssemblyName System.IO.Compression
  if ($MyInvocation.MyCommand.Path) {
    $root = Split-Path -Parent $MyInvocation.MyCommand.Path
  } else {
    $root = (Get-Location).Path
  }
  if (-not (Test-Path "$root\ModlinAirport.rbxmx.gz.b64")) {
    throw "Nie ma pliku ModlinAirport.rbxmx.gz.b64 w tym folderze: $root"
  }
  $b64 = [IO.File]::ReadAllText("$root\ModlinAirport.rbxmx.gz.b64")
  $in = New-Object IO.MemoryStream (,[Convert]::FromBase64String($b64))
  $out = New-Object IO.MemoryStream
  $gz = New-Object IO.Compression.GzipStream($in, [IO.Compression.CompressionMode]::Decompress)
  $gz.CopyTo($out)
  $gz.Dispose()
  [IO.File]::WriteAllBytes("$root\ModlinAirport.rbxmx", $out.ToArray())
  Write-Host "GOTOWE! Powstal plik ModlinAirport.rbxmx - zaimportuj go w Roblox Studio." -ForegroundColor Green
} catch {
  Write-Host "BLAD: $_" -ForegroundColor Red
}
