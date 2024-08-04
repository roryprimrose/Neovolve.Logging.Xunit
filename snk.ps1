$file = "Neovolve.Logging.Xunit.snk"

function Get-SnPath {
    $possiblePaths = @(
        "C:\Program Files\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\sn.exe",
        "C:\Program Files\Microsoft SDKs\Windows\v7.1\Bin\sn.exe",
        "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\sn.exe",
        "C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1\Bin\sn.exe"
    )
    
    foreach ($path in $possiblePaths) {
        if (Test-Path $path) {
            Write-Host "Found sn.exe at $path"

            return $path
        }
    }
    
    throw "sn.exe not found in common installation paths."
}

$snPath = Get-SnPath

if (-not (Test-Path $file)) {

    Write-Output "Creating  $file";
    & $snPath -k $file
}

$bytesFromFile = Get-Content $file -Raw -AsByteStream;
$encodedBytes = [System.Convert]::ToBase64String($bytesFromFile);
# Display base 64 string
Write-Output "File: $file converted to base64:";
Write-Output " ";
Write-Output $encodedBytes;
Write-Output " ";
# Compute and show hash of original file
$fileHashInfo = Get-FileHash $file;
Write-Output "Hash: $($fileHashInfo.Hash)";

$tempPath = [System.IO.Path]::GetTempFileName()
& $snPath -p $file $tempPath
& $snPath -tp $tempPath
Remove-Item $tempPath