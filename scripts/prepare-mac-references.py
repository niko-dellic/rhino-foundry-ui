"""Extract pinned genuine Rhino 8 SDK references without installing Rhino (macOS only)."""
import hashlib
import json
from pathlib import Path
import shutil
import subprocess
import tempfile
import urllib.request

root = Path(__file__).resolve().parents[1]
manifest = json.loads((root / 'scripts/mac-reference-manifest.json').read_text())
destination = root / 'artifacts/rhino8-mac-refs'
def valid(path, expected):
    return path.exists() and hashlib.sha256(path.read_bytes()).hexdigest() == expected
if all(valid(destination / name, digest) for name, digest in manifest['sha256'].items()):
    print('Pinned Rhino Mac references already verified.')
    raise SystemExit(0)
with tempfile.TemporaryDirectory(prefix='foundry-rhino-refs-') as temporary:
    temporary = Path(temporary)
    image, mount = temporary / 'rhino.dmg', temporary / 'mount'
    urllib.request.urlretrieve(manifest['source'], image)
    subprocess.run(['hdiutil', 'attach', str(image), '-readonly', '-nobrowse', '-mountpoint', str(mount)], check=True)
    try:
        source = mount / 'Rhino 8.app/Contents/Frameworks/RhCore.framework/Versions/A/Resources'
        for name, digest in manifest['sha256'].items():
            if not valid(source / name, digest):
                raise RuntimeError('Reference checksum mismatch: ' + name)
        destination.mkdir(parents=True, exist_ok=True)
        for name in manifest['sha256']:
            shutil.copy2(source / name, destination / name)
    finally:
        subprocess.run(['hdiutil', 'detach', str(mount)], check=True)
print('Verified genuine Rhino 8 Mac references: ' + str(destination))
