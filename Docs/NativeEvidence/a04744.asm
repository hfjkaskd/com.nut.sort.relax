// CoreGame.ScrewInfo$$GetNullNutInfo RVA 0xa04744; next known entry 0xa048ac; boundary requires review.
00a04744: sub      sp, sp, #0x40
00a04748: stp      x21, x20, [sp, #0x20]
00a0474c: stp      x19, x30, [sp, #0x30]
00a04750: adrp     x20, #0x2ca6000
00a04754: ldrb     w8, [x20, #0x9c7]
00a04758: mov      x19, x0
00a0475c: tbnz     w8, #0, #0xa04798
00a04760: adrp     x0, #0x2ae7000
00a04764: ldr      x0, [x0, #0x6b0] | 'Method$System.Collections.Generic.List.Enumerator<NutInfo>.Dispose()'
00a04768: bl       #0x925a30 | 
00a0476c: adrp     x0, #0x2ae8000
00a04770: ldr      x0, [x0, #0xc20] | 'Method$System.Collections.Generic.List.Enumerator<NutInfo>.MoveNext()'
00a04774: bl       #0x925a30 | 
00a04778: adrp     x0, #0x2ad5000
00a0477c: ldr      x0, [x0, #0x5e0] | 'Method$System.Collections.Generic.List.Enumerator<NutInfo>.get_Current()'
00a04780: bl       #0x925a30 | 
00a04784: adrp     x0, #0x2ab1000
00a04788: ldr      x0, [x0, #0x948] | 'Method$System.Collections.Generic.List<NutInfo>.GetEnumerator()'
00a0478c: bl       #0x925a30 | 
00a04790: mov      w8, #1
00a04794: strb     w8, [x20, #0x9c7]
00a04798: stp      xzr, xzr, [sp, #0x10]
00a0479c: str      xzr, [sp, #8]
00a047a0: ldr      x0, [x19, #0x28]
00a047a4: cbz      x0, #0xa04824
00a047a8: adrp     x8, #0x2ab1000
00a047ac: ldr      x8, [x8, #0x948] | 'Method$System.Collections.Generic.List<NutInfo>.GetEnumerator()'
00a047b0: adrp     x21, #0x2ae8000
00a047b4: adrp     x19, #0x2ae7000
00a047b8: ldr      x1, [x8]
00a047bc: ldr      x21, [x21, #0xc20] | 'Method$System.Collections.Generic.List.Enumerator<NutInfo>.MoveNext()'
00a047c0: ldr      x19, [x19, #0x6b0] | 'Method$System.Collections.Generic.List.Enumerator<NutInfo>.Dispose()'
00a047c4: add      x8, sp, #8
00a047c8: bl       #0xf9db8c | System.Collections.Generic.List<object>$$GetEnumerator
00a047cc: ldr      x1, [x21]
00a047d0: add      x0, sp, #8
00a047d4: bl       #0xdaa75c | System.Collections.Generic.List.Enumerator<object>$$MoveNext
00a047d8: tbz      w0, #0, #0xa047f4
00a047dc: ldr      x20, [sp, #0x18]
00a047e0: cbz      x20, #0xa04820
00a047e4: ldr      x8, [x20, #0x18]
00a047e8: cbnz     x8, #0xa047cc
00a047ec: mov      w21, #4
00a047f0: b        #0xa047fc | 
00a047f4: mov      x20, xzr
00a047f8: mov      w21, #5
00a047fc: ldr      x1, [x19]
00a04800: add      x0, sp, #8
00a04804: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
00a04808: cmp      w21, #4
00a0480c: csel     x0, x20, xzr, eq
00a04810: ldp      x19, x30, [sp, #0x30]
00a04814: ldp      x21, x20, [sp, #0x20]
00a04818: add      sp, sp, #0x40
00a0481c: ret      
00a04820: bl       #0x925b54 | 
00a04824: bl       #0x925b54 | 
00a04828: b        #0xa0482c | 
00a0482c: mov      x20, x0
00a04830: cmp      w1, #1
00a04834: b.ne     #0xa0486c
00a04838: mov      x0, x20
00a0483c: bl       #0x6c0f60 | 
00a04840: ldr      x19, [x0]
00a04844: bl       #0x6c03b0 | 
00a04848: adrp     x8, #0x2ae7000
00a0484c: ldr      x8, [x8, #0x6b0] | 'Method$System.Collections.Generic.List.Enumerator<NutInfo>.Dispose()'
00a04850: add      x0, sp, #8
00a04854: ldr      x1, [x8]
00a04858: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
00a0485c: cbnz     x19, #0xa04874
00a04860: mov      x20, xzr
00a04864: mov      w21, wzr
00a04868: b        #0xa04808 | 
00a0486c: mov      x19, xzr
00a04870: b        #0xa04880 | 
00a04874: mov      x0, x19
00a04878: bl       #0x89bb08 | 
00a0487c: mov      x20, x0
00a04880: adrp     x8, #0x2ae7000
00a04884: ldr      x8, [x8, #0x6b0] | 'Method$System.Collections.Generic.List.Enumerator<NutInfo>.Dispose()'
00a04888: ldr      x1, [x8]
00a0488c: add      x0, sp, #8
00a04890: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
00a04894: cbnz     x19, #0xa048a0
00a04898: mov      x0, x20
00a0489c: bl       #0x6c0230 | 
00a048a0: mov      x0, x19
00a048a4: bl       #0x89bb08 | 
00a048a8: bl       #0x6c29a8 | 