# ZXE

A ZX Spectrum emulator. Mostly about just seeing if I can.

128/48 BASIC both seem to work.

A bunch of games also, see https://www.outsidecontextproblem.com/zxe-more-progress.html.

## ZXE.Desktop.Host

Open `ZXE.Desktop.sln`.

Cross platform host for the emulator. When running, press F10 for the menu.

I did have one issue building on macOS on an M1 processor. Solved by https://stackoverflow.com/a/74317078.

- `brew install freeimage`
- `sudo ln -s /opt/homebrew/Cellar/freeimage/3.18.0/lib/libfreeimage.dylib /usr/local/lib/libfreeimage`

## ZXE.Windows.Host

Open `ZXE.Windows.sln`.

Windows host for the emulator. When running, press F10 for the menu.

Should retire this in favour of ZXE.Desktop.Host if it proves reliable across platforms.

## Current tested games list

Some games require 128 emulation, some 48. Will update list to indicate this when I can.

- Barbarian 2
- Batman: The Movie
- Cybernoid
- Cybernoid 2
- Dan Dare 3
- Dizzy
- Exolon
- Fantasy World Dizzy
- Great Escape, The
- Head Over Heels
- Indiana Jones and the Temple of Doom 
- Jet Pac
- Manic Miner
- Myth: History in the Making
- Robocop
- Robocop 2
- Robocop 3
- Teenage Mutant Hero Turtles
- Treasure Island Dizzy
- Wizard's Lair

## ROMS

- https://github.com/archtaurus/RetroPieBIOS

## Sinclair Wiki

- https://sinclair.wiki.zxnet.co.uk/wiki/Main_Page

## Useful Resources

- https://clrhome.org/table/
- https://floooh.github.io/
- http://www.breakintoprogram.co.uk/hardware/computers/zx-spectrum/memory-map
- https://worldofspectrum.org/faq/reference/128kreference.htm
- https://github.com/floooh/chips-test/tree/master/tests/fuse
- https://dotneteer.github.io/zx-spectrum/2018/01/26/zxspectrum-part-4.html

## Games

- https://www.planetemu.net/roms/sinclair-zx-spectrum-z80

## Game pokes

- https://www.the-tipshop.co.uk/

