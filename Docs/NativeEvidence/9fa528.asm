// CoreGame.LevelInfo$$AddScrewData RVA 0x9fa528; next known entry 0x9fab20; boundary requires review.
009fa528: sub      sp, sp, #0xa0
009fa52c: stp      x28, x27, [sp, #0x40]
009fa530: stp      x26, x25, [sp, #0x50]
009fa534: stp      x24, x23, [sp, #0x60]
009fa538: stp      x22, x21, [sp, #0x70]
009fa53c: stp      x20, x19, [sp, #0x80]
009fa540: stp      x29, x30, [sp, #0x90]
009fa544: adrp     x23, #0x2ca6000
009fa548: adrp     x19, #0x2ae6000
009fa54c: ldrb     w8, [x23, #0x95e]
009fa550: ldr      x19, [x19, #0xb8] | 'CoreGame.ScrewInfo_TypeInfo'
009fa554: mov      w22, w2
009fa558: mov      x20, x1
009fa55c: mov      x21, x0
009fa560: tbnz     w8, #0, #0x9fa620
009fa564: adrp     x0, #0x2add000
009fa568: ldr      x0, [x0, #0xd38] | 'Method$System.Collections.Generic.List.Enumerator<OBIMData>.Dispose()'
009fa56c: bl       #0x925a30 | 
009fa570: adrp     x0, #0x2ab0000
009fa574: ldr      x0, [x0, #0xe40] | 'Method$System.Collections.Generic.List.Enumerator<OBIMData>.MoveNext()'
009fa578: bl       #0x925a30 | 
009fa57c: adrp     x0, #0x2acd000
009fa580: ldr      x0, [x0, #0x618] | 'Method$System.Collections.Generic.List.Enumerator<OBIMData>.get_Current()'
009fa584: bl       #0x925a30 | 
009fa588: adrp     x0, #0x2ad4000
009fa58c: ldr      x0, [x0, #0xa20] | 'Method$System.Collections.Generic.List<ScrewTypeData>.Add()'
009fa590: bl       #0x925a30 | 
009fa594: adrp     x0, #0x2ab5000
009fa598: ldr      x0, [x0, #0x360] | 'Method$System.Collections.Generic.List<ScrewInfo>.Add()'
009fa59c: bl       #0x925a30 | 
009fa5a0: adrp     x0, #0x2ac0000
009fa5a4: ldr      x0, [x0, #0xf78] | 'Method$System.Collections.Generic.List<NutInfo>.Add()'
009fa5a8: bl       #0x925a30 | 
009fa5ac: adrp     x0, #0x2ae5000
009fa5b0: ldr      x0, [x0, #0x1f0] | 'Method$System.Collections.Generic.List<OBIMData>.GetEnumerator()'
009fa5b4: bl       #0x925a30 | 
009fa5b8: adrp     x0, #0x2ada000
009fa5bc: ldr      x0, [x0, #0xad8] | 'Method$System.Collections.Generic.List<CData>.get_Count()'
009fa5c0: bl       #0x925a30 | 
009fa5c4: adrp     x0, #0x2aee000
009fa5c8: ldr      x0, [x0, #0x1f0] | 'Method$System.Collections.Generic.List<CData>.get_Item()'
009fa5cc: bl       #0x925a30 | 
009fa5d0: adrp     x0, #0x2ae1000
009fa5d4: ldr      x0, [x0, #0x630] | 'CoreGame.NutData_TypeInfo'
009fa5d8: bl       #0x925a30 | 
009fa5dc: adrp     x0, #0x2af0000
009fa5e0: ldr      x0, [x0, #0x158] | 'CoreGame.NutInfo_TypeInfo'
009fa5e4: bl       #0x925a30 | 
009fa5e8: adrp     x0, #0x2af4000
009fa5ec: ldr      x0, [x0, #0xb90] | 'CoreGame.NutPos_TypeInfo'
009fa5f0: bl       #0x925a30 | 
009fa5f4: adrp     x0, #0x2ae6000
009fa5f8: ldr      x0, [x0, #0xb8] | 'CoreGame.ScrewInfo_TypeInfo'
009fa5fc: bl       #0x925a30 | 
009fa600: adrp     x0, #0x2ac1000
009fa604: ldr      x0, [x0, #0x558] | 'CoreGame.ScrewTypeDataObj_TypeInfo'
009fa608: bl       #0x925a30 | 
009fa60c: adrp     x0, #0x2aec000
009fa610: ldr      x0, [x0, #0x560] | 'CoreGame.ScrewTypeData_TypeInfo'
009fa614: bl       #0x925a30 | 
009fa618: mov      w8, #1
009fa61c: strb     w8, [x23, #0x95e]
009fa620: ldr      x0, [x19]
009fa624: stp      xzr, xzr, [sp, #0x28]
009fa628: str      xzr, [sp, #0x20]
009fa62c: bl       #0x925b44 | 
009fa630: cbz      x0, #0x9fa900
009fa634: mov      x1, xzr
009fa638: mov      x19, x0
009fa63c: bl       #0xa08128 | CoreGame.ScrewInfo$$.ctor
009fa640: cbz      x20, #0x9fa900
009fa644: ldr      w8, [x20, #0x10]
009fa648: stp      w8, w22, [x19, #0x10]
009fa64c: ldr      x8, [x20, #0x18]
009fa650: cbz      x8, #0x9fa900
009fa654: ldr      w8, [x8, #0x18]
009fa658: str      w8, [x19, #0x18]
009fa65c: ldr      x0, [x21, #0x10]
009fa660: cbz      x0, #0x9fa900
009fa664: adrp     x9, #0x2ab5000
009fa668: ldr      x9, [x9, #0x360] | 'Method$System.Collections.Generic.List<ScrewInfo>.Add()'
009fa66c: ldr      w10, [x0, #0x1c]
009fa670: ldr      x8, [x0, #0x10]
009fa674: ldr      x9, [x9]
009fa678: add      w10, w10, #1
009fa67c: str      w10, [x0, #0x1c]
009fa680: cbz      x8, #0x9fa900
009fa684: ldrsw    x10, [x0, #0x18]
009fa688: ldr      w11, [x8, #0x18]
009fa68c: cmp      w10, w11
009fa690: b.hs     #0x9fa6b4
009fa694: add      w9, w10, #1
009fa698: add      x8, x8, x10, lsl #3
009fa69c: str      w9, [x0, #0x18]
009fa6a0: str      x19, [x8, #0x20]!
009fa6a4: mov      x0, x8
009fa6a8: mov      x1, x19
009fa6ac: bl       #0x9259e4 | 
009fa6b0: b        #0x9fa6cc | 
009fa6b4: ldr      x8, [x9, #0x20]
009fa6b8: mov      x1, x19
009fa6bc: ldr      x8, [x8, #0xc0]
009fa6c0: ldr      x2, [x8, #0x58]
009fa6c4: ldr      x8, [x2, #8]
009fa6c8: blr      x8
009fa6cc: ldr      x8, [x20, #0x18]
009fa6d0: cbz      x8, #0x9fa900
009fa6d4: adrp     x24, #0x2af0000
009fa6d8: adrp     x29, #0x2af4000
009fa6dc: adrp     x28, #0x2aee000
009fa6e0: adrp     x26, #0x2ac0000
009fa6e4: adrp     x27, #0x2ae1000
009fa6e8: adrp     x25, #0x2ab0000
009fa6ec: ldr      x24, [x24, #0x158] | 'CoreGame.NutInfo_TypeInfo'
009fa6f0: ldr      x29, [x29, #0xb90] | 'CoreGame.NutPos_TypeInfo'
009fa6f4: ldr      x28, [x28, #0x1f0] | 'Method$System.Collections.Generic.List<CData>.get_Item()'
009fa6f8: ldr      x26, [x26, #0xf78] | 'Method$System.Collections.Generic.List<NutInfo>.Add()'
009fa6fc: ldr      x27, [x27, #0x630] | 'CoreGame.NutData_TypeInfo'
009fa700: ldr      x25, [x25, #0xe40] | 'Method$System.Collections.Generic.List.Enumerator<OBIMData>.MoveNext()'
009fa704: mov      w21, wzr
009fa708: ldr      w8, [x8, #0x18]
009fa70c: cmp      w21, w8
009fa710: b.ge     #0x9fa904
009fa714: ldr      x0, [x24]
009fa718: bl       #0x925b44 | 
009fa71c: cbz      x0, #0x9fa900
009fa720: mov      x1, xzr
009fa724: mov      x22, x0
009fa728: bl       #0xa048e8 | CoreGame.NutInfo$$.ctor
009fa72c: ldr      x0, [x29]
009fa730: bl       #0x925b44 | 
009fa734: cbz      x0, #0x9fa900
009fa738: mov      x1, xzr
009fa73c: mov      x23, x0
009fa740: bl       #0xa023b8 | CoreGame.NutPos$$.ctor
009fa744: ldr      x0, [x20, #0x18]
009fa748: cbz      x0, #0x9fa900
009fa74c: ldr      x2, [x28]
009fa750: mov      w1, w21
009fa754: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
009fa758: cbz      x0, #0x9fa900
009fa75c: ldr      x8, [x0, #0x10]
009fa760: cbz      x8, #0x9fa900
009fa764: ldr      w8, [x8, #0x10]
009fa768: str      w8, [x23, #0x10]
009fa76c: ldr      x0, [x20, #0x18]
009fa770: cbz      x0, #0x9fa900
009fa774: ldr      x2, [x28]
009fa778: mov      w1, w21
009fa77c: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
009fa780: cbz      x0, #0x9fa900
009fa784: ldr      x8, [x0, #0x10]
009fa788: cbz      x8, #0x9fa900
009fa78c: ldr      w8, [x8, #0x14]
009fa790: str      w8, [x23, #0x14]
009fa794: ldr      x0, [x20, #0x18]
009fa798: cbz      x0, #0x9fa900
009fa79c: ldr      x2, [x28]
009fa7a0: mov      w1, w21
009fa7a4: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
009fa7a8: cbz      x0, #0x9fa900
009fa7ac: ldr      x8, [x0, #0x10]
009fa7b0: cbz      x8, #0x9fa900
009fa7b4: ldr      w8, [x8, #0x18]
009fa7b8: mov      x0, x22
009fa7bc: mov      x1, x23
009fa7c0: str      w8, [x23, #0x18]
009fa7c4: str      x23, [x0, #0x10]!
009fa7c8: bl       #0x9259e4 | 
009fa7cc: ldr      x0, [x20, #0x18]
009fa7d0: cbz      x0, #0x9fa900
009fa7d4: ldr      x2, [x28]
009fa7d8: mov      w1, w21
009fa7dc: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
009fa7e0: cbz      x0, #0x9fa900
009fa7e4: ldr      x8, [x0, #0x18]
009fa7e8: cbz      x8, #0x9fa88c
009fa7ec: ldr      x0, [x27]
009fa7f0: bl       #0x925b44 | 
009fa7f4: cbz      x0, #0x9fa900
009fa7f8: mov      x1, xzr
009fa7fc: mov      x23, x0
009fa800: bl       #0xa04114 | CoreGame.NutData$$.ctor
009fa804: ldr      x0, [x20, #0x18]
009fa808: cbz      x0, #0x9fa900
009fa80c: ldr      x2, [x28]
009fa810: mov      w1, w21
009fa814: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
009fa818: cbz      x0, #0x9fa900
009fa81c: ldr      x8, [x0, #0x18]
009fa820: cbz      x8, #0x9fa900
009fa824: ldr      w8, [x8, #0x10]
009fa828: str      w8, [x23, #0x10]
009fa82c: ldr      x0, [x20, #0x18]
009fa830: cbz      x0, #0x9fa900
009fa834: ldr      x2, [x28]
009fa838: mov      w1, w21
009fa83c: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
009fa840: cbz      x0, #0x9fa900
009fa844: ldr      x8, [x0, #0x18]
009fa848: cbz      x8, #0x9fa900
009fa84c: ldr      w8, [x8, #0x14]
009fa850: str      w8, [x23, #0x14]
009fa854: ldr      x0, [x20, #0x18]
009fa858: cbz      x0, #0x9fa900
009fa85c: ldr      x2, [x28]
009fa860: mov      w1, w21
009fa864: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
009fa868: cbz      x0, #0x9fa900
009fa86c: ldr      x8, [x0, #0x18]
009fa870: cbz      x8, #0x9fa900
009fa874: ldrb     w8, [x8, #0x18]
009fa878: mov      x0, x22
009fa87c: mov      x1, x23
009fa880: strb     w8, [x23, #0x18]
009fa884: str      x23, [x0, #0x18]!
009fa888: bl       #0x9259e4 | 
009fa88c: ldr      x0, [x19, #0x28]
009fa890: cbz      x0, #0x9fa900
009fa894: ldr      w10, [x0, #0x1c]
009fa898: ldr      x8, [x0, #0x10]
009fa89c: ldr      x9, [x26]
009fa8a0: add      w10, w10, #1
009fa8a4: str      w10, [x0, #0x1c]
009fa8a8: cbz      x8, #0x9fa900
009fa8ac: ldrsw    x10, [x0, #0x18]
009fa8b0: ldr      w11, [x8, #0x18]
009fa8b4: cmp      w10, w11
009fa8b8: b.hs     #0x9fa8dc
009fa8bc: add      w9, w10, #1
009fa8c0: add      x8, x8, x10, lsl #3
009fa8c4: str      w9, [x0, #0x18]
009fa8c8: str      x22, [x8, #0x20]!
009fa8cc: mov      x0, x8
009fa8d0: mov      x1, x22
009fa8d4: bl       #0x9259e4 | 
009fa8d8: b        #0x9fa8f4 | 
009fa8dc: ldr      x8, [x9, #0x20]
009fa8e0: mov      x1, x22
009fa8e4: ldr      x8, [x8, #0xc0]
009fa8e8: ldr      x2, [x8, #0x58]
009fa8ec: ldr      x8, [x2, #8]
009fa8f0: blr      x8
009fa8f4: ldr      x8, [x20, #0x18]
009fa8f8: add      w21, w21, #1
009fa8fc: cbnz     x8, #0x9fa708
009fa900: bl       #0x925b54 | 
009fa904: ldr      x0, [x20, #0x20]
009fa908: cbz      x0, #0x9faa50
009fa90c: adrp     x8, #0x2ae5000
009fa910: ldr      x8, [x8, #0x1f0] | 'Method$System.Collections.Generic.List<OBIMData>.GetEnumerator()'
009fa914: ldr      x1, [x8]
009fa918: add      x8, sp, #8
009fa91c: bl       #0xf9db8c | System.Collections.Generic.List<object>$$GetEnumerator
009fa920: ldur     q0, [sp, #8]
009fa924: ldr      x8, [sp, #0x18]
009fa928: adrp     x23, #0x2aec000
009fa92c: adrp     x24, #0x2ad4000
009fa930: str      q0, [sp, #0x20]
009fa934: str      x8, [sp, #0x30]
009fa938: adrp     x26, #0x2ac1000
009fa93c: ldr      x23, [x23, #0x560] | 'CoreGame.ScrewTypeData_TypeInfo'
009fa940: ldr      x24, [x24, #0xa20] | 'Method$System.Collections.Generic.List<ScrewTypeData>.Add()'
009fa944: ldr      x26, [x26, #0x558] | 'CoreGame.ScrewTypeDataObj_TypeInfo'
009fa948: ldr      x1, [x25]
009fa94c: add      x0, sp, #0x20
009fa950: bl       #0xdaa75c | System.Collections.Generic.List.Enumerator<object>$$MoveNext
009fa954: tbz      w0, #0, #0x9faa3c
009fa958: ldr      x22, [sp, #0x30]
009fa95c: ldr      x0, [x23]
009fa960: bl       #0x925b44 | 
009fa964: mov      x20, x0
009fa968: cbz      x0, #0x9faa70
009fa96c: mov      x0, x20
009fa970: mov      x1, xzr
009fa974: bl       #0xa0b50c | CoreGame.ScrewTypeData$$.ctor
009fa978: cbz      x22, #0x9faa74
009fa97c: ldr      x8, [x22, #0x10]
009fa980: str      x8, [x20, #0x10]
009fa984: ldr      x8, [x22, #0x18]
009fa988: cbz      x8, #0x9fa9d0
009fa98c: ldr      x0, [x26]
009fa990: bl       #0x925b44 | 
009fa994: mov      x21, x0
009fa998: cbz      x0, #0x9faa84
009fa99c: mov      x0, x21
009fa9a0: mov      x1, xzr
009fa9a4: bl       #0xa0b51c | CoreGame.ScrewTypeDataObj$$.ctor
009fa9a8: ldr      x8, [x22, #0x18]
009fa9ac: cbz      x8, #0x9faa80
009fa9b0: ldr      w9, [x8, #0x18]
009fa9b4: mov      x0, x20
009fa9b8: str      w9, [x21, #0x18]
009fa9bc: ldr      x8, [x8, #0x10]
009fa9c0: str      x8, [x21, #0x10]
009fa9c4: str      x21, [x0, #0x18]!
009fa9c8: mov      x1, x21
009fa9cc: bl       #0x9259e4 | 
009fa9d0: ldr      x0, [x19, #0x30]
009fa9d4: cbz      x0, #0x9faa78
009fa9d8: ldr      w10, [x0, #0x1c]
009fa9dc: ldr      x8, [x0, #0x10]
009fa9e0: ldr      x9, [x24]
009fa9e4: add      w10, w10, #1
009fa9e8: str      w10, [x0, #0x1c]
009fa9ec: cbz      x8, #0x9faa7c
009fa9f0: ldrsw    x10, [x0, #0x18]
009fa9f4: ldr      w11, [x8, #0x18]
009fa9f8: cmp      w10, w11
009fa9fc: b.hs     #0x9faa20
009faa00: add      w9, w10, #1
009faa04: add      x8, x8, x10, lsl #3
009faa08: str      w9, [x0, #0x18]
009faa0c: str      x20, [x8, #0x20]!
009faa10: mov      x0, x8
009faa14: mov      x1, x20
009faa18: bl       #0x9259e4 | 
009faa1c: b        #0x9fa948 | 
009faa20: ldr      x8, [x9, #0x20]
009faa24: ldr      x8, [x8, #0xc0]
009faa28: ldr      x2, [x8, #0x58]
009faa2c: ldr      x8, [x2, #8]
009faa30: mov      x1, x20
009faa34: blr      x8
009faa38: b        #0x9fa948 | 
009faa3c: adrp     x8, #0x2add000
009faa40: ldr      x8, [x8, #0xd38] | 'Method$System.Collections.Generic.List.Enumerator<OBIMData>.Dispose()'
009faa44: add      x0, sp, #0x20
009faa48: ldr      x1, [x8]
009faa4c: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
009faa50: ldp      x29, x30, [sp, #0x90]
009faa54: ldp      x20, x19, [sp, #0x80]
009faa58: ldp      x22, x21, [sp, #0x70]
009faa5c: ldp      x24, x23, [sp, #0x60]
009faa60: ldp      x26, x25, [sp, #0x50]
009faa64: ldp      x28, x27, [sp, #0x40]
009faa68: add      sp, sp, #0xa0
009faa6c: ret      
009faa70: bl       #0x925b54 | 
009faa74: bl       #0x925b54 | 
009faa78: bl       #0x925b54 | 
009faa7c: bl       #0x925b54 | 
009faa80: bl       #0x925b54 | 
009faa84: bl       #0x925b54 | 
009faa88: b        #0x9faaac | 
009faa8c: b        #0x9faaac | 
009faa90: b        #0x9faaac | 
009faa94: b        #0x9faaac | 
009faa98: b        #0x9faaac | 
009faa9c: b        #0x9faaac | 
009faaa0: b        #0x9faaac | 
009faaa4: b        #0x9faaac | 
009faaa8: b        #0x9faaac | 
009faaac: mov      x19, x0
009faab0: cmp      w1, #1
009faab4: b.ne     #0x9faae8
009faab8: mov      x0, x19
009faabc: bl       #0x6c0f60 | 
009faac0: ldr      x20, [x0]
009faac4: bl       #0x6c03b0 | 
009faac8: adrp     x8, #0x2add000
009faacc: ldr      x8, [x8, #0xd38] | 'Method$System.Collections.Generic.List.Enumerator<OBIMData>.Dispose()'
009faad0: add      x0, sp, #0x20
009faad4: ldr      x1, [x8]
009faad8: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
009faadc: cbz      x20, #0x9faa50
009faae0: mov      x0, x20
009faae4: bl       #0x89bb08 | 
009faae8: mov      x20, xzr
009faaec: b        #0x9faaf4 | 
009faaf0: mov      x19, x0
009faaf4: adrp     x8, #0x2add000
009faaf8: ldr      x8, [x8, #0xd38] | 'Method$System.Collections.Generic.List.Enumerator<OBIMData>.Dispose()'
009faafc: ldr      x1, [x8]
009fab00: add      x0, sp, #0x20
009fab04: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
009fab08: cbnz     x20, #0x9fab14
009fab0c: mov      x0, x19
009fab10: bl       #0x6c0230 | 
009fab14: mov      x0, x20
009fab18: bl       #0x89bb08 | 
009fab1c: bl       #0x6c29a8 | 