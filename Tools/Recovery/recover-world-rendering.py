from pathlib import Path
import re, json, shutil, argparse

R = Path(__file__).resolve().parent
SOURCE = R / 'reference-typed/ExportedProject'
TARGET = R / 'rendering-stage'
PACKAGES = Path(r'C:\Projects\com.nut.sort.relax\Library\PackageCache')
parser = argparse.ArgumentParser(description='Recover source world assets using official Unity package shader/script GUIDs.')
parser.add_argument('--source-root', type=Path, default=SOURCE)
parser.add_argument('--package-root', type=Path, default=PACKAGES)
parser.add_argument('--output-root', type=Path, default=TARGET)
parser.add_argument('--report-dir', type=Path, default=R)
args = parser.parse_args()
SOURCE, PACKAGES, TARGET, R = args.source_root, args.package_root, args.output_root, args.report_dir
source_guids = {}
official_shaders = {}
official_scripts = {}

def guid(path):
    return re.search(r'^guid: (\w+)', Path(str(path) + '.meta').read_text(encoding='utf-8'), re.M)[1]

for meta in (SOURCE / 'Assets').rglob('*.meta'):
    path = Path(str(meta)[:-5])
    source_guids[guid(path)] = path

for package in PACKAGES.glob('com.unity.*'):
    for path in package.rglob('*.shader'):
        match = re.search(r'Shader\s+"([^"]+)"', path.read_text(encoding='utf-8-sig'))
        if match and Path(str(path) + '.meta').exists():
            official_shaders[match[1]] = path
    for path in package.rglob('*.cs'):
        if Path(str(path) + '.meta').exists():
            official_scripts.setdefault(path.name, []).append(path)

script_types = {'URP-Balanced': 'UniversalRenderPipelineAsset',
    'URP-Balanced-Renderer': 'UniversalRendererData',
    'SSAO': 'ScreenSpaceAmbientOcclusion', 'XRSystemData': 'XRSystemData'}
queue = [SOURCE / ('Assets/MonoBehaviour/' + name + '.asset') for name in script_types]
queue += list((SOURCE / 'Assets/Mesh').glob('*.asset'))
for prefab in (SOURCE / 'Assets/Resources/game').rglob('*.prefab'):
    # Import only mesh/material dependencies here. Native behaviours and
    # particle/skeletal effects are restored separately, not as dummy DLLs.
    for reference in re.findall(r'guid: (\w+)', prefab.read_text(encoding='utf-8')):
        path = source_guids.get(reference)
        if path and path.suffix == '.mat' and ('/Material/' in path.as_posix() or '/nuts/' in path.as_posix()):
            queue.append(path)

visited = set()
report = {'assets': [], 'shader_mappings': {}, 'script_mappings': {}, 'unresolved': {}}
expected = {'meshes': [], 'materials': []}
while queue:
    path = queue.pop(0)
    if path in visited:
        continue
    visited.add(path)
    if path.suffix in ('.png', '.jpg', '.tga'):
        relative = path.relative_to(SOURCE)
        output = TARGET / relative
        output.parent.mkdir(parents=True, exist_ok=True)
        shutil.copyfile(path, output)
        shutil.copyfile(str(path) + '.meta', str(output) + '.meta')
        report['assets'].append(str(relative))
        continue
    text = path.read_text(encoding='utf-8-sig')
    if path.parent.name == 'Mesh':
        expected['meshes'].append({'path': path.relative_to(SOURCE).as_posix(),
            'vertices': int(re.search(r'm_VertexCount: (\d+)', text)[1])})
    if path.suffix == '.mat':
        properties = dict(re.findall(r'^      (_\w+): ([^\r\n]+)', text, re.M))
        base_color = dict((k, float(v)) for k, v in re.findall(r'([rgba]): ([\d.Ee+-]+)', properties['_BaseColor']))
        expected['materials'].append({'path': path.relative_to(SOURCE).as_posix(),
            'baseColor': base_color, 'smoothness': float(properties['_Smoothness']),
            'metallic': float(properties['_Metallic']), 'surface': float(properties['_Surface'])})
    if path.stem in script_types:
        candidates = official_scripts.get(script_types[path.stem] + '.cs', [])
        if len(candidates) != 1:
            raise RuntimeError((path, candidates))
        script = candidates[0]
        text = re.sub(r'm_Script: \{[^\n]+\}',
            'm_Script: {fileID: 11500000, guid: ' + guid(script) + ', type: 3}', text)
        report['script_mappings'][path.stem] = str(script.relative_to(PACKAGES))
    for reference in set(re.findall(r'guid: (\w+)', text)):
        dep = source_guids.get(reference)
        if not dep:
            continue
        if dep.suffix == '.shader':
            name = re.search(r'Shader\s+"([^"]+)"', dep.read_text(encoding='utf-8-sig'))[1]
            shader = official_shaders.get(name)
            # These two fields are explicitly bound by URP's official
            # UniversalRendererData.ShaderResources Reload attributes. Source
            # names use the older kMotion prefix; keep this migration visible.
            if path.stem == 'URP-Balanced-Renderer' and name in (
                'Hidden/kMotion/CameraMotionVectors', 'Hidden/kMotion/ObjectMotionVectors'):
                shader = PACKAGES / 'com.unity.render-pipelines.universal@14.0.12/Shaders' / (name.rsplit('/', 1)[1] + '.shader')
            if shader:
                text = text.replace('guid: ' + reference, 'guid: ' + guid(shader))
                report['shader_mappings'][name] = str(shader.relative_to(PACKAGES))
            else:
                report['unresolved'][name] = str(dep.relative_to(SOURCE))
        elif dep.suffix in ('.dll', '.cs'):
            report['unresolved'][reference] = str(dep.relative_to(SOURCE))
        elif dep.suffix in ('.mat', '.asset', '.png', '.jpg', '.tga'):
            queue.append(dep)
        else:
            report['unresolved'][reference] = str(dep.relative_to(SOURCE))
    relative = path.relative_to(SOURCE)
    output = TARGET / relative
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(text, encoding='utf-8')
    shutil.copyfile(str(path) + '.meta', str(output) + '.meta')
    report['assets'].append(str(relative))
(R / 'rendering-recovery-report.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
(R / 'rendering-expectations.json').write_text(json.dumps(expected, indent=2), encoding='utf-8')
print(json.dumps({'asset_count': len(report['assets']), 'shader_mappings': report['shader_mappings'],
    'script_mappings': report['script_mappings'], 'unresolved': report['unresolved']}, indent=2))
