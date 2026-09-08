// CoreGame.NutInfo$$Move RVA 0xa0458c; next known entry 0xa04744; boundary requires review.
00a0458c: stp      x27, x26, [sp, #-0x50]!
00a04590: stp      x25, x24, [sp, #0x10]
00a04594: stp      x23, x22, [sp, #0x20]
00a04598: stp      x21, x20, [sp, #0x30]
00a0459c: stp      x19, x30, [sp, #0x40]
00a045a0: adrp     x24, #0x2ca6000
00a045a4: ldrb     w8, [x24, #0x9a0]
00a045a8: mov      x20, x4
00a045ac: mov      x19, x3
00a045b0: mov      w22, w2
00a045b4: mov      x23, x1
00a045b8: mov      x21, x0
00a045bc: tbnz     w8, #0, #0xa045ec
00a045c0: adrp     x0, #0x2aee000
00a045c4: ldr      x0, [x0, #0x398] | 'UnityEngine.Debug_TypeInfo'
00a045c8: bl       #0x925a30 | 
00a045cc: adrp     x0, #0x2abc000
00a045d0: ldr      x0, [x0, #0x368] | 'UnityEngine.Object_TypeInfo'
00a045d4: bl       #0x925a30 | 
00a045d8: adrp     x0, #0x2af6000
00a045dc: ldr      x0, [x0, #0x438] | 'nullNutInfo == null'
00a045e0: bl       #0x925a30 | 
00a045e4: mov      w8, #1
00a045e8: strb     w8, [x24, #0x9a0]
00a045ec: cbz      x23, #0xa04740
00a045f0: mov      x0, x23
00a045f4: bl       #0xa04744 | CoreGame.ScrewInfo$$GetNullNutInfo
00a045f8: cbz      x0, #0xa046c0
00a045fc: mov      x24, x21
00a04600: ldr      x1, [x24, #0x18]!
00a04604: adrp     x27, #0x2abc000
00a04608: ldr      x27, [x27, #0x368] | 'UnityEngine.Object_TypeInfo'
00a0460c: mov      x23, x0
00a04610: str      x1, [x0, #0x18]!
00a04614: bl       #0x9259e4 | 
00a04618: mov      x25, x21
00a0461c: ldr      x1, [x25, #0x28]!
00a04620: mov      x26, x23
00a04624: str      x1, [x26, #0x28]!
00a04628: mov      x0, x26
00a0462c: bl       #0x9259e4 | 
00a04630: ldr      x0, [x27]
00a04634: ldr      x27, [x26]
00a04638: ldr      w8, [x0, #0xe0]
00a0463c: cbnz     w8, #0xa04644
00a04640: bl       #0x925b30 | 
00a04644: mov      x0, x27
00a04648: mov      x1, xzr
00a0464c: mov      x2, xzr
00a04650: bl       #0x1e34dfc | UnityEngine.Object$$op_Inequality
00a04654: tbz      w0, #0, #0xa04684
00a04658: ldr      x0, [x26]
00a0465c: cbz      x0, #0xa04740
00a04660: str      x23, [x0, #0x28]!
00a04664: mov      x1, x23
00a04668: bl       #0x9259e4 | 
00a0466c: ldr      x0, [x23, #0x28]
00a04670: cbz      x0, #0xa04740
00a04674: ldrb     w2, [x21, #0x20]
00a04678: mov      w1, w22
00a0467c: mov      x3, x20
00a04680: bl       #0xa03678 | CoreGame.Nut$$Move
00a04684: mov      x0, x24
00a04688: mov      x1, xzr
00a0468c: strb     wzr, [x23, #0x20]
00a04690: strb     wzr, [x21, #0x20]
00a04694: str      xzr, [x21, #0x18]
00a04698: bl       #0x9259e4 | 
00a0469c: mov      x0, x25
00a046a0: mov      x1, xzr
00a046a4: str      xzr, [x21, #0x28]
00a046a8: bl       #0x9259e4 | 
00a046ac: cbz      x19, #0xa04728
00a046b0: ldr      x2, [x19, #0x18]
00a046b4: ldr      x0, [x19, #0x40]
00a046b8: ldr      x1, [x19, #0x28]
00a046bc: b        #0xa04710 | 
00a046c0: adrp     x8, #0x2aee000
00a046c4: ldr      x8, [x8, #0x398] | 'UnityEngine.Debug_TypeInfo'
00a046c8: adrp     x21, #0x2af6000
00a046cc: ldr      x0, [x8]
00a046d0: ldr      w8, [x0, #0xe0]
00a046d4: ldr      x21, [x21, #0x438] | 'nullNutInfo == null'
00a046d8: cbnz     w8, #0xa046e0
00a046dc: bl       #0x925b30 | 
00a046e0: ldr      x0, [x21]
00a046e4: mov      x1, xzr
00a046e8: bl       #0x1e1b538 | UnityEngine.Debug$$LogError
00a046ec: cbz      x19, #0xa04700
00a046f0: ldr      x8, [x19, #0x18]
00a046f4: ldr      x0, [x19, #0x40]
00a046f8: ldr      x1, [x19, #0x28]
00a046fc: blr      x8
00a04700: cbz      x20, #0xa04728
00a04704: ldr      x2, [x20, #0x18]
00a04708: ldr      x0, [x20, #0x40]
00a0470c: ldr      x1, [x20, #0x28]
00a04710: ldp      x19, x30, [sp, #0x40]
00a04714: ldp      x21, x20, [sp, #0x30]
00a04718: ldp      x23, x22, [sp, #0x20]
00a0471c: ldp      x25, x24, [sp, #0x10]
00a04720: ldp      x27, x26, [sp], #0x50
00a04724: br       x2
00a04728: ldp      x19, x30, [sp, #0x40]
00a0472c: ldp      x21, x20, [sp, #0x30]
00a04730: ldp      x23, x22, [sp, #0x20]
00a04734: ldp      x25, x24, [sp, #0x10]
00a04738: ldp      x27, x26, [sp], #0x50
00a0473c: ret      
00a04740: bl       #0x925b54 | 