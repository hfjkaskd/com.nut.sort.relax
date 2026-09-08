// Util.FileLSSUtil$$Decrypt RVA 0x9b4718; next known entry 0x9b4f34; boundary requires review.
009b4718: str      x26, [sp, #-0x50]!
009b471c: stp      x25, x24, [sp, #0x10]
009b4720: stp      x23, x22, [sp, #0x20]
009b4724: stp      x21, x20, [sp, #0x30]
009b4728: stp      x19, x30, [sp, #0x40]
009b472c: adrp     x22, #0x2ca6000
009b4730: adrp     x23, #0x2ae8000
009b4734: ldrb     w8, [x22, #0x720]
009b4738: ldr      x23, [x23, #0x5c8] | 'System.Convert_TypeInfo'
009b473c: mov      x19, x2
009b4740: mov      x20, x1
009b4744: mov      x21, x0
009b4748: tbnz     w8, #0, #0x9b479c
009b474c: adrp     x0, #0x2aef000
009b4750: ldr      x0, [x0, #0x688] | 'System.Security.Cryptography.Aes_TypeInfo'
009b4754: bl       #0x925a30 | 
009b4758: adrp     x0, #0x2ae8000
009b475c: ldr      x0, [x0, #0x5c8] | 'System.Convert_TypeInfo'
009b4760: bl       #0x925a30 | 
009b4764: adrp     x0, #0x2abc000
009b4768: ldr      x0, [x0, #0xa18] | 'System.Security.Cryptography.CryptoStream_TypeInfo'
009b476c: bl       #0x925a30 | 
009b4770: adrp     x0, #0x2afa000
009b4774: ldr      x0, [x0, #0xd60] | 'System.IDisposable_TypeInfo'
009b4778: bl       #0x925a30 | 
009b477c: adrp     x0, #0x2aec000
009b4780: ldr      x0, [x0, #0x7f8] | 'System.IO.MemoryStream_TypeInfo'
009b4784: bl       #0x925a30 | 
009b4788: adrp     x0, #0x2acf000
009b478c: ldr      x0, [x0, #0x9b0] | 'System.IO.StreamReader_TypeInfo'
009b4790: bl       #0x925a30 | 
009b4794: mov      w8, #1
009b4798: strb     w8, [x22, #0x720]
009b479c: ldr      x0, [x23]
009b47a0: ldr      w8, [x0, #0xe0]
009b47a4: cbnz     w8, #0x9b47ac
009b47a8: bl       #0x925b30 | 
009b47ac: mov      x0, x21
009b47b0: mov      x1, xzr
009b47b4: bl       #0x14cb844 | System.Convert$$FromBase64String
009b47b8: mov      x21, x0
009b47bc: mov      x0, xzr
009b47c0: bl       #0x13e2058 | System.Text.Encoding$$get_UTF8
009b47c4: cbz      x0, #0x9b4ab4
009b47c8: ldr      x8, [x0]
009b47cc: ldr      x9, [x8, #0x248]
009b47d0: ldr      x2, [x8, #0x250]
009b47d4: mov      x1, x20
009b47d8: blr      x9
009b47dc: mov      x20, x0
009b47e0: mov      x0, xzr
009b47e4: bl       #0x13e2058 | System.Text.Encoding$$get_UTF8
009b47e8: cbz      x0, #0x9b4ab8
009b47ec: ldr      x8, [x0]
009b47f0: ldr      x9, [x8, #0x248]
009b47f4: ldr      x2, [x8, #0x250]
009b47f8: mov      x1, x19
009b47fc: blr      x9
009b4800: adrp     x8, #0x2aef000
009b4804: ldr      x8, [x8, #0x688] | 'System.Security.Cryptography.Aes_TypeInfo'
009b4808: mov      x22, x0
009b480c: ldr      x0, [x8]
009b4810: ldr      w8, [x0, #0xe0]
009b4814: cbnz     w8, #0x9b481c
009b4818: bl       #0x925b30 | 
009b481c: mov      x0, xzr
009b4820: bl       #0x13f4558 | System.Security.Cryptography.Aes$$Create
009b4824: adrp     x26, #0x2afa000
009b4828: ldr      x26, [x26, #0xd60] | 'System.IDisposable_TypeInfo'
009b482c: mov      x19, x0
009b4830: cbz      x0, #0x9b4abc
009b4834: ldr      x8, [x19]
009b4838: ldp      x9, x2, [x8, #0x1f8]
009b483c: mov      x0, x19
009b4840: mov      x1, x20
009b4844: blr      x9
009b4848: ldr      x8, [x19]
009b484c: ldp      x9, x2, [x8, #0x1d8]
009b4850: mov      x0, x19
009b4854: mov      x1, x22
009b4858: blr      x9
009b485c: ldr      x8, [x19]
009b4860: ldr      x9, [x8, #0x248]
009b4864: ldr      x2, [x8, #0x250]
009b4868: mov      w1, #1
009b486c: mov      x0, x19
009b4870: blr      x9
009b4874: ldr      x8, [x19]
009b4878: ldr      x9, [x8, #0x268]
009b487c: ldr      x2, [x8, #0x270]
009b4880: mov      w1, #2
009b4884: mov      x0, x19
009b4888: blr      x9
009b488c: ldr      x8, [x19]
009b4890: ldp      x9, x1, [x8, #0x1e8]
009b4894: mov      x0, x19
009b4898: blr      x9
009b489c: ldr      x8, [x19]
009b48a0: mov      x20, x0
009b48a4: ldp      x9, x1, [x8, #0x1c8]
009b48a8: mov      x0, x19
009b48ac: blr      x9
009b48b0: ldr      x8, [x19]
009b48b4: mov      x2, x0
009b48b8: ldr      x9, [x8, #0x2a8]
009b48bc: ldr      x3, [x8, #0x2b0]
009b48c0: mov      x0, x19
009b48c4: mov      x1, x20
009b48c8: blr      x9
009b48cc: adrp     x8, #0x2aec000
009b48d0: ldr      x8, [x8, #0x7f8] | 'System.IO.MemoryStream_TypeInfo'
009b48d4: mov      x22, x0
009b48d8: ldr      x0, [x8]
009b48dc: bl       #0x925b44 | 
009b48e0: mov      x20, x0
009b48e4: cbz      x0, #0x9b4ac0
009b48e8: mov      x0, x20
009b48ec: mov      x1, x21
009b48f0: mov      x2, xzr
009b48f4: bl       #0x148f448 | System.IO.MemoryStream$$.ctor
009b48f8: adrp     x8, #0x2abc000
009b48fc: ldr      x8, [x8, #0xa18] | 'System.Security.Cryptography.CryptoStream_TypeInfo'
009b4900: ldr      x0, [x8]
009b4904: bl       #0x925b44 | 
009b4908: mov      x24, x0
009b490c: cbz      x0, #0x9b4ac4
009b4910: mov      x0, x24
009b4914: mov      x1, x20
009b4918: mov      x2, x22
009b491c: mov      w3, wzr
009b4920: mov      x4, xzr
009b4924: bl       #0x13ef1a4 | System.Security.Cryptography.CryptoStream$$.ctor
009b4928: adrp     x8, #0x2acf000
009b492c: ldr      x8, [x8, #0x9b0] | 'System.IO.StreamReader_TypeInfo'
009b4930: ldr      x0, [x8]
009b4934: bl       #0x925b44 | 
009b4938: mov      x25, x0
009b493c: cbz      x0, #0x9b4ac8
009b4940: mov      x0, x25
009b4944: mov      x1, x24
009b4948: mov      x2, xzr
009b494c: bl       #0x14927c8 | System.IO.StreamReader$$.ctor
009b4950: ldr      x8, [x25]
009b4954: ldr      x1, [x8, #0x210]
009b4958: ldr      x9, [x8, #0x208]
009b495c: mov      x0, x25
009b4960: blr      x9
009b4964: mov      x21, x0
009b4968: mov      x22, xzr
009b496c: mov      w23, #2
009b4970: ldr      x8, [x25]
009b4974: ldr      x1, [x26]
009b4978: ldrh     w9, [x8, #0x12a]
009b497c: cbz      x9, #0x9b49a0
009b4980: ldr      x10, [x8, #0xb0]
009b4984: add      x10, x10, #8
009b4988: ldur     x11, [x10, #-8]
009b498c: cmp      x11, x1
009b4990: b.eq     #0x9b49b0
009b4994: subs     x9, x9, #1
009b4998: add      x10, x10, #0x10
009b499c: b.ne     #0x9b4988
009b49a0: mov      x0, x25
009b49a4: mov      w2, wzr
009b49a8: bl       #0x8b75f8 | 
009b49ac: b        #0x9b49bc | 
009b49b0: ldrsw    x9, [x10]
009b49b4: add      x8, x8, x9, lsl #4
009b49b8: add      x0, x8, #0x138
009b49bc: ldp      x8, x1, [x0]
009b49c0: mov      x0, x25
009b49c4: blr      x8
009b49c8: cbnz     x22, #0x9b4adc
009b49cc: mov      x25, xzr
009b49d0: cbnz     w23, #0x9b49dc
009b49d4: mov      w23, wzr
009b49d8: mov      x22, x25
009b49dc: ldr      x8, [x24]
009b49e0: ldr      x1, [x26]
009b49e4: ldrh     w9, [x8, #0x12a]
009b49e8: cbz      x9, #0x9b4a0c
009b49ec: ldr      x10, [x8, #0xb0]
009b49f0: add      x10, x10, #8
009b49f4: ldur     x11, [x10, #-8]
009b49f8: cmp      x11, x1
009b49fc: b.eq     #0x9b4a1c
009b4a00: subs     x9, x9, #1
009b4a04: add      x10, x10, #0x10
009b4a08: b.ne     #0x9b49f4
009b4a0c: mov      x0, x24
009b4a10: mov      w2, wzr
009b4a14: bl       #0x8b75f8 | 
009b4a18: b        #0x9b4a28 | 
009b4a1c: ldrsw    x9, [x10]
009b4a20: add      x8, x8, x9, lsl #4
009b4a24: add      x0, x8, #0x138
009b4a28: ldp      x8, x1, [x0]
009b4a2c: mov      x0, x24
009b4a30: blr      x8
009b4a34: cbnz     x22, #0x9b4ad4
009b4a38: mov      x24, xzr
009b4a3c: cbnz     w23, #0x9b4a48
009b4a40: mov      w23, wzr
009b4a44: mov      x22, x24
009b4a48: ldr      x8, [x20]
009b4a4c: ldr      x1, [x26]
009b4a50: ldrh     w9, [x8, #0x12a]
009b4a54: cbz      x9, #0x9b4a78
009b4a58: ldr      x10, [x8, #0xb0]
009b4a5c: add      x10, x10, #8
009b4a60: ldur     x11, [x10, #-8]
009b4a64: cmp      x11, x1
009b4a68: b.eq     #0x9b4a88
009b4a6c: subs     x9, x9, #1
009b4a70: add      x10, x10, #0x10
009b4a74: b.ne     #0x9b4a60
009b4a78: mov      x0, x20
009b4a7c: mov      w2, wzr
009b4a80: bl       #0x8b75f8 | 
009b4a84: b        #0x9b4a94 | 
009b4a88: ldrsw    x9, [x10]
009b4a8c: add      x8, x8, x9, lsl #4
009b4a90: add      x0, x8, #0x138
009b4a94: ldp      x8, x1, [x0]
009b4a98: mov      x0, x20
009b4a9c: blr      x8
009b4aa0: cbnz     x22, #0x9b4acc
009b4aa4: mov      x20, xzr
009b4aa8: cbz      w23, #0x9b4d50
009b4aac: cbnz     x19, #0x9b4d5c
009b4ab0: b        #0x9b4db4 | 
009b4ab4: bl       #0x925b54 | 
009b4ab8: bl       #0x925b54 | 
009b4abc: bl       #0x925b54 | 
009b4ac0: bl       #0x925b54 | 
009b4ac4: bl       #0x925b54 | 
009b4ac8: bl       #0x925b54 | 
009b4acc: mov      x0, x22
009b4ad0: bl       #0x89bb08 | 
009b4ad4: mov      x0, x22
009b4ad8: bl       #0x89bb08 | 
009b4adc: mov      x0, x22
009b4ae0: bl       #0x89bb08 | 
009b4ae4: b        #0x9b4e5c | 
009b4ae8: b        #0x9b4e5c | 
009b4aec: b        #0x9b4e5c | 
009b4af0: mov      x22, x1
009b4af4: cmp      w22, #1
009b4af8: mov      x23, x0
009b4afc: b.ne     #0x9b4b24
009b4b00: mov      x0, x23
009b4b04: bl       #0x6c0f60 | 
009b4b08: ldr      x8, [x0]
009b4b0c: str      x8, [sp, #8]
009b4b10: bl       #0x6c03b0 | 
009b4b14: ldr      x22, [sp, #8]
009b4b18: mov      w23, wzr
009b4b1c: mov      x21, xzr
009b4b20: b        #0x9b4970 | 
009b4b24: str      xzr, [sp, #8]
009b4b28: ldr      x8, [x25]
009b4b2c: ldr      x1, [x26]
009b4b30: ldrh     w9, [x8, #0x12a]
009b4b34: cbz      x9, #0x9b4b58
009b4b38: ldr      x10, [x8, #0xb0]
009b4b3c: add      x10, x10, #8
009b4b40: ldur     x11, [x10, #-8]
009b4b44: cmp      x11, x1
009b4b48: b.eq     #0x9b4b68
009b4b4c: subs     x9, x9, #1
009b4b50: add      x10, x10, #0x10
009b4b54: b.ne     #0x9b4b40
009b4b58: mov      x0, x25
009b4b5c: mov      w2, wzr
009b4b60: bl       #0x8b75f8 | 
009b4b64: b        #0x9b4b74 | 
009b4b68: ldrsw    x9, [x10]
009b4b6c: add      x8, x8, x9, lsl #4
009b4b70: add      x0, x8, #0x138
009b4b74: ldp      x8, x1, [x0]
009b4b78: mov      x0, x25
009b4b7c: blr      x8
009b4b80: ldr      x8, [sp, #8]
009b4b84: cbz      x8, #0x9b4bfc
009b4b88: ldr      x0, [sp, #8]
009b4b8c: bl       #0x89bb08 | 
009b4b90: mov      x22, x1
009b4b94: mov      x23, x0
009b4b98: b        #0x9b4b28 | 
009b4b9c: b        #0x9b4d2c | 
009b4ba0: b        #0x9b4d2c | 
009b4ba4: b        #0x9b4d2c | 
009b4ba8: b        #0x9b4d2c | 
009b4bac: b        #0x9b4d2c | 
009b4bb0: b        #0x9b4d2c | 
009b4bb4: b        #0x9b4d2c | 
009b4bb8: b        #0x9b4e5c | 
009b4bbc: b        #0x9b4e5c | 
009b4bc0: b        #0x9b4e5c | 
009b4bc4: b        #0x9b4e5c | 
009b4bc8: b        #0x9b4e5c | 
009b4bcc: b        #0x9b4e5c | 
009b4bd0: mov      x22, x1
009b4bd4: mov      x23, x0
009b4bd8: b        #0x9b4c00 | 
009b4bdc: mov      x22, x1
009b4be0: mov      x23, x0
009b4be4: b        #0x9b4c9c | 
009b4be8: mov      x22, x1
009b4bec: mov      x23, x0
009b4bf0: b        #0x9b4d38 | 
009b4bf4: mov      x22, x1
009b4bf8: mov      x23, x0
009b4bfc: mov      x21, xzr
009b4c00: cmp      w22, #1
009b4c04: b.ne     #0x9b4c1c
009b4c08: mov      x0, x23
009b4c0c: bl       #0x6c0f60 | 
009b4c10: ldr      x25, [x0]
009b4c14: bl       #0x6c03b0 | 
009b4c18: b        #0x9b49d4 | 
009b4c1c: mov      x25, xzr
009b4c20: ldr      x8, [x24]
009b4c24: ldr      x1, [x26]
009b4c28: ldrh     w9, [x8, #0x12a]
009b4c2c: cbz      x9, #0x9b4c50
009b4c30: ldr      x10, [x8, #0xb0]
009b4c34: add      x10, x10, #8
009b4c38: ldur     x11, [x10, #-8]
009b4c3c: cmp      x11, x1
009b4c40: b.eq     #0x9b4c60
009b4c44: subs     x9, x9, #1
009b4c48: add      x10, x10, #0x10
009b4c4c: b.ne     #0x9b4c38
009b4c50: mov      x0, x24
009b4c54: mov      w2, wzr
009b4c58: bl       #0x8b75f8 | 
009b4c5c: b        #0x9b4c6c | 
009b4c60: ldrsw    x9, [x10]
009b4c64: add      x8, x8, x9, lsl #4
009b4c68: add      x0, x8, #0x138
009b4c6c: ldp      x8, x1, [x0]
009b4c70: mov      x0, x24
009b4c74: blr      x8
009b4c78: cbz      x25, #0x9b4c9c
009b4c7c: mov      x0, x25
009b4c80: bl       #0x89bb08 | 
009b4c84: mov      x22, x1
009b4c88: mov      x23, x0
009b4c8c: b        #0x9b4c20 | 
009b4c90: mov      x22, x1
009b4c94: mov      x23, x0
009b4c98: mov      x21, xzr
009b4c9c: cmp      w22, #1
009b4ca0: b.ne     #0x9b4cb8
009b4ca4: mov      x0, x23
009b4ca8: bl       #0x6c0f60 | 
009b4cac: ldr      x24, [x0]
009b4cb0: bl       #0x6c03b0 | 
009b4cb4: b        #0x9b4a40 | 
009b4cb8: mov      x24, xzr
009b4cbc: ldr      x8, [x20]
009b4cc0: ldr      x1, [x26]
009b4cc4: ldrh     w9, [x8, #0x12a]
009b4cc8: cbz      x9, #0x9b4cec
009b4ccc: ldr      x10, [x8, #0xb0]
009b4cd0: add      x10, x10, #8
009b4cd4: ldur     x11, [x10, #-8]
009b4cd8: cmp      x11, x1
009b4cdc: b.eq     #0x9b4cfc
009b4ce0: subs     x9, x9, #1
009b4ce4: add      x10, x10, #0x10
009b4ce8: b.ne     #0x9b4cd4
009b4cec: mov      x0, x20
009b4cf0: mov      w2, wzr
009b4cf4: bl       #0x8b75f8 | 
009b4cf8: b        #0x9b4d08 | 
009b4cfc: ldrsw    x9, [x10]
009b4d00: add      x8, x8, x9, lsl #4
009b4d04: add      x0, x8, #0x138
009b4d08: ldp      x8, x1, [x0]
009b4d0c: mov      x0, x20
009b4d10: blr      x8
009b4d14: cbz      x24, #0x9b4d38
009b4d18: mov      x0, x24
009b4d1c: bl       #0x89bb08 | 
009b4d20: mov      x22, x1
009b4d24: mov      x23, x0
009b4d28: b        #0x9b4cbc | 
009b4d2c: mov      x22, x1
009b4d30: mov      x23, x0
009b4d34: mov      x21, xzr
009b4d38: cmp      w22, #1
009b4d3c: b.ne     #0x9b4dd8
009b4d40: mov      x0, x23
009b4d44: bl       #0x6c0f60 | 
009b4d48: ldr      x20, [x0]
009b4d4c: bl       #0x6c03b0 | 
009b4d50: mov      w23, wzr
009b4d54: mov      x22, x20
009b4d58: cbz      x19, #0x9b4db4
009b4d5c: ldr      x8, [x19]
009b4d60: ldr      x1, [x26]
009b4d64: ldrh     w9, [x8, #0x12a]
009b4d68: cbz      x9, #0x9b4d8c
009b4d6c: ldr      x10, [x8, #0xb0]
009b4d70: add      x10, x10, #8
009b4d74: ldur     x11, [x10, #-8]
009b4d78: cmp      x11, x1
009b4d7c: b.eq     #0x9b4d9c
009b4d80: subs     x9, x9, #1
009b4d84: add      x10, x10, #0x10
009b4d88: b.ne     #0x9b4d74
009b4d8c: mov      x0, x19
009b4d90: mov      w2, wzr
009b4d94: bl       #0x8b75f8 | 
009b4d98: b        #0x9b4da8 | 
009b4d9c: ldrsw    x9, [x10]
009b4da0: add      x8, x8, x9, lsl #4
009b4da4: add      x0, x8, #0x138
009b4da8: ldp      x8, x1, [x0]
009b4dac: mov      x0, x19
009b4db0: blr      x8
009b4db4: cbnz     x22, #0x9b4e44
009b4db8: cbz      w23, #0x9b4efc
009b4dbc: mov      x0, x21
009b4dc0: ldp      x19, x30, [sp, #0x40]
009b4dc4: ldp      x21, x20, [sp, #0x30]
009b4dc8: ldp      x23, x22, [sp, #0x20]
009b4dcc: ldp      x25, x24, [sp, #0x10]
009b4dd0: ldr      x26, [sp], #0x50
009b4dd4: ret      
009b4dd8: mov      x20, xzr
009b4ddc: cbz      x19, #0x9b4e38
009b4de0: ldr      x8, [x19]
009b4de4: ldr      x1, [x26]
009b4de8: ldrh     w9, [x8, #0x12a]
009b4dec: cbz      x9, #0x9b4e10
009b4df0: ldr      x10, [x8, #0xb0]
009b4df4: add      x10, x10, #8
009b4df8: ldur     x11, [x10, #-8]
009b4dfc: cmp      x11, x1
009b4e00: b.eq     #0x9b4e20
009b4e04: subs     x9, x9, #1
009b4e08: add      x10, x10, #0x10
009b4e0c: b.ne     #0x9b4df8
009b4e10: mov      x0, x19
009b4e14: mov      w2, wzr
009b4e18: bl       #0x8b75f8 | 
009b4e1c: b        #0x9b4e2c | 
009b4e20: ldrsw    x9, [x10]
009b4e24: add      x8, x8, x9, lsl #4
009b4e28: add      x0, x8, #0x138
009b4e2c: ldp      x8, x1, [x0]
009b4e30: mov      x0, x19
009b4e34: blr      x8
009b4e38: cbz      x20, #0x9b4e64
009b4e3c: mov      x0, x20
009b4e40: bl       #0x89bb08 | 
009b4e44: mov      x0, x22
009b4e48: bl       #0x89bb08 | 
009b4e4c: mov      x22, x1
009b4e50: mov      x23, x0
009b4e54: cbnz     x19, #0x9b4de0
009b4e58: b        #0x9b4e38 | 
009b4e5c: mov      x22, x1
009b4e60: mov      x23, x0
009b4e64: cmp      w22, #1
009b4e68: b.ne     #0x9b4f28
009b4e6c: mov      x0, x23
009b4e70: bl       #0x6c0f60 | 
009b4e74: mov      x19, x0
009b4e78: adrp     x0, #0x2ad7000
009b4e7c: ldr      x0, [x0, #0xfd8] | 'System.Exception_TypeInfo'
009b4e80: bl       #0x925a44 | 
009b4e84: ldr      x8, [x19]
009b4e88: ldr      x1, [x8]
009b4e8c: bl       #0x925f58 | 
009b4e90: tbz      w0, #0, #0x9b4f00
009b4e94: ldr      x19, [x19]
009b4e98: bl       #0x6c03b0 | 
009b4e9c: cbz      x19, #0x9b4efc
009b4ea0: ldr      x8, [x19]
009b4ea4: mov      x0, x19
009b4ea8: ldp      x9, x1, [x8, #0x188]
009b4eac: blr      x9
009b4eb0: mov      x19, x0
009b4eb4: adrp     x0, #0x2ac3000
009b4eb8: ldr      x0, [x0, #0x400] | 'Decrypt fail: '
009b4ebc: bl       #0x925a44 | 
009b4ec0: mov      x1, x19
009b4ec4: mov      x2, xzr
009b4ec8: bl       #0x13bdfc0 | System.String$$Concat
009b4ecc: mov      x19, x0
009b4ed0: adrp     x0, #0x2aee000
009b4ed4: ldr      x0, [x0, #0x398] | 'UnityEngine.Debug_TypeInfo'
009b4ed8: bl       #0x925a44 | 
009b4edc: ldr      w8, [x0, #0xe0]
009b4ee0: cbnz     w8, #0x9b4ee8
009b4ee4: bl       #0x925b30 | 
009b4ee8: mov      x0, x19
009b4eec: mov      x1, xzr
009b4ef0: bl       #0x1e1b538 | UnityEngine.Debug$$LogError
009b4ef4: mov      x21, xzr
009b4ef8: b        #0x9b4dbc | 
009b4efc: bl       #0x925b54 | 
009b4f00: mov      w0, #8
009b4f04: bl       #0x6bfa70 | 
009b4f08: ldr      x8, [x19]
009b4f0c: str      x8, [x0]
009b4f10: adrp     x1, #0x2980000
009b4f14: add      x1, x1, #0xc08
009b4f18: mov      x2, xzr
009b4f1c: bl       #0x6c0cb0 | 
009b4f20: mov      x23, x0
009b4f24: bl       #0x6c03b0 | 
009b4f28: mov      x0, x23
009b4f2c: bl       #0x6c0230 | 
009b4f30: bl       #0x6c29a8 | 