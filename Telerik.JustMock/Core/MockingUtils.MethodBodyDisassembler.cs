/*
 JustMock Lite
 Copyright © 2018 Progress Software Corporation

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Telerik.JustMock.Core
{
    internal static partial class MockingUtil
    {
        [StructLayout(LayoutKind.Explicit)]
        internal struct Operand
        {
            [FieldOffset(0)]
            public byte Byte;
            [FieldOffset(0)]
            public short Short;
            [FieldOffset(0)]
            public ushort UShort;
            [FieldOffset(0)]
            public int Int;
            [FieldOffset(0)]
            public uint UInt;
            [FieldOffset(0)]
            public float Float;
            [FieldOffset(0)]
            public double Double;
            [FieldOffset(0)]
            public long Long;
        }

        internal class Instruction
        {
            public readonly OpCode OpCode;
            public readonly int OperandSize;
            private readonly Operand? operand;
            public readonly Module Module;

            public Operand Operand
            {
                get { return operand.Value; }
            }

            public int Length { get { return OpCode.Size + OperandSize; } }

            internal Instruction(OpCode opCode, int operandSize, Operand? operand, Module module)
            {
                this.OpCode = opCode;
                this.OperandSize = operandSize;
                this.operand = operand;
                Module = module;
            }
        }

        internal class MethodBodyDisassembler
        {
            private static readonly HashSet<int> prefixes = new HashSet<int>
            {
                OpCodes.Prefix1.Value,
                OpCodes.Prefix2.Value,
                OpCodes.Prefix3.Value,
                OpCodes.Prefix4.Value,
                OpCodes.Prefix5.Value,
                OpCodes.Prefix6.Value,
                OpCodes.Prefix7.Value,
            };

            private static readonly Dictionary<short, OpCode> opCodeMap = new Dictionary<short, OpCode>();

            static MethodBodyDisassembler()
            {
                var opcodes = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(field => field.FieldType == typeof(OpCode));

                foreach (var opcodeField in opcodes)
                {
                    var opcode = (OpCode)opcodeField.GetValue(null);
                    opCodeMap.Add(opcode.Value, opcode);
                }
            }

            public static IEnumerable<Instruction> DisassembleMethodInfo(MethodBase method)
            {
                var body = method.GetMethodBody();
                if (body != null)
                {
                    var il = body.GetILAsByteArray();

                    for (int i = 0; i < il.Length;)
                    {
                        int baseIdx = i;
                        int code1 = il[i++];
                        var code = (short)(prefixes.Contains(code1) ? (code1 << 8) | il[i++] : code1);
                        var opcode = opCodeMap[code];

                        int operandSize;
                        bool validOperand = true;
                        var operand = default(Operand);
                        switch (opcode.OperandType)
                        {
                            case OperandType.InlineBrTarget:
                            case OperandType.InlineField:
                            case OperandType.InlineI:
                            case OperandType.InlineMethod:
                            case OperandType.InlineSig:
                            case OperandType.InlineString:
                            case OperandType.InlineTok:
                            case OperandType.InlineType:
                            case OperandType.ShortInlineR:
                                operand.Int = BitConverter.ToInt32(il, i);
                                operandSize = 4;
                                break;
                            case OperandType.InlineI8:
                            case OperandType.InlineR:
                                operand.Long = BitConverter.ToInt64(il, i);
                                operandSize = 8;
                                break;
                            case OperandType.InlineNone:
                                validOperand = false;
                                operandSize = 0;
                                break;
                            case OperandType.InlineVar:
                                operand.Short = BitConverter.ToInt16(il, i);
                                operandSize = 2;
                                break;
                            case OperandType.ShortInlineBrTarget:
                            case OperandType.ShortInlineI:
                            case OperandType.ShortInlineVar:
                                operand.Byte = il[i];
                                operandSize = 1;
                                break;
                            case OperandType.InlineSwitch:
                                var branchCount = BitConverter.ToInt32(il, i);
                                operandSize = 4 + 4 * branchCount;
                                validOperand = false;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        i += operandSize;

                        var instrBuffer = il.Skip(baseIdx).Take(i - baseIdx).ToArray();

                        yield return new Instruction(opcode, operandSize, validOperand ? (Operand?)operand : null, method.Module);
                    }
                }
            }
        }
    }
}
