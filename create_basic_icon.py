#!/usr/bin/env python3
"""
Create a basic icon using text-based approach
"""

# Create a simple HTML file that can be used to generate an icon
html_content = """
<!DOCTYPE html>
<html>
<head>
    <title>MP3Cut Icon Generator</title>
    <style>
        body {
            margin: 0;
            padding: 20px;
            background: #f0f0f0;
            font-family: Arial, sans-serif;
        }
        .icon {
            width: 256px;
            height: 256px;
            border-radius: 50%;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 20px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.2);
        }
        .content {
            text-align: center;
            color: white;
        }
        .music {
            font-size: 120px;
            margin-bottom: 10px;
        }
        .text {
            font-size: 24px;
            font-weight: bold;
        }
        .instructions {
            margin: 20px;
            padding: 15px;
            background: white;
            border-radius: 8px;
            max-width: 400px;
        }
        button {
            padding: 10px 20px;
            background: #667eea;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            margin: 5px;
        }
        button:hover {
            background: #764ba2;
        }
    </style>
</head>
<body>
    <div style="display: flex; align-items: flex-start;">
        <div class="icon">
            <div class="content">
                <div class="music">🎵</div>
                <div class="text">MP3Cut</div>
            </div>
        </div>
        
        <div class="instructions">
            <h3>图标生成说明</h3>
            <p>1. 右键点击左侧图标</p>
            <p>2. 选择"复制图片"</p>
            <p>3. 粘贴到画图或其他图像编辑器</p>
            <p>4. 保存为PNG格式</p>
            
            <button onclick="downloadIcon()">下载图标</button>
            <button onclick="window.print()">打印图标</button>
        </div>
    </div>

    <canvas id="canvas" width="256" height="256" style="display: none;"></canvas>

    <script>
        function downloadIcon() {
            const canvas = document.getElementById('canvas');
            const ctx = canvas.getContext('2d');
            
            // Create gradient background
            const gradient = ctx.createLinearGradient(0, 0, 256, 256);
            gradient.addColorStop(0, '#667eea');
            gradient.addColorStop(1, '#764ba2');
            
            ctx.fillStyle = gradient;
            ctx.beginPath();
            ctx.arc(128, 128, 128, 0, 2 * Math.PI);
            ctx.fill();
            
            // Add music note emoji
            ctx.font = '120px Arial';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';
            ctx.fillStyle = 'white';
            ctx.fillText('🎵', 128, 110);
            
            // Add text
            ctx.font = 'bold 24px Arial';
            ctx.fillText('MP3Cut', 128, 180);
            
            // Download
            const link = document.createElement('a');
            link.download = 'mp3cut.png';
            link.href = canvas.toDataURL();
            link.click();
        }
    </script>
</body>
</html>
"""

with open('icon_generator.html', 'w') as f:
    f.write(html_content)

print("图标生成器已创建: icon_generator.html")
print("请在浏览器中打开此文件来生成图标")

# Also create a basic placeholder icon using text
with open('icon_placeholder.txt', 'w') as f:
    f.write("""
MP3Cut Icon Placeholder

由于环境限制，这里提供几种创建图标的方法：

方法1: 使用在线工具
- 访问 icon_generator.html 文件
- 按照页面说明操作
- 或使用在线图标生成器

方法2: 手动创建
- 创建 256x256 PNG 图像
- 背景：紫色渐变 (#667eea 到 #764ba2)
- 添加音乐符号 🎵
- 添加 "MP3Cut" 文字

方法3: 使用现成图标
- 从免费图标网站下载音乐相关图标
- 推荐使用: https://www.flaticon.com/
- 搜索 "music cut" 或 "audio scissors"

建议尺寸：
- 256x256 (主图标)
- 128x128 (中等图标)
- 64x64 (小图标)
- 32x32 (超小图标)
- 16x16 (最小图标)
""")

print("\n已创建以下文件：")
print("- icon_generator.html: 浏览器图标生成器")
print("- icon_placeholder.txt: 图标创建说明")