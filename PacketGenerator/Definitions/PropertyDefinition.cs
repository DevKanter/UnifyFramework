using System.Text;

namespace PacketGenerator.Definitions
{
    internal class PropertyDefinition
    {
        public string[] Prefix = new[] {""};
        public string Name;
        public string Type;

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach(var pre in Prefix)
            {
                sb.Append($"{pre} ");
            }
            sb.Append($"{Type} ");
            sb.Append($"{Name}; ");
            return sb.ToString();
        }
    }

}
