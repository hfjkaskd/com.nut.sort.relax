// CoreGame.ScrewInfo$$get_IsColorMask RVA 0xa07f40; next known entry 0xa07ff4; boundary requires review.
00a07f40: str      x20, [sp, #-0x20]!
00a07f44: stp      x19, x30, [sp, #0x10]
00a07f48: adrp     x20, #0x2ca6000
00a07f4c: ldrb     w8, [x20, #0x9bd]
00a07f50: mov      x19, x0
00a07f54: tbnz     w8, #0, #0xa07f78
00a07f58: adrp     x0, #0x2ad4000
00a07f5c: ldr      x0, [x0, #0xe40] | 'Method$System.Collections.Generic.List<ScrewTypeData>.get_Count()'
00a07f60: bl       #0x925a30 | 
00a07f64: adrp     x0, #0x2ac2000
00a07f68: ldr      x0, [x0, #0xd20] | 'Method$System.Collections.Generic.List<ScrewTypeData>.get_Item()'
00a07f6c: bl       #0x925a30 | 
00a07f70: mov      w8, #1
00a07f74: strb     w8, [x20, #0x9bd]
00a07f78: ldr      x0, [x19, #0x30]
00a07f7c: cbz      x0, #0xa07ff0
00a07f80: ldr      w8, [x0, #0x18]
00a07f84: cmp      w8, #1
00a07f88: b.lt     #0xa07fe0
00a07f8c: adrp     x20, #0x2ac2000
00a07f90: ldr      x20, [x20, #0xd20] | 'Method$System.Collections.Generic.List<ScrewTypeData>.get_Item()'
00a07f94: mov      w1, wzr
00a07f98: ldr      x2, [x20]
00a07f9c: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
00a07fa0: cbz      x0, #0xa07ff0
00a07fa4: ldr      w8, [x0, #0x10]
00a07fa8: cmp      w8, #4
00a07fac: b.ne     #0xa07fe0
00a07fb0: ldr      x0, [x19, #0x30]
00a07fb4: cbz      x0, #0xa07ff0
00a07fb8: ldr      x2, [x20]
00a07fbc: mov      w1, wzr
00a07fc0: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
00a07fc4: cbz      x0, #0xa07ff0
00a07fc8: ldr      x8, [x0, #0x18]
00a07fcc: cbz      x8, #0xa07ff0
00a07fd0: ldrb     w8, [x8, #0x1c]
00a07fd4: cmp      w8, #0
00a07fd8: cset     w0, ne
00a07fdc: b        #0xa07fe4 | 
00a07fe0: mov      w0, wzr
00a07fe4: ldp      x19, x30, [sp, #0x10]
00a07fe8: ldr      x20, [sp], #0x20
00a07fec: ret      
00a07ff0: bl       #0x925b54 | 