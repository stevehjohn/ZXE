# General Notes

This document just contains notes for me to refer to during implementation.

## Assembly Language

- The destination is always the first parameter. `LD BC, nn` puts `nn` into the `BC` register pair.
- Except for some instructions, notably DDCB. E.g. `SLA (IX + d), C` stores the result of the operation on `(IX + d)` in register `C`.

## Coding Conventions

- In code the alternate registers are suffixed with `_`. E.g. `B'` is `B_`, `C'` is `C_` etc...
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

Failing test, but I think the test parameters are not correct: FD 18

### Fuse Tests Failing

Count: 61

```
 27_1: DAA
27: DAA
37_1: SCF
37_2: SCF
3f: CCF
76: HALT
db_1: IN A, (n)
db_2: IN A, (n)
db_3: IN A, (n)
db: IN A, (n)
dd35: DEC (IX + d)
dd39: ADD IX, SP
dd95: SUB A, IXl
dd96: SUB A, (IX + d)
ed40: IN B, (C)
ed42: SBC HL, BC
ed48: IN C, (C)
ed4a: ADC HL, BC
ed50: IN D, (C)
ed58: IN E, (C)
ed5a: ADC HL, DE
ed60: IN H, (C)
ed68: IN L, (C)
ed6a: ADC HL, HL
ed70: IN (BC)
ed72: SBC HL, SP
ed78: IN A, (C)
ed7a: ADC HL, SP
eda1: CPI
eda2: INI
eda2_01: INI
eda2_02: INI
eda2_03: INI
eda3_02: OUTI
eda3_04: OUTI
eda3_06: OUTI
eda3_08: OUTI
eda9: CPD
edaa: IND
edaa_01: IND
edaa_02: IND
edaa_03: IND
edab: OUTD
edab_01: OUTD
edab_02: OUTD
edb0: LDIR
edb1: CPIR
edb2: INIR
edb3: OTIR
edb8: LDDR
edb9: CPDR
edba: INDR
edbb: OTDR
fd25: DEC IYh
fd26: LD IYh, n
fd2a: LD IY, (nn)
fd35: DEC (IY + d)
fd39: ADD IY, SP
fd6e: LD L, (IY + d)
fd94: SUB A, IYh
fd96: SUB A, (IY + d)
```