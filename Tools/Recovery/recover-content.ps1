param(
    [string]$SourceResources = 'C:\Projects\Golden  Dragon Legend\reconstruction-nut\reference-typed\ExportedProject\Assets\Resources\levelconfig',
    [string]$EvidenceRoot = 'C:\Projects\Golden  Dragon Legend\reconstruction-nut\decoded-content'
)
$ErrorActionPreference = 'Stop'
New-Item -ItemType Directory -Path $EvidenceRoot -Force | Out-Null
$records = [System.Collections.Generic.List[object]]::new()
foreach ($resourceFile in Get-ChildItem -LiteralPath $SourceResources -Filter '*.txt' -Recurse | Sort-Object FullName) {
    $relative = [System.IO.Path]::GetRelativePath($SourceResources, $resourceFile.FullName).Replace('\','/')
    $cipherText = [System.IO.File]::ReadAllText($resourceFile.FullName).TrimStart([char]0xFEFF)
    $cipherBytes = [Convert]::FromBase64String($cipherText)
    $aes = [System.Security.Cryptography.Aes]::Create()
    try {
        # Original callers 0x9FBAE4, 0x9FBC04, 0x9FCE64 use the same UTF-8 key and IV.
        $aes.Key = [System.Text.Encoding]::UTF8.GetBytes('d2d6e6b5210738f3')
        $aes.IV = $aes.Key
        $aes.Mode = [System.Security.Cryptography.CipherMode]::CBC
        $aes.Padding = [System.Security.Cryptography.PaddingMode]::PKCS7
        $transform = $aes.CreateDecryptor()
        try { $decoded = $transform.TransformFinalBlock($cipherBytes,0,$cipherBytes.Length) }
        finally { $transform.Dispose() }
    } finally { $aes.Dispose() }
    $plain = [System.Text.Encoding]::UTF8.GetString($decoded)
    $document = $plain | ConvertFrom-Json
    $destination = Join-Path $EvidenceRoot ($relative + '.json')
    New-Item -ItemType Directory -Path ([System.IO.Path]::GetDirectoryName($destination)) -Force | Out-Null
    [System.IO.File]::WriteAllText($destination,$plain)
    $records.Add([ordered]@{
        resourcePath = 'LevelConfig/' + $relative.Substring(0,$relative.Length - 4)
        sha256 = [Convert]::ToHexString([System.Security.Cryptography.SHA256]::HashData($decoded)).ToLowerInvariant()
        isIndex = $null -ne $document.LevelDataInfos
        entryCount = if ($null -ne $document.LevelDataInfos) { $document.LevelDataInfos.Count } else { $document.B.Count }
        cellCount = if ($null -ne $document.B) { [int](($document.B | ForEach-Object { $_.C.Count } | Measure-Object -Sum).Sum) } else { 0 }
        filledCount = if ($null -ne $document.B) { [int](($document.B | ForEach-Object { $_.C } | Where-Object { $null -ne $_.BIM } | Measure-Object).Count) } else { 0 }
        levelId = if ($null -ne $document.LId) { [string]$document.LId } else { '' }
    })
}
$manifest = @{entries=$records.ToArray()}
$manifest | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath "$EvidenceRoot\content-manifest.json" -Encoding utf8
Write-Output "Decoded resources: $($records.Count); indexes: $(($records | Where-Object isIndex).Count); boards: $(($records | Where-Object { -not $_.isIndex }).Count)"
