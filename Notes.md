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

76 0000 (HALT), DD 76 0000 (HALT), FD 76 0000 (HALT)
ED 40 0000, ED 48 0000, ED 50 0000, ED 58 0000, ED 60 0000, ED 78 0000 (IN R, (C))
See TODO note in IN_R_C method.

Failing test, but I think the test parameters are not correct: FD 18

### Fuse Tests Failing

Count: 34

```
db_1: IN A, (n)
db_2: IN A, (n)
db_3: IN A, (n)
db: IN A, (n)
ed40: IN B, (C)
ed48: IN C, (C)
ed50: IN D, (C)
ed58: IN E, (C)
ed60: IN H, (C)
ed68: IN L, (C)
ed70: IN (BC)
ed78: IN A, (C)
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
edb1: CPIR
edb2: INIR
edb3: OTIR
edb9: CPDR
edba: INDR
edbb: OTDR
```