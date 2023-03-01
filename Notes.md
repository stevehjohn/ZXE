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

### Third Party Tests Failing

Failing test, but I think the test parameters are not correct:

FD 18

### Fuse Tests Failing

Count: 100

27_1, 27, 37_1, 37_2, 3f, db_1, db_2, db_3, db, dd25, dd2d, dd34, dd35, dd39, dd70, dd71, dd72, dd73, dd74, dd75, dd86, dd95, dd96, dda5, dda6, ddad, ddb4, ddb6, ddbc, ddbe, ed40, ed42, ed44, ed48, ed4a, ed4c, ed50, ed54, ed57, ed58, ed5a, ed5c, ed5f, ed60, ed64, ed67, ed68, ed6a, ed6c, ed6f, ed70, ed72, ed74, ed78, ed7a, ed7c, eda0, eda1, eda2, eda2_01, eda2_02, eda2_03, eda3_02, eda3_04, eda3_06, eda3_08, eda3_11, eda8, eda9, edaa, edaa_01, edaa_02, edaa_03, edab, edab_01, edab_02, edb0, edb1, edb2, edb3, edb8, edb9, edba, edbb, fd25, fd26, fd2a, fd2d, fd34, fd35, fd39, fd6e, fd94, fd96, fda5, fda6, fdac, fdb4, fdb6, fe