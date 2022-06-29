using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator.Definitions
{
    internal abstract class ClassDefinition
    {
        protected string Header = "using Unify;\nusing Unify.Common;\nusing PacketLib.Enums;\n\nnamespace PacketLib\n{\n";
        public string Name;
        protected string Prefix="public class";
        protected string BaseClass;
        public PropertyDefinition[] Properties = new PropertyDefinition[0];
        public MethodDefinition[] Methods = new MethodDefinition[0];

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Header);
            sb.Append($"\t{Prefix} {Name} ");
            if (BaseClass != null) sb.Append($": {BaseClass}");
            sb.Append("\n\t{\n");
            foreach (var prop in Properties)
            {
                sb.Append($"\t\t{prop}\n");
            }
            sb.Append("\n");
            
            foreach (var method in Methods)
            {
                sb.Append(method);
                sb.AppendLine();
            }
            sb.Append("\t}\n");
            sb.Append("}\n");
            return sb.ToString();
        }
       
    }
    internal class PacketClass : ClassDefinition
    {
        public readonly PacketType PacketType;
        public readonly string Category;
        public PacketClass(string name,PacketType type,string category, PropertyDefinition[] properties)
        {
            Name = name;
            PacketType = type;
            Category = category;
            BaseClass = "Packet";
            Properties = new PropertyDefinition[properties.Length+2];
            Properties[0] = new PropertyDefinition()
            {
                Name = "Category",
                Type = "PacketCategory",
                Prefix = new[] { "private ", "readonly " },
                DefaultValue = $"PacketCategory.{category}",
            };
            Properties[1] = new PropertyDefinition() 
            { 
                Name = "CategoryID", 
                Type = $"{category}ID", 
                Prefix = new[] { "private ", "readonly " }, 
                DefaultValue = $"{category}ID.{Name}",
            };

            for (int i = 0; i < properties.Length; i++)
            {
                Properties[i + 2] = properties[i];
            }

            Methods = new MethodDefinition[]
            {
                new BufferConstructor(this),
                new Defaultconstructor(this),
                new WriteBufferMethod(Properties),
                new GetSizeMethod(Properties)
            };
        }
    }
    internal class DataClass: ClassDefinition
    {
        public DataClass(string name, PropertyDefinition[] properties)
        {
            Name = name;
            Properties = properties;
            BaseClass = "ISizeable";           
            Methods = new MethodDefinition[]
            {
                new Defaultconstructor(this),
                new BufferConstructor(this,false),
                new WriteBufferMethod(properties,false),
                new GetSizeMethod(properties,false),
            };
        }
    }
    internal class UnifyClientClass : ClassDefinition
    {
        public UnifyClientClass(PacketClass[] serverPackets)
        {
            Header = "using Unify;\nusing Unify.Common;\nusing PacketLib.Enums;\nusing PacketLib;\n\nnamespace Unify.ClientSide\n{\n";
            Name = "UnifyClient";
            Prefix = "public abstract partial class";

            var methods = new List<MethodDefinition>(serverPackets.Length*2+1);

            foreach (var packet in serverPackets)
            {
                methods.Add(new ClientAction(packet));
                methods.Add(new ClientAbstractAction(packet));
            }
            methods.Add(new RegisterClientAction(serverPackets));
            Methods = methods.ToArray();

        }
    }

    internal enum PacketType
    {
        ServerPacket,
        ClientPacket,
    }
}
