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

Number failing: 20

```
DD 8C 0037: ADC A, IXh
DD 8D 000B: ADC A, IXl
DD 8E 0006: ADC A, (IX + d)
ED 4A 0032: ADC HL, BC
ED 5A 0047: ADC HL, DE
ED 62 0289: SBC HL, HL
ED 70 0000: IN (BC)
ED 72 0002: SBC HL, SP
ED 7A 0001: ADC HL, SP
ED A2 0000: INI
ED A3 0000: OUTI
ED AA 0000: IND
ED AB 0000: OUTD
ED B2 0000: INIR
ED B3 0000: OTIR
ED BA 0000: INDR
ED BB 0002: OTDR
FD 8C 000A: ADC A, IYh
FD 8D 0034: ADC A, IYl
FD 8E 001D: ADC A, (IY + d)
```

### Fuse Tests Failing - not considering undocumented flags.

Number failing: 31

```
76: HALT
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
edb2: INIR
edb3: OTIR
edba: INDR
edbb: OTDR
```