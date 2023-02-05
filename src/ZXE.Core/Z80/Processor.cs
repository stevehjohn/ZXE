using ZXE.Core.Infrastructure.Interfaces;
using ZXE.Core.System;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantCast
// ReSharper disable StringLiteralTypo

namespace ZXE.Core.Z80;

public class Processor
{
    private State _state;

    private readonly Instruction[] _instructions;

    private readonly ITracer? _tracer;

    public Processor()
    {
        _state = new State();

        _instructions = InitialiseInstructions();
    }

    public Processor(ITracer tracer)
    {
        _state = new State();

        _instructions = InitialiseInstructions();

        _tracer = tracer;
    }

    public string ProcessInstruction(Ram ram, bool trace = false)
    {
        var instruction = _instructions[ram[_state.ProgramCounter]];

        var data = ram[_state.ProgramCounter..(_state.ProgramCounter + instruction.Length)];

        if (_tracer != null)
        {
            _tracer.TraceBefore(instruction.Mnemonic, data, _state, ram);
        }

        instruction.Action(new Input(data, _state, ram));

        _state.ProgramCounter += instruction.Length;

        if (_tracer != null)
        {
            _tracer.TraceAfter(instruction.Mnemonic, data, _state, ram);
        }

        return string.Empty;
    }

    internal void SetState(State state)
    {
        _state = state;
    }

    private Instruction[] InitialiseInstructions()
    {
        var instructions = new Dictionary<int, Instruction>();

        InitialiseInstructions(instructions);

        var instructionArray = new Instruction[instructions.Max(i => i.Key) + 1];

        foreach (var instruction in instructions)
        {
            instructionArray[instruction.Key] = instruction.Value;
        }

        return instructionArray;
    }

    private static void InitialiseInstructions(Dictionary<int, Instruction> instructions)
    {
        instructions[0x00] = new Instruction("NOP", 1, _ => NOP(), 4);

        instructions[0x01] = new Instruction("LD BC, nn", 3, i => LD_rr_nn(i, Register.BC), 10);

        instructions[0x02] = new Instruction("LD (BC), A", 1, i => LD_addr_rr_A(i, Register.BC), 7);

        instructions[0x03] = new Instruction("INC BC", 1, i => INC_rr(i, Register.BC), 6);
        
        instructions[0x04] = new Instruction("INC B", 1, i => INC_r(i, Register.B), 4);
        
        instructions[0x05] = new Instruction("DEC B", 1, i => DEC_r(i, Register.B), 4);
        
        instructions[0x06] = new Instruction("LD B, n", 2, i => LD_r_n(i, Register.B), 7);

        instructions[0x07] = new Instruction("RLCA", 1, RLCA, 4);

        instructions[0x32] = new Instruction("LD (nn), A", 3, i => LD_addr_nn_r(i, Register.A), 13);

        instructions[0x3A] = new Instruction("LD A, (nn)", 3, i => LD_r_addr_nn(i, Register.A), 13);

        instructions[0x3E] = new Instruction("LD A, n", 2, i => LD_r_n(i, Register.A), 7);

        instructions[0x76] = new Instruction("HALT", 1, HALT, 4);
    }

    private static void NOP()
    {
    }

    private static void LD_rr_nn(Input input, Register register)
    {
        input.State.Registers.LoadFromRam(register, input.Data[1..3]);
    }

    private static void LD_addr_rr_A(Input input, Register register)
    {
        input.Ram[input.State.Registers.ReadPair(register)] = input.State.Registers[Register.A];
    }

    private static void INC_rr(Input input, Register register)
    {
        input.State.Registers.WritePair(register, (ushort) (input.State.Registers.ReadPair(register) + 1));
    }

    private static void INC_r(Input input, Register register)
    {
        var value = input.State.Registers[register];

        var result = (byte) (value + 1);

        input.State.Registers[register] = result;

        // FLAGS
        // Carry unaffected
        input.State.Flags.AddSubtract = false;
        input.State.Flags.ParityOverflow = value == 0x7F;
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = (value & 0x0F) + 1 > 0xF;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = (sbyte) result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;
    }

    private static void DEC_r(Input input, Register register)
    {
        var value = input.State.Registers[register];

        var result = (byte) (value - 1);

        input.State.Registers[register] = result;

        // FLAGS
        // Carry unaffected
        input.State.Flags.AddSubtract = true;
        input.State.Flags.ParityOverflow = value == 0x80;
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = (value & 0x0F) < 1;
        input.State.Flags.X2 = (result & 0x20) > 0;
        input.State.Flags.Zero = (sbyte) result == 0;
        input.State.Flags.Sign = (sbyte) result < 0;
    }

    private static void LD_r_n(Input input, Register register)
    {
        input.State.Registers[register] = input.Data[1];
    }

    private static void RLCA(Input input)
    {
        var topBit = (byte) ((input.State.Registers[Register.A] & 0x80) >> 7);

        var result = (byte) (((input.State.Registers[Register.A] << 1) & 0xFE) | topBit);

        input.State.Registers[Register.A] = result;

        // FLAGS
        input.State.Flags.Carry = topBit == 1;
        input.State.Flags.AddSubtract = false;
        // ParityOverflow unaffected
        input.State.Flags.X1 = (result & 0x08) > 0;
        input.State.Flags.HalfCarry = false;
        input.State.Flags.X2 = (result & 0x20) > 0;
        // Zero unaffected
        // Sign unaffected
    }

    private static void LD_addr_nn_r(Input input, Register register)
    {
        input.Ram[(input.Data[2] << 8) | input.Data[1]] = input.State.Registers[register];
    }

    private static void LD_r_addr_nn(Input input, Register register)
    {
        input.State.Registers[register] = input.Ram[(input.Data[2] << 8) | input.Data[1]];
    }

    private static void HALT(Input input)
    {
        input.State.Halted = true;
    }
}