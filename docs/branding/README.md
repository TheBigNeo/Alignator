# Alignator branding assets

The editable icon artwork and its generated exports live in this directory.
Extract the archive at the repository root to add `assets/branding/` without
replacing the repository's main README or build configuration.

## Editable source

Edit [`source/alignator.svg`](source/alignator.svg) in a vector editor such as
Inkscape, then regenerate the exports. The SVG uses named shapes and paths for
the head, eye, nose, teeth, neck spines and code rows. No fonts are required by
the icon itself.

The icon has a transparent background and uses the same colors on light and
dark surfaces. Background panels in the previews are for presentation only.

| Element | Color |
|---|---|
| Alligator, aligned code rows and divider | `#7DAA2D` |
| Unaligned code rows | `#85878B` |
| Eye, nostril and mouth | `#111315` |
| Eye highlight | `#FFFFFF` |

The four green rows have equal lengths. The gray rows have different lengths
and share a right edge. The divider ends at the bottom edge of the final row.

## Files

| Path | Purpose |
|---|---|
| [`source/alignator.svg`](source/alignator.svg) | Editable SVG master |
| `png/alignator-{size}.png` | Transparent PNG exports at 16, 32, 40, 48, 64, 128, 180, 192, 256, 512 and 1024 pixels |
| [`app/alignator.ico`](app/alignator.ico) | Windows icon containing 16, 32, 48, 64, 128 and 256-pixel images |
| [`favicon/favicon.svg`](favicon/favicon.svg) | Scalable favicon |
| [`favicon/favicon.ico`](favicon/favicon.ico) | Favicon containing 16, 32 and 48-pixel images |
| `favicon/favicon-{size}.png` | PNG favicons at 16, 32 and 48 pixels |
| [`rider/META-INF/pluginIcon.svg`](rider/META-INF/pluginIcon.svg) | Compact 40 x 40 plugin icon with transparent padding |
| [`preview/alignator-preview.png`](preview/alignator-preview.png) | Overview on light and dark backgrounds |
| [`preview/index.html`](preview/index.html) | Local SVG/PNG comparison at different display sizes |
| [`tools/export_assets.py`](tools/export_assets.py) | Regenerate all exports and previews |
| [`tools/requirements.txt`](tools/requirements.txt) | Python dependency for the export script |

## Regenerate exports

Requirements:

- Python 3.10 or later.
- Inkscape installed and available on `PATH`, or supplied using `--inkscape`.
- Pillow, installed using the requirements file below.

Run from the repository root:

```bash
python -m pip install -r assets/branding/tools/requirements.txt
python assets/branding/tools/export_assets.py
```

On Windows, if Inkscape is not on `PATH`:

```powershell
python assets/branding/tools/export_assets.py --inkscape "C:\Program Files\Inkscape\bin\inkscape.exe"
```

The script also works from other working directories. It resolves paths relative
to its own location and overwrites the generated exports using the current SVG
master. Edit the master rather than the generated PNG or ICO files.

## Use in a Rider plugin

Copy `rider/META-INF/pluginIcon.svg` to the plugin's resources directory so that
it is packaged as `META-INF/pluginIcon.svg` in the plugin JAR. The exact resources
directory depends on the repository layout.

The standalone SVG and generic PNG exports use the master canvas. The Rider
export adds transparent padding for its plugin-manager presentation.

## Use on a website

Copy the favicon files to the public asset directory and reference their deployed
URLs, for example:

```html
<link rel="icon" href="/favicon.svg" type="image/svg+xml" sizes="any">
<link rel="icon" href="/favicon.ico" sizes="16x16 32x32 48x48">
<link rel="apple-touch-icon" href="/apple-touch-icon.png" sizes="180x180">
```

For the touch-icon URL in this example, copy `png/alignator-180.png` to
`apple-touch-icon.png` in the same public directory. Adapt the URLs to the
application's deployment path.

## Inspect small sizes

Open `preview/index.html` locally and compare at 100% browser zoom. Keep the
directory structure intact because the preview references neighboring assets.
It displays SVG and PNG at 16, 32 and 48 CSS pixels, plus a 32-pixel PNG displayed
at 16 CSS pixels to demonstrate a higher-density source at the same visible size.

At a physical resolution of 16 x 16 pixels, very small facial details are still
limited by the available pixel grid. Every export retains the complete artwork;
there are no size-specific simplified designs in this package.
