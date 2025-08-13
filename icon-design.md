# 图标设计方案

## 图标概念

这个图标设计结合了音乐符号和剪切功能，采用现代渐变风格：

- **主色调**: 紫色渐变 (#667eea → #764ba2)
- **辅助元素**: 音乐符号、剪刀图标、音频波形
- **风格**: 现代扁平化设计，带有微妙的光影效果

## 文件列表

1. **icon.svg** - 主图标SVG源文件
2. **icon-256.png** - 256x256 PNG格式
3. **icon-128.png** - 128x128 PNG格式  
4. **icon-64.png** - 64x64 PNG格式
5. **icon-32.png** - 32x32 PNG格式
6. **app.ico** - Windows应用程序图标

## 使用说明

### 替换应用程序图标
1. 将 `app.ico` 设置为项目图标
2. 在 `.csproj` 文件中添加：
   ```xml
   <PropertyGroup>
     <ApplicationIcon>app.ico</ApplicationIcon>
   </PropertyGroup>
   ```

### 用于GitHub仓库
- `icon-256.png` 可用于GitHub仓库头像
- `icon.svg` 可用于README中的logo展示