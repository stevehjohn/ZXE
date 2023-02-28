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

27_1, 27, 37_1, 37_2, 37_3, 37, 3f, 76, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 8a, 8b, 8c, 8d, 8e, 8f, a0, a3, a4, a6, a7, a8, ad, af, b0, b1, b2, b7, c0_1, c4_1, c7, c8_2, cb00, cb01, cb03, cb04, cb06, cb07, cb08, cb09, cb0a, cb0d, cb0e, cb0f, cb10, cb11, cb12, cb13, cb14, cb15, cb16, cb17, cb18, cb19, cb1b, cb1c, cb1d, cb1e, cb79, cb7b, cb7d, cb7e, cb7f_1, cc_1, ce, cf, d0_1, d4_1, d6, d7, d8_2, db_1, db_2, db_3, db, dc_1, dd24, dd25, dd2c, dd2d, dd34, dd35, dd39, dd70, dd71, dd72, dd73, dd74, dd75, dd86, dd8c, dd95, dd96, dda5, dda6, ddad, ddb4, ddb6, ddbc, ddbe, ddcb40, ddcb41, ddcb42, ddcb44, ddcb45, ddcb46, ddcb47, ddcb48, ddcb49, ddcb4a, ddcb4b, ddcb4c, ddcb4d, ddcb50, ddcb51, ddcb52, ddcb53, ddcb54, ddcb55, ddcb56, ddcb58, ddcb5a, ddcb5d, ddcb5e, ddcb60, ddcb61, ddcb62, ddcb63, ddcb64, ddcb66, ddcb67, ddcb68, ddcb69, ddcb6a, ddcb6b, ddcb6d, ddcb6e, ddcb6f, ddcb70, ddcb73, ddcb75, ddcb76, ddcb78, ddcb79, ddcb7a, ddcb7b, ddcb7c, ddcb7d, ddcb7e, ddcb7f, dde9, df, e0_1, e4_1, e6, e7, e8_2, e9, ec_1, ed40, ed42, ed44, ed45, ed48, ed4a, ed4c, ed4d, ed50, ed54, ed55, ed57, ed58, ed5a, ed5c, ed5d, ed5f, ed60, ed64, ed65, ed67, ed68, ed6a, ed6c, ed6d, ed6f, ed70, ed72, ed74, ed75, ed78, ed7a, ed7c, ed7d, eda0, eda1, eda2, eda2_01, eda2_02, eda2_03, eda3, eda3_01, eda3_02, eda3_03, eda3_04, eda3_05, eda3_06, eda3_07, eda3_08, eda3_09, eda3_10, eda3_11, eda8, eda9, edaa, edaa_01, edaa_02, edaa_03, edab, edab_01, edab_02, edb0, edb1, edb2, edb3, edb8, edb9, edba, edbb, ee, ef, f0_1, f4_1, f7, f8_2, fc_1, fd24, fd25, fd26, fd2a, fd2c, fd2d, fd34, fd35, fd39, fd6e, fd86, fd8c, fd8d, fd8e, fd94, fd96, fda4, fda5, fda6, fdac, fdb4, fdb6, fdcb40, fdcb41, fdcb42, fdcb43, fdcb44, fdcb45, fdcb46, fdcb47, fdcb48, fdcb4a, fdcb4c, fdcb4d, fdcb4e, fdcb50, fdcb51, fdcb52, fdcb53, fdcb54, fdcb55, fdcb56, fdcb57, fdcb58, fdcb5c, fdcb5e, fdcb60, fdcb61, fdcb62, fdcb63, fdcb64, fdcb66, fdcb67, fdcb69, fdcb6a, fdcb6c, fdcb6e, fdcb6f, fdcb70, fdcb71, fdcb72, fdcb74, fdcb76, fdcb78, fdcb79, fdcb7a, fdcb7b, fdcb7c, fdcb7d, fdcb7e, fdcb7f, fde9, fe