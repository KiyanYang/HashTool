# https://github.com/KiyanYang

param(
    [Parameter(Mandatory, HelpMessage = "输入更新文件（.zip 格式）的下载链接。")]
    [ValidatePattern("^(https?|ftp)://(-\.)?([^\s/?\.#-]+\.?)+(/[^\s]*)?$")]
    [string]$Url,
    [Parameter(Mandatory, HelpMessage = '输入下载文件的 SHA256 校验值。')]
    [ValidatePattern('^[A-Za-z0-9]{64}$')]
    [string]$SHA256,
    [Parameter(Mandatory, HelpMessage = '输入下载文件解压的完整目标路径，请以盘符开始、以 \ 或 / 结尾。')]
    [ValidatePattern('^\w+:[\\/].*[\\/]$')]
    [string]$TargetPath,
    [switch]$Force
)

function Write-WithDate {
    param (
        $Object
    )
    Get-Date -Format "yyyy/MM/dd HH:mm:ss : " | Write-Host  -NoNewline
    Write-Host $Object
}

function Start-SleepWithExit {
    param (
        [int]$Seconds,
        [string]$Text
    )
    Write-WithDate "$Text，窗口将在 $Seconds 秒后自动关闭。"
    Start-Sleep -Seconds $Seconds
    Exit
}

# 是否更新
if (!$Force) {
    $canUpdate = Read-Host "是否继续更新任务 [输入 Yes(Y) 继续，输入其他内容则退出]"
    if ($canUpdate -ne "Y" -and $canUpdate -ne "Yes") {
        Start-SleepWithExit 3 "任务已终止"
    }
}

# 下载文件临时储存路径
$path = ".\download-$([guid]::NewGuid()).zip"

# 下载文件
Write-WithDate "即将开始下载文件……"
Start-BitsTransfer -Source $Url -Destination $path
Write-WithDate "下载完成。"

# 哈希校验
Write-WithDate "开始哈希校验。"
$fileHash = Get-FileHash $path -Algorithm SHA256
if ($fileHash.Hash -eq $SHA256) {
    Write-WithDate "哈希校验成功。"
}
else {
    Write-WithDate "哈希校验失败，SHA256 校验值不相等。"
    Start-SleepWithExit 3 "任务未完成"
}

# 解压更新
Write-WithDate "开始更新。"
Expand-Archive -Path $path -DestinationPath $TargetPath -Force
Write-WithDate "更新已完成。"

# 删除下载文件
Write-WithDate "删除下载文件。"
Remove-Item -Path $path
Write-WithDate "删除成功。"

# 完成并退出
Start-SleepWithExit 3 "任务已完成"
