// CoreGame.GameScene$$FixCameraSize RVA 0xa05324; next known entry 0xa053f8; boundary requires review.
00a05324: stp      x21, x20, [sp, #-0x20]!
00a05328: stp      x19, x30, [sp, #0x10]
00a0532c: mov      x19, x0
00a05330: mov      x0, xzr
00a05334: mov      w20, w1
00a05338: bl       #0x1e27830 | UnityEngine.Screen$$get_height
00a0533c: mov      w21, w0
00a05340: mov      x0, xzr
00a05344: bl       #0x1e27808 | UnityEngine.Screen$$get_width
00a05348: adrp     x8, #0x2017000
00a0534c: scvtf    s0, w21
00a05350: scvtf    s1, w0
00a05354: ldr      s2, [x8, #0x93c]
00a05358: adrp     x8, #0x2017000
00a0535c: fdiv     s0, s0, s1
00a05360: ldr      s1, [x8, #0x940]
00a05364: fadd     s2, s0, s2
00a05368: ldr      x0, [x19, #0x18]
00a0536c: fmov     s0, #1.00000000
00a05370: fdiv     s1, s2, s1
00a05374: fmin     s3, s1, s0
00a05378: fcmp     s1, #0.0
00a0537c: fmov     s2, wzr
00a05380: fcsel    s1, s3, s2, pl
00a05384: cbz      x0, #0xa053f4
00a05388: sub      w8, w20, #5
00a0538c: fmul     s2, s1, s2
00a05390: fmov     s3, #5.00000000
00a05394: scvtf    s6, w8
00a05398: fadd     s2, s2, s0
00a0539c: fdiv     s3, s6, s3
00a053a0: fmov     s4, #3.00000000
00a053a4: fmul     s2, s3, s2
00a053a8: fmov     s5, #7.50000000
00a053ac: fmul     s1, s1, s4
00a053b0: fadd     s2, s2, s0
00a053b4: fadd     s1, s1, s5
00a053b8: fmax     s0, s2, s0
00a053bc: fmul     s0, s1, s0
00a053c0: mov      x1, xzr
00a053c4: bl       #0x1e2a35c | UnityEngine.Camera$$set_orthographicSize
00a053c8: ldr      x0, [x19, #0x18]
00a053cc: cbz      x0, #0xa053f4
00a053d0: ldr      x19, [x19, #0x20]
00a053d4: mov      x1, xzr
00a053d8: bl       #0x1e2a320 | UnityEngine.Camera$$get_orthographicSize
00a053dc: cbz      x19, #0xa053f4
00a053e0: mov      x0, x19
00a053e4: ldp      x19, x30, [sp, #0x10]
00a053e8: mov      x1, xzr
00a053ec: ldp      x21, x20, [sp], #0x20
00a053f0: b        #0x1e2a35c | UnityEngine.Camera$$set_orthographicSize
00a053f4: bl       #0x925b54 | 