Add-Type -AssemblyName System.IO.Compression
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$in = [IO.MemoryStream]::new([Convert]::FromBase64String([IO.File]::ReadAllText("$root\ModlinAirport.rbxmx.gz.b64")))
$out = [IO.MemoryStream]::new()
$gz = [IO.Compression.GzipStream]::new($in, [IO.Compression.CompressionMode]::Decompress)
$gz.CopyTo($out)
$gz.Dispose()
[IO.File]::WriteAllBytes("$root\ModlinAirport.rbxmx", $out.ToArray())
Write-Host "Gotowe! Powstal plik ModlinAirport.rbxmx - zaimportuj go w Roblox Studio."
