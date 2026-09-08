# Hollow mask mesh port

OriginalHollowMaskGeometry.PopulateMesh now restores the mesh from native OnPopulateMesh 0x9B5740. The eight initial vertices are outer BL/TL/TR/BR then inner BL/TL/TR/BR. They retain UIVertex.simpleVert UVs and the Graphic Color32 tint. Hollow mode uses the original eight frame triangles, then four fans anchored at the inner rectangle corners. Start angles are pi, pi/2, 0 and 3pi/2; each advances by pi/2 divided by TriangleNum. Fan points use the original inner-width/height UV mapping. Solid mode retains all eight vertices but emits only the two outer triangles.

The six-segment configuration emits 40 vertices and 32 triangles. The port generates points directly into VertexHelper and removes the original per-rebuild corner/angle/arc lists and per-vertex temporary arrays. Topology, iteration order and numeric radius semantics are preserved. No automatic clipping, height radius clamp, UV clamp or hole-based raycast filter was introduced.

OriginalHollowMaskGraphic uses native Unity Graphic/VertexHelper, the restored geometry, serialized target/settings, Awake bounds capture, and Update refresh only when realtimeRefresh is enabled. Explicit RefreshBounds updates the cache and marks vertices dirty. Constructor defaults follow 0x9B6F08: Radius 10, TriangleNum 6, ShowHollowOut true; the original guide prefab uses its own Radius 25 and realtimeRefresh true. No Editor-only runtime branch exists.

Validation checks exact vertex/index counts, initial and fan triangle order, initial UV/tint, fan UV, corner tangent endpoints, total coverage area against a polygonal rounded hole, and switching to solid mode clearing former fans. Existing rotated/descendant/destroyed-target and UV/radius edge tests remain. Complete NewbieGuidePanel prefab binding and current rendered guide comparison remain outstanding; the mesh test does not establish full screenshot parity.

Final Unity 2022.3.62f3 regression: 74 PASS markers in Library/unity-hollow-mesh-final-validation.log, no compile or validation exceptions, preferences restored.
