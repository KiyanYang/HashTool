# KiyanYang

param(
    [string]$Url = $(throw "Parameter missing: -Name") ,
    [string]$SHA256 = $(throw "Parameter missing: -SHA256"),
    [string]$TargetPath = $(throw "Parameter missing: -TargetPath"),
    [switch]$Force = $false
)

# 是否更新
if (!$Force) {
    $canUpdate = Read-Host "是否开始更新 [输入 Yes(Y) 继续，输入其他内容则退出]"
    if ($canUpdate -ne "Y" -and $canUpdate -ne "Yes") {
        Write-Host "更新已终止。`n将在 2 秒后自动关闭。"
        Start-Sleep -s 2
        exit
    }
}

# 下载文件临时储存路径
$path = ".\download-$([guid]::NewGuid()).zip"

# 下载文件
Write-Host "即将开始下载文件……"
Start-BitsTransfer -Source $Url -Destination $path
Write-Host "下载完成。"

# 哈希校验
$fileHash = Get-FileHash $path -Algorithm SHA256
if ($fileHash.Hash -eq $SHA256) {
    Write-Host "哈希校验成功。`n开始更新。"
}
else {
    Write-Host "哈希校验失败，SHA256 校验值不相等。`n将在 5 秒后自动关闭窗口。"
    Start-Sleep -s 5
    exit
}

# 解压
Expand-Archive -Path $path -DestinationPath $TargetPath -Force

# 删除下载文件
Remove-Item -Path $path

# 完成并退出
Write-Host "更新已完成。`n将在 5 秒后自动关闭窗口。"
Start-Sleep -s 5
