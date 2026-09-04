"""Copy validated local packages into the four explicitly supplied sibling repositories."""
import argparse
import json
import hashlib
import shutil
from pathlib import Path

parser = argparse.ArgumentParser()
parser.add_argument('feed', type=Path)
parser.add_argument('repositories', type=Path, help='Parent containing the four consumer repositories')
args = parser.parse_args()
manifest = json.loads((args.feed / 'foundry-ui-manifest.json').read_text())
for name, expected in manifest['packages'].items():
    assert hashlib.sha256((args.feed / name).read_bytes()).hexdigest() == expected, name
for name in ['rhino-layout-foundry', 'rhino-layout-foundry-ai', 'rhino-block-foundry', 'rhino-maps']:
    root = args.repositories / name
    assert (root / 'Directory.Packages.props').is_file(), 'Not a consumer: ' + str(root)
    (root / 'packages').mkdir(exist_ok=True)
    for filename in [*manifest['packages'], 'foundry-ui-manifest.json']:
        shutil.copy2(args.feed / filename, root / 'packages' / filename)
    shutil.copy2(Path(__file__).with_name('verify-shared-ui.py'), root / 'scripts/verify-shared-ui.py')
    shutil.copy2(Path(__file__).with_name('verify-shared-ui.ps1'), root / 'scripts/verify-shared-ui.ps1')
    print('Synced ' + name)
