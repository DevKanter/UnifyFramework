using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator.Definitions
{
    internal abstract class ClassDefinition
    {
        protected string Name;
        protected PropertyDefinition[] Properties;
        protected MethodDefinition[] Methods;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"\tpublic class {Name}");
            sb.Append("{\n");
            foreach (var prop in Properties)
            {
                sb.Append($"\t\t{prop}\n");
            }
            sb.Append("}\n");
            
            foreach (var method in Methods)
            {
                sb.Append(method);
            }
            return sb.ToString();
        }

    }
    internal class PacketClass : ClassDefinition
    {
        public PacketClass(string name, PropertyDefinition[] properties)
        {
            Name = name;
            Properties = properties;
            Methods = new MethodDefinition[]
            {
                new WriteBufferMethod(properties)
            };
        }
    }
}
