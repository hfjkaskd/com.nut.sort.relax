// CoreGame.ScrewInfo$$Ready RVA 0xa091f4; next known entry 0xa092c8; boundary requires review.
00a091f4: stp      x21, x20, [sp, #-0x20]!
00a091f8: stp      x19, x30, [sp, #0x10]
00a091fc: adrp     x21, #0x2ca6000
00a09200: ldrb     w8, [x21, #0x9ca]
00a09204: mov      x19, x1
00a09208: mov      x20, x0
00a0920c: tbnz     w8, #0, #0xa09248
00a09210: adrp     x0, #0x2aee000
00a09214: ldr      x0, [x0, #0x398] | 'UnityEngine.Debug_TypeInfo'
00a09218: bl       #0x925a30 | 
00a0921c: adrp     x0, #0x2aae000
00a09220: ldr      x0, [x0, #0xc28] | 'Method$System.Collections.Generic.List<NutInfo>.get_Count()'
00a09224: bl       #0x925a30 | 
00a09228: adrp     x0, #0x2ad1000
00a0922c: ldr      x0, [x0, #0x1b8] | 'Method$System.Collections.Generic.List<NutInfo>.get_Item()'
00a09230: bl       #0x925a30 | 
00a09234: adrp     x0, #0x2adb000
00a09238: ldr      x0, [x0, #0x7c0] | 'nutInfos.NutMaxCount == 0'
00a0923c: bl       #0x925a30 | 
00a09240: mov      w8, #1
00a09244: strb     w8, [x21, #0x9ca]
00a09248: cbnz     x19, #0xa09260
00a0924c: mov      w1, #1
00a09250: mov      x0, x20
00a09254: bl       #0xa08eb0 | CoreGame.ScrewInfo$$GetTopSameNutInfos
00a09258: mov      x19, x0
00a0925c: cbz      x0, #0xa092c4
00a09260: ldr      w8, [x19, #0x18]
00a09264: cbz      w8, #0xa09290
00a09268: adrp     x8, #0x2ad1000
00a0926c: ldr      x8, [x8, #0x1b8] | 'Method$System.Collections.Generic.List<NutInfo>.get_Item()'
00a09270: mov      x0, x19
00a09274: mov      w1, wzr
00a09278: ldr      x2, [x8]
00a0927c: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
00a09280: cbz      x0, #0xa092c4
00a09284: ldp      x19, x30, [sp, #0x10]
00a09288: ldp      x21, x20, [sp], #0x20
00a0928c: b        #0xa04470 | CoreGame.NutInfo$$Ready
00a09290: adrp     x8, #0x2aee000
00a09294: ldr      x8, [x8, #0x398] | 'UnityEngine.Debug_TypeInfo'
00a09298: adrp     x19, #0x2adb000
00a0929c: ldr      x0, [x8]
00a092a0: ldr      w8, [x0, #0xe0]
00a092a4: ldr      x19, [x19, #0x7c0] | 'nutInfos.NutMaxCount == 0'
00a092a8: cbnz     w8, #0xa092b0
00a092ac: bl       #0x925b30 | 
00a092b0: ldr      x0, [x19]
00a092b4: ldp      x19, x30, [sp, #0x10]
00a092b8: mov      x1, xzr
00a092bc: ldp      x21, x20, [sp], #0x20
00a092c0: b        #0x1e1b538 | UnityEngine.Debug$$LogError
00a092c4: bl       #0x925b54 | 