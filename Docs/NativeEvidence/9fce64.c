/* CoreGame.LuoSiSortMgr$$GetLevelData | ELF RVA 0x9fce64 | Ghidra VA 00afce64
Verified ELF prefix: ff8302d1fc6f04a9fa6705a9f85f06a9
Native pseudocode, not original C#. Boundaries and inferred types require review. */

undefined8 CoreGame_LuoSiSortMgr__GetLevelData_9fce64(long param_1)

{
  undefined8 *puVar1;
  bool bVar2;
  undefined *puVar3;
  undefined *puVar4;
  undefined *puVar5;
  undefined *puVar6;
  undefined *puVar7;
  undefined *puVar8;
  undefined *puVar9;
  undefined *puVar10;
  int iVar11;
  undefined4 uVar12;
  long lVar13;
  long lVar14;
  long lVar15;
  ulong uVar16;
  undefined8 uVar17;
  long *plVar18;
  undefined8 uStack_98;
  undefined8 uStack_90;
  long lStack_88;
  undefined8 uStack_80;
  undefined8 uStack_78;
  long lStack_70;
  
  puVar7 = PTR_DAT_02bf4b98;
  if ((bRam0000000002da696e & 1) == 0) {
    native_925a30_925a30(PTR_DAT_02bee398);
    native_925a30_925a30(PTR_DAT_02bf4738);
    native_925a30_925a30(PTR_DAT_02bfef20);
    native_925a30_925a30(PTR_DAT_02bee5a8);
    native_925a30_925a30(PTR_DAT_02bb9718);
    native_925a30_925a30(PTR_DAT_02be9ee8);
    native_925a30_925a30(PTR_DAT_02bc41e0);
    native_925a30_925a30(PTR_DAT_02bc7708);
    native_925a30_925a30(PTR_DAT_02bbcf60);
    native_925a30_925a30(PTR_DAT_02bea898);
    native_925a30_925a30(PTR_DAT_02bfe230);
    native_925a30_925a30(PTR_DAT_02be9458);
    native_925a30_925a30(PTR_DAT_02bf4b98);
    native_925a30_925a30(PTR_DAT_02bd3d28);
    native_925a30_925a30(PTR_DAT_02bebbb8);
    native_925a30_925a30(PTR_DAT_02bfc9d8);
    native_925a30_925a30(PTR_DAT_02bbc190);
    native_925a30_925a30(PTR_DAT_02bbe098);
    native_925a30_925a30(PTR_DAT_02bc32a0);
    bRam0000000002da696e = 1;
  }
  puVar9 = PTR_DAT_02bfe230;
  uStack_78 = 0;
  lStack_70 = 0;
  uStack_80 = 0;
  if (*(int *)(*(long *)puVar7 + 0xe0) == 0) {
    native_925b30_925b30();
  }
  lVar13 = Util_Singleton_object___get_Instance_11fd9e8(*(undefined8 *)puVar9);
  if ((lVar13 == 0) || (*(long *)(lVar13 + 0x18) == 0)) goto native_9fd44c_9fd44c;
  iVar11 = *(int *)(*(long *)(lVar13 + 0x18) + 0x2c);
  if (1 < iVar11) {
    iVar11 = iVar11 + 3;
  }
  if (*(long *)(param_1 + 0x10) == 0) goto native_9fd44c_9fd44c;
  if (0xdb < iVar11) {
    if (*(long *)(param_1 + 0x18) == 0) goto native_9fd44c_9fd44c;
    lVar13 = *(long *)(*(long *)(param_1 + 0x18) + 0x10);
    if (*(int *)(*(long *)puVar7 + 0xe0) == 0) {
      native_925b30_925b30();
    }
    lVar14 = Util_Singleton_object___get_Instance_11fd9e8(*(undefined8 *)puVar9);
    if (lVar14 == 0) goto native_9fd44c_9fd44c;
    lVar14 = *(long *)(lVar14 + 0x18);
    if (299 < iVar11) {
      iVar11 = UnityEngine_Random__Range_1e11ad4(0xdc,300,0);
    }
    if (lVar14 == 0) goto native_9fd44c_9fd44c;
    bVar2 = true;
    goto native_9fd10c_9fd10c;
  }
  lVar13 = *(long *)(*(long *)(param_1 + 0x10) + 0x10);
  if (*(int *)(*(long *)puVar7 + 0xe0) == 0) {
    native_925b30_925b30();
  }
  lVar14 = Util_Singleton_object___get_Instance_11fd9e8(*(undefined8 *)puVar9);
  if (((lVar14 == 0) || (*(long *)(lVar14 + 0x18) == 0)) ||
     (lVar14 = *(long *)(*(long *)(lVar14 + 0x18) + 0x88), lVar14 == 0)) goto native_9fd44c_9fd44c;
  if (*(char *)(lVar14 + 0x58) == '\0') {
LAB_00afd0c4:
    if (*(int *)(*(long *)puVar7 + 0xe0) == 0) {
      native_925b30_925b30();
    }
    lVar14 = Util_Singleton_object___get_Instance_11fd9e8(*(undefined8 *)puVar9);
    if ((lVar14 == 0) || (lVar14 = *(long *)(lVar14 + 0x18), lVar14 == 0))
    goto native_9fd44c_9fd44c;
    bVar2 = false;
  }
  else if (iVar11 == 5) {
    if (*(int *)(*(long *)puVar7 + 0xe0) == 0) {
      native_925b30_925b30();
    }
    lVar14 = Util_Singleton_object___get_Instance_11fd9e8(*(undefined8 *)puVar9);
    if (lVar14 == 0) goto native_9fd44c_9fd44c;
    lVar14 = *(long *)(lVar14 + 0x18);
    lVar15 = Util_Singleton_object___get_Instance_11fd9e8(*(undefined8 *)puVar9);
    if (((lVar15 == 0) || (*(long *)(lVar15 + 0x18) == 0)) ||
       ((lVar15 = *(long *)(*(long *)(lVar15 + 0x18) + 0x88), lVar15 == 0 || (lVar14 == 0))))
    goto native_9fd44c_9fd44c;
    bVar2 = false;
    iVar11 = *(int *)(lVar15 + 0x5c) + 4;
  }
  else {
    if (iVar11 != 1) goto LAB_00afd0c4;
    if (*(int *)(*(long *)puVar7 + 0xe0) == 0) {
      native_925b30_925b30();
    }
    lVar14 = Util_Singleton_object___get_Instance_11fd9e8(*(undefined8 *)puVar9);
    if (lVar14 == 0) goto native_9fd44c_9fd44c;
    lVar14 = *(long *)(lVar14 + 0x18);
    lVar15 = Util_Singleton_object___get_Instance_11fd9e8(*(undefined8 *)puVar9);
    if (((lVar15 == 0) || (*(long *)(lVar15 + 0x18) == 0)) ||
       (lVar15 = *(long *)(*(long *)(lVar15 + 0x18) + 0x88), lVar15 == 0))
    goto native_9fd44c_9fd44c;
    iVar11 = *(int *)(lVar15 + 0x5c);
    lVar15 = Util_Singleton_object___get_Instance_11fd9e8(*(undefined8 *)puVar9);
    if (((lVar15 == 0) || (*(long *)(lVar15 + 0x18) == 0)) || (lVar14 == 0))
    goto native_9fd44c_9fd44c;
    bVar2 = false;
    iVar11 = *(int *)(*(long *)(lVar15 + 0x18) + 0x34) + iVar11;
  }
native_9fd10c_9fd10c:
  *(int *)(lVar14 + 0x30) = iVar11;
  puVar10 = PTR_DAT_02bfef20;
  puVar8 = PTR_DAT_02bfc9d8;
  puVar6 = PTR_DAT_02bea898;
  puVar5 = PTR_DAT_02be9458;
  puVar4 = PTR_DAT_02bc32a0;
  puVar3 = PTR_DAT_02bbcf60;
  if (lVar13 != 0) {
    System_Collections_Generic_List_object___GetEnumerator_f9db8c
              (&uStack_98,lVar13,*(undefined8 *)PTR_DAT_02bc41e0);
    uStack_78 = uStack_90;
    uStack_80 = uStack_98;
    lStack_70 = lStack_88;
    do {
      uVar16 = System_Collections_Generic_List_Enumerator_object___MoveNext_daa75c
                         (&uStack_80,*(undefined8 *)puVar10);
      lVar13 = lStack_70;
      if ((uVar16 & 1) == 0) {
        System_Collections_Generic_List_Enumerator_object___Dispose_daa758
                  (&uStack_80,*(undefined8 *)PTR_DAT_02bf4738);
        if (*(int *)(*(long *)PTR_DAT_02bee398 + 0xe0) == 0) {
          native_925b30_925b30();
        }
        UnityEngine_Debug__LogError_1e1b538(*(undefined8 *)PTR_DAT_02bebbb8,0);
        return 0;
      }
      if (*(int *)(*(long *)puVar7 + 0xe0) == 0) {
        native_925b30_925b30();
      }
      lVar14 = Util_Singleton_object___get_Instance_11fd9e8(*(undefined8 *)puVar9);
      if (lVar14 == 0) {
                    /* WARNING: Subroutine does not return */
        native_925b54_925b54();
      }
      if (*(long *)(lVar14 + 0x18) == 0) {
                    /* WARNING: Subroutine does not return */
        native_925b54_925b54();
      }
      if (lVar13 == 0) {
                    /* WARNING: Subroutine does not return */
        native_925b54_925b54();
      }
    } while (*(int *)(*(long *)(lVar14 + 0x18) + 0x30) != *(int *)(lVar13 + 0x10));
    plVar18 = (long *)(param_1 + 0x20);
    *plVar18 = lVar13;
    native_9259e4_9259e4(plVar18,lVar13);
    if (*(int *)(*(long *)puVar7 + 0xe0) == 0) {
      native_925b30_925b30();
    }
    lVar13 = Util_Singleton_object___get_Instance_11fd9e8(*(undefined8 *)puVar9);
    if (lVar13 == 0) {
                    /* WARNING: Subroutine does not return */
      native_925b54_925b54();
    }
    if (*(long *)(lVar13 + 0x18) == 0) {
                    /* WARNING: Subroutine does not return */
      native_925b54_925b54();
    }
    uVar12 = *(undefined4 *)(*(long *)(lVar13 + 0x18) + 0x34);
    lVar13 = Util_Singleton_object___get_Instance_11fd9e8(*(undefined8 *)puVar9);
    if (lVar13 == 0) {
                    /* WARNING: Subroutine does not return */
      native_925b54_925b54();
    }
    if (*(long *)(lVar13 + 0x18) == 0) {
                    /* WARNING: Subroutine does not return */
      native_925b54_925b54();
    }
    if (*plVar18 == 0) {
                    /* WARNING: Subroutine does not return */
      native_925b54_925b54();
    }
    lVar14 = *(long *)(*plVar18 + 0x18);
    if (lVar14 == 0) {
                    /* WARNING: Subroutine does not return */
      native_925b54_925b54();
    }
    if (*(int *)(lVar14 + 0x18) <= *(int *)(*(long *)(lVar13 + 0x18) + 0x34)) {
      if (*(int *)(*(long *)puVar7 + 0xe0) == 0) {
        native_925b30_925b30();
      }
      lVar13 = Util_Singleton_object___get_Instance_11fd9e8(*(undefined8 *)puVar9);
      if (lVar13 == 0) {
                    /* WARNING: Subroutine does not return */
        native_925b54_925b54();
      }
      if (*(long *)(lVar13 + 0x18) == 0) {
                    /* WARNING: Subroutine does not return */
        native_925b54_925b54();
      }
      *(undefined1 *)(*(long *)(lVar13 + 0x18) + 0x3c) = 1;
    }
    if (*(int *)(*(long *)puVar7 + 0xe0) == 0) {
      native_925b30_925b30();
    }
    lVar13 = Util_Singleton_object___get_Instance_11fd9e8(*(undefined8 *)puVar9);
    if (lVar13 == 0) {
                    /* WARNING: Subroutine does not return */
      native_925b54_925b54();
    }
    if (*(long *)(lVar13 + 0x18) == 0) {
                    /* WARNING: Subroutine does not return */
      native_925b54_925b54();
    }
    if (*(char *)(*(long *)(lVar13 + 0x18) + 0x3c) != '\0') {
      if (*plVar18 == 0) {
                    /* WARNING: Subroutine does not return */
        native_925b54_925b54();
      }
      lVar13 = *(long *)(*plVar18 + 0x18);
      if (lVar13 == 0) {
                    /* WARNING: Subroutine does not return */
        native_925b54_925b54();
      }
      uVar12 = UnityEngine_Random__Range_1e11ad4(0,*(undefined4 *)(lVar13 + 0x18),0);
    }
    if (*plVar18 == 0) {
                    /* WARNING: Subroutine does not return */
      native_925b54_925b54();
    }
    lVar13 = *(long *)(*plVar18 + 0x18);
    if (lVar13 != 0) {
      uVar17 = System_Collections_Generic_List_object___get_Item_f9cf74
                         (lVar13,uVar12,*(undefined8 *)puVar3);
      if (*(int *)(*(long *)puVar5 + 0xe0) == 0) {
        native_925b30_925b30();
      }
      lVar13 = Util_Singleton_object___get_Instance_11fd9e8(*(undefined8 *)puVar6);
      puVar1 = (undefined8 *)PTR_DAT_02bbe098;
      if (!bVar2) {
        puVar1 = (undefined8 *)puVar8;
      }
      uVar17 = System_String__Concat_13c8c70
                         (*(undefined8 *)puVar4,*puVar1,*(undefined8 *)PTR_DAT_02bbc190,uVar17,0);
      if (lVar13 != 0) {
        lVar13 = Manager_ResourceMgr__GetTextAsset_9eb028(uVar17,uVar17);
        if (lVar13 != 0) {
          uVar17 = UnityEngine_TextAsset__get_text_1e2fdbc(lVar13,0);
          uVar17 = Util_FileLSSUtil__Decrypt_9b4718
                             (uVar17,*(undefined8 *)PTR_DAT_02bd3d28,*(undefined8 *)PTR_DAT_02bd3d28
                              ,0);
          if (*(int *)(*(long *)PTR_DAT_02be9ee8 + 0xe0) == 0) {
            native_925b30_925b30();
          }
          uVar17 = Newtonsoft_Json_JsonConvert__DeserializeObject_object__ae5918
                             (uVar17,*(undefined8 *)PTR_DAT_02bb9718);
          System_Collections_Generic_List_Enumerator_object___Dispose_daa758
                    (&uStack_80,*(undefined8 *)PTR_DAT_02bf4738);
          return uVar17;
        }
                    /* WARNING: Subroutine does not return */
        native_925b54_925b54();
      }
                    /* WARNING: Subroutine does not return */
      native_925b54_925b54();
    }
                    /* WARNING: Subroutine does not return */
    native_925b54_925b54();
  }
native_9fd44c_9fd44c:
                    /* WARNING: Subroutine does not return */
  native_925b54_925b54();
}

