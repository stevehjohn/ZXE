using System.Text;
using ZXE.Core.System;
using ZXE.Core.System.Interfaces;
using ZXE.Core.Z80;

namespace ZXE.ZexTests.Infrastructure;

public class CpmProcessorExtension : IProcessorExtension
{
    private readonly Action _onComplete;

    public CpmProcessorExtension(Action onComplete)
    {
        _onComplete = onComplete;
    }

    public void InstructionProcessed(State state, Ram ram)
    {
        if (state.ProgramCounter != 0x05)
        {
            return;
        }

        switch (state.Registers[Register.C])
        {
            case 2:
                Console.Write((char) state.Registers[Register.E]);

                break;

            case 9:
                var text = new StringBuilder();

                var address = state.Registers.ReadPair(Register.DE);

                char c;

                while ((c = (char) ram[address]) != '$')
                {
                    Console.Write(c);

                    text.Append(c);

                    address++;
                }

                if (text.ToString().Contains("Tests complete"))
                {
                    _onComplete();
                }

                break;
        }
    }
}