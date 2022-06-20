using System.Text;

namespace PacketGenerator.Definitions
{
    internal abstract class MethodDefinition
    {
        protected const string PLACEHOLDER = "//[PLACEHOLDER]";

        protected string[] Decorators;
        protected string Name;
        protected string ReturnType;
        protected ParameterDefinition[] Parameters;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("\t\t");
            foreach(var decorator in Decorators)
            {
                sb.Append($"{decorator} ");
            }
            sb.Append($"{ReturnType} ");
            sb.Append($"{Name} (");
            foreach(var param in Parameters)
            {
                sb.Append($"{param},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(")\n{\n"+ PLACEHOLDER +"\n}");
            return sb.ToString();
        }
    }
    internal class WriteBufferMethod : MethodDefinition
    {
        private PropertyDefinition[] _properties;
        public WriteBufferMethod(PropertyDefinition[] properties)
        {
            Decorators = new[] {"public override void"};
            Name = "GetBytes";
            Parameters = new[] { new ParameterDefinition() { Name = "buffer", Type = "ByteBuffer" } };
            _properties = properties;
        }

        public override string ToString()
        {
            var baseString = base.ToString();
            var sb = new StringBuilder();
            foreach(var prop in _properties)
            {
                sb.Append($"\t\t\t{GetBufferString(prop)}");
            }
            return baseString.Replace(PLACEHOLDER, sb.ToString());
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
    }
}
