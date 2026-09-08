// CoreGame.ScrewInfo$$GetTopSameNutInfos RVA 0xa08eb0; next known entry 0xa0905c; boundary requires review.
00a08eb0: stp      x25, x24, [sp, #-0x40]!
00a08eb4: stp      x23, x22, [sp, #0x10]
00a08eb8: stp      x21, x20, [sp, #0x20]
00a08ebc: stp      x19, x30, [sp, #0x30]
00a08ec0: adrp     x21, #0x2ca6000
00a08ec4: adrp     x22, #0x2ab4000
00a08ec8: ldrb     w8, [x21, #0x9c8]
00a08ecc: ldr      x22, [x22, #0x600] | 'System.Collections.Generic.List<NutInfo>_TypeInfo'
00a08ed0: mov      w19, w1
00a08ed4: mov      x20, x0
00a08ed8: tbnz     w8, #0, #0xa08f20
00a08edc: adrp     x0, #0x2ac0000
00a08ee0: ldr      x0, [x0, #0xf78] | 'Method$System.Collections.Generic.List<NutInfo>.Add()'
00a08ee4: bl       #0x925a30 | 
00a08ee8: adrp     x0, #0x2ac9000
00a08eec: ldr      x0, [x0, #0xd40] | 'Method$System.Collections.Generic.List<NutInfo>..ctor()'
00a08ef0: bl       #0x925a30 | 
00a08ef4: adrp     x0, #0x2aae000
00a08ef8: ldr      x0, [x0, #0xc28] | 'Method$System.Collections.Generic.List<NutInfo>.get_Count()'
00a08efc: bl       #0x925a30 | 
00a08f00: adrp     x0, #0x2ad1000
00a08f04: ldr      x0, [x0, #0x1b8] | 'Method$System.Collections.Generic.List<NutInfo>.get_Item()'
00a08f08: bl       #0x925a30 | 
00a08f0c: adrp     x0, #0x2ab4000
00a08f10: ldr      x0, [x0, #0x600] | 'System.Collections.Generic.List<NutInfo>_TypeInfo'
00a08f14: bl       #0x925a30 | 
00a08f18: mov      w8, #1
00a08f1c: strb     w8, [x21, #0x9c8]
00a08f20: ldr      x0, [x22]
00a08f24: bl       #0x925b44 | 
00a08f28: cbz      x0, #0xa09040
00a08f2c: adrp     x8, #0x2ac9000
00a08f30: ldr      x8, [x8, #0xd40] | 'Method$System.Collections.Generic.List<NutInfo>..ctor()'
00a08f34: mov      x21, x0
00a08f38: ldr      x1, [x8]
00a08f3c: bl       #0xf9ca04 | System.Collections.Generic.List<object>$$.ctor
00a08f40: ldr      x0, [x20, #0x28]
00a08f44: cbz      x0, #0xa09040
00a08f48: ldr      w8, [x0, #0x18]
00a08f4c: subs     w22, w8, #1
00a08f50: b.mi     #0xa09044
00a08f54: adrp     x24, #0x2ad1000
00a08f58: adrp     x25, #0x2ac0000
00a08f5c: ldr      x24, [x24, #0x1b8] | 'Method$System.Collections.Generic.List<NutInfo>.get_Item()'
00a08f60: ldr      x25, [x25, #0xf78] | 'Method$System.Collections.Generic.List<NutInfo>.Add()'
00a08f64: ldr      x2, [x24]
00a08f68: mov      w1, w22
00a08f6c: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
00a08f70: cbz      x0, #0xa09030
00a08f74: ldr      x8, [x0, #0x18]
00a08f78: mov      x23, x0
00a08f7c: cbz      x8, #0xa09030
00a08f80: ldr      w8, [x21, #0x18]
00a08f84: cmp      w8, #1
00a08f88: b.lt     #0xa08fd0
00a08f8c: ldr      x2, [x24]
00a08f90: mov      x0, x21
00a08f94: mov      w1, wzr
00a08f98: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
00a08f9c: cbz      x0, #0xa09040
00a08fa0: ldr      x9, [x0, #0x18]
00a08fa4: cbz      x9, #0xa09040
00a08fa8: ldr      x8, [x23, #0x18]
00a08fac: cbz      x8, #0xa09040
00a08fb0: ldr      w9, [x9, #0x14]
00a08fb4: ldr      w10, [x8, #0x14]
00a08fb8: cmp      w9, w10
00a08fbc: b.ne     #0xa09044
00a08fc0: tbz      w19, #0, #0xa08fd0
00a08fc4: ldr      w8, [x8, #0x10]
00a08fc8: cmp      w8, #2
00a08fcc: b.eq     #0xa09044
00a08fd0: ldr      w10, [x21, #0x1c]
00a08fd4: ldr      x8, [x21, #0x10]
00a08fd8: ldr      x9, [x25]
00a08fdc: add      w10, w10, #1
00a08fe0: str      w10, [x21, #0x1c]
00a08fe4: cbz      x8, #0xa09040
00a08fe8: ldrsw    x10, [x21, #0x18]
00a08fec: ldr      w11, [x8, #0x18]
00a08ff0: cmp      w10, w11
00a08ff4: b.hs     #0xa09014
00a08ff8: add      w9, w10, #1
00a08ffc: add      x0, x8, x10, lsl #3
00a09000: str      w9, [x21, #0x18]
00a09004: str      x23, [x0, #0x20]!
00a09008: mov      x1, x23
00a0900c: bl       #0x9259e4 | 
00a09010: b        #0xa09030 | 
00a09014: ldr      x8, [x9, #0x20]
00a09018: mov      x0, x21
00a0901c: mov      x1, x23
00a09020: ldr      x8, [x8, #0xc0]
00a09024: ldr      x2, [x8, #0x58]
00a09028: ldr      x8, [x2, #8]
00a0902c: blr      x8
00a09030: subs     w22, w22, #1
00a09034: b.mi     #0xa09044
00a09038: ldr      x0, [x20, #0x28]
00a0903c: cbnz     x0, #0xa08f64
00a09040: bl       #0x925b54 | 
00a09044: mov      x0, x21
00a09048: ldp      x19, x30, [sp, #0x30]
00a0904c: ldp      x21, x20, [sp, #0x20]
00a09050: ldp      x23, x22, [sp, #0x10]
00a09054: ldp      x25, x24, [sp], #0x40
00a09058: ret      