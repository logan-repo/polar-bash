"""Generate the Windows icon from the same simple bear shapes as the SVG asset."""

from pathlib import Path

from PIL import Image, ImageDraw


ROOT = Path(__file__).resolve().parents[1]
SIZE = 1024
SCALE = SIZE / 128


def box(coords):
    return tuple(round(value * SCALE) for value in coords)


navy = "#012c4e"
cream = "#ffe8a9"
image = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
draw = ImageDraw.Draw(image)
draw.rounded_rectangle(box((0, 0, 127, 127)), radius=round(25 * SCALE), fill=navy)
draw.ellipse(box((30, 20, 56, 46)), fill=cream)
draw.ellipse(box((72, 20, 98, 46)), fill=cream)
draw.ellipse(box((35, 11, 93, 73)), fill=cream)
draw.ellipse(box((30, 49, 98, 112)), fill=cream)
draw.rounded_rectangle(box((27, 78, 101, 110)), radius=round(18 * SCALE), fill=cream)
draw.ellipse(box((25, 99, 55, 113)), fill=cream)
draw.ellipse(box((73, 99, 103, 113)), fill=cream)
draw.ellipse(box((45, 41, 52, 48)), fill=navy)
draw.ellipse(box((76, 41, 83, 48)), fill=navy)
draw.ellipse(box((58, 51, 70, 60)), fill=navy)
draw.line([box((64, 59))[:2], box((64, 64))[:2]], fill=navy, width=round(3 * SCALE))
draw.arc(box((55, 58, 65, 68)), 0, 150, fill=navy, width=round(2.5 * SCALE))
draw.arc(box((63, 58, 73, 68)), 30, 180, fill=navy, width=round(2.5 * SCALE))
for points in (((43, 78), (42, 88), (45, 99)), ((85, 78), (86, 88), (83, 99))):
    draw.line([box((x, y))[:2] for x, y in points], fill=navy, width=round(4 * SCALE), joint="curve")
for x in (56, 72):
    draw.line([box((x, 91))[:2], box((x, 109))[:2]], fill=navy, width=round(3 * SCALE))

asset = ROOT / "assets"
asset.mkdir(exist_ok=True)
image.save(asset / "PolarBash.png")
image.save(asset / "PolarBash.ico", sizes=[(16, 16), (24, 24), (32, 32), (48, 48), (64, 64), (128, 128), (256, 256)])
