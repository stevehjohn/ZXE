# General Notes

This document just contains notes for me to refer to during implementation.

## TODOs

- https://worldofspectrum.org/faq/reference/128kreference.htm#Plus3
- Tricky one: Render screen when 14336 TStates have passed since 50Hz refresh.
- Setting Q and Memptr.
- Update README to better reflect the additional projects.

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

Number failing: 20

```
37 0000: SCF
3F 0001: CCFt
DD 37 0009: SCF
DD 3F 0001: CCF
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
FD 37 0003: SCF
FD 3F 0000: CCF
```

### Fuse Tests Failing - not considering undocumented flags.

Number failing: 31

```
76: HALT
db_2: IN A, (n)
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
LDD
CPI
CPD
INI
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
