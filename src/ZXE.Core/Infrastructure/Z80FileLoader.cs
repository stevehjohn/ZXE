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

        LoadRam(data);
    }

    private void LoadRam(byte[] data)
    {
        if (_state.ProgramCounter == 0)
        {
            throw new Exception("V2. Not currently supported.");
        }

        var compressed = (data[12] & 0x20) > 0;

        // 30 == V1 header length
        var dataToLoad = compressed ? Decompress(data[30..]) : data[30..];

        _ram.Load(dataToLoad, 0x4000);
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