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

DD 00, DD 01, DD 02, DD 03, DD 04, DD 05, DD 06, DD 07, DD 08, DD 0A, DD 0B, DD 0C, DD 0D, DD 0E, DD 0F, DD 10, DD 100, DD 101, DD 11, DD 12, DD 13, DD 14, DD 15, DD 16, DD 17, DD 18, DD 1A, DD 1B, DD 1C, DD 1D, DD 1E, DD 1F, DD 20, DD 27, DD 28, DD 2F, DD 30, DD 31, DD 32, DD 33, DD 37, DD 38, DD 3A, DD 3B, DD 3C, DD 3D, DD 3E, DD 3F, DD 40, DD 41, DD 42, DD 43, DD 47, DD 48, DD 49, DD 4A, DD 4B, DD 4F, DD 50, DD 51, DD 52, DD 53, DD 57, DD 58, DD 59, DD 5A, DD 5B, DD 5F, DD 76, DD 78, DD 79, DD 7A, DD 7B, DD 7F, DD 80, DD 81, DD 82, DD 83, DD 87, DD 88, DD 89, DD 8A, DD 8B, DD 8F, DD 90, DD 91, DD 92, DD 93, DD 97, DD 98, DD 99, DD 9A, DD 9B, DD 9F, DD A0, DD A1, DD A2, DD A3, DD A7, DD A8, DD A9, DD AA, DD AB, DD AF, DD B0, DD B1, DD B2, DD B3, DD B7, DD B8, DD B9, DD BA, DD BB, DD BF, DD C0, DD C1, DD C2, DD C3, DD C4, DD C5, DD C6, DD C7, DD C8, DD C9, DD CA, DD CC, DD CD, DD CE, DD CF, DD D0, DD D1, DD D2, DD D3, DD D4, DD D5, DD D6, DD D7, DD D8, DD D9, DD DA, DD DB, DD DC, DD DE, DD DF, DD E0, DD E2, DD E4, DD E6, DD E7, DD E8, DD EA, DD EB, DD EC, DD EE, DD EF, DD F0, DD F1, DD F2, DD F3, DD F4, DD F5, DD F6, DD F7, DD F8, DD FA, DD FB, DD FC, DD FE, DD FF, ED 4E, ED 54, ED 55, ED 5D, ED 64, ED 65, ED 66, ED 6C, ED 6D, ED 6E, ED 74, ED 75, ED 77, ED 7C, ED 7D, ED 7F, FD 00, FD 01, FD 02, FD 03, FD 04, FD 05, FD 06, FD 07, FD 08, FD 0A, FD 0B, FD 0C, FD 0D, FD 0E, FD 0F, FD 10, FD 100, FD 101, FD 11, FD 12, FD 13, FD 14, FD 15, FD 16, FD 17, FD 1A, FD 1B, FD 1C, FD 1D, FD 1E, FD 1F, FD 20, FD 27, FD 28, FD 2F, FD 30, FD 31, FD 32, FD 33, FD 37, FD 38, FD 3A, FD 3B, FD 3C, FD 3D, FD 3E, FD 3F, FD 40, FD 41, FD 42, FD 43, FD 47, FD 48, FD 49, FD 4A, FD 4B, FD 4F, FD 50, FD 51, FD 52, FD 53, FD 57, FD 58, FD 59, FD 5A, FD 5B, FD 5F, FD 76, FD 78, FD 79, FD 7A, FD 7B, FD 7F, FD 80, FD 81, FD 82, FD 83, FD 87, FD 88, FD 89, FD 8A, FD 8B, FD 8F, FD 90, FD 91, FD 92, FD 93, FD 97, FD 98, FD 99, FD 9A, FD 9B, FD 9F, FD A0, FD A1, FD A2, FD A3, FD A7, FD A8, FD A9, FD AA, FD AB, FD AF, FD B0, FD B1, FD B2, FD B3, FD B7, FD B8, FD B9, FD BA, FD BB, FD BF, FD C0, FD C1, FD C2, FD C3, FD C4, FD C5, FD C6, FD C7, FD C8, FD C9, FD CA, FD CC, FD CD, FD CE, FD CF, FD D0, FD D1, FD D2, FD D3, FD D4, FD D5, FD D6, FD D7, FD D8, FD D9, FD DA, FD DB, FD DC, FD DE, FD DF, FD E0, FD E2, FD E4, FD E6, FD E7, FD E8, FD EA, FD EB, FD EC, FD EE, FD EF, FD F0, FD F1, FD F2, FD F3, FD F4, FD F5, FD F6, FD F7, FD F8, FD FA, FD FB, FD FC, FD FE, FD FF

Failing test. I think the parameters are not correct:

FD 18