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

Tests exist, but I can't verify what the instructions should be:

DD EF, DD F0, DD F1, DD F2, DD F3, DD F4, DD F5, DD F6, DD F7, DD F8, DD FA, DD FB, DD FC, DD FE, DD FF, ED 4E, ED 54, ED 55, ED 5D, ED 64, ED 65, ED 66, ED 6C, ED 6D, ED 6E, ED 74, ED 75, ED 77, ED 7C, ED 7D, ED 7F, FD 00, FD 01, FD 02, FD 03, FD 04, FD 05, FD 06, FD 07, FD 08, FD 0A, FD 0B, FD 0C, FD 0D, FD 0E, FD 0F, FD 10, FD 100, FD 101, FD 11, FD 12, FD 13, FD 14, FD 15, FD 16, FD 17, FD 1A, FD 1B, FD 1C, FD 1D, FD 1E, FD 1F, FD 20, FD 27, FD 28, FD 2F, FD 30, FD 31, FD 32, FD 33, FD 37, FD 38, FD 3A, FD 3B, FD 3C, FD 3D, FD 3E, FD 3F, FD 40, FD 41, FD 42, FD 43, FD 47, FD 48, FD 49, FD 4A, FD 4B, FD 4F, FD 50, FD 51, FD 52, FD 53, FD 57, FD 58, FD 59, FD 5A, FD 5B, FD 5F, FD 76, FD 78, FD 79, FD 7A, FD 7B, FD 7F, FD 80, FD 81, FD 82, FD 83, FD 87, FD 88, FD 89, FD 8A, FD 8B, FD 8F, FD 90, FD 91, FD 92, FD 93, FD 97, FD 98, FD 99, FD 9A, FD 9B, FD 9F, FD A0, FD A1, FD A2, FD A3, FD A7, FD A8, FD A9, FD AA, FD AB, FD AF, FD B0, FD B1, FD B2, FD B3, FD B7, FD B8, FD B9, FD BA, FD BB, FD BF, FD C0, FD C1, FD C2, FD C3, FD C4, FD C5, FD C6, FD C7, FD C8, FD C9, FD CA, FD CC, FD CD, FD CE, FD CF, FD D0, FD D1, FD D2, FD D3, FD D4, FD D5, FD D6, FD D7, FD D8, FD D9, FD DA, FD DB, FD DC, FD DE, FD DF, FD E0, FD E2, FD E4, FD E6, FD E7, FD E8, FD EA, FD EB, FD EC, FD EE, FD EF, FD F0, FD F1, FD F2, FD F3, FD F4, FD F5, FD F6, FD F7, FD F8, FD FA, FD FB, FD FC, FD FE, FD FF

Failing test. I think the parameters are not correct:

FD 18