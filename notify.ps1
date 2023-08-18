# from <https://cpoint-lab.co.jp/article/202304/23911/>

# 引数を受け取る
$notificationTitle = $args[0]
$notificationMessage = $args[1]
 
# NotifyIconオブジェクトを作成
Add-Type -AssemblyName System.Windows.Forms
$notification = New-Object System.Windows.Forms.NotifyIcon
 
# アイコンを設定
$notification.Icon = [System.Drawing.Icon]::ExtractAssociatedIcon((Get-Command powershell).Path)
 
# バルーン チップの内容を設定
$notification.BalloonTipTitle = $notificationTitle
$notification.BalloonTipText = $notificationMessage
$notification.BalloonTipIcon = [System.Windows.Forms.ToolTipIcon]::Info
 
# バルーン チップを表示して通知を行う
$notification.Visible = $true
# 5000ミリ秒表示
$notification.ShowBalloonTip(5000)