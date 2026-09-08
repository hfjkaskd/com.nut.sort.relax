// CoreGame.ScrewInfo$$UnlockHidden RVA 0xa092c8; next known entry 0xa09418; boundary requires review.
00a092c8: sub      sp, sp, #0x40
00a092cc: str      x20, [sp, #0x20]
00a092d0: stp      x19, x30, [sp, #0x30]
00a092d4: adrp     x20, #0x2ca6000
00a092d8: ldrb     w8, [x20, #0x9cc]
00a092dc: mov      x19, x0
00a092e0: tbnz     w8, #0, #0xa0931c
00a092e4: adrp     x0, #0x2ae7000
00a092e8: ldr      x0, [x0, #0x6b0] | 'Method$System.Collections.Generic.List.Enumerator<NutInfo>.Dispose()'
00a092ec: bl       #0x925a30 | 
00a092f0: adrp     x0, #0x2ae8000
00a092f4: ldr      x0, [x0, #0xc20] | 'Method$System.Collections.Generic.List.Enumerator<NutInfo>.MoveNext()'
00a092f8: bl       #0x925a30 | 
00a092fc: adrp     x0, #0x2ad5000
00a09300: ldr      x0, [x0, #0x5e0] | 'Method$System.Collections.Generic.List.Enumerator<NutInfo>.get_Current()'
00a09304: bl       #0x925a30 | 
00a09308: adrp     x0, #0x2ab1000
00a0930c: ldr      x0, [x0, #0x948] | 'Method$System.Collections.Generic.List<NutInfo>.GetEnumerator()'
00a09310: bl       #0x925a30 | 
00a09314: mov      w8, #1
00a09318: strb     w8, [x20, #0x9cc]
00a0931c: mov      x0, x19
00a09320: mov      w1, wzr
00a09324: stp      xzr, xzr, [sp, #0x10]
00a09328: str      xzr, [sp, #8]
00a0932c: bl       #0xa08eb0 | CoreGame.ScrewInfo$$GetTopSameNutInfos
00a09330: cbz      x0, #0xa09398
00a09334: adrp     x8, #0x2ab1000
00a09338: ldr      x8, [x8, #0x948] | 'Method$System.Collections.Generic.List<NutInfo>.GetEnumerator()'
00a0933c: adrp     x20, #0x2ae8000
00a09340: adrp     x19, #0x2ae7000
00a09344: ldr      x1, [x8]
00a09348: ldr      x20, [x20, #0xc20] | 'Method$System.Collections.Generic.List.Enumerator<NutInfo>.MoveNext()'
00a0934c: ldr      x19, [x19, #0x6b0] | 'Method$System.Collections.Generic.List.Enumerator<NutInfo>.Dispose()'
00a09350: add      x8, sp, #8
00a09354: bl       #0xf9db8c | System.Collections.Generic.List<object>$$GetEnumerator
00a09358: ldr      x1, [x20]
00a0935c: add      x0, sp, #8
00a09360: bl       #0xdaa75c | System.Collections.Generic.List.Enumerator<object>$$MoveNext
00a09364: tbz      w0, #0, #0xa09378
00a09368: ldr      x0, [sp, #0x18]
00a0936c: cbz      x0, #0xa09394
00a09370: bl       #0xa048ac | CoreGame.NutInfo$$UnlockHidden
00a09374: b        #0xa09358 | 
00a09378: ldr      x1, [x19]
00a0937c: add      x0, sp, #8
00a09380: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
00a09384: ldp      x19, x30, [sp, #0x30]
00a09388: ldr      x20, [sp, #0x20]
00a0938c: add      sp, sp, #0x40
00a09390: ret      
00a09394: bl       #0x925b54 | 
00a09398: bl       #0x925b54 | 
00a0939c: b        #0xa093a4 | 
00a093a0: b        #0xa093a4 | 
00a093a4: mov      x19, x0
00a093a8: cmp      w1, #1
00a093ac: b.ne     #0xa093e0
00a093b0: mov      x0, x19
00a093b4: bl       #0x6c0f60 | 
00a093b8: ldr      x20, [x0]
00a093bc: bl       #0x6c03b0 | 
00a093c0: adrp     x8, #0x2ae7000
00a093c4: ldr      x8, [x8, #0x6b0] | 'Method$System.Collections.Generic.List.Enumerator<NutInfo>.Dispose()'
00a093c8: add      x0, sp, #8
00a093cc: ldr      x1, [x8]
00a093d0: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
00a093d4: cbz      x20, #0xa09384
00a093d8: mov      x0, x20
00a093dc: bl       #0x89bb08 | 
00a093e0: mov      x20, xzr
00a093e4: b        #0xa093ec | 
00a093e8: mov      x19, x0
00a093ec: adrp     x8, #0x2ae7000
00a093f0: ldr      x8, [x8, #0x6b0] | 'Method$System.Collections.Generic.List.Enumerator<NutInfo>.Dispose()'
00a093f4: ldr      x1, [x8]
00a093f8: add      x0, sp, #8
00a093fc: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
00a09400: cbnz     x20, #0xa0940c
00a09404: mov      x0, x19
00a09408: bl       #0x6c0230 | 
00a0940c: mov      x0, x20
00a09410: bl       #0x89bb08 | 
00a09414: bl       #0x6c29a8 | 