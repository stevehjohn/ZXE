# General Notes

This document just contains notes for me to refer to during implementation.

## TODOs

- Instructions that modify F (except pop af) copy F to Q. Instructions that don't modify F set Q to 0.
  Then, the flags of ccf and scf are computed as explained here: https://github.com/redcode/Z80/blob/master/sources/Z80.c#L1220
  More: https://github.com/hoglet67/Z80Decoder/wiki/Undocumented-Flags#scfccf
- https://worldofspectrum.org/faq/reference/128kreference.htm#Plus3
- Tricky one: Render screen when 14336 TStates have passed since 50Hz refresh.
- Setting Q and Memptr.
- Update README to better reflect the additional projects.
- Make changing emulated machine easier.

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

### Running ZexAll/ZexDoc

Load zex[all|doc]-spectrum.com into 0x8000 and run `RANDOMIZE USR 32768`.

### Third Party Tests Failing - not considering undocumented flags.

Number failing: 8

```
ED A2 0000: INI
ED A3 0000: OUTI
ED AA 0000: IND
ED AB 0000: OUTD
ED B2 0000: INIR
ED B3 0000: OTIR
ED BA 0000: INDR
ED BB 0002: OTDR
```

### Third Party Tests Failing - including undocumented flags.

Number failing: 87

```
37 0008: SCF
3F 0000: CCF
CB 40 0001: BIT 0, B
CB 41 0000: BIT 0, C
CB 42 0000: BIT 0, D
CB 43 0000: BIT 0, E
CB 44 0001: BIT 0, H
CB 45 0000: BIT 0, L
CB 47 0000: BIT 0, A
CB 48 0000: BIT 1, B
CB 49 0000: BIT 1, C
CB 4A 0003: BIT 1, D
CB 4B 0000: BIT 1, E
CB 4C 0000: BIT 1, H
CB 4D 0000: BIT 1, L
CB 4F 0001: BIT 1, A
CB 50 0000: BIT 2, B
CB 51 0000: BIT 2, C
CB 52 0000: BIT 2, D
CB 53 0001: BIT 2, E
CB 54 0000: BIT 2, H
CB 55 0000: BIT 2, L
CB 57 0002: BIT 2, A
CB 58 0000: BIT 3, B
CB 59 0000: BIT 3, C
CB 5A 0000: BIT 3, D
CB 5B 0000: BIT 3, E
CB 5C 0000: BIT 3, H
CB 5D 0000: BIT 3, L
CB 5F 0000: BIT 3, A
CB 60 0002: BIT 4, B
CB 61 0000: BIT 4, C
CB 62 0000: BIT 4, D
CB 63 0000: BIT 4, E
CB 64 0001: BIT 4, H
CB 65 0000: BIT 4, L
CB 67 0000: BIT 4, A
CB 68 0000: BIT 5, B
CB 69 0000: BIT 5, C
CB 6A 0000: BIT 5, D
CB 6B 0000: BIT 5, E
CB 6C 0000: BIT 5, H
CB 6D 0000: BIT 5, L
CB 6F 0000: BIT 5, A
CB 70 0000: BIT 6, B
CB 71 0000: BIT 6, C
CB 72 0000: BIT 6, D
CB 73 0001: BIT 6, E
CB 74 0000: BIT 6, H
CB 75 0001: BIT 6, L
CB 77 0000: BIT 6, A
CB 78 0001: BIT 7, B
CB 79 0000: BIT 7, C
CB 7A 0000: BIT 7, D
CB 7B 0000: BIT 7, E
CB 7C 0000: BIT 7, H
CB 7D 0000: BIT 7, L
CB 7F 0000: BIT 7, A
DB 0066: IN A, (n)
DD 37 0001: SCF
DD 3F 0002: CCF
DD DB 0010: IN A, (n)
ED 40 00E7: IN B, (C)
ED 48 00B7: IN C, (C)
ED 50 000D: IN D, (C)
ED 58 003E: IN E, (C)
ED 60 000C: IN H, (C)
ED 68 0002: IN L, (C)
ED 70 0013: IN (BC)
ED 78 0001: IN A, (C)
ED A1 0004: CPI
ED A2 0157: INI
ED A3 0000: OUTI
ED A9 0000: CPD
ED AA 0000: IND
ED AB 0000: OUTD
ED B0 0001: LDIR
ED B1 0000: CPIR
ED B2 0000: INIR
ED B3 0000: OTIR
ED B8 0000: LDDR
ED B9 0000: CPDR
ED BA 0000: INDR
ED BB 0000: OTDR
FD 37 0000: SCF
FD 3F 0001: CCF
FD DB 00D7: IN A, (n)
```

### Fuse Tests Failing - not considering undocumented flags.

Number failing: 25

```
76: HALT
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
edaa_01: IND
edaa_02: IND
edab: OUTD
edab_01: OUTD
edb2: INIR
edb3: OTIR
edba: INDR
edbb: OTDR
```

### ZexDoc Failures.

None.

### ZexAll failures.

bit n,[r]          bb37bfd1
cpd[r]             133e3d03
cpi[r]             4a8c416b
daa,cpl,scf,ccf    431bb7a6

### Woody Tests - Flags faiures.

CCF
SBC 8
ADC 16
SBC 16
BIT n,(HL)
LDD
CPI
CPD
IND
OUTI
OUTD
DD CB (00 - FF) ROM
FD CB (00 - FF) ROM

### Woody Tests - Memptr results.

LD A, (nn)
LD (nn), A
LD A, (BC)
LD A, (DE)
LD A, (HL)
LD (BC), A
LD (DE), A
LD (HL), A
LD HL, (nn)
LD HL, (nn) [ED]
LD DE, (nn)
LD BC, (nn)
LD IX, (nn)

PASSED: LD IY, (nn)

LD SP, (nn)
LD (nn), HL
LD (nn), HL [ED]
LD (nn), DE
LD (nn), BC
LD (nn), IX
LD (nn), IY
LD (nn), SP
EX (SP), HL
EX (SP), IX
EX (SP), IY
ADD HL, BC
ADD IX, BC
ADD IY, BC
ADC HL, BC
SBC HL, BC
DJNZ [NZ]
DJNZ [Z]
RLD
RRD
IN A, (nn)
IN A, (C)
OUT (nn), A
OUT (C), A
LDI,
LDD,
LDIR [BC = 1]
LDIR [BC > 1]
LDDR [BC = 1]
LDDR [BC > 1]
CPI
CPD
CPIR [BC = 1]
CPIR [BC > 1]
CPDR [BC = 1]
CPDR [BC > 1]

PASSED: INI

IND
INIR
ONDR
OUTI
OUTD
OTIR
OTDR
