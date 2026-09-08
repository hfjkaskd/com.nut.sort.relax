// CoreGame.LuoSiSortMgr$$InitLoopLevelConfig RVA 0x9fbc04; next known entry 0x9fbd24; boundary requires review.
009fbc04: str      x22, [sp, #-0x30]!
009fbc08: stp      x21, x20, [sp, #0x10]
009fbc0c: stp      x19, x30, [sp, #0x20]
009fbc10: adrp     x21, #0x2ca6000
009fbc14: adrp     x20, #0x2ae9000
009fbc18: ldrb     w8, [x21, #0x96a]
009fbc1c: ldr      x20, [x20, #0x458] | 'Util.Singleton<ResourceMgr>_TypeInfo'
009fbc20: mov      x19, x0
009fbc24: tbnz     w8, #0, #0x9fbc78
009fbc28: adrp     x0, #0x2acf000
009fbc2c: ldr      x0, [x0, #0xa78] | 'Method$Newtonsoft.Json.JsonConvert.DeserializeObject<LevelDataConfig>()'
009fbc30: bl       #0x925a30 | 
009fbc34: adrp     x0, #0x2ae9000
009fbc38: ldr      x0, [x0, #0xee8] | 'Newtonsoft.Json.JsonConvert_TypeInfo'
009fbc3c: bl       #0x925a30 | 
009fbc40: adrp     x0, #0x2aea000
009fbc44: ldr      x0, [x0, #0x898] | 'Method$Util.Singleton<ResourceMgr>.get_Instance()'
009fbc48: bl       #0x925a30 | 
009fbc4c: adrp     x0, #0x2ae9000
009fbc50: ldr      x0, [x0, #0x458] | 'Util.Singleton<ResourceMgr>_TypeInfo'
009fbc54: bl       #0x925a30 | 
009fbc58: adrp     x0, #0x2ad3000
009fbc5c: ldr      x0, [x0, #0xd28] | 'd2d6e6b5210738f3'
009fbc60: bl       #0x925a30 | 
009fbc64: adrp     x0, #0x2ad6000
009fbc68: ldr      x0, [x0, #0x560] | 'LevelConfig/LevelConfig1'
009fbc6c: bl       #0x925a30 | 
009fbc70: mov      w8, #1
009fbc74: strb     w8, [x21, #0x96a]
009fbc78: ldr      x0, [x20]
009fbc7c: adrp     x20, #0x2aea000
009fbc80: ldr      w8, [x0, #0xe0]
009fbc84: ldr      x20, [x20, #0x898] | 'Method$Util.Singleton<ResourceMgr>.get_Instance()'
009fbc88: cbnz     w8, #0x9fbc90
009fbc8c: bl       #0x925b30 | 
009fbc90: ldr      x0, [x20]
009fbc94: bl       #0x11fd9e8 | Util.Singleton<object>$$get_Instance
009fbc98: cbz      x0, #0x9fbd20
009fbc9c: adrp     x8, #0x2ad6000
009fbca0: ldr      x8, [x8, #0x560] | 'LevelConfig/LevelConfig1'
009fbca4: ldr      x1, [x8]
009fbca8: bl       #0x9eb028 | Manager.ResourceMgr$$GetTextAsset
009fbcac: cbz      x0, #0x9fbd20
009fbcb0: adrp     x20, #0x2ad3000
009fbcb4: adrp     x22, #0x2ae9000
009fbcb8: adrp     x21, #0x2acf000
009fbcbc: ldr      x20, [x20, #0xd28] | 'd2d6e6b5210738f3'
009fbcc0: ldr      x22, [x22, #0xee8] | 'Newtonsoft.Json.JsonConvert_TypeInfo'
009fbcc4: ldr      x21, [x21, #0xa78] | 'Method$Newtonsoft.Json.JsonConvert.DeserializeObject<LevelDataConfig>()'
009fbcc8: mov      x1, xzr
009fbccc: bl       #0x1e2fdbc | UnityEngine.TextAsset$$get_text
009fbcd0: ldr      x1, [x20]
009fbcd4: mov      x3, xzr
009fbcd8: mov      x2, x1
009fbcdc: bl       #0x9b4718 | Util.FileLSSUtil$$Decrypt
009fbce0: ldr      x8, [x22]
009fbce4: mov      x20, x0
009fbce8: ldr      w9, [x8, #0xe0]
009fbcec: cbnz     w9, #0x9fbcf8
009fbcf0: mov      x0, x8
009fbcf4: bl       #0x925b30 | 
009fbcf8: ldr      x1, [x21]
009fbcfc: mov      x0, x20
009fbd00: bl       #0xae5918 | Newtonsoft.Json.JsonConvert$$DeserializeObject<object>
009fbd04: str      x0, [x19, #0x18]!
009fbd08: mov      x1, x0
009fbd0c: mov      x0, x19
009fbd10: ldp      x19, x30, [sp, #0x20]
009fbd14: ldp      x21, x20, [sp, #0x10]
009fbd18: ldr      x22, [sp], #0x30
009fbd1c: b        #0x9259e4 | 
009fbd20: bl       #0x925b54 | 