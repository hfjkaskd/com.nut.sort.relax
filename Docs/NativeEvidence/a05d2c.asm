// CoreGame.ScenePosGroup$$ResetScrewPos RVA 0xa05d2c; next known entry 0xa05fbc; boundary requires review.
00a05d2c: sub      sp, sp, #0x70
00a05d30: stp      d9, d8, [sp, #0x10]
00a05d34: stp      x27, x26, [sp, #0x20]
00a05d38: stp      x25, x24, [sp, #0x30]
00a05d3c: stp      x23, x22, [sp, #0x40]
00a05d40: stp      x21, x20, [sp, #0x50]
00a05d44: stp      x19, x30, [sp, #0x60]
00a05d48: adrp     x23, #0x2ca6000
00a05d4c: adrp     x22, #0x2ac7000
00a05d50: adrp     x21, #0x2afc000
00a05d54: ldrb     w8, [x23, #0x9a9]
00a05d58: ldr      x22, [x22, #0x408] | 'int_TypeInfo'
00a05d5c: ldr      x21, [x21, #0x7d8] | 'Row_{0}'
00a05d60: mov      w19, w1
00a05d64: mov      x20, x0
00a05d68: tbnz     w8, #0, #0xa05dbc
00a05d6c: adrp     x0, #0x2aee000
00a05d70: ldr      x0, [x0, #0x398] | 'UnityEngine.Debug_TypeInfo'
00a05d74: bl       #0x925a30 | 
00a05d78: adrp     x0, #0x2ac7000
00a05d7c: ldr      x0, [x0, #0x408] | 'int_TypeInfo'
00a05d80: bl       #0x925a30 | 
00a05d84: adrp     x0, #0x2abc000
00a05d88: ldr      x0, [x0, #0x368] | 'UnityEngine.Object_TypeInfo'
00a05d8c: bl       #0x925a30 | 
00a05d90: adrp     x0, #0x2aba000
00a05d94: ldr      x0, [x0, #0x3d8] | 'Pos_{0}'
00a05d98: bl       #0x925a30 | 
00a05d9c: adrp     x0, #0x2afc000
00a05da0: ldr      x0, [x0, #0x7d8] | 'Row_{0}'
00a05da4: bl       #0x925a30 | 
00a05da8: adrp     x0, #0x2af1000
00a05dac: ldr      x0, [x0, #0x110] | 'posTransform == null rowIndex:{0} pos:{1}'
00a05db0: bl       #0x925a30 | 
00a05db4: mov      w8, #1
00a05db8: strb     w8, [x23, #0x9a9]
00a05dbc: mov      x0, x20
00a05dc0: mov      x1, xzr
00a05dc4: bl       #0x1e2f1e0 | UnityEngine.Component$$get_transform
00a05dc8: mov      x20, x0
00a05dcc: ldr      x0, [x22]
00a05dd0: add      x1, sp, #0xc
00a05dd4: str      w19, [sp, #0xc]
00a05dd8: bl       #0x925b38 | 
00a05ddc: ldr      x8, [x21]
00a05de0: mov      x1, x0
00a05de4: mov      x2, xzr
00a05de8: mov      x0, x8
00a05dec: bl       #0x13bec24 | System.String$$Format
00a05df0: cbz      x20, #0xa05fb8
00a05df4: mov      x1, x0
00a05df8: mov      x0, x20
00a05dfc: mov      x2, xzr
00a05e00: bl       #0x1e3647c | UnityEngine.Transform$$Find
00a05e04: cbz      x0, #0xa05fb8
00a05e08: mov      x1, xzr
00a05e0c: mov      x20, x0
00a05e10: bl       #0x1e3636c | UnityEngine.Transform$$get_childCount
00a05e14: cmp      w0, #4
00a05e18: cset     w21, lt
00a05e1c: tbnz     w0, #0, #0xa05e40
00a05e20: sub      w8, w0, #1
00a05e24: cmp      w8, #0
00a05e28: csel     w8, w0, w8, lt
00a05e2c: asr      w8, w8, #1
00a05e30: scvtf    s0, w8
00a05e34: fmov     s1, #0.50000000
00a05e38: fadd     s9, s0, s1
00a05e3c: b        #0xa05e50 | 
00a05e40: cmp      w0, #0
00a05e44: cinc     w8, w0, lt
00a05e48: asr      w8, w8, #1
00a05e4c: scvtf    s9, w8
00a05e50: mov      x0, x20
00a05e54: mov      x1, xzr
00a05e58: bl       #0x1e3636c | UnityEngine.Transform$$get_childCount
00a05e5c: cmp      w0, #1
00a05e60: b.lt     #0xa05f98
00a05e64: adrp     x8, #0x2017000
00a05e68: add      x8, x8, #0x96c
00a05e6c: ldr      s8, [x8, w21, uxtw #2]
00a05e70: adrp     x24, #0x2aba000
00a05e74: adrp     x25, #0x2abc000
00a05e78: adrp     x26, #0x2af1000
00a05e7c: adrp     x27, #0x2aee000
00a05e80: ldr      x24, [x24, #0x3d8] | 'Pos_{0}'
00a05e84: ldr      x25, [x25, #0x368] | 'UnityEngine.Object_TypeInfo'
00a05e88: ldr      x26, [x26, #0x110] | 'posTransform == null rowIndex:{0} pos:{1}'
00a05e8c: ldr      x27, [x27, #0x398] | 'UnityEngine.Debug_TypeInfo'
00a05e90: mov      w23, wzr
00a05e94: fmul     s9, s8, s9
00a05e98: ldr      x0, [x22]
00a05e9c: add      x1, sp, #0xc
00a05ea0: str      w23, [sp, #0xc]
00a05ea4: bl       #0x925b38 | 
00a05ea8: ldr      x8, [x24]
00a05eac: mov      x1, x0
00a05eb0: mov      x2, xzr
00a05eb4: mov      x0, x8
00a05eb8: bl       #0x13bec24 | System.String$$Format
00a05ebc: mov      x1, x0
00a05ec0: mov      x0, x20
00a05ec4: mov      x2, xzr
00a05ec8: bl       #0x1e3647c | UnityEngine.Transform$$Find
00a05ecc: ldr      x8, [x25]
00a05ed0: mov      x21, x0
00a05ed4: ldr      w9, [x8, #0xe0]
00a05ed8: cbnz     w9, #0xa05ee4
00a05edc: mov      x0, x8
00a05ee0: bl       #0x925b30 | 
00a05ee4: mov      x0, x21
00a05ee8: mov      x1, xzr
00a05eec: mov      x2, xzr
00a05ef0: bl       #0x1e22e88 | UnityEngine.Object$$op_Equality
00a05ef4: tbz      w0, #0, #0xa05f5c
00a05ef8: ldr      x0, [x22]
00a05efc: add      x1, sp, #0xc
00a05f00: str      w19, [sp, #0xc]
00a05f04: bl       #0x925b38 | 
00a05f08: mov      x21, x0
00a05f0c: ldr      x0, [x22]
00a05f10: add      x1, sp, #8
00a05f14: str      w23, [sp, #8]
00a05f18: bl       #0x925b38 | 
00a05f1c: ldr      x8, [x26]
00a05f20: mov      x2, x0
00a05f24: mov      x1, x21
00a05f28: mov      x3, xzr
00a05f2c: mov      x0, x8
00a05f30: bl       #0x13c9090 | System.String$$Format
00a05f34: ldr      x8, [x27]
00a05f38: mov      x21, x0
00a05f3c: ldr      w9, [x8, #0xe0]
00a05f40: cbnz     w9, #0xa05f4c
00a05f44: mov      x0, x8
00a05f48: bl       #0x925b30 | 
00a05f4c: mov      x0, x21
00a05f50: mov      x1, xzr
00a05f54: bl       #0x1e1b538 | UnityEngine.Debug$$LogError
00a05f58: b        #0xa05f80 | 
00a05f5c: cbz      x21, #0xa05fb8
00a05f60: scvtf    s0, w23
00a05f64: fmul     s0, s8, s0
00a05f68: fsub     s0, s0, s9
00a05f6c: fmov     s1, wzr
00a05f70: fmov     s2, wzr
00a05f74: mov      x0, x21
00a05f78: mov      x1, xzr
00a05f7c: bl       #0x1e354e0 | UnityEngine.Transform$$set_localPosition
00a05f80: mov      x0, x20
00a05f84: mov      x1, xzr
00a05f88: add      w23, w23, #1
00a05f8c: bl       #0x1e3636c | UnityEngine.Transform$$get_childCount
00a05f90: cmp      w23, w0
00a05f94: b.lt     #0xa05e98
00a05f98: ldp      x19, x30, [sp, #0x60]
00a05f9c: ldp      x21, x20, [sp, #0x50]
00a05fa0: ldp      x23, x22, [sp, #0x40]
00a05fa4: ldp      x25, x24, [sp, #0x30]
00a05fa8: ldp      x27, x26, [sp, #0x20]
00a05fac: ldp      d9, d8, [sp, #0x10]
00a05fb0: add      sp, sp, #0x70
00a05fb4: ret      
00a05fb8: bl       #0x925b54 | 