$file = "Neovolve.Logging.Xunit.snk"
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