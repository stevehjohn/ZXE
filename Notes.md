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

90 0000, 91 0000, 95 0000, 96 0000, 98 0000, 9B 0000, 9E 0000, B8 0000, C6 0000, CB 46 0000, CB 4E 0000, CB 5E 0000, CB 66 0000, CB 6E 0000, CB 76 0000, CB 7E 0000, CE 0000, DD 10 0000, DD 24 0000, DD 25 0000, DD 34 0000, DD 85 0000, DD 92 0000, DD 99 0000, DD 9A 0000, DD 9B 0000, DD 9E 0000, DD BA 0000, DD DE 0000, ED 40 0000, ED 48 0000, ED 50 0000, ED 57 0000, ED 58 0000, ED 60 0000, ED 68 0000, ED 70 0000, ED 78 0000, ED A1 0000, ED A2 0000, ED A3 0000, ED A9 0000, ED AA 0000, ED AB 0000, ED B1 0000, ED B2 0000, ED B3 0000, ED B8 0000, ED B9 0000, ED BA 0000, ED BB 0000, FD 10 0000, FD 24 0000, FD 3F 0000, FD 85 0000, FD 90 0000, FD 93 0000, FD 9A 0000, FD 9B 0000, FD 9E 0000, FD BB 0000, FD C6 0000

### Fuse Tests Failing

Count: 31

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
edaa: IND
edaa_01: IND
edaa_02: IND
edaa_03: IND
edab: OUTD
edab_01: OUTD
edab_02: OUTD
edb2: INIR
edb3: OTIR
edba: INDR
edbb: OTDR
```