// CoreGame.ScrewInfo$$get_IsDontMove RVA 0xa080a0; next known entry 0xa08128; boundary requires review.
00a080a0: str      x20, [sp, #-0x20]!
00a080a4: stp      x19, x30, [sp, #0x10]
00a080a8: adrp     x20, #0x2ca6000
00a080ac: ldrb     w8, [x20, #0x9bf]
00a080b0: mov      x19, x0
00a080b4: tbnz     w8, #0, #0xa080d8
00a080b8: adrp     x0, #0x2ad4000
00a080bc: ldr      x0, [x0, #0xe40] | 'Method$System.Collections.Generic.List<ScrewTypeData>.get_Count()'
00a080c0: bl       #0x925a30 | 
00a080c4: adrp     x0, #0x2ac2000
00a080c8: ldr      x0, [x0, #0xd20] | 'Method$System.Collections.Generic.List<ScrewTypeData>.get_Item()'
00a080cc: bl       #0x925a30 | 
00a080d0: mov      w8, #1
00a080d4: strb     w8, [x20, #0x9bf]
00a080d8: ldr      x0, [x19, #0x30]
00a080dc: cbz      x0, #0xa08124
00a080e0: ldr      w8, [x0, #0x18]
00a080e4: cmp      w8, #1
00a080e8: b.lt     #0xa08114
00a080ec: adrp     x8, #0x2ac2000
00a080f0: ldr      x8, [x8, #0xd20] | 'Method$System.Collections.Generic.List<ScrewTypeData>.get_Item()'
00a080f4: mov      w1, wzr
00a080f8: ldr      x2, [x8]
00a080fc: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
00a08100: cbz      x0, #0xa08124
00a08104: ldr      w8, [x0, #0x10]
00a08108: cmp      w8, #6
00a0810c: cset     w0, eq
00a08110: b        #0xa08118 | 
00a08114: mov      w0, wzr
00a08118: ldp      x19, x30, [sp, #0x10]
00a0811c: ldr      x20, [sp], #0x20
00a08120: ret      
00a08124: bl       #0x925b54 | 