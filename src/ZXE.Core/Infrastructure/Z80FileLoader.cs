using ZXE.Core.System;
using ZXE.Core.Z80;

namespace ZXE.Core.Infrastructure;

public class Z80FileLoader
{
    private readonly State _state;

    private readonly Ram _ram;

    public Z80FileLoader(State state, Ram ram)
    {
        _state = state;

        _ram = ram;
    }

    public void Load(string fileName)
    {
        var data = File.ReadAllBytes(fileName);

        LoadRegisters(data);

        if (_state.ProgramCounter != 0)
        {
            LoadRam(data);

            return;
        }

        if (data[30] is not (23 or 54 or 55))
        {
            // TODO: Replace with proper exception.
            throw new Exception("Dodgy file.");
        }

        var ramStart = 32 + data[30];

        LoadPages(data, ramStart);

        LoadV2Parameters(data);
    }

    private void LoadPages(byte[] data, int offset)
    {
        // https://worldofspectrum.org/faq/reference/z80format.htm

        while (offset < data.Length)
        {
            var pageLength = data[offset + 1] << 8 | data[offset];

            byte[] pageData;

            if (pageLength == 0xFF)
            {
                pageData = data[(offset + 3)..(offset + 3 + 0xFFFF)];
            }
            else
            {
                pageData = DecompressV2(data[(offset + 3)..(offset + 3 + pageLength)]);
            }

            _ram.LoadIntoPage(data[offset + 2] - 3, pageData);

            offset += pageLength + 3;
        }
    }

    private void LoadV2Parameters(byte[] data)
    {
        _state.ProgramCounter = data[33] << 8 | data[32];

        if (data[34] is 3 or 4)
        {
            _ram.SetBank(data[35] & 0b0000_0111);

            _ram.SetScreen((data[35] & 0b0000_1000) > 0 ? 2 : 1);
        }
    }

    private void LoadRam(byte[] data)
    {
        var compressed = (data[12] & 0x20) > 0;

        // 30 == V1 header length
        var dataToLoad = compressed ? Decompress(data[30..]) : data[30..];

        _ram.Load(dataToLoad, 0x4000);
    }

    private byte[] DecompressV2(byte[] data)
    {
        var decompressed = new List<byte>();

        var i = 0;

        while (i < data.Length)
        {
            if (data[i] == 0xED)
            {
                if (data[i + 1] == 0xED)
                {
                    var length = data[i + 2];

                    for (var r = 0; r < length; r++)
                    {
                        decompressed.Add(data[i + 3]);
                    }

                    i += 4;

                    continue;
                }

                decompressed.Add(data[i]);

                i++;
            }
            else
            {
                decompressed.Add(data[i]);

                i++;
            }
        }

        return decompressed.ToArray();
    }

    private byte[] Decompress(byte[] data)
    {
        var decompressed = new List<byte>();

        var i = 0;

        while (i < data.Length)
        {
            if (data[i] == 0x00)
            {
                if (data[i + 1] == 0xED)
                {
                    if (data[i + 2] == 0xED)
                    {
                        if (data[i + 3] == 0x00)
                        {
                            break;
                        }
                    }
                }

                decompressed.Add(0);

                i++;
            }
            else if (data[i] == 0xED)
            {
                if (data[i + 1] == 0xED)
                {
                    var length = data[i + 2];

                    for (var r = 0; r < length; r++)
                    {
                        decompressed.Add(data[i + 3]);
                    }

                    i += 4;

                    continue;
                }

                decompressed.Add(data[i]);

                i++;
            }
            else
            {
                decompressed.Add(data[i]);

                i++;
            }
        }

        return decompressed.ToArray();
    }

    private void LoadRegisters(byte[] data)
    {
        _state.Registers.WritePair(Register.AF, (ushort) (data[0] << 8 | data[1]));

        _state.Registers.WritePair(Register.BC, (ushort) (data[3] << 8 | data[2]));

        _state.Registers.WritePair(Register.HL, (ushort) (data[5] << 8 | data[4]));

        _state.ProgramCounter = (ushort) (data[7] << 8 | data[6]);

        _state.StackPointer = (ushort) (data[9] << 8 | data[8]);

        _state.Registers[Register.I] = data[10];

        var byte12 = data[12];

        if (byte12 == 0xFF)
        {
            byte12 = 0x01;
        }

        _state.Registers[Register.R] = (byte) ((data[11] & 0x7F) | ((byte12 & 0x01) << 7));

        _state.Registers.WritePair(Register.DE, (ushort) (data[14] << 8 | data[13]));

        _state.Registers.WritePair(Register.BC_, (ushort) (data[16] << 8 | data[15]));

        _state.Registers.WritePair(Register.DE_, (ushort) (data[18] << 8 | data[17]));

        _state.Registers.WritePair(Register.HL_, (ushort) (data[20] << 8 | data[19]));

        _state.Registers.WritePair(Register.AF_, (ushort) (data[21] << 8 | data[22]));

        _state.Registers.WritePair(Register.IY, (ushort) (data[24] << 8 | data[23]));

        _state.Registers.WritePair(Register.IX, (ushort) (data[26] << 8 | data[25]));

        _state.InterruptFlipFlop1 = data[27] != 0;

        _state.InterruptFlipFlop2 = data[28] != 0;

        _state.InterruptMode = (InterruptMode) (data[29] & 0x03);

        _state.Flags = Flags.FromByte(_state.Registers[Register.F]);

        _state.OpcodePrefix = 0;
    }
}