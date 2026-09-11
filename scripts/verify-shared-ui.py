"""Vendored from rhino-foundry-ui/scripts. Verify shared DLLs in a consumer bundle."""
import argparse
import hashlib
import json
from pathlib import Path

parser = argparse.ArgumentParser()
parser.add_argument('directory', type=Path)
parser.add_argument('platform', choices=['MacOS', 'Windows'])
parser.add_argument('--manifest', type=Path, required=True, help='Manifest supplied with the exact package bundle')
args = parser.parse_args()
manifest = json.loads(args.manifest.read_text())
for name, expected in manifest['files'].items():
    path = args.directory / name
    if name.endswith('.MacOS.dll') and args.platform == 'Windows':
        if path.exists(): raise SystemExit('Windows bundle contains Mac adapter: ' + str(path))
        continue
    if not path.exists(): raise SystemExit('Missing shared binary: ' + str(path))
    if hashlib.sha256(path.read_bytes()).hexdigest() != expected:
        raise SystemExit('Shared binary hash mismatch: ' + str(path))
print('Shared UI ' + manifest['version'] + ': exact ' + args.platform + ' package binaries verified')
