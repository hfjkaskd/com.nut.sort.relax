// Manager.ResourceMgr$$GetTextAsset RVA 0x9eb028; next known entry 0x9eb120; boundary requires review.
009eb028: str      x22, [sp, #-0x30]!
009eb02c: stp      x21, x20, [sp, #0x10]
009eb030: stp      x19, x30, [sp, #0x20]
009eb034: adrp     x21, #0x2ca6000
009eb038: adrp     x22, #0x2af8000
009eb03c: adrp     x20, #0x2abc000
009eb040: ldrb     w8, [x21, #0x939]
009eb044: ldr      x22, [x22, #0xdc8] | 'Method$UnityEngine.Resources.Load<TextAsset>()'
009eb048: ldr      x20, [x20, #0x368] | 'UnityEngine.Object_TypeInfo'
009eb04c: mov      x19, x1
009eb050: tbnz     w8, #0, #0x9eb08c
009eb054: adrp     x0, #0x2aee000
009eb058: ldr      x0, [x0, #0x398] | 'UnityEngine.Debug_TypeInfo'
009eb05c: bl       #0x925a30 | 
009eb060: adrp     x0, #0x2abc000
009eb064: ldr      x0, [x0, #0x368] | 'UnityEngine.Object_TypeInfo'
009eb068: bl       #0x925a30 | 
009eb06c: adrp     x0, #0x2af8000
009eb070: ldr      x0, [x0, #0xdc8] | 'Method$UnityEngine.Resources.Load<TextAsset>()'
009eb074: bl       #0x925a30 | 
009eb078: adrp     x0, #0x2ab9000
009eb07c: ldr      x0, [x0, #0x8b0] | 'GetTextAsset not find path: '
009eb080: bl       #0x925a30 | 
009eb084: mov      w8, #1
009eb088: strb     w8, [x21, #0x939]
009eb08c: ldr      x1, [x22]
009eb090: mov      x0, x19
009eb094: bl       #0xb093a8 | UnityEngine.Resources$$Load<object>
009eb098: ldr      x8, [x20]
009eb09c: mov      x20, x0
009eb0a0: ldr      w9, [x8, #0xe0]
009eb0a4: cbnz     w9, #0x9eb0b0
009eb0a8: mov      x0, x8
009eb0ac: bl       #0x925b30 | 
009eb0b0: mov      x0, x20
009eb0b4: mov      x1, xzr
009eb0b8: mov      x2, xzr
009eb0bc: bl       #0x1e22e88 | UnityEngine.Object$$op_Equality
009eb0c0: tbz      w0, #0, #0x9eb10c
009eb0c4: adrp     x8, #0x2ab9000
009eb0c8: ldr      x8, [x8, #0x8b0] | 'GetTextAsset not find path: '
009eb0cc: adrp     x20, #0x2aee000
009eb0d0: mov      x1, x19
009eb0d4: mov      x2, xzr
009eb0d8: ldr      x0, [x8]
009eb0dc: ldr      x20, [x20, #0x398] | 'UnityEngine.Debug_TypeInfo'
009eb0e0: bl       #0x13bdfc0 | System.String$$Concat
009eb0e4: ldr      x8, [x20]
009eb0e8: mov      x19, x0
009eb0ec: ldr      w9, [x8, #0xe0]
009eb0f0: cbnz     w9, #0x9eb0fc
009eb0f4: mov      x0, x8
009eb0f8: bl       #0x925b30 | 
009eb0fc: mov      x0, x19
009eb100: mov      x1, xzr
009eb104: bl       #0x1e1b538 | UnityEngine.Debug$$LogError
009eb108: mov      x20, xzr
009eb10c: mov      x0, x20
009eb110: ldp      x19, x30, [sp, #0x20]
009eb114: ldp      x21, x20, [sp, #0x10]
009eb118: ldr      x22, [sp], #0x30
009eb11c: ret      