using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using Monster;

namespace DigitalNeuralNetwork
{
    // method.Invoke(null, new object[] { null, null });
    public class BrainCompiler
    {
        const string Start = @"using System;
using Monster;
namespace DigitalNeuralNetwork
{
public class CompiledBrain
{
public static void Process(IBitInput input, IBitOutput output)
{
";
        const string End = @"}
}
}";
        static void AddNeuronResult(DigitalNeuron neuron, StringBuilder code, string input)
        {
            int start = code.Length;
            int count = 0;

            code.Append(neuron.InitValue ? "true" : "false");

            int inputCount = neuron.InputCount;
            var logicOperations = neuron.LogicOperations;
            for (int i = 0; i < inputCount; i++)
            {
                switch (logicOperations[i])
                {
                    case LogicOperation.None:
                        continue;

                    case LogicOperation.Or:
                        code.Append(" | " + input + "[" + i + "])");
                        count++;
                        break;

                    case LogicOperation.And:
                        code.Append(" & " + input + "[" + i + "])");
                        count++;
                        break;

                    case LogicOperation.Xor:
                        code.Append(" ^ " + input + "[" + i + "])");
                        count++;
                        break;
                }
            }

            if (neuron.Negative)
                code.Append(" ^ true");
            else
                code.Append(" ^ false");

            for (int i = 0; i < count; i++)
                code.Insert(start, '(');
        }

        public static MethodInfo CompileProcessor(IBrain brain)
        {
            var code = new StringBuilder(Start);

            string input = "input";
            string output;
            for (int i = 0; i < brain.Levels.Count; i++)
            {
                var level = brain.Levels[i];

                // if last layer
                if (i == brain.Levels.Count - 1)
                    output = "output";
                else
                {
                    output = "n" + i;
                    code.AppendLine("bool[] " + output + " = new bool[" + level.OutputCount + "];");
                }

                for (int bitPos = 0; bitPos < level.OutputCount; bitPos++)
                {
                    DigitalNeuron neuron = level.Neurons[bitPos] as DigitalNeuron;
                    code.Append(output + "[" + bitPos + "] = ");
                    AddNeuronResult(neuron, code, input);
                    code.AppendLine(";");
                }

                input = output;
            }

            code.Append(End);
            return CompileProcessor(code);
        }

        public static List<Assembly> assemblies = new List<Assembly>();
        public static MethodInfo CompileProcessor(StringBuilder code)
        {
            CompilerParameters CompilerParams = new CompilerParameters();
            CompilerParams.GenerateInMemory = true;
            CompilerParams.TreatWarningsAsErrors = false;
            CompilerParams.GenerateExecutable = false;
            CompilerParams.IncludeDebugInformation = false;
            CompilerParams.CompilerOptions = "/optimize";

            string[] references = { "Monster.dll" };
            CompilerParams.ReferencedAssemblies.AddRange(references);

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerResults compile = provider.CompileAssemblyFromSource(CompilerParams, code.ToString());

            if (compile.Errors.HasErrors)
            {
                string text = "Compile error: ";
                foreach (CompilerError ce in compile.Errors)
                {
                    text += "rn" + ce.ToString();
                }
                throw new Exception(text);
            }

            var assembly = compile.CompiledAssembly;
            assemblies.Add(assembly);
            var type = assembly.GetType("DigitalNeuralNetwork.CompiledBrain");
            var method = type.GetMethod("Process");

            return method;
        }
    }
}
