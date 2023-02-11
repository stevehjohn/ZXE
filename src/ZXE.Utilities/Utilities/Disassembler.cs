using ZXE.Core.Z80;
using ZXE.Utilities.Infrastructure;

namespace ZXE.Utilities.Utilities;

public class Disassembler
{
    private byte[]? _data;

    public void LoadData(byte[] data)
    {
        _data = data;
    }

    public List<CodeLine> Disassemble()
    {
        // TODO: This in not pretty.
        var processor = new Processor();

        var instructions = processor.Instructions;

        var location = 0;

        var code = new List<CodeLine>();

        while (location < _data.Length)
        {
            var instruction = instructions[_data[location]];

            if (instruction != null)
            {
                var line = new CodeLine(location, instruction.Mnemonic, _data[(location + 1)..(location + instruction.Length)]);

                code.Add(line);

                location += instruction.Length;
            }
            else
            {
                var line = new CodeLine(location, "FUCK");

                code.Add(line);

                break;
            }
        }

        return code;
    }
}