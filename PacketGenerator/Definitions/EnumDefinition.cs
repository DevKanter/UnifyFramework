using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator.Definitions
{
    internal class EnumDefinition
    {
        public string Name;
        private string[] _members;
        private readonly string header = "using Unify;\nusing Unify.Common;\n\nnamespace PacketLib.Enums\n{\n";
        public EnumDefinition(string name,string[] members)
        {
            Name = name;
            _members = members;
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(header);
            sb.Append($"\tpublic enum {Name}");
            sb.Append("\n\t{");
            foreach (var member in _members)
            {
                sb.Append($"\n\t\t{member},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("\n\t}\n");
            sb.Append("}");
            return sb.ToString();
        }
    }
}
