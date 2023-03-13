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

### Third Party Tests Failing - not considering undocumented flags.

DD 34 0000, DD 84 0001, DD 85 0000, DD 8C 0032, DD 8D 000B, DD 8E 0006, DD 9C 0009, DD 9D 000D, DD 9E 0000, DD DE 0008, DE 0001, 
ED 40 0000, ED 48 0000, ED 4A 0032, ED 50 0000, ED 57 0000, ED 58 0000, ED 5A 0047, ED 5F 0001, ED 60 0000, ED 62 0289, ED 68 0000, ED 70 0000, ED 72 0002, 
ED 78 0000, ED 7A 0001, ED A1 0000, ED A2 0000, ED A3 0000, ED AA 0000, ED AB 0000, ED B1 0001, ED B2 0000, ED B3 0000, ED BA 0000, ED BB 0002, 
FD 25 0005, FD 2D 0088, FD 34 0012, FD 84 0004, FD 85 0000, FD 8C 000A, FD 8D 0034, FD 8E 001D, FD 9C 0009, FD 9D 0011, FD 9E 0000, FD DE 000E

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