# ZXE

A ZX Spectrum emulator. Mostly about just seeing if I can.

## Main TODOs

- Check which instructions set/reset Q.
- With some conditional opcodes (e.g. CALL cc, JP cc, JR cc, RET cc), more cycles are used if condition is met. Account for this in the timings. Actually, maybe this can be handled with the condition checking being a separate function...
- Check undocumented flag implementation against https://worldofspectrum.org/faq/reference/z80reference.htm
- IN/OUT stuff.
- Interrupt stuff.
- R register shenannigans.

## Features

So far, apart from a WIP emulator, I have...

### Colourised Tracing of Machine Code Execution

![Console Tracing](Images/Tracing-1.png)

## Solution Structure

### ZXE.Core

This is the emulator. I have tried to keep all the Z80 specific code in the Z80 folder so it can be used in contexts other than only a ZX Spectrum emulator.

### ZXE.Core.Tests

Unit tests for the above.

### ZXE.Core.Tests.Console

Run some select tests in the console so the developer has easy access to the test output.

### ZXE.Core.ThirdPartyTests

Runs tests against this awesome suite:

- https://github.com/raddad772/jsmoo/tree/main/misc/tests/GeneratedTests/z80/v1

## ROMS

- https://github.com/archtaurus/RetroPieBIOS

## Sinclair Wiki

- https://sinclair.wiki.zxnet.co.uk/wiki/Main_Page

## Useful Resources

- https://github.com/raxoft/z80test
- https://floooh.github.io/2021/12/17/cycle-stepped-z80.html#testing
- https://floooh.github.io/2021/12/06/z80-instruction-timing.html
- https://clrhome.org/asm/
- https://ps4star.github.io/z80studio/

## Miscellaneous References

- https://clrhome.org/table/
- http://www.z80.info/z80syntx.htm
- http://www.z80.info/z80code.htm
- http://www.z80.info/z80oplist.txt
- http://www.z80.info/decoding.htm
- http://www.breakintoprogram.co.uk/hardware/computers/zx-spectrum/memory-map
- https://landley.net/history/mirror/cpm/z80.html
- https://worldofspectrum.org/faq/reference/z80reference.htm
- https://github.com/reclaimed/prettybasic/tree/master/doc
- https://www.tablix.org/~avian/spectrum/rom/zx82.htm
- http://www.breakintoprogram.co.uk/hardware/computers/zx-spectrum/keyboard
- https://skoolkid.github.io/rom/maps/all.html
- https://luckyredfish.com/interrupts-on-the-zx-spectrum/
- https://worldofspectrum.org/
- https://github.com/floooh/chips-test/tree/master/tests/fuse
- http://rk.nvg.ntnu.no/sinclair/faq/tech_z80.html
- https://dotneteer.github.io/zx-spectrum/2018/01/26/zxspectrum-part-4.html