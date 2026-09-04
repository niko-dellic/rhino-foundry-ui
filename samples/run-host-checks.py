"""Run from Rhino's RunPythonScript after building HostChecks. No document mutations."""
import clr
import System
import os
import tempfile
from System.Reflection import Assembly

# Set this to the isolated Release/net8.0 output containing the matching shared DLLs.
OUTPUT = os.environ.get('FOUNDRY_UI_CHECK_OUTPUT', '/private/tmp/foundry-migration/ui/Release/net8.0')
for name in ['RhinoFoundry.UI.Primitives', 'RhinoFoundry.UI', 'RhinoFoundry.UI.MacOS', 'RhinoFoundry.UI.HostChecks']:
    path = os.path.join(OUTPUT, name + '.dll')
    if os.path.exists(path):
        Assembly.LoadFrom(path)
checks = next(a for a in System.AppDomain.CurrentDomain.GetAssemblies() if a.GetName().Name == "RhinoFoundry.UI.HostChecks").GetType("RhinoFoundry.UI.HostChecks.ComponentChecks")
report = checks.GetMethod("Run").Invoke(None, None)
System.IO.File.WriteAllText(os.path.join(tempfile.gettempdir(), 'foundry-ui-contracts.json'), report)
print(report)
gallery = checks.GetMethod("ShowGallery").Invoke(None, None)
