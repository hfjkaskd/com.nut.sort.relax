// CoreGame.LuoSiSortMgr$$InitLevelConfig RVA 0x9fbae4; next known entry 0x9fbc04; boundary requires review.
009fbae4: str      x22, [sp, #-0x30]!
009fbae8: stp      x21, x20, [sp, #0x10]
009fbaec: stp      x19, x30, [sp, #0x20]
009fbaf0: adrp     x21, #0x2ca6000
009fbaf4: adrp     x20, #0x2ae9000
009fbaf8: ldrb     w8, [x21, #0x969]
009fbafc: ldr      x20, [x20, #0x458] | 'Util.Singleton<ResourceMgr>_TypeInfo'
009fbb00: mov      x19, x0
009fbb04: tbnz     w8, #0, #0x9fbb58
009fbb08: adrp     x0, #0x2acf000
009fbb0c: ldr      x0, [x0, #0xa78] | 'Method$Newtonsoft.Json.JsonConvert.DeserializeObject<LevelDataConfig>()'
009fbb10: bl       #0x925a30 | 
009fbb14: adrp     x0, #0x2ae9000
009fbb18: ldr      x0, [x0, #0xee8] | 'Newtonsoft.Json.JsonConvert_TypeInfo'
009fbb1c: bl       #0x925a30 | 
009fbb20: adrp     x0, #0x2aea000
009fbb24: ldr      x0, [x0, #0x898] | 'Method$Util.Singleton<ResourceMgr>.get_Instance()'
009fbb28: bl       #0x925a30 | 
009fbb2c: adrp     x0, #0x2ae9000
009fbb30: ldr      x0, [x0, #0x458] | 'Util.Singleton<ResourceMgr>_TypeInfo'
009fbb34: bl       #0x925a30 | 
009fbb38: adrp     x0, #0x2ad3000
009fbb3c: ldr      x0, [x0, #0xd28] | 'd2d6e6b5210738f3'
009fbb40: bl       #0x925a30 | 
009fbb44: adrp     x0, #0x2ad8000
009fbb48: ldr      x0, [x0, #0x10] | 'LevelConfig/LevelConfig0'
009fbb4c: bl       #0x925a30 | 
009fbb50: mov      w8, #1
009fbb54: strb     w8, [x21, #0x969]
009fbb58: ldr      x0, [x20]
009fbb5c: adrp     x20, #0x2aea000
009fbb60: ldr      w8, [x0, #0xe0]
009fbb64: ldr      x20, [x20, #0x898] | 'Method$Util.Singleton<ResourceMgr>.get_Instance()'
009fbb68: cbnz     w8, #0x9fbb70
009fbb6c: bl       #0x925b30 | 
009fbb70: ldr      x0, [x20]
009fbb74: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fbb78: cbz      x0, #0x9fbc00
009fbb7c: adrp     x8, #0x2ad8000
009fbb80: ldr      x8, [x8, #0x10] | 'LevelConfig/LevelConfig0'
009fbb84: ldr      x1, [x8]
009fbb88: bl       #0x9eb028 | Manager.ResourceMgr$$GetTextAsset
009fbb8c: cbz      x0, #0x9fbc00
009fbb90: adrp     x20, #0x2ad3000
009fbb94: adrp     x22, #0x2ae9000
009fbb98: adrp     x21, #0x2acf000
009fbb9c: ldr      x20, [x20, #0xd28] | 'd2d6e6b5210738f3'
009fbba0: ldr      x22, [x22, #0xee8] | 'Newtonsoft.Json.JsonConvert_TypeInfo'
009fbba4: ldr      x21, [x21, #0xa78] | 'Method$Newtonsoft.Json.JsonConvert.DeserializeObject<LevelDataConfig>()'
009fbba8: mov      x1, xzr
009fbbac: bl       #0x1e2fdbc | UnityEngine.TextAsset$$get_text
009fbbb0: ldr      x1, [x20]
009fbbb4: mov      x3, xzr
009fbbb8: mov      x2, x1
009fbbbc: bl       #0x9b4718 | Util.FileLSSUtil$$Decrypt
009fbbc0: ldr      x8, [x22]
009fbbc4: mov      x20, x0
009fbbc8: ldr      w9, [x8, #0xe0]
009fbbcc: cbnz     w9, #0x9fbbd8
009fbbd0: mov      x0, x8
009fbbd4: bl       #0x925b30 | 
009fbbd8: ldr      x1, [x21]
009fbbdc: mov      x0, x20
009fbbe0: bl       #0xae5918 | Newtonsoft.Json.JsonConvert$$DeserializeObject<object>
009fbbe4: str      x0, [x19, #0x10]!
009fbbe8: mov      x1, x0
009fbbec: mov      x0, x19
009fbbf0: ldp      x19, x30, [sp, #0x20]
009fbbf4: ldp      x21, x20, [sp, #0x10]
009fbbf8: ldr      x22, [sp], #0x30
009fbbfc: b        #0x9259e4 | 
009fbc00: bl       #0x925b54 | 