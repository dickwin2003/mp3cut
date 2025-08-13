@echo off
echo Creating simple icons for MP3Cut...

REM Create a simple icon using built-in tools
if exist icon-256.png del icon-256.png
if exist icon-128.png del icon-128.png
if exist icon-64.png del icon-64.png
if exist icon-32.png del icon-32.png
if exist app.ico del app.ico

REM Use PowerShell to create a simple icon
powershell -Command "
Add-Type -AssemblyName System.Drawing
$bitmap = New-Object System.Drawing.Bitmap(256, 256)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

REM Background gradient
$brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
    [System.Drawing.Rectangle]::new(0, 0, 256, 256),
    [System.Drawing.Color]::FromArgb(102, 126, 234),
    [System.Drawing.Color]::FromArgb(118, 75, 162),
    45.0
)
$graphics.FillEllipse($brush, 8, 8, 240, 240)

REM Music note
$musicNote = [System.Drawing.Drawing2D.GraphicsPath]::new()
$musicNote.AddLine(100, 80, 100, 180)
$musicNote.AddEllipse(85, 60, 30, 25)
$musicNote.AddEllipse(115, 60, 30, 25)
$musicNote.CloseFigure()

REM Scissors
$scissors = [System.Drawing.Drawing2D.GraphicsPath]::new()
$scissors.AddLine(150, 120, 200, 150)
$scissors.AddLine(150, 150, 200, 120)
$scissors.CloseFigure()

REM Draw elements
$graphics.FillPath([System.Drawing.Brushes]::White, $musicNote)
$graphics.DrawPath([System.Drawing.Pens]::White, $scissors)

REM Save as PNG
$bitmap.Save('icon-256.png', [System.Drawing.Imaging.ImageFormat]::Png)

REM Create smaller versions
$128 = New-Object System.Drawing.Bitmap(128, 128)
$g128 = [System.Drawing.Graphics]::FromImage($128)
$g128.DrawImage($bitmap, 0, 0, 128, 128)
$128.Save('icon-128.png', [System.Drawing.Imaging.ImageFormat]::Png)

$64 = New-Object System.Drawing.Bitmap(64, 64)
$g64 = [System.Drawing.Graphics]::FromImage($64)
$g64.DrawImage($bitmap, 0, 0, 64, 64)
$64.Save('icon-64.png', [System.Drawing.Imaging.ImageFormat]::Png)

$32 = New-Object System.Drawing.Bitmap(32, 32)
$g32 = [System.Drawing.Graphics]::FromImage($32)
$g32.DrawImage($bitmap, 0, 0, 32, 32)
$32.Save('icon-32.png', [System.Drawing.Imaging.ImageFormat]::Png)

REM Copy main icon
$bitmap.Save('mp3cut.png', [System.Drawing.Imaging.ImageFormat]::Png)

Write-Host 'Icons created successfully!'
"

echo.
echo Icon generation complete!
echo Files created:
dir icon-*.png 2>nul
dir mp3cut.png 2>nul
pause