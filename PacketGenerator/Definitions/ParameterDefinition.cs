namespace PacketGenerator.Definitions
{
    internal class ParameterDefinition
    {
        public string Name;
        public string Type;
        public string PreFix = "";

        public override string ToString()
        {
            return $"{Type} {Name}";
        }
    }
}
