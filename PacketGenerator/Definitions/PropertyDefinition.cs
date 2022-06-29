using System.Text;

namespace PacketGenerator.Definitions
{
    internal class PropertyDefinition
    {
        public string[] Prefix = new[] {"public"};
        public string Name;
        public string Type;
        public string DefaultValue;
        public string ParseAs;

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach(var pre in Prefix)
            {
                sb.Append($"{pre} ");
            }
            sb.Append($"{Type} ");
            sb.Append($"{Name} ");
            if (DefaultValue != null)
            {
                sb.Append($" = {DefaultValue}");
            }
            sb.Append(";");
            return sb.ToString();
        }
    }

}
