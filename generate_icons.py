#!/usr/bin/env python3
"""
Generate PNG and ICO icons from SVG
"""

import os
import sys

try:
    from PIL import Image
    import cairosvg
except ImportError:
    print("Installing required packages...")
    os.system("pip install Pillow cairosvg")
    from PIL import Image
    import cairosvg

def generate_icons():
    """Generate PNG and ICO files from SVG"""
    
    svg_file = "icon.svg"
    if not os.path.exists(svg_file):
        print(f"Error: {svg_file} not found!")
        return False
    
    sizes = [16, 32, 48, 64, 128, 256]
    png_files = []
    
    print("Generating PNG icons...")
    for size in sizes:
        png_file = f"icon-{size}.png"
        try:
            cairosvg.svg2png(
                url=svg_file,
                write_to=png_file,
                output_width=size,
                output_height=size
            )
            png_files.append(png_file)
            print(f"✓ Generated {png_file}")
        except Exception as e:
            print(f"Error generating {png_file}: {e}")
            return False
    
    print("Generating ICO file...")
    try:
        # Create ICO file with multiple sizes
        images = []
        for png_file in png_files:
            if os.path.exists(png_file):
                img = Image.open(png_file)
                images.append(img)
        
        if images:
            images[0].save('app.ico', format='ICO', sizes=[(size, size) for size in sizes])
            print("✓ Generated app.ico")
            
            # Copy the largest PNG as main icon
            largest_png = "icon-256.png"
            if os.path.exists(largest_png):
                # Copy to mp3cut.png for README
                Image.open(largest_png).save("mp3cut.png")
                print("✓ Updated mp3cut.png")
        
    except Exception as e:
        print(f"Error generating ICO: {e}")
        return False
    
    print("\nIcon generation complete!")
    print("Files created:")
    for png_file in png_files:
        if os.path.exists(png_file):
            print(f"  - {png_file}")
    if os.path.exists('app.ico'):
        print("  - app.ico")
    if os.path.exists('mp3cut.png'):
        print("  - mp3cut.png")
    
    return True

if __name__ == "__main__":
    generate_icons()