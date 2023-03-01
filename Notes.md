# General Notes

This document just contains notes for me to refer to during implementation.

## Assembly Language

- The destination is always the first parameter. `LD BC, nn` puts `nn` into the `BC` register pair.
- Except for some instructions, notably DDCB. E.g. `SLA (IX + d), C` stores the result of the operation on `(IX + d)` in register `C`.

## Coding Conventions

- In code the alternate registers are suffixed with `1`. E.g. `B'` is `B1`, `C'` is `C1` etc...
- Dereferencing registers in method names are prefixed with `addr`. E.g. `LD (BC), nn` would be called `LD_addr_BC_nn` as a C# method.

## Instruction sets

xx = Opcode, dd = Displacement value (byte).

- Base: `xx`
- DD: `DDxx`
- FD: `FDxx`
- ED: `EXxx`
- CB: `CBxx`
- DDCB: `DDCBddxx`
- FDCB: `FDCBddxx`

## Tests

### Third Party Tests Failing

Failing test, but I think the test parameters are not correct:

FD 18

### Fuse Tests Failing

Count: 87

27_1 (DAA), 27 (DAA), 37_1 (SCF), 37_2 (SCF), 3f (CCF), db_1 (IN A, (n)), db_2 (IN A, (n)), db_3 (IN A, (n)), db (IN A, (n)), dd2d (DEC IXl), dd34 (INC (IX + d)), dd35 (DEC (IX + d)), dd39 (ADD IX, SP), dd70 (LD (IX + d), B), dd71 (LD (IX + d), C), dd72 (LD (IX + d), D), dd73 (LD (IX + d), E), dd74 (LD (IX + d), H), dd75 (LD (IX + d), L), dd86 (ADD A, (IX + d)), dd95 (SUB A, IXl), dd96 (SUB A, (IX + d)), ddbc (CP A, IXh), ddbe (CP A, (IX + d)), ed40 (IN B, (C)), ed42 (SBC HL, BC), ed44 (NEG A), ed48 (IN C, (C)), ed4a (ADC HL, BC), ed4c (NEG A), ed50 (IN D, (C)), ed54 (NEG A), ed57 (LD A, I), ed58 (IN E, (C)), ed5a (ADC HL, DE), ed5c (NEG A), ed5f (LD A, R), ed60 (IN H, (C)), ed64 (NEG A), ed67 (RRD), ed68 (IN L, (C)), ed6a (ADC HL, HL), ed6c (NEG A), ed6f (RLD), ed70 (IN (BC)), ed72 (SBC HL, SP), ed74 (NEG A), ed78 (IN A, (C)), ed7a (ADC HL, SP), ed7c (NEG A), eda1 (CPI), eda2 (INI), eda2_01 (INI), eda2_02 (INI), eda2_03 (INI), eda3_02 (OUTI), eda3_04 (OUTI), eda3_06 (OUTI), eda3_08 (OUTI), eda8 (LDD), eda9 (CPD), edaa (IND), edaa_01 (IND), edaa_02 (IND), edaa_03 (IND), edab (OUTD), edab_01 (OUTD), edab_02 (OUTD), edb0 (LDIR), edb1 (CPIR), edb2 (INIR), edb3 (OTIR), edb8 (LDDR), edb9 (CPDR), edba (INDR), edbb (OTDR), fd25 (DEC IYh), fd26 (LD IYh, n), fd2a (LD IY, (nn)), fd2d (DEC IYl), fd34 (INC (IY + d)), fd35 (DEC (IY + d)), fd39 (ADD IY, SP), fd6e (LD L, (IY + d)), fd94 (SUB A, IYh), fd96 (SUB A, (IY + d)), fe (CP A, n)