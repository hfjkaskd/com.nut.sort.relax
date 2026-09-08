// CoreGame.ScrewInfo$$get_IsDone RVA 0xa03054; next known entry 0xa03144; boundary requires review.
00a03054: str      x22, [sp, #-0x30]!
00a03058: stp      x21, x20, [sp, #0x10]
00a0305c: stp      x19, x30, [sp, #0x20]
00a03060: adrp     x20, #0x2ca6000
00a03064: ldrb     w8, [x20, #0x9ba]
00a03068: mov      x19, x0
00a0306c: tbnz     w8, #0, #0xa03090
00a03070: adrp     x0, #0x2aae000
00a03074: ldr      x0, [x0, #0xc28] | 'Method$System.Collections.Generic.List<NutInfo>.get_Count()'
00a03078: bl       #0x925a30 | 
00a0307c: adrp     x0, #0x2ad1000
00a03080: ldr      x0, [x0, #0x1b8] | 'Method$System.Collections.Generic.List<NutInfo>.get_Item()'
00a03084: bl       #0x925a30 | 
00a03088: mov      w8, #1
00a0308c: strb     w8, [x20, #0x9ba]
00a03090: ldr      x0, [x19, #0x28]
00a03094: cbz      x0, #0xa03124
00a03098: adrp     x21, #0x2ad1000
00a0309c: ldr      x21, [x21, #0x1b8] | 'Method$System.Collections.Generic.List<NutInfo>.get_Item()'
00a030a0: mov      w20, #1
00a030a4: ldr      w8, [x0, #0x18]
00a030a8: cmp      w20, w8
00a030ac: b.ge     #0xa03130
00a030b0: ldr      x2, [x21]
00a030b4: mov      w1, w20
00a030b8: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
00a030bc: cbz      x0, #0xa03124
00a030c0: ldr      x8, [x0, #0x18]
00a030c4: cbz      x8, #0xa03128
00a030c8: ldr      x0, [x19, #0x28]
00a030cc: cbz      x0, #0xa03124
00a030d0: ldr      x2, [x21]
00a030d4: mov      w1, w20
00a030d8: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
00a030dc: cbz      x0, #0xa03124
00a030e0: ldr      x8, [x0, #0x18]
00a030e4: cbz      x8, #0xa03124
00a030e8: ldr      x0, [x19, #0x28]
00a030ec: cbz      x0, #0xa03124
00a030f0: ldr      x2, [x21]
00a030f4: ldr      w22, [x8, #0x14]
00a030f8: mov      w1, wzr
00a030fc: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
00a03100: cbz      x0, #0xa03124
00a03104: ldr      x8, [x0, #0x18]
00a03108: cbz      x8, #0xa03124
00a0310c: ldr      w8, [x8, #0x14]
00a03110: cmp      w22, w8
00a03114: b.ne     #0xa03128
00a03118: ldr      x0, [x19, #0x28]
00a0311c: add      w20, w20, #1
00a03120: cbnz     x0, #0xa030a4
00a03124: bl       #0x925b54 | 
00a03128: mov      w0, wzr
00a0312c: b        #0xa03134 | 
00a03130: mov      w0, #1
00a03134: ldp      x19, x30, [sp, #0x20]
00a03138: ldp      x21, x20, [sp, #0x10]
00a0313c: ldr      x22, [sp], #0x30
00a03140: ret      