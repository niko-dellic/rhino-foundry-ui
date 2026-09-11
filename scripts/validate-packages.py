"""Validate a coordinated local feed and emit exact shared DLL/package hashes."""
import argparse
import hashlib
import json
from pathlib import Path
import xml.etree.ElementTree as ET
import zipfile

parser = argparse.ArgumentParser()
parser.add_argument('directory', type=Path, nargs='?', default=Path(__file__).resolve().parents[1] / 'artifacts/packages')
parser.add_argument('--version', default=ET.parse(Path(__file__).resolve().parents[1] / 'Directory.Build.props').findtext('.//Version'))
args = parser.parse_args()
manifest = {'version': args.version, 'assemblyVersion': '0.3.0.0', 'files': {}, 'packages': {}}
for name in ['RhinoFoundry.UI.Primitives', 'RhinoFoundry.UI', 'RhinoFoundry.UI.MacOS']:
    package = args.directory / (name + '.' + args.version + '.nupkg')
    with zipfile.ZipFile(package) as archive:
        names = archive.namelist()
        assert len(names) == len(set(names)), 'Duplicate entries: ' + name
        dlls = [item for item in names if item.endswith('.dll')]
        assert dlls == ['lib/net8.0/' + name + '.dll'], 'Unexpected DLL payload: ' + str(dlls)
        nuspec = ET.fromstring(archive.read(name + '.nuspec'))
        version = next(element.text for element in nuspec.iter() if element.tag.endswith('}version'))
        assert version == args.version, 'Mismatched package version: ' + name
        for dependency in nuspec.iter():
            if dependency.tag.endswith('}dependency') and dependency.attrib['id'].startswith('RhinoFoundry.UI'):
                assert dependency.attrib['version'].strip('[]') == args.version, 'Mixed shared dependencies'
        assert 'README.md' in names, 'Missing package readme'
        manifest['files'][name + '.dll'] = hashlib.sha256(archive.read(dlls[0])).hexdigest()
    manifest['packages'][package.name] = hashlib.sha256(package.read_bytes()).hexdigest()
output = args.directory / 'foundry-ui-manifest.json'
output.write_text(json.dumps(manifest, indent=2) + '\n')
print('Validated 3 packages: ' + str(output))
