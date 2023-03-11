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

37 0008, 3F 0001
BE 000C, CB 46 0000, CB 4E 0000, CB 56 0002, CB 5E 0000, CB 66 0000, CB 6E 0000, CB 76 0000, 
CB 7E 0000, 
DD 24 0000, DD 25 0000, DD 2C 00FC, DD 2D 005C, DD 34 0000, DD 37 0001, DD 3F 0001, DD 84 0001, DD 85 0000, DD 88 0002, DD 89 0027, 
DD 8A 0056, DD 8B 001F, DD 8C 0032, DD 8D 000B, DD 8E 0006, DD 8F 000C, DD 98 002C, DD 99 0043, DD 9A 0075, DD 9B 000F, DD 9C 0009, DD 9D 000D, DD 9E 0000, 
DD 9F 00C1, DD B8 000B, DD B9 000A, DD BA 0000, DD BB 000B, DD C6 0002, DD CE 0005, DD DE 0000, DE 0001, ED 40 0000, ED 48 0000, ED 4A 0032, ED 50 0000, 
ED 57 0000, ED 58 0000, ED 5A 0047, ED 5F 0001, ED 60 0000, ED 62 0289, ED 68 0000, ED 70 0000, ED 72 0002, ED 78 0000, ED 7A 0001, ED A1 0000, ED A2 0000, 
ED A3 0000, ED A9 0000, ED AA 0000, ED AB 0000, ED B0 0001, ED B1 0000, ED B2 0000, ED B3 0000, ED B8 0000, ED B9 0000, ED BA 0000, ED BB 0000, FD 24 0000, 
FD 25 0005, FD 2C 0036, FD 2D 0088, FD 34 0012, FD 37 0000, FD 3F 0000, FD 84 0004, FD 85 0000, FD 88 0055, FD 89 0001, FD 8A 002F, FD 8B 0003, FD 8C 000A, 
FD 8D 0034, FD 8E 001D, FD 8F 002E, FD 98 026B, FD 99 03CA, FD 9B 0032, FD 9C 0009, FD 9D 0011, FD 9E 0000, FD 9F 013C, FD B8 0003, FD B9 0002, FD BA 0002, 
FD BB 0000, FD C6 0000, FD CE 0001, FD DE 0007

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