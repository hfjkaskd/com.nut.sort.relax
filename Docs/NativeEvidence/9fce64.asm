// CoreGame.LuoSiSortMgr$$GetLevelData RVA 0x9fce64; next known entry 0x9fd578; boundary requires review.
009fce64: sub      sp, sp, #0xa0
009fce68: stp      x28, x27, [sp, #0x40]
009fce6c: stp      x26, x25, [sp, #0x50]
009fce70: stp      x24, x23, [sp, #0x60]
009fce74: stp      x22, x21, [sp, #0x70]
009fce78: stp      x20, x19, [sp, #0x80]
009fce7c: stp      x29, x30, [sp, #0x90]
009fce80: adrp     x20, #0x2ca6000
009fce84: adrp     x24, #0x2af4000
009fce88: ldrb     w8, [x20, #0x96e]
009fce8c: ldr      x24, [x24, #0xb98] | 'Util.Singleton<UserMgr>_TypeInfo'
009fce90: mov      x19, x0
009fce94: tbnz     w8, #0, #0x9fcf84
009fce98: adrp     x0, #0x2aee000
009fce9c: ldr      x0, [x0, #0x398] | 'UnityEngine.Debug_TypeInfo'
009fcea0: bl       #0x925a30 | 
009fcea4: adrp     x0, #0x2af4000
009fcea8: ldr      x0, [x0, #0x738] | 'Method$System.Collections.Generic.List.Enumerator<LevelDataInfo>.Dispose()'
009fceac: bl       #0x925a30 | 
009fceb0: adrp     x0, #0x2afe000
009fceb4: ldr      x0, [x0, #0xf20] | 'Method$System.Collections.Generic.List.Enumerator<LevelDataInfo>.MoveNext()'
009fceb8: bl       #0x925a30 | 
009fcebc: adrp     x0, #0x2aee000
009fcec0: ldr      x0, [x0, #0x5a8] | 'Method$System.Collections.Generic.List.Enumerator<LevelDataInfo>.get_Current()'
009fcec4: bl       #0x925a30 | 
009fcec8: adrp     x0, #0x2ab9000
009fcecc: ldr      x0, [x0, #0x718] | 'Method$Newtonsoft.Json.JsonConvert.DeserializeObject<LevelData>()'
009fced0: bl       #0x925a30 | 
009fced4: adrp     x0, #0x2ae9000
009fced8: ldr      x0, [x0, #0xee8] | 'Newtonsoft.Json.JsonConvert_TypeInfo'
009fcedc: bl       #0x925a30 | 
009fcee0: adrp     x0, #0x2ac4000
009fcee4: ldr      x0, [x0, #0x1e0] | 'Method$System.Collections.Generic.List<LevelDataInfo>.GetEnumerator()'
009fcee8: bl       #0x925a30 | 
009fceec: adrp     x0, #0x2ac7000
009fcef0: ldr      x0, [x0, #0x708] | 'Method$System.Collections.Generic.List<string>.get_Count()'
009fcef4: bl       #0x925a30 | 
009fcef8: adrp     x0, #0x2abc000
009fcefc: ldr      x0, [x0, #0xf60] | 'Method$System.Collections.Generic.List<string>.get_Item()'
009fcf00: bl       #0x925a30 | 
009fcf04: adrp     x0, #0x2aea000
009fcf08: ldr      x0, [x0, #0x898] | 'Method$Util.Singleton<ResourceMgr>.get_Instance()'
009fcf0c: bl       #0x925a30 | 
009fcf10: adrp     x0, #0x2afe000
009fcf14: ldr      x0, [x0, #0x230] | 'Method$Util.Singleton<UserMgr>.get_Instance()'
009fcf18: bl       #0x925a30 | 
009fcf1c: adrp     x0, #0x2ae9000
009fcf20: ldr      x0, [x0, #0x458] | 'Util.Singleton<ResourceMgr>_TypeInfo'
009fcf24: bl       #0x925a30 | 
009fcf28: adrp     x0, #0x2af4000
009fcf2c: ldr      x0, [x0, #0xb98] | 'Util.Singleton<UserMgr>_TypeInfo'
009fcf30: bl       #0x925a30 | 
009fcf34: adrp     x0, #0x2ad3000
009fcf38: ldr      x0, [x0, #0xd28] | 'd2d6e6b5210738f3'
009fcf3c: bl       #0x925a30 | 
009fcf40: adrp     x0, #0x2aeb000
009fcf44: ldr      x0, [x0, #0xbb8] | 'GetLevelData is null !'
009fcf48: bl       #0x925a30 | 
009fcf4c: adrp     x0, #0x2afc000
009fcf50: ldr      x0, [x0, #0x9d8] | 'Level0'
009fcf54: bl       #0x925a30 | 
009fcf58: adrp     x0, #0x2abc000
009fcf5c: ldr      x0, [x0, #0x190] | '/'
009fcf60: bl       #0x925a30 | 
009fcf64: adrp     x0, #0x2abe000
009fcf68: ldr      x0, [x0, #0x98] | 'Level1'
009fcf6c: bl       #0x925a30 | 
009fcf70: adrp     x0, #0x2ac3000
009fcf74: ldr      x0, [x0, #0x2a0] | 'LevelConfig/'
009fcf78: bl       #0x925a30 | 
009fcf7c: mov      w8, #1
009fcf80: strb     w8, [x20, #0x96e]
009fcf84: ldr      x0, [x24]
009fcf88: stp      xzr, xzr, [sp, #0x28]
009fcf8c: str      xzr, [sp, #0x20]
009fcf90: adrp     x25, #0x2afe000
009fcf94: ldr      w8, [x0, #0xe0]
009fcf98: ldr      x25, [x25, #0x230] | 'Method$Util.Singleton<UserMgr>.get_Instance()'
009fcf9c: cbnz     w8, #0x9fcfa4
009fcfa0: bl       #0x925b30 | 
009fcfa4: ldr      x0, [x25]
009fcfa8: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fcfac: cbz      x0, #0x9fd44c
009fcfb0: ldr      x8, [x0, #0x18]
009fcfb4: cbz      x8, #0x9fd44c
009fcfb8: ldr      w9, [x8, #0x2c]
009fcfbc: ldr      x8, [x19, #0x10]
009fcfc0: add      w10, w9, #3
009fcfc4: cmp      w9, #2
009fcfc8: csel     w21, w9, w10, lt
009fcfcc: cbz      x8, #0x9fd44c
009fcfd0: cmp      w21, #0xdc
009fcfd4: b.ge     #0x9fd088
009fcfd8: ldr      x0, [x24]
009fcfdc: ldr      x20, [x8, #0x10]
009fcfe0: ldr      w9, [x0, #0xe0]
009fcfe4: cbnz     w9, #0x9fcfec
009fcfe8: bl       #0x925b30 | 
009fcfec: ldr      x0, [x25]
009fcff0: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fcff4: cbz      x0, #0x9fd44c
009fcff8: ldr      x8, [x0, #0x18]
009fcffc: cbz      x8, #0x9fd44c
009fd000: ldr      x8, [x8, #0x88]
009fd004: cbz      x8, #0x9fd44c
009fd008: ldrb     w8, [x8, #0x58]
009fd00c: cbz      w8, #0x9fd0c4
009fd010: cmp      w21, #5
009fd014: b.eq     #0x9fd3f0
009fd018: cmp      w21, #1
009fd01c: b.ne     #0x9fd0c4
009fd020: ldr      x0, [x24]
009fd024: ldr      w8, [x0, #0xe0]
009fd028: cbnz     w8, #0x9fd030
009fd02c: bl       #0x925b30 | 
009fd030: ldr      x0, [x25]
009fd034: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fd038: cbz      x0, #0x9fd44c
009fd03c: ldr      x22, [x0, #0x18]
009fd040: ldr      x0, [x25]
009fd044: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fd048: cbz      x0, #0x9fd44c
009fd04c: ldr      x8, [x0, #0x18]
009fd050: cbz      x8, #0x9fd44c
009fd054: ldr      x8, [x8, #0x88]
009fd058: cbz      x8, #0x9fd44c
009fd05c: ldr      x0, [x25]
009fd060: ldr      w21, [x8, #0x5c]
009fd064: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fd068: cbz      x0, #0x9fd44c
009fd06c: ldr      x8, [x0, #0x18]
009fd070: cbz      x8, #0x9fd44c
009fd074: cbz      x22, #0x9fd44c
009fd078: ldr      w8, [x8, #0x34]
009fd07c: mov      w27, wzr
009fd080: add      w21, w8, w21
009fd084: b        #0x9fd10c | 
009fd088: ldr      x8, [x19, #0x18]
009fd08c: cbz      x8, #0x9fd44c
009fd090: ldr      x0, [x24]
009fd094: ldr      x20, [x8, #0x10]
009fd098: ldr      w9, [x0, #0xe0]
009fd09c: cbnz     w9, #0x9fd0a4
009fd0a0: bl       #0x925b30 | 
009fd0a4: ldr      x0, [x25]
009fd0a8: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fd0ac: cbz      x0, #0x9fd44c
009fd0b0: ldr      x22, [x0, #0x18]
009fd0b4: cmp      w21, #0x12c
009fd0b8: b.ge     #0x9fd0f0
009fd0bc: cbnz     x22, #0x9fd108
009fd0c0: b        #0x9fd44c | 
009fd0c4: ldr      x0, [x24]
009fd0c8: ldr      w8, [x0, #0xe0]
009fd0cc: cbnz     w8, #0x9fd0d4
009fd0d0: bl       #0x925b30 | 
009fd0d4: ldr      x0, [x25]
009fd0d8: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fd0dc: cbz      x0, #0x9fd44c
009fd0e0: ldr      x22, [x0, #0x18]
009fd0e4: cbz      x22, #0x9fd44c
009fd0e8: mov      w27, wzr
009fd0ec: b        #0x9fd10c | 
009fd0f0: mov      w0, #0xdc
009fd0f4: mov      w1, #0x12c
009fd0f8: mov      x2, xzr
009fd0fc: bl       #0x1e11ad4 | UnityEngine.Random$$Range
009fd100: cbz      x22, #0x9fd44c
009fd104: mov      w21, w0
009fd108: mov      w27, #1
009fd10c: str      w21, [x22, #0x30]
009fd110: cbz      x20, #0x9fd44c
009fd114: adrp     x8, #0x2ac4000
009fd118: ldr      x8, [x8, #0x1e0] | 'Method$System.Collections.Generic.List<LevelDataInfo>.GetEnumerator()'
009fd11c: adrp     x22, #0x2afe000
009fd120: adrp     x21, #0x2abc000
009fd124: adrp     x29, #0x2ae9000
009fd128: adrp     x28, #0x2aea000
009fd12c: adrp     x23, #0x2ac3000
009fd130: adrp     x26, #0x2afc000
009fd134: ldr      x22, [x22, #0xf20] | 'Method$System.Collections.Generic.List.Enumerator<LevelDataInfo>.MoveNext()'
009fd138: ldr      x21, [x21, #0xf60] | 'Method$System.Collections.Generic.List<string>.get_Item()'
009fd13c: ldr      x29, [x29, #0x458] | 'Util.Singleton<ResourceMgr>_TypeInfo'
009fd140: ldr      x28, [x28, #0x898] | 'Method$Util.Singleton<ResourceMgr>.get_Instance()'
009fd144: ldr      x23, [x23, #0x2a0] | 'LevelConfig/'
009fd148: ldr      x26, [x26, #0x9d8] | 'Level0'
009fd14c: ldr      x1, [x8]
009fd150: add      x8, sp, #8
009fd154: mov      x0, x20
009fd158: bl       #0xf9db8c | System.Collections.Generic.List<object>$$GetEnumerator
009fd15c: ldur     q0, [sp, #8]
009fd160: ldr      x8, [sp, #0x18]
009fd164: str      q0, [sp, #0x20]
009fd168: str      x8, [sp, #0x30]
009fd16c: ldr      x1, [x22]
009fd170: add      x0, sp, #0x20
009fd174: bl       #0xdaa75c | System.Collections.Generic.List.Enumerator<object>$$MoveNext
009fd178: tbz      w0, #0, #0x9fd388
009fd17c: ldr      x0, [x24]
009fd180: ldr      x20, [sp, #0x30]
009fd184: ldr      w8, [x0, #0xe0]
009fd188: cbnz     w8, #0x9fd190
009fd18c: bl       #0x925b30 | 
009fd190: ldr      x0, [x25]
009fd194: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fd198: cbz      x0, #0x9fd444
009fd19c: ldr      x8, [x0, #0x18]
009fd1a0: cbz      x8, #0x9fd448
009fd1a4: cbz      x20, #0x9fd440
009fd1a8: ldr      w8, [x8, #0x30]
009fd1ac: ldr      w9, [x20, #0x10]
009fd1b0: cmp      w8, w9
009fd1b4: b.ne     #0x9fd16c
009fd1b8: str      x20, [x19, #0x20]!
009fd1bc: mov      x0, x19
009fd1c0: mov      x1, x20
009fd1c4: bl       #0x9259e4 | 
009fd1c8: ldr      x0, [x24]
009fd1cc: ldr      w8, [x0, #0xe0]
009fd1d0: cbnz     w8, #0x9fd1d8
009fd1d4: bl       #0x925b30 | 
009fd1d8: ldr      x0, [x25]
009fd1dc: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fd1e0: cbz      x0, #0x9fd450
009fd1e4: ldr      x8, [x0, #0x18]
009fd1e8: cbz      x8, #0x9fd454
009fd1ec: ldr      w20, [x8, #0x34]
009fd1f0: ldr      x0, [x25]
009fd1f4: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fd1f8: cbz      x0, #0x9fd458
009fd1fc: ldr      x8, [x0, #0x18]
009fd200: cbz      x8, #0x9fd45c
009fd204: ldr      x9, [x19]
009fd208: cbz      x9, #0x9fd460
009fd20c: ldr      x9, [x9, #0x18]
009fd210: cbz      x9, #0x9fd464
009fd214: ldr      w8, [x8, #0x34]
009fd218: ldr      w9, [x9, #0x18]
009fd21c: cmp      w8, w9
009fd220: b.lt     #0x9fd250
009fd224: ldr      x0, [x24]
009fd228: ldr      w8, [x0, #0xe0]
009fd22c: cbnz     w8, #0x9fd234
009fd230: bl       #0x925b30 | 
009fd234: ldr      x0, [x25]
009fd238: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fd23c: cbz      x0, #0x9fd488
009fd240: ldr      x8, [x0, #0x18]
009fd244: cbz      x8, #0x9fd48c
009fd248: mov      w9, #1
009fd24c: strb     w9, [x8, #0x3c]
009fd250: ldr      x0, [x24]
009fd254: ldr      w8, [x0, #0xe0]
009fd258: cbnz     w8, #0x9fd260
009fd25c: bl       #0x925b30 | 
009fd260: ldr      x0, [x25]
009fd264: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fd268: cbz      x0, #0x9fd468
009fd26c: ldr      x8, [x0, #0x18]
009fd270: cbz      x8, #0x9fd46c
009fd274: ldrb     w8, [x8, #0x3c]
009fd278: cbz      w8, #0x9fd2a0
009fd27c: ldr      x8, [x19]
009fd280: cbz      x8, #0x9fd480
009fd284: ldr      x8, [x8, #0x18]
009fd288: cbz      x8, #0x9fd484
009fd28c: ldr      w1, [x8, #0x18]
009fd290: mov      w0, wzr
009fd294: mov      x2, xzr
009fd298: bl       #0x1e11ad4 | UnityEngine.Random$$Range
009fd29c: mov      w20, w0
009fd2a0: ldr      x8, [x19]
009fd2a4: cbz      x8, #0x9fd470
009fd2a8: ldr      x0, [x8, #0x18]
009fd2ac: cbz      x0, #0x9fd474
009fd2b0: ldr      x2, [x21]
009fd2b4: mov      w1, w20
009fd2b8: bl       #0xf9cf74 | System.Collections.Generic.List<object>$$get_Item
009fd2bc: mov      x19, x0
009fd2c0: ldr      x0, [x29]
009fd2c4: ldr      w8, [x0, #0xe0]
009fd2c8: cbnz     w8, #0x9fd2d0
009fd2cc: bl       #0x925b30 | 
009fd2d0: ldr      x0, [x28]
009fd2d4: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fd2d8: adrp     x8, #0x2abe000
009fd2dc: mov      x20, x0
009fd2e0: ldr      x0, [x23]
009fd2e4: ldr      x8, [x8, #0x98] | 'Level1'
009fd2e8: cmp      w27, #0
009fd2ec: csel     x8, x8, x26, ne
009fd2f0: ldr      x1, [x8]
009fd2f4: adrp     x8, #0x2abc000
009fd2f8: ldr      x8, [x8, #0x190] | '/'
009fd2fc: ldr      x2, [x8]
009fd300: mov      x3, x19
009fd304: mov      x4, xzr
009fd308: bl       #0x13c8c70 | System.String$$Concat
009fd30c: cbz      x20, #0x9fd478
009fd310: mov      x1, x0
009fd314: bl       #0x9eb028 | Manager.ResourceMgr$$GetTextAsset
009fd318: cbz      x0, #0x9fd47c
009fd31c: mov      x1, xzr
009fd320: bl       #0x1e2fdbc | UnityEngine.TextAsset$$get_text
009fd324: adrp     x8, #0x2ad3000
009fd328: ldr      x8, [x8, #0xd28] | 'd2d6e6b5210738f3'
009fd32c: ldr      x1, [x8]
009fd330: mov      x2, x1
009fd334: mov      x3, xzr
009fd338: bl       #0x9b4718 | Util.FileLSSUtil$$Decrypt
009fd33c: adrp     x8, #0x2ae9000
009fd340: ldr      x8, [x8, #0xee8] | 'Newtonsoft.Json.JsonConvert_TypeInfo'
009fd344: mov      x19, x0
009fd348: ldr      x0, [x8]
009fd34c: ldr      w8, [x0, #0xe0]
009fd350: cbnz     w8, #0x9fd358
009fd354: bl       #0x925b30 | 
009fd358: adrp     x8, #0x2ab9000
009fd35c: ldr      x8, [x8, #0x718] | 'Method$Newtonsoft.Json.JsonConvert.DeserializeObject<LevelData>()'
009fd360: ldr      x1, [x8]
009fd364: mov      x0, x19
009fd368: bl       #0xae5918 | Newtonsoft.Json.JsonConvert$$DeserializeObject<object>
009fd36c: adrp     x8, #0x2af4000
009fd370: ldr      x8, [x8, #0x738] | 'Method$System.Collections.Generic.List.Enumerator<LevelDataInfo>.Dispose()'
009fd374: mov      x19, x0
009fd378: add      x0, sp, #0x20
009fd37c: ldr      x1, [x8]
009fd380: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
009fd384: b        #0x9fd3cc | 
009fd388: adrp     x8, #0x2af4000
009fd38c: ldr      x8, [x8, #0x738] | 'Method$System.Collections.Generic.List.Enumerator<LevelDataInfo>.Dispose()'
009fd390: add      x0, sp, #0x20
009fd394: ldr      x1, [x8]
009fd398: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
009fd39c: adrp     x8, #0x2aee000
009fd3a0: ldr      x8, [x8, #0x398] | 'UnityEngine.Debug_TypeInfo'
009fd3a4: ldr      x0, [x8]
009fd3a8: ldr      w8, [x0, #0xe0]
009fd3ac: cbnz     w8, #0x9fd3b4
009fd3b0: bl       #0x925b30 | 
009fd3b4: adrp     x8, #0x2aeb000
009fd3b8: ldr      x8, [x8, #0xbb8] | 'GetLevelData is null !'
009fd3bc: mov      x1, xzr
009fd3c0: ldr      x0, [x8]
009fd3c4: bl       #0x1e1b538 | UnityEngine.Debug$$LogError
009fd3c8: mov      x19, xzr
009fd3cc: mov      x0, x19
009fd3d0: ldp      x29, x30, [sp, #0x90]
009fd3d4: ldp      x20, x19, [sp, #0x80]
009fd3d8: ldp      x22, x21, [sp, #0x70]
009fd3dc: ldp      x24, x23, [sp, #0x60]
009fd3e0: ldp      x26, x25, [sp, #0x50]
009fd3e4: ldp      x28, x27, [sp, #0x40]
009fd3e8: add      sp, sp, #0xa0
009fd3ec: ret      
009fd3f0: ldr      x0, [x24]
009fd3f4: ldr      w8, [x0, #0xe0]
009fd3f8: cbnz     w8, #0x9fd400
009fd3fc: bl       #0x925b30 | 
009fd400: ldr      x0, [x25]
009fd404: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fd408: cbz      x0, #0x9fd44c
009fd40c: ldr      x22, [x0, #0x18]
009fd410: ldr      x0, [x25]
009fd414: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fd418: cbz      x0, #0x9fd44c
009fd41c: ldr      x8, [x0, #0x18]
009fd420: cbz      x8, #0x9fd44c
009fd424: ldr      x8, [x8, #0x88]
009fd428: cbz      x8, #0x9fd44c
009fd42c: cbz      x22, #0x9fd44c
009fd430: ldr      w8, [x8, #0x5c]
009fd434: mov      w27, wzr
009fd438: add      w21, w8, #4
009fd43c: b        #0x9fd10c | 
009fd440: bl       #0x925b54 | 
009fd444: bl       #0x925b54 | 
009fd448: bl       #0x925b54 | 
009fd44c: bl       #0x925b54 | 
009fd450: bl       #0x925b54 | 
009fd454: bl       #0x925b54 | 
009fd458: bl       #0x925b54 | 
009fd45c: bl       #0x925b54 | 
009fd460: bl       #0x925b54 | 
009fd464: bl       #0x925b54 | 
009fd468: bl       #0x925b54 | 
009fd46c: bl       #0x925b54 | 
009fd470: bl       #0x925b54 | 
009fd474: bl       #0x925b54 | 
009fd478: bl       #0x925b54 | 
009fd47c: bl       #0x925b54 | 
009fd480: bl       #0x925b54 | 
009fd484: bl       #0x925b54 | 
009fd488: bl       #0x925b54 | 
009fd48c: bl       #0x925b54 | 
009fd490: b        #0x9fd504 | 
009fd494: b        #0x9fd504 | 
009fd498: b        #0x9fd504 | 
009fd49c: b        #0x9fd504 | 
009fd4a0: b        #0x9fd504 | 
009fd4a4: b        #0x9fd504 | 
009fd4a8: b        #0x9fd504 | 
009fd4ac: b        #0x9fd504 | 
009fd4b0: b        #0x9fd504 | 
009fd4b4: b        #0x9fd504 | 
009fd4b8: b        #0x9fd504 | 
009fd4bc: b        #0x9fd504 | 
009fd4c0: b        #0x9fd504 | 
009fd4c4: b        #0x9fd504 | 
009fd4c8: b        #0x9fd504 | 
009fd4cc: b        #0x9fd504 | 
009fd4d0: b        #0x9fd504 | 
009fd4d4: b        #0x9fd504 | 
009fd4d8: b        #0x9fd504 | 
009fd4dc: b        #0x9fd504 | 
009fd4e0: b        #0x9fd504 | 
009fd4e4: b        #0x9fd504 | 
009fd4e8: b        #0x9fd504 | 
009fd4ec: b        #0x9fd504 | 
009fd4f0: b        #0x9fd504 | 
009fd4f4: b        #0x9fd504 | 
009fd4f8: b        #0x9fd504 | 
009fd4fc: b        #0x9fd504 | 
009fd500: b        #0x9fd504 | 
009fd504: mov      x19, x0
009fd508: cmp      w1, #1
009fd50c: b.ne     #0x9fd540
009fd510: mov      x0, x19
009fd514: bl       #0x6c0f60 | 
009fd518: ldr      x20, [x0]
009fd51c: bl       #0x6c03b0 | 
009fd520: adrp     x8, #0x2af4000
009fd524: ldr      x8, [x8, #0x738] | 'Method$System.Collections.Generic.List.Enumerator<LevelDataInfo>.Dispose()'
009fd528: add      x0, sp, #0x20
009fd52c: ldr      x1, [x8]
009fd530: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
009fd534: cbz      x20, #0x9fd39c
009fd538: mov      x0, x20
009fd53c: bl       #0x89bb08 | 
009fd540: mov      x20, xzr
009fd544: b        #0x9fd54c | 
009fd548: mov      x19, x0
009fd54c: adrp     x8, #0x2af4000
009fd550: ldr      x8, [x8, #0x738] | 'Method$System.Collections.Generic.List.Enumerator<LevelDataInfo>.Dispose()'
009fd554: ldr      x1, [x8]
009fd558: add      x0, sp, #0x20
009fd55c: bl       #0xdaa758 | System.Collections.Generic.List.Enumerator<object>$$Dispose
009fd560: cbnz     x20, #0x9fd56c
009fd564: mov      x0, x19
009fd568: bl       #0x6c0230 | 
009fd56c: mov      x0, x20
009fd570: bl       #0x89bb08 | 
009fd574: bl       #0x6c29a8 | 