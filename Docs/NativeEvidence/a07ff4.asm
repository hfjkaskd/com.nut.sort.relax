// CoreGame.ScrewInfo$$get_IsHidden RVA 0xa07ff4; next known entry 0xa080a0; boundary requires review.
00a07ff4: str      x20, [sp, #-0x20]!
00a07ff8: stp      x19, x30, [sp, #0x10]
00a07ffc: adrp     x20, #0x2ca6000
00a08000: ldrb     w8, [x20, #0x9be]
00a08004: mov      x19, x0
00a08008: tbnz     w8, #0, #0xa0802c
00a0800c: adrp     x0, #0x2ad4000
00a08010: ldr      x0, [x0, #0xe40] | 'Method$System.Collections.Generic.List<ScrewTypeData>.get_Count()'
00a08014: bl       #0x925a30 | 
00a08018: adrp     x0, #0x2ac2000
00a0801c: ldr      x0, [x0, #0xd20] | 'Method$System.Collections.Generic.List<ScrewTypeData>.get_Item()'
00a08020: bl       #0x925a30 | 
00a08024: mov      w8, #1
00a08028: strb     w8, [x20, #0x9be]
00a0802c: ldr      x0, [x19, #0x30]
00a08030: cbz      x0, #0xa0809c
00a08034: ldr      w8, [x0, #0x18]
00a08038: cmp      w8, #1
00a0803c: b.lt     #0xa0808c
00a08040: adrp     x20, #0x2ac2000
00a08044: ldr      x20, [x20, #0xd20] | 'Method$System.Collections.Generic.List<ScrewTypeData>.get_Item()'
00a08048: mov      w1, wzr
00a0804c: ldr      x2, [x20]
00a08050: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
00a08054: cbz      x0, #0xa0809c
00a08058: ldr      w8, [x0, #0x10]
00a0805c: cmp      w8, #7
00a08060: b.ne     #0xa0808c
00a08064: ldr      x0, [x19, #0x30]
00a08068: cbz      x0, #0xa0809c
00a0806c: ldr      x2, [x20]
00a08070: mov      w1, wzr
00a08074: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
00a08078: cbz      x0, #0xa0809c
00a0807c: ldrb     w8, [x0, #0x20]
00a08080: cmp      w8, #0
00a08084: cset     w0, ne
00a08088: b        #0xa08090 | 
00a0808c: mov      w0, wzr
00a08090: ldp      x19, x30, [sp, #0x10]
00a08094: ldr      x20, [sp], #0x20
00a08098: ret      
00a0809c: bl       #0x925b54 | 