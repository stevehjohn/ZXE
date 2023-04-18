# ZXE

A ZX Spectrum emulator. Mostly about just seeing if I can.

128/48 BASIC both seem to work.

A bunch of games also, see https://www.outsidecontextproblem.com/zxe-more-progress.html.

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

- http://slady.net/Sinclair-ZX-Spectrum-keyboard/
- https://gist.github.com/drhelius/8497817
- https://mdfs.net/Software/Z80/Exerciser/Spectrum/
- https://github.com/raxoft/z80test
- https://floooh.github.io/
- https://floooh.github.io/2021/12/17/cycle-stepped-z80.html#testing
- https://floooh.github.io/2021/12/06/z80-instruction-timing.html
- https://clrhome.org/asm/
- https://ps4star.github.io/z80studio/
- https://github.com/redcode/Z80/wiki/Z80-Test-Suite
- https://github.com/anotherlin/z80emu/blob/master/zextest.c
- https://www.angelfire.com/ga/jamiemylandshome/download.html

## Miscellaneous References

- https://github.com/ArjunNair/Zero-Emulator/blob/master/Ziggy/Speccy/Z80.cs
- https://worldofspectrum.org/faq/reference/128kreference.htm
- https://worldofspectrum.org/forums/discussion/20345/redirect/p1
- https://worldofspectrum.org/forums/discussion/41704
- https://clrhome.org/table/
- http://www.z80.info/z80syntx.htm
- http://www.z80.info/z80code.htm
- http://www.z80.info/z80oplist.txt
- http://www.z80.info/decoding.htm
- http://www.z80.info/z80sflag.htm
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