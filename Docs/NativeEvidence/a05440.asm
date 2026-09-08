// CoreGame.ScenePosGroup$$Init RVA 0xa05440; next known entry 0xa059f0; boundary requires review.
00a05440: sub      sp, sp, #0xc0
00a05444: stp      d9, d8, [sp, #0x60]
00a05448: str      x26, [sp, #0x70]
00a0544c: stp      x25, x24, [sp, #0x80]
00a05450: stp      x23, x22, [sp, #0x90]
00a05454: stp      x21, x20, [sp, #0xa0]
00a05458: stp      x19, x30, [sp, #0xb0]
00a0545c: adrp     x22, #0x2ca6000
00a05460: ldrb     w8, [x22, #0x9a5]
00a05464: mov      w21, w2
00a05468: mov      x20, x1
00a0546c: mov      x19, x0
00a05470: tbnz     w8, #0, #0xa054f4
00a05474: adrp     x0, #0x2aef000
00a05478: ldr      x0, [x0, #0xb88] | 'Method$System.Collections.Generic.List.Enumerator<ScrewInfo>.Dispose()'
00a0547c: bl       #0x925a30 | 
00a05480: adrp     x0, #0x2abd000
00a05484: ldr      x0, [x0, #0x28] | 'Method$System.Collections.Generic.List.Enumerator<ScrewPos>.Dispose()'
00a05488: bl       #0x925a30 | 
00a0548c: adrp     x0, #0x2af0000
00a05490: ldr      x0, [x0, #0x1f0] | 'Method$System.Collections.Generic.List.Enumerator<ScrewInfo>.MoveNext()'
00a05494: bl       #0x925a30 | 
00a05498: adrp     x0, #0x2adb000
00a0549c: ldr      x0, [x0, #0xce8] | 'Method$System.Collections.Generic.List.Enumerator<ScrewPos>.MoveNext()'
00a054a0: bl       #0x925a30 | 
00a054a4: adrp     x0, #0x2ad5000
00a054a8: ldr      x0, [x0, #0x38] | 'Method$System.Collections.Generic.List.Enumerator<ScrewPos>.get_Current()'
00a054ac: bl       #0x925a30 | 
00a054b0: adrp     x0, #0x2ac5000
00a054b4: ldr      x0, [x0, #0xad8] | 'Method$System.Collections.Generic.List.Enumerator<ScrewInfo>.get_Current()'
00a054b8: bl       #0x925a30 | 
00a054bc: adrp     x0, #0x2aba000
00a054c0: ldr      x0, [x0, #0xfe0] | 'Method$System.Collections.Generic.List<ScrewPos>.Clear()'
00a054c4: bl       #0x925a30 | 
00a054c8: adrp     x0, #0x2afe000
00a054cc: ldr      x0, [x0, #0xd20] | 'Method$System.Collections.Generic.List<ScrewPos>.GetEnumerator()'
00a054d0: bl       #0x925a30 | 
00a054d4: adrp     x0, #0x2ae8000
00a054d8: ldr      x0, [x0, #0x218] | 'Method$System.Collections.Generic.List<ScrewInfo>.GetEnumerator()'
00a054dc: bl       #0x925a30 | 
00a054e0: adrp     x0, #0x2ae5000
00a054e4: ldr      x0, [x0, #0x748] | 'Method$System.Collections.Generic.List<ScrewInfo>.get_Count()'
00a054e8: bl       #0x925a30 | 
00a054ec: mov      w8, #1
00a054f0: strb     w8, [x22, #0x9a5]
00a054f4: stp      xzr, xzr, [sp, #0x48]
00a054f8: str      xzr, [sp, #0x40]
00a054fc: stp      xzr, xzr, [sp, #0x28]
00a05500: str      xzr, [sp, #0x20]
00a05504: ldr      x8, [x19, #0x18]
00a05508: cbz      x8, #0xa058f4
00a0550c: ldp      w2, w9, [x8, #0x18]
00a05510: add      w9, w9, #1
00a05514: cmp      w2, #1
00a05518: stp      wzr, w9, [x8, #0x18]
00a0551c: b.lt     #0xa05530
00a05520: ldr      x0, [x8, #0x10]
00a05524: mov      w1, wzr
00a05528: mov      x3, xzr
00a0552c: bl       #0x15689b8 | System.Array$$Clear
00a05530: cbz      x20, #0xa058f4
00a05534: ldr      x0, [x20, #0x10]
00a05538: cbz      x0, #0xa058f4
00a0553c: ldr      w8, [x0, #0x18]
00a05540: str      w8, [x19, #0x28]
00a05544: tbz      w21, #0, #0xa0577c
00a05548: cmp      w8, #5
00a0554c: b.le     #0xa05568
00a05550: cmp      w8, #0xa
00a05554: b.le     #0xa05578
00a05558: mov      w9, #3
00a0555c: fmov     s8, #3.00000000
00a05560: str      w9, [x19, #0x24]
00a05564: b        #0xa05584 | 
00a05568: mov      w9, #1
00a0556c: str      w9, [x19, #0x24]
00a05570: fmov     s8, #1.00000000
00a05574: b        #0xa05584 | 
00a05578: mov      w9, #2
00a0557c: str      w9, [x19, #0x24]
00a05580: fmov     s8, #2.00000000
00a05584: adrp     x21, #0x2ca6000
00a05588: ldrb     w9, [x21, #0x9de]
00a0558c: scvtf    s9, w8
00a05590: cbnz     w9, #0xa055a8
00a05594: adrp     x0, #0x2ac3000
00a05598: ldr      x0, [x0, #0x620] | 'System.Math_TypeInfo'
00a0559c: bl       #0x925a30 | 
00a055a0: mov      w8, #1
00a055a4: strb     w8, [x21, #0x9de]
00a055a8: adrp     x8, #0x2ac3000
00a055ac: ldr      x8, [x8, #0x620] | 'System.Math_TypeInfo'
00a055b0: fdiv     s8, s9, s8
00a055b4: ldr      x0, [x8]
00a055b8: ldr      w8, [x0, #0xe0]
00a055bc: cbnz     w8, #0xa055c4
00a055c0: bl       #0x925b30 | 
00a055c4: ldr      w10, [x19, #0x24]
00a055c8: mov      w9, #0x7f800000
00a055cc: frintp   s0, s8
00a055d0: fmov     s1, w9
00a055d4: fcvtps   w8, s8
00a055d8: fcmp     s0, s1
00a055dc: mov      w9, #-0xffffffff80000000
00a055e0: csel     w23, w9, w8, eq
00a055e4: cmp      w10, #1
00a055e8: str      w23, [x19, #0x20]
00a055ec: b.lt     #0xa05654
00a055f0: ldr      w8, [x19, #0x28]
00a055f4: mov      w24, wzr
00a055f8: mov      w21, wzr
00a055fc: subs     w25, w8, w23
00a05600: csel     w26, w8, w23, lt
00a05604: cmp      w26, #1
00a05608: b.lt     #0xa05634
00a0560c: mov      w22, wzr
00a05610: add      w3, w24, w22
00a05614: mov      x0, x19
00a05618: mov      w1, w21
00a0561c: mov      w2, w22
00a05620: bl       #0xa059f0 | CoreGame.ScenePosGroup$$AddScrewPos
00a05624: add      w22, w22, #1
00a05628: cmp      w22, w26
00a0562c: b.lt     #0xa05610
00a05630: add      w24, w24, w22
00a05634: mov      x0, x19
00a05638: mov      w1, w21
00a0563c: bl       #0xa05d2c | CoreGame.ScenePosGroup$$ResetScrewPos
00a05640: ldr      w8, [x19, #0x24]
00a05644: add      w21, w21, #1
00a05648: cmp      w21, w8
00a0564c: mov      w8, w25
00a05650: b.lt     #0xa055fc
00a05654: ldr      x0, [x19, #0x18]
00a05658: cbz      x0, #0xa058f4
00a0565c: adrp     x8, #0x2afe000
00a05660: ldr      x8, [x8, #0xd20] | 'Method$System.Collections.Generic.List<ScrewPos>.GetEnumerator()'
00a05664: adrp     x24, #0x2adb000
00a05668: adrp     x23, #0x2abd000
00a0566c: ldr      x1, [x8]
00a05670: ldr      x24, [x24, #0xce8] | 'Method$System.Collections.Generic.List.Enumerator<ScrewPos>.MoveNext()'
00a05674: ldr      x23, [x23, #0x28] | 'Method$System.Collections.Generic.List.Enumerator<ScrewPos>.Dispose()'
00a05678: add      x8, sp, #8
00a0567c: bl       #0xf9db8c | System.Collections.Generic.List<object>$$GetEnumerator
00a05680: ldur     q0, [sp, #8]
00a05684: ldr      x8, [sp, #0x18]
00a05688: fmov     s8, #-1.00000000
00a0568c: str      q0, [sp, #0x40]
00a05690: str      x8, [sp, #0x50]
00a05694: ldr      x1, [x24]
00a05698: add      x0, sp, #0x40
00a0569c: bl       #0xdaa75c | System.Collections.Generic.List.Enumerator<object>$$MoveNext
00a056a0: tbz      w0, #0, #0xa05710
00a056a4: ldr      x21, [sp, #0x50]
00a056a8: cbz      x21, #0xa05728
00a056ac: mov      w22, wzr
00a056b0: mov      x0, x21
00a056b4: mov      x1, xzr
00a056b8: bl       #0x1e2f1e0 | UnityEngine.Component$$get_transform
00a056bc: cbz      x0, #0xa0571c
00a056c0: mov      x1, xzr
00a056c4: bl       #0x1e3636c | UnityEngine.Transform$$get_childCount
00a056c8: cmp      w22, w0
00a056cc: b.ge     #0xa05694
00a056d0: mov      x0, x21
00a056d4: mov      x1, xzr
00a056d8: bl       #0x1e2f1e0 | UnityEngine.Component$$get_transform
00a056dc: cbz      x0, #0xa05720
00a056e0: mov      w1, w22
00a056e4: mov      x2, xzr
00a056e8: bl       #0x1e36728 | UnityEngine.Transform$$GetChild
00a056ec: cbz      x0, #0xa05724
00a056f0: mov      x1, xzr
00a056f4: bl       #0x1e2f21c | UnityEngine.Component$$get_gameObject
00a056f8: mov      w1, wzr
00a056fc: mov      v0.16b, v8.16b
00a05700: mov      x2, xzr
00a05704: bl       #0x9b7588 | Util.UILSSUtil$$SetGameObjectLSSActive
00a05708: add      w22, w22, #1
00a0570c: b        #0xa056b0 | 
00a05710: ldr      x1, [x23]
00a05714: add      x0, sp, #0x40
00a05718: b        #0xa058c4 | 
00a0571c: bl       #0x925b54 | 
00a05720: bl       #0x925b54 | 
00a05724: bl       #0x925b54 | 
00a05728: bl       #0x925b54 | 
00a0572c: b        #0xa05750 | 
00a05730: b        #0xa05750 | 
00a05734: b        #0xa05750 | 
00a05738: b        #0xa05750 | 
00a0573c: b        #0xa05750 | 
00a05740: b        #0xa05750 | 
00a05744: b        #0xa05750 | 
00a05748: b        #0xa05750 | 
00a0574c: b        #0xa05750 | 
00a05750: cmp      w1, #1
00a05754: b.ne     #0xa058f8
00a05758: bl       #0x6c0f60 | 
00a0575c: ldr      x21, [x0]
00a05760: bl       #0x6c03b0 | 
00a05764: ldr      x1, [x23]
00a05768: add      x0, sp, #0x40
00a0576c: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
00a05770: cbnz     x21, #0xa059c0
00a05774: ldr      x0, [x20, #0x10]
00a05778: cbz      x0, #0xa058f4
00a0577c: adrp     x24, #0x2ae8000
00a05780: ldr      x24, [x24, #0x218] | 'Method$System.Collections.Generic.List<ScrewInfo>.GetEnumerator()'
00a05784: adrp     x23, #0x2af0000
00a05788: adrp     x22, #0x2aef000
00a0578c: add      x8, sp, #8
00a05790: ldr      x1, [x24]
00a05794: ldr      x23, [x23, #0x1f0] | 'Method$System.Collections.Generic.List.Enumerator<ScrewInfo>.MoveNext()'
00a05798: ldr      x22, [x22, #0xb88] | 'Method$System.Collections.Generic.List.Enumerator<ScrewInfo>.Dispose()'
00a0579c: bl       #0xf9db8c | System.Collections.Generic.List<object>$$GetEnumerator
00a057a0: ldur     q0, [sp, #8]
00a057a4: ldr      x8, [sp, #0x18]
00a057a8: str      q0, [sp, #0x20]
00a057ac: str      x8, [sp, #0x30]
00a057b0: ldr      x1, [x23]
00a057b4: add      x0, sp, #0x20
00a057b8: bl       #0xdaa75c | System.Collections.Generic.List.Enumerator<object>$$MoveNext
00a057bc: tbz      w0, #0, #0xa057fc
00a057c0: ldr      x8, [sp, #0x30]
00a057c4: cbz      x8, #0xa058e8
00a057c8: ldr      x8, [x8, #0x20]
00a057cc: cbz      x8, #0xa058ac
00a057d0: ldr      w9, [x8, #0x10]
00a057d4: ldr      w10, [x19, #0x24]
00a057d8: cmp      w9, w10
00a057dc: b.le     #0xa057e4
00a057e0: str      w9, [x19, #0x24]
00a057e4: ldr      w8, [x8, #0x14]
00a057e8: ldr      w9, [x19, #0x20]
00a057ec: cmp      w8, w9
00a057f0: b.le     #0xa057b0
00a057f4: str      w8, [x19, #0x20]
00a057f8: b        #0xa057b0 | 
00a057fc: ldr      x1, [x22]
00a05800: add      x0, sp, #0x20
00a05804: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
00a05808: ldr      d0, [x19, #0x20]
00a0580c: movi     v1.2s, #1
00a05810: add      v0.2s, v0.2s, v1.2s
00a05814: str      d0, [x19, #0x20]
00a05818: ldr      x0, [x20, #0x10]
00a0581c: cbz      x0, #0xa058f4
00a05820: ldr      x1, [x24]
00a05824: add      x8, sp, #8
00a05828: bl       #0xf9db8c | System.Collections.Generic.List<object>$$GetEnumerator
00a0582c: ldur     q0, [sp, #8]
00a05830: ldr      x8, [sp, #0x18]
00a05834: str      q0, [sp, #0x20]
00a05838: str      x8, [sp, #0x30]
00a0583c: ldr      x1, [x23]
00a05840: add      x0, sp, #0x20
00a05844: bl       #0xdaa75c | System.Collections.Generic.List.Enumerator<object>$$MoveNext
00a05848: tbz      w0, #0, #0xa05870
00a0584c: ldr      x8, [sp, #0x30]
00a05850: cbz      x8, #0xa058ec
00a05854: ldr      x9, [x8, #0x20]
00a05858: cbz      x9, #0xa058f0
00a0585c: ldp      w1, w2, [x9, #0x10]
00a05860: ldr      w3, [x8, #0x14]
00a05864: mov      x0, x19
00a05868: bl       #0xa059f0 | CoreGame.ScenePosGroup$$AddScrewPos
00a0586c: b        #0xa0583c | 
00a05870: ldr      x1, [x22]
00a05874: add      x0, sp, #0x20
00a05878: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
00a0587c: ldr      w8, [x19, #0x24]
00a05880: cmp      w8, #1
00a05884: b.lt     #0xa058c8
00a05888: mov      w20, wzr
00a0588c: mov      x0, x19
00a05890: mov      w1, w20
00a05894: bl       #0xa05d2c | CoreGame.ScenePosGroup$$ResetScrewPos
00a05898: ldr      w8, [x19, #0x24]
00a0589c: add      w20, w20, #1
00a058a0: cmp      w20, w8
00a058a4: b.lt     #0xa0588c
00a058a8: b        #0xa058c8 | 
00a058ac: mov      w2, #1
00a058b0: mov      x0, x19
00a058b4: mov      x1, x20
00a058b8: bl       #0xa05440 | CoreGame.ScenePosGroup$$Init
00a058bc: ldr      x1, [x22]
00a058c0: add      x0, sp, #0x20
00a058c4: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
00a058c8: ldp      x19, x30, [sp, #0xb0]
00a058cc: ldp      x21, x20, [sp, #0xa0]
00a058d0: ldp      x23, x22, [sp, #0x90]
00a058d4: ldp      x25, x24, [sp, #0x80]
00a058d8: ldr      x26, [sp, #0x70]
00a058dc: ldp      d9, d8, [sp, #0x60]
00a058e0: add      sp, sp, #0xc0
00a058e4: ret      
00a058e8: bl       #0x925b54 | 
00a058ec: bl       #0x925b54 | 
00a058f0: bl       #0x925b54 | 
00a058f4: bl       #0x925b54 | 
00a058f8: str      x0, [sp, #0x78]
00a058fc: mov      x21, xzr
00a05900: b        #0xa05914 | 
00a05904: b        #0xa0593c | 
00a05908: b        #0xa0593c | 
00a0590c: b        #0xa05994 | 
00a05910: str      x0, [sp, #0x78]
00a05914: ldr      x1, [x23]
00a05918: add      x0, sp, #0x40
00a0591c: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
00a05920: cbnz     x21, #0xa0592c
00a05924: ldr      x0, [sp, #0x78]
00a05928: bl       #0x6c0230 | 
00a0592c: mov      x0, x21
00a05930: bl       #0x89bb08 | 
00a05934: b        #0xa05994 | 
00a05938: b        #0xa0593c | 
00a0593c: cmp      w1, #1
00a05940: str      x0, [sp, #0x78]
00a05944: b.ne     #0xa05970
00a05948: ldr      x0, [sp, #0x78]
00a0594c: bl       #0x6c0f60 | 
00a05950: ldr      x20, [x0]
00a05954: bl       #0x6c03b0 | 
00a05958: ldr      x1, [x22]
00a0595c: add      x0, sp, #0x20
00a05960: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
00a05964: cbz      x20, #0xa0587c
00a05968: mov      x0, x20
00a0596c: bl       #0x89bb08 | 
00a05970: mov      x20, xzr
00a05974: b        #0xa0597c | 
00a05978: str      x0, [sp, #0x78]
00a0597c: ldr      x1, [x22]
00a05980: add      x0, sp, #0x20
00a05984: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
00a05988: cbz      x20, #0xa05924
00a0598c: mov      x0, x20
00a05990: bl       #0x89bb08 | 
00a05994: cmp      w1, #1
00a05998: str      x0, [sp, #0x78]
00a0599c: b.ne     #0xa059c8
00a059a0: ldr      x0, [sp, #0x78]
00a059a4: bl       #0x6c0f60 | 
00a059a8: ldr      x21, [x0]
00a059ac: bl       #0x6c03b0 | 
00a059b0: ldr      x1, [x22]
00a059b4: add      x0, sp, #0x20
00a059b8: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
00a059bc: cbz      x21, #0xa05808
00a059c0: mov      x0, x21
00a059c4: bl       #0x89bb08 | 
00a059c8: mov      x21, xzr
00a059cc: b        #0xa059d4 | 
00a059d0: str      x0, [sp, #0x78]
00a059d4: ldr      x1, [x22]
00a059d8: add      x0, sp, #0x20
00a059dc: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
00a059e0: cbz      x21, #0xa05924
00a059e4: mov      x0, x21
00a059e8: bl       #0x89bb08 | 
00a059ec: bl       #0x6c29a8 | 