// CoreGame.ScenePosGroup$$AddScrewPos RVA 0xa059f0; next known entry 0xa05d2c; boundary requires review.
00a059f0: sub      sp, sp, #0x60
00a059f4: str      d8, [sp, #0x10]
00a059f8: str      x26, [sp, #0x18]
00a059fc: stp      x25, x24, [sp, #0x20]
00a05a00: stp      x23, x22, [sp, #0x30]
00a05a04: stp      x21, x20, [sp, #0x40]
00a05a08: stp      x19, x30, [sp, #0x50]
00a05a0c: adrp     x23, #0x2ca6000
00a05a10: adrp     x25, #0x2ac7000
00a05a14: adrp     x24, #0x2afc000
00a05a18: ldrb     w8, [x23, #0x9a8]
00a05a1c: ldr      x25, [x25, #0x408] | 'int_TypeInfo'
00a05a20: ldr      x24, [x24, #0x7d8] | 'Row_{0}'
00a05a24: mov      w20, w3
00a05a28: mov      w19, w2
00a05a2c: mov      w22, w1
00a05a30: mov      x21, x0
00a05a34: tbnz     w8, #0, #0xa05a94
00a05a38: adrp     x0, #0x2ab0000
00a05a3c: ldr      x0, [x0, #0xa08] | 'Method$UnityEngine.GameObject.AddComponent<ScrewPos>()'
00a05a40: bl       #0x925a30 | 
00a05a44: adrp     x0, #0x2ab0000
00a05a48: ldr      x0, [x0, #0xd10] | 'UnityEngine.GameObject_TypeInfo'
00a05a4c: bl       #0x925a30 | 
00a05a50: adrp     x0, #0x2ac7000
00a05a54: ldr      x0, [x0, #0x408] | 'int_TypeInfo'
00a05a58: bl       #0x925a30 | 
00a05a5c: adrp     x0, #0x2ae8000
00a05a60: ldr      x0, [x0, #0xb88] | 'Method$System.Collections.Generic.List<ScrewPos>.Add()'
00a05a64: bl       #0x925a30 | 
00a05a68: adrp     x0, #0x2abc000
00a05a6c: ldr      x0, [x0, #0x368] | 'UnityEngine.Object_TypeInfo'
00a05a70: bl       #0x925a30 | 
00a05a74: adrp     x0, #0x2aba000
00a05a78: ldr      x0, [x0, #0x3d8] | 'Pos_{0}'
00a05a7c: bl       #0x925a30 | 
00a05a80: adrp     x0, #0x2afc000
00a05a84: ldr      x0, [x0, #0x7d8] | 'Row_{0}'
00a05a88: bl       #0x925a30 | 
00a05a8c: mov      w8, #1
00a05a90: strb     w8, [x23, #0x9a8]
00a05a94: mov      x0, x21
00a05a98: mov      x1, xzr
00a05a9c: bl       #0x1e2f1e0 | UnityEngine.Component$$get_transform
00a05aa0: mov      x23, x0
00a05aa4: ldr      x0, [x25]
00a05aa8: add      x1, sp, #0xc
00a05aac: str      w22, [sp, #0xc]
00a05ab0: bl       #0x925b38 | 
00a05ab4: ldr      x8, [x24]
00a05ab8: mov      x1, x0
00a05abc: mov      x2, xzr
00a05ac0: mov      x0, x8
00a05ac4: bl       #0x13bec24 | System.String$$Format
00a05ac8: cbz      x23, #0xa05d28
00a05acc: adrp     x26, #0x2abc000
00a05ad0: ldr      x26, [x26, #0x368] | 'UnityEngine.Object_TypeInfo'
00a05ad4: mov      x1, x0
00a05ad8: mov      x0, x23
00a05adc: mov      x2, xzr
00a05ae0: bl       #0x1e3647c | UnityEngine.Transform$$Find
00a05ae4: ldr      x8, [x26]
00a05ae8: mov      x23, x0
00a05aec: ldr      w9, [x8, #0xe0]
00a05af0: cbnz     w9, #0xa05afc
00a05af4: mov      x0, x8
00a05af8: bl       #0x925b30 | 
00a05afc: adrp     x26, #0x2ab0000
00a05b00: ldr      x26, [x26, #0xd10] | 'UnityEngine.GameObject_TypeInfo'
00a05b04: mov      x0, x23
00a05b08: mov      x1, xzr
00a05b0c: mov      x2, xzr
00a05b10: bl       #0x1e22e88 | UnityEngine.Object$$op_Equality
00a05b14: tbz      w0, #0, #0xa05c10
00a05b18: ldr      x0, [x25]
00a05b1c: add      x1, sp, #8
00a05b20: str      w22, [sp, #8]
00a05b24: bl       #0x925b38 | 
00a05b28: ldr      x8, [x24]
00a05b2c: mov      x1, x0
00a05b30: mov      x2, xzr
00a05b34: mov      x0, x8
00a05b38: bl       #0x13bec24 | System.String$$Format
00a05b3c: ldr      x8, [x26]
00a05b40: mov      x24, x0
00a05b44: mov      x0, x8
00a05b48: bl       #0x925b44 | 
00a05b4c: cbz      x0, #0xa05d28
00a05b50: mov      x1, x24
00a05b54: mov      x2, xzr
00a05b58: mov      x23, x0
00a05b5c: bl       #0x1e317c8 | UnityEngine.GameObject$$.ctor
00a05b60: ldr      w8, [x21, #0x24]
00a05b64: tbnz     w8, #0, #0xa05b74
00a05b68: adrp     x8, #0x2017000
00a05b6c: ldr      s8, [x8, #0x944]
00a05b70: b        #0xa05b90 | 
00a05b74: adrp     x9, #0x2017000
00a05b78: cmp      w8, #0
00a05b7c: ldr      s0, [x9, #0x948]
00a05b80: cinc     w8, w8, lt
00a05b84: asr      w8, w8, #1
00a05b88: scvtf    s1, w8
00a05b8c: fmul     s8, s1, s0
00a05b90: mov      x0, x23
00a05b94: mov      x1, xzr
00a05b98: bl       #0x1e315f4 | UnityEngine.GameObject$$get_transform
00a05b9c: cbz      x0, #0xa05d28
00a05ba0: ldr      w8, [x21, #0x24]
00a05ba4: adrp     x9, #0x2017000
00a05ba8: ldr      s0, [x9, #0x94c]
00a05bac: mvn      w9, w22
00a05bb0: add      w8, w8, w9
00a05bb4: scvtf    s1, w8
00a05bb8: fmul     s0, s1, s0
00a05bbc: fadd     s2, s8, s0
00a05bc0: fmov     s0, wzr
00a05bc4: fmov     s1, wzr
00a05bc8: mov      x1, xzr
00a05bcc: bl       #0x1e354e0 | UnityEngine.Transform$$set_localPosition
00a05bd0: mov      x0, x23
00a05bd4: mov      x1, xzr
00a05bd8: bl       #0x1e315f4 | UnityEngine.GameObject$$get_transform
00a05bdc: cbz      x0, #0xa05d28
00a05be0: mov      x1, xzr
00a05be4: mov      x23, x0
00a05be8: bl       #0x1e2f1e0 | UnityEngine.Component$$get_transform
00a05bec: mov      x24, x0
00a05bf0: mov      x0, x21
00a05bf4: mov      x1, xzr
00a05bf8: bl       #0x1e2f1e0 | UnityEngine.Component$$get_transform
00a05bfc: cbz      x24, #0xa05d28
00a05c00: mov      x1, x0
00a05c04: mov      x0, x24
00a05c08: mov      x2, xzr
00a05c0c: bl       #0x1e35d10 | UnityEngine.Transform$$SetParent
00a05c10: ldr      x0, [x25]
00a05c14: adrp     x24, #0x2aba000
00a05c18: ldr      x24, [x24, #0x3d8] | 'Pos_{0}'
00a05c1c: add      x1, sp, #4
00a05c20: str      w19, [sp, #4]
00a05c24: bl       #0x925b38 | 
00a05c28: ldr      x8, [x24]
00a05c2c: mov      x1, x0
00a05c30: mov      x2, xzr
00a05c34: mov      x0, x8
00a05c38: bl       #0x13bec24 | System.String$$Format
00a05c3c: ldr      x8, [x26]
00a05c40: mov      x25, x0
00a05c44: mov      x0, x8
00a05c48: bl       #0x925b44 | 
00a05c4c: cbz      x0, #0xa05d28
00a05c50: adrp     x26, #0x2ab0000
00a05c54: ldr      x26, [x26, #0xa08] | 'Method$UnityEngine.GameObject.AddComponent<ScrewPos>()'
00a05c58: mov      x1, x25
00a05c5c: mov      x2, xzr
00a05c60: mov      x24, x0
00a05c64: bl       #0x1e317c8 | UnityEngine.GameObject$$.ctor
00a05c68: ldr      x1, [x26]
00a05c6c: mov      x0, x24
00a05c70: bl       #0xae2db8 | UnityEngine.GameObject$$AddComponent<object>
00a05c74: cbz      x0, #0xa05d28
00a05c78: stp      w19, w20, [x0, #0x1c]
00a05c7c: str      w22, [x0, #0x18]
00a05c80: mov      x1, x0
00a05c84: ldr      x0, [x21, #0x18]
00a05c88: cbz      x0, #0xa05d28
00a05c8c: adrp     x9, #0x2ae8000
00a05c90: ldr      x9, [x9, #0xb88] | 'Method$System.Collections.Generic.List<ScrewPos>.Add()'
00a05c94: ldr      w10, [x0, #0x1c]
00a05c98: ldr      x8, [x0, #0x10]
00a05c9c: ldr      x9, [x9]
00a05ca0: add      w10, w10, #1
00a05ca4: str      w10, [x0, #0x1c]
00a05ca8: cbz      x8, #0xa05d28
00a05cac: ldrsw    x10, [x0, #0x18]
00a05cb0: ldr      w11, [x8, #0x18]
00a05cb4: cmp      w10, w11
00a05cb8: b.hs     #0xa05cd8
00a05cbc: add      w9, w10, #1
00a05cc0: add      x8, x8, x10, lsl #3
00a05cc4: str      w9, [x0, #0x18]
00a05cc8: str      x1, [x8, #0x20]!
00a05ccc: mov      x0, x8
00a05cd0: bl       #0x9259e4 | 
00a05cd4: b        #0xa05cec | 
00a05cd8: ldr      x8, [x9, #0x20]
00a05cdc: ldr      x8, [x8, #0xc0]
00a05ce0: ldr      x2, [x8, #0x58]
00a05ce4: ldr      x8, [x2, #8]
00a05ce8: blr      x8
00a05cec: mov      x0, x24
00a05cf0: mov      x1, xzr
00a05cf4: bl       #0x1e315f4 | UnityEngine.GameObject$$get_transform
00a05cf8: cbz      x0, #0xa05d28
00a05cfc: mov      x1, x23
00a05d00: mov      x2, xzr
00a05d04: bl       #0x1e35d10 | UnityEngine.Transform$$SetParent
00a05d08: ldp      x19, x30, [sp, #0x50]
00a05d0c: ldp      x21, x20, [sp, #0x40]
00a05d10: ldp      x23, x22, [sp, #0x30]
00a05d14: ldp      x25, x24, [sp, #0x20]
00a05d18: ldr      x26, [sp, #0x18]
00a05d1c: ldr      d8, [sp, #0x10]
00a05d20: add      sp, sp, #0x60
00a05d24: ret      
00a05d28: bl       #0x925b54 | 