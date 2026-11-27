#!/usr/bin/env python3
"""Export Alignator branding assets from the editable SVG master.

Requires Inkscape and Pillow. Run this script from any working directory.
"""

from __future__ import annotations

import argparse
import shutil
import subprocess
from pathlib import Path
from xml.etree import ElementTree as ET

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "source" / "alignator.svg"
SVG_NAMESPACE = "http://www.w3.org/2000/svg"
PNG_SIZES = (16, 32, 40, 48, 64, 128, 180, 192, 256, 512, 1024)
ET.register_namespace("", SVG_NAMESPACE)


def render_png(inkscape: str, size: int) -> Path:
    """Render directly from the master SVG at the requested resolution."""
    destination = ROOT / "png" / f"alignator-{size}.png"
    destination.parent.mkdir(parents=True, exist_ok=True)
    result = subprocess.run(
        [inkscape, str(SOURCE), "--export-type=png",
         f"--export-filename={destination}", f"--export-width={size}",
         f"--export-height={size}", "--export-background-opacity=0"],
        capture_output=True, text=True,
    )
    if result.returncode:
        raise RuntimeError(result.stderr or result.stdout or "PNG export failed.")
    with Image.open(destination) as image:
        if image.size != (size, size) or image.mode != "RGBA":
            raise RuntimeError(f"Unexpected image size or mode: {destination}")
    return destination


def export_ico(destination: Path, sizes: tuple[int, ...]) -> None:
    """Package exact-size PNG exports as individual ICO entries."""
    destination.parent.mkdir(parents=True, exist_ok=True)
    images = []
    for size in sizes:
        with Image.open(ROOT / "png" / f"alignator-{size}.png") as image:
            images.append(image.convert("RGBA"))
    images[-1].save(
        destination, format="ICO", sizes=[(size, size) for size in sizes],
        append_images=images[:-1],
    )


def export_favicons() -> None:
    destination = ROOT / "favicon"
    destination.mkdir(parents=True, exist_ok=True)
    shutil.copyfile(SOURCE, destination / "favicon.svg")
    export_ico(destination / "favicon.ico", (16, 32, 48))
    for size in (16, 32, 48):
        shutil.copyfile(
            ROOT / "png" / f"alignator-{size}.png",
            destination / f"favicon-{size}.png",
        )


def export_rider_icon() -> None:
    """Create the 40-pixel plugin icon with transparent padding."""
    root = ET.fromstring(SOURCE.read_bytes())
    for child in list(root):
        if child.tag.rsplit("}", 1)[-1] in ("title", "desc"):
            root.remove(child)
    for element in root.iter():
        for attribute in ("id", "role", "aria-labelledby"):
            element.attrib.pop(attribute, None)
    root.set("width", "40")
    root.set("height", "40")
    root.set("viewBox", "-20 -20 640 640")
    ET.indent(root, space="")
    destination = ROOT / "rider" / "META-INF" / "pluginIcon.svg"
    destination.parent.mkdir(parents=True, exist_ok=True)
    destination.write_bytes(ET.tostring(root, encoding="utf-8") + b"\n")


def preview_font(size: int, bold: bool = False) -> ImageFont.FreeTypeFont:
    """Choose a local font for preview labels; the icon requires no fonts."""
    candidates = (
        "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf" if bold else
        "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
        "DejaVuSans-Bold.ttf" if bold else "DejaVuSans.ttf",
        "arialbd.ttf" if bold else "arial.ttf",
    )
    for candidate in candidates:
        try:
            return ImageFont.truetype(candidate, size)
        except OSError:
            pass
    return ImageFont.load_default(size=size)


def export_preview_png() -> None:
    board = Image.new("RGB", (720, 680), "#F2F3F4")
    draw = ImageDraw.Draw(board)

    def label(position: tuple[int, int], text: str, size: int, bold: bool = False) -> None:
        draw.text(position, text, font=preview_font(size, bold),
                  fill="#17191C", anchor="mt")

    label((360, 18), "Alignator | Vector artwork", 25, True)
    label((360, 57), "Alligator icon with four aligned code rows", 15)
    with Image.open(ROOT / "png" / "alignator-256.png") as image:
        large = image.convert("RGBA")
    large.thumbnail((216, 216), Image.Resampling.LANCZOS)
    for left, background in ((84, "#111315"), (396, "#FFFFFF")):
        draw.rounded_rectangle((left, 91, left + 240, 331), radius=23, fill=background)
        board.paste(large, (left + 12, 103), large)
    label((360, 354), "Direct SVG exports", 19, True)
    for center, size in zip((144, 360, 576), (16, 32, 48)):
        label((center, 394), f"{size} x {size} px", 16, True)
        with Image.open(ROOT / "png" / f"alignator-{size}.png") as image:
            icon = image.convert("RGBA")
        for top, background in ((430, "#111315"), (526, "#FFFFFF")):
            draw.rounded_rectangle((center - 80, top, center + 80, top + 80),
                                   radius=11, fill=background)
            board.paste(icon, (center - size // 2, top + (80 - size) // 2), icon)
    label((360, 633), "Open preview/index.html for the browser comparison.", 14)
    destination = ROOT / "preview" / "alignator-preview.png"
    destination.parent.mkdir(parents=True, exist_ok=True)
    board.save(destination)


def export_preview_html() -> None:
    """Use relative paths so source changes remain visible in the preview."""
    samples = []
    for size in (16, 32, 48):
        boxes = []
        for kind, url in (("SVG", "../source/alignator.svg"),
                          ("PNG", f"../png/alignator-{size}.png")):
            boxes.append(
                f'<div class="sample"><h3>{kind} | {size} x {size}</h3>'
                f'<div class="pair"><div class="tile dark"><img src="{url}" '
                f'width="{size}" height="{size}" alt="Alignator icon on a dark background"></div>'
                f'<div class="tile light"><img src="{url}" width="{size}" height="{size}" '
                'alt="Alignator icon on a light background"></div></div></div>'
            )
        samples.append("<section>" + "".join(boxes) + "</section>")
    document = '''<!doctype html>
<html lang="en">
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>Alignator | Icon preview</title>
<link rel="icon" type="image/svg+xml" sizes="any" href="../favicon/favicon.svg">
<style>
*{box-sizing:border-box}body{margin:0;background:#f2f3f4;color:#17191c;font:15px/1.55 system-ui,sans-serif}
main{max-width:1000px;margin:32px auto;padding:0 24px}h1{font-size:28px;margin-bottom:8px}h2{margin:30px 0 12px;font-size:20px}h3{font-size:15px;font-weight:600;margin:0 0 8px}
p{max-width:800px}section{display:grid;grid-template-columns:1fr 1fr;gap:24px;margin:22px 0}.pair{display:flex;gap:12px}.tile{height:100px;flex:1;display:flex;align-items:center;justify-content:center;border-radius:12px}.dark{background:#111315}.light{background:white;border:1px solid #ddd}.hero .tile{height:270px}.hero img{width:240px;height:240px}img{display:block;object-fit:contain}.note{color:#555}
@media(max-width:640px){section{grid-template-columns:1fr}.hero .tile{height:180px}.hero img{width:160px;height:160px}}
</style>
<main>
<h1>Alignator icon</h1>
<p>A friendly alligator with a nose, an eye highlight, a smile, three teeth and four aligned code rows.</p>
<div class="pair hero"><div class="tile dark"><img src="../source/alignator.svg" alt="Alignator on a dark background"></div><div class="tile light"><img src="../source/alignator.svg" alt="Alignator on a light background"></div></div>
<h2>SVG and PNG at the same display size</h2>
<p>Compare at 100% browser zoom. Sizes below are CSS pixels. High-density displays may use multiple physical pixels per CSS pixel. SVG is rendered at the display resolution, while each PNG has the indicated file resolution.</p>
''' + "".join(samples) + '''
<h2>Higher-resolution PNG at 16 CSS pixels</h2>
<div class="pair"><div class="tile dark"><img src="../png/alignator-32.png" width="16" height="16" alt="32-pixel PNG displayed at 16 CSS pixels"></div><div class="tile light"><img src="../png/alignator-32.png" width="16" height="16" alt="32-pixel PNG displayed at 16 CSS pixels"></div></div>
<p class="note">This 32 x 32 PNG is displayed at 16 x 16 CSS pixels. A display with a pixel density ratio of 2 can use the additional image detail.</p>
</main>
</html>
'''
    destination = ROOT / "preview" / "index.html"
    destination.parent.mkdir(parents=True, exist_ok=True)
    destination.write_text(document, encoding="utf-8", newline="\n")


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--inkscape", default=shutil.which("inkscape") or "inkscape",
                        help="Path to the Inkscape executable.")
    args = parser.parse_args()
    if not SOURCE.is_file():
        parser.error(f"SVG master not found: {SOURCE}")
    if not shutil.which(args.inkscape):
        parser.error("Inkscape was not found. Install it or pass --inkscape with its full path.")
    for size in PNG_SIZES:
        render_png(args.inkscape, size)
    export_ico(ROOT / "app" / "alignator.ico", (16, 32, 48, 64, 128, 256))
    export_favicons()
    export_rider_icon()
    export_preview_png()
    export_preview_html()
    print("Exported Alignator PNGs, ICOs, favicons, Rider icon and previews.")


if __name__ == "__main__":
    main()
