try {
  Add-Type -AssemblyName System.IO.Compression
  $root = Split-Path -Parent $MyInvocation.MyCommand.Path
  $in = New-Object IO.MemoryStream (,[Convert]::FromBase64String([IO.File]::ReadAllText("$root\ModlinAirport.rbxmx.gz.b64")))
  $out = New-Object IO.MemoryStream
  $gz = New-Object IO.Compression.GzipStream($in, [IO.Compression.CompressionMode]::Decompress)
  $gz.CopyTo($out)
  $gz.Dispose()
  [IO.File]::WriteAllBytes("$root\ModlinAirport.rbxmx", $out.ToArray())
  Write-Host "GOTOWE! Powstal plik ModlinAirport.rbxmx - zaimportuj go w Roblox Studio." -ForegroundColor Green
} catch {
  Write-Host "BLAD: $_" -ForegroundColor Red
  Write-Host "Jesli widzisz, ze skrypty sa zablokowane, otworz PowerShell i wpisz:"
  Write-Host ' powershell -ExecutionPolicy Bypass -File Odtworz_Model.ps1' -ForegroundColor Yellow
}
Read-Host "Nacisnij Enter, aby zamknac okno"
