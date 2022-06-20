using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using PacketGenerator.Definitions;
using System;
using System.Diagnostics;
using System.Text;

namespace PacketGenerator
{
    [Generator]
    public class TestGenerator : ISourceGenerator
    {
        private readonly string header = @"using Unify;
using Unify.Common;

namespace PacketLib
{";
        public void Execute(GeneratorExecutionContext context)
        {
            var lines = new[]
            {
                "S2CHello;Key:int,Name:string",
                "CharacterData;x:int,y:int,z:int",
                "C2SHello;Key:int,IpAddress:string,CharData:CharacterData"
            };
            
            foreach (var line in lines)
            {
                var def = ParsePacketDefinition(line);

                var sb = new StringBuilder();
                var classDef = $"\n\tpublic class {def.Name} : Packet";
                sb.Append(header);
                sb.Append(classDef);
                sb.Append("\n\t{\n");
                foreach(var prop in def.Properties)
                {
                    sb.AppendLine($"\t\tpublic {prop.Type} {prop.Name};");
                }
                sb.Append("\t\t");
                sb.Append(@"public override void GetBytes(ByteBuffer buffer)");
                sb.Append("\n\t\t{\n");
                foreach (var prop in def.Properties)
                {
                    sb.AppendLine($"\t\t\t{GetBufferString(prop)};");
                }
                sb.Append("\n\t\t}\n");
                sb.AppendLine();
                sb.Append("\t\t");
                sb.Append(@"public override int GetSize()");
                sb.Append("\n\t\t{\n");
                sb.Append("\t\t\treturn ");
                foreach (var prop in def.Properties)
                {
                    sb.Append(GetSizeString(prop));
                    sb.Append(" + ");
                }
                sb.Remove(sb.Length - 3, 3);
                sb.Append(";");
                sb.Append("\n\t\t}\n");
                sb.Append("\n\t}\n");
                sb.Append("\n}\n");


                context.AddSource($"{def.Name}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
            }

            

//                var code = @"
//using Unify;
//using Unify.Common;

//namespace PacketLib
//{
//    public class S2CHello : Packet
//    {
//        public int Key;
//        public override void GetBytes(ByteBuffer buffer)
//        {
//            buffer.WriteInt32(Key);
//        }

//        public override int GetSize()
//        {
//            return sizeof(int);
//        }
//    }
//}

//";

            

        }

        public void Initialize(GeneratorInitializationContext context)
        {
//#if DEBUG
//            if (!Debugger.IsAttached)
//            {
//                Debugger.Launch();
//            }
//#endif
            Debug.WriteLine("Initalize code generator");
        }

        private PacketDefinition ParsePacketDefinition(string line)
        {
            //"S2CHello;Key:int,Name:string"
            var definition = new PacketDefinition();

            var name = line.Split(';')[0];
            var props = line.Split(';')[1].Split(',');

            definition.Name = name;
            definition.Properties = new PropertyDefinition[props.Length];

            for (int i = 0; i < props.Length; i++)
            {
                string prop = props[i];
                var propDef = new PropertyDefinition();
                propDef.Name = prop.Split(':')[0];
                propDef.Type = prop.Split(':')[1];

                definition.Properties[i] = propDef;
            }
            return definition;
        }
        private string GetBufferString(PropertyDefinition prop)
        {
            switch (prop.Type)
            {
                case "int": return $"buffer.WriteInt32({prop.Name})";
                case "string": return $"buffer.WriteString({prop.Name})";
                default:
                    return $"{prop.Name}.GetBytes(buffer)";
            }

        }
        private string GetSizeString(PropertyDefinition prop)
        {
            switch (prop.Type)
            {
                case "int": return "sizeof(int)";
                case "string": return $"{prop.Name}.Length + 1";
                default:
                    return $"{prop.Name}.GetSize()";
            }
        }
    }

}
