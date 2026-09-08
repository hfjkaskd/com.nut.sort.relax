# Hollow-mask geometry audit

Evidence: UIHollowOutImage.CalcBounds 0x9B5604, OnPopulateMesh radius normalization 0x9B5840-0x9B5884, and GetLLSUVS 0x9B6DE4. Original NewbieGuidePanel serializes Radius=25, TriangleNum=6, realtimeRefresh=true and ShowHollowOut=true; its Graphic remains raycastTarget=true. No hole-specific raycast override is declared in the recovered UIHollowOutImage type, so a visible hole must not be assumed to imply click-through.

OriginalHollowMaskGeometry caches the source four corners. Missing or Unity-destroyed inner targets return before changing either inner or outer cached bounds. The relative Unity bounds calculation includes target descendants and their transformations; outer corners come from the actual rect, including its pivot.

The native corner size is min(width / Radius, width * 0.5) using its explicit comparison semantics, not Radius pixels. Negative Radius mutates to zero, nonpositive TriangleNum mutates to one. There is no height clamp; zero division and NaN are preserved. UV is position / dimensions + 0.5 with no clamping or pivot correction.

The per-vertex UV calculation avoids the source temporary one-element input/output arrays while preserving numeric results. Full mesh topology and the actual Graphic/view binding are still outstanding. Original realtimeRefresh requires attention to hierarchy traversal, mesh rebuild CPU and GC; buffer reuse is recommended for the mesh port while retaining the original refresh behavior. This helper does not yet schedule updates or render a replacement mask.

Validation uses real RectTransforms with translation, noncentral outer pivot, a descendant extending beyond its target, rotation and destroyed targets. It also checks native radius/count mutations, the serialized radius divisor, UV out-of-range and zero-dimension behavior. This is geometry verification, not screenshot proof of full visual parity.

Unity 2022.3.62f3: 74 full regression PASS markers in Library/unity-hollow-geometry-validation.log, no compile or validation exceptions.
