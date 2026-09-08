// CoreGame.ScrewInfo.<>c__DisplayClass47_1$$<Operator>b__1 RVA 0xa0adf4; next known entry 0xa0af38; boundary requires review.
00a0adf4: str      x20, [sp, #-0x20]!
00a0adf8: stp      x19, x30, [sp, #0x10]
00a0adfc: adrp     x20, #0x2ca6000
00a0ae00: ldrb     w8, [x20, #0x9d3]
00a0ae04: mov      x19, x0
00a0ae08: tbnz     w8, #0, #0xa0ae44
00a0ae0c: adrp     x0, #0x2ad6000
00a0ae10: ldr      x0, [x0, #0xa48] | 'System.Action_TypeInfo'
00a0ae14: bl       #0x925a30 | 
00a0ae18: adrp     x0, #0x2af5000
00a0ae1c: ldr      x0, [x0, #0xdf0] | 'Method$CoreGame.ScrewInfo.<Operator>b__47_3()'
00a0ae20: bl       #0x925a30 | 
00a0ae24: adrp     x0, #0x2aef000
00a0ae28: ldr      x0, [x0, #0x668] | 'Method$Util.Singleton<LuoSiSortMgr>.get_Instance()'
00a0ae2c: bl       #0x925a30 | 
00a0ae30: adrp     x0, #0x2abc000
00a0ae34: ldr      x0, [x0, #0xb88] | 'Util.Singleton<LuoSiSortMgr>_TypeInfo'
00a0ae38: bl       #0x925a30 | 
00a0ae3c: mov      w8, #1
00a0ae40: strb     w8, [x20, #0x9d3]
00a0ae44: ldr      x8, [x19, #0x18]
00a0ae48: cbz      x8, #0xa0af34
00a0ae4c: ldr      w9, [x8, #0x10]
00a0ae50: ldr      w10, [x19, #0x10]
00a0ae54: sub      w9, w9, #1
00a0ae58: cmp      w10, w9
00a0ae5c: b.ne     #0xa0af28
00a0ae60: ldr      x0, [x8, #0x18]
00a0ae64: cbz      x0, #0xa0af34
00a0ae68: bl       #0xa03054 | CoreGame.ScrewInfo$$get_IsDone
00a0ae6c: ldr      x8, [x19, #0x18]
00a0ae70: cbz      x8, #0xa0af34
00a0ae74: ldr      x20, [x8, #0x18]
00a0ae78: tbz      w0, #0, #0xa0aec8
00a0ae7c: adrp     x8, #0x2ad6000
00a0ae80: ldr      x8, [x8, #0xa48] | 'System.Action_TypeInfo'
00a0ae84: ldr      x0, [x8]
00a0ae88: bl       #0x925b44 | 
00a0ae8c: cbz      x0, #0xa0af34
00a0ae90: adrp     x8, #0x2af5000
00a0ae94: ldr      x8, [x8, #0xdf0] | 'Method$CoreGame.ScrewInfo.<Operator>b__47_3()'
00a0ae98: mov      x1, x20
00a0ae9c: mov      x3, xzr
00a0aea0: mov      x19, x0
00a0aea4: ldr      x2, [x8]
00a0aea8: bl       #0x14bcd00 | System.Action$$.ctor
00a0aeac: cbz      x20, #0xa0af34
00a0aeb0: ldr      x0, [x20, #0x40]
00a0aeb4: cbz      x0, #0xa0af34
00a0aeb8: mov      x1, x19
00a0aebc: ldp      x19, x30, [sp, #0x10]
00a0aec0: ldr      x20, [sp], #0x20
00a0aec4: b        #0xa073bc | CoreGame.Screw$$Done
00a0aec8: cbz      x20, #0xa0af34
00a0aecc: adrp     x8, #0x2abc000
00a0aed0: ldr      x8, [x8, #0xb88] | 'Util.Singleton<LuoSiSortMgr>_TypeInfo'
00a0aed4: mov      w9, #1
00a0aed8: strb     w9, [x20, #0x48]
00a0aedc: adrp     x20, #0x2aef000
00a0aee0: ldr      x0, [x8]
00a0aee4: ldr      w8, [x0, #0xe0]
00a0aee8: ldr      x20, [x20, #0x668] | 'Method$Util.Singleton<LuoSiSortMgr>.get_Instance()'
00a0aeec: cbnz     w8, #0xa0aef4
00a0aef0: bl       #0x925b30 | 
00a0aef4: ldr      x0, [x20]
00a0aef8: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
00a0aefc: cbz      x0, #0xa0af34
00a0af00: ldrb     w8, [x0, #0x35]
00a0af04: cbz      w8, #0xa0af28
00a0af08: ldr      x8, [x19, #0x18]
00a0af0c: cbz      x8, #0xa0af34
00a0af10: ldr      x0, [x8, #0x18]
00a0af14: cbz      x0, #0xa0af34
00a0af18: ldp      x19, x30, [sp, #0x10]
00a0af1c: mov      w1, #1
00a0af20: ldr      x20, [sp], #0x20
00a0af24: b        #0xa09bc4 | CoreGame.ScrewInfo$$DoneEvent
00a0af28: ldp      x19, x30, [sp, #0x10]
00a0af2c: ldr      x20, [sp], #0x20
00a0af30: ret      
00a0af34: bl       #0x925b54 | 