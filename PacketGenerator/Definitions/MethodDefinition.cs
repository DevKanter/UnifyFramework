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
        protected string BaseCall;
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("\t\t");
            foreach(var decorator in Decorators)
            {
                sb.Append($"{decorator} ");
            }
            if(ReturnType != null)
                sb.Append($"{ReturnType} ");
            sb.Append($"{Name}(");
            foreach(var param in Parameters)
            {
                sb.Append($"{param},");
            }
            if (Parameters.Length > 0)
            { 
                sb.Remove(sb.Length - 1, 1); 
            }
            sb.Append(")");
            if (BaseCall != null)
            {
                sb.Append(BaseCall);
            }
            sb.Append("\n\t\t{\n" + PLACEHOLDER + "\n\t\t}\n");
            return sb.ToString();
        }
    }
    internal class WriteBufferMethod : MethodDefinition
    {
        private PropertyDefinition[] _properties;
        private bool _isPacket;
        public WriteBufferMethod(PropertyDefinition[] properties, bool isPacket=true)
        {
            if (isPacket)   Decorators = new[] { "public", "override" };
            else            Decorators = new[] {"public"};
            Name = "GetBytes";
            ReturnType = "void";
            Parameters = new[] { new ParameterDefinition() { Name = "buffer", Type = "ByteBuffer" } };
            _properties = properties;
            _isPacket = isPacket;
        }

        public override string ToString()
        {
            var baseString = base.ToString();
            var sb = new StringBuilder();
            int startIndex = 0;
            if (_isPacket)
            {
                sb.Append($"\t\t\tbuffer.WriteBlock(new byte[]");
                sb.Append("{(byte) " + _properties[0].Name + ",(byte) " + _properties[1].Name + "});\n");
                startIndex = 2;
            }
            for (int i= startIndex; i < _properties.Length; i++)
            {
                PropertyDefinition prop = _properties[i];
                sb.Append($"\t\t\t{GetBufferString(prop)};\n");
            }
            return baseString.Replace(PLACEHOLDER, sb.ToString());
        }

        private string GetBufferString(PropertyDefinition prop)
        {
            string type = prop.Type;
            if(prop.ParseAs != null)
            {
                type = prop.ParseAs;
            }
            switch (type)
            {
                case "int": return $"buffer.WriteInt32({prop.Name})";
                case "string": return $"buffer.WriteString({prop.Name})";
                case "byte": return $"buffer.WriteByte((byte){prop.Name})";
                default:
                    return $"{prop.Name}.GetBytes(buffer)";
            }

        }
    }
    internal class GetSizeMethod : MethodDefinition
    {
        private PropertyDefinition[] _properties;
        private bool _isPacket;
        public GetSizeMethod(PropertyDefinition[] properties, bool isPacket=true)
        {
            _properties = properties;
            if (isPacket)   Decorators = new[] { "public", "override" };
            else            Decorators = new[] { "public" };
            ReturnType = "int";
            Name = "GetSize";
            Parameters = new ParameterDefinition[0];
            _isPacket = isPacket;
        }
        public override string ToString()
        {
            var baseString = base.ToString();
            var sb = new StringBuilder();
            if (_properties.Length == 0)
            {
                sb.Append("\t\t\treturn 0;");
                return baseString.Replace(PLACEHOLDER, sb.ToString());
            }
            else
            {
                sb.Append("\t\t\treturn ");
            }
            var statIndex = 0;
            if (_isPacket)
            {
                sb.Append("2 + ");
                statIndex = 2;
            }
            for (int i = statIndex; i < _properties.Length; i++)
            {
                PropertyDefinition prop = _properties[i];
                sb.Append(GetSizeString(prop));
                sb.Append(" + ");
            }
            if (_properties.Length > 0)
            {
                sb.Remove(sb.Length - 3, 3);
            }
            sb.Append(";");
            return baseString.Replace(PLACEHOLDER, sb.ToString());
        }
        private string GetSizeString(PropertyDefinition prop)
        {
            string type = prop.Type;
            if (prop.ParseAs != null)
            {
                type = prop.ParseAs;
            }
            switch (type)
            {
                case "int": return "sizeof(int)";
                case "string": return $"{prop.Name}.Length + 1";
                case "byte": return "sizeof(byte)";
                default:
                    return $"{prop.Name}.GetSize()";
            }
        }
    }

    internal class RegisterClientAction : MethodDefinition
    {
        private PacketClass[] _serverPackets;
        public RegisterClientAction(PacketClass[] serverPackets)
        {
            Decorators = new[]{ "private"};
            ReturnType = "void";
            Name = "RegisterActions";
            Parameters = new ParameterDefinition[0];
            _serverPackets = serverPackets;
        }
        public override string ToString()
        {
            var baseString = base.ToString();
            var sb = new StringBuilder();
            foreach(var packet in _serverPackets)
            {
                sb.Append($"\t\t\tRegisterAction(PacketCategory.{packet.Category}, (byte){packet.Category}ID.{packet.Name},On{packet.Name} );\n");
            }
            //RegisterAction(PacketCategory.Authentification, (byte)AuthentificationID.S2CHello, OnS2CHello);
            return baseString.Replace(PLACEHOLDER, sb.ToString());
        }
    }
    internal class ClientAction : MethodDefinition
    {
        private PacketClass _serverPacket;

        public ClientAction(PacketClass serverPacket)
        {
            Decorators = new[] { "public" };
            ReturnType = "void";
            Name = $"On{serverPacket.Name}";
            Parameters = new ParameterDefinition[]
            {
                new ParameterDefinition()
                {
                    Type = "ByteBuffer",
                    Name ="buffer"
                }
            };
            _serverPacket = serverPacket;
        }
        public override string ToString()
        {
            var baseString = base.ToString();
            var sb = new StringBuilder();
            sb.Append($"\t\t\tOn{_serverPacket.Name}(new {_serverPacket.Name}(buffer));");
            return baseString.Replace(PLACEHOLDER, sb.ToString());
        }
    }
    internal class ClientAbstractAction : MethodDefinition
    {
        private PacketClass _serverPacket;
        public ClientAbstractAction(PacketClass serverPacket)
        {
            Name = $"On{serverPacket.Name}";
            _serverPacket = serverPacket;
        }
        public override string ToString()
        {
            return $"\t\tpublic abstract void {Name}({_serverPacket.Name} packet);\n";
        }
    }
    internal class BufferConstructor : MethodDefinition
    {
        private PropertyDefinition[] _properties;
        private bool _isPacket;
        public BufferConstructor(ClassDefinition classDefinition, bool isPacket = true)
        {
            _properties = classDefinition.Properties;
            Decorators = new[] { "public"};
            Name = classDefinition.Name;
            Parameters = new ParameterDefinition[] {new ParameterDefinition() { Name="buffer",Type="ByteBuffer"} };
            _isPacket = isPacket;
            if (_isPacket)
                BaseCall = ": base(buffer)";
        }
        public override string ToString()
        {
            var baseString = base.ToString();
            var sb = new StringBuilder();
            var startIndex = 0;
            if (_isPacket)
            {
                sb.Append($"\t\t\t{_properties[0].Name} =(PacketCategory) buffer.ReadByte();\n");
                sb.Append($"\t\t\t{_properties[1].Name} =({_properties[1].Type}) buffer.ReadByte();\n");
                startIndex = 2;
            }
            for (int i = startIndex; i < _properties.Length; i++)
            {
                PropertyDefinition prop = _properties[i];
                sb.Append($"\t\t\t{GetBufferString(prop)};\n");
            }
            return baseString.Replace(PLACEHOLDER, sb.ToString());
            //return result.Replace("ByteBuffer buffer)", "ByteBuffer buffer): base(buffer)");
        }
        private string GetBufferString(PropertyDefinition prop)
        {
            string type = prop.Type;
            if (prop.ParseAs != null)
            {
                type = prop.ParseAs;
            }
            switch (type)
            {
                case "int": return $"{prop.Name} = buffer.ReadInt32()";
                case "string": return $"{prop.Name} = buffer.ReadString()";
                case "byte": return $"{prop.Name} = buffer.ReadByte()";
                default:
                    return $"{prop.Name} = new {prop.Type}(buffer)";
            }

        }
    }
    internal class Defaultconstructor : MethodDefinition
    {
        public Defaultconstructor(ClassDefinition classDefinition)
        {
            Decorators = new[] { "public" };
            Name = classDefinition.Name;
            Parameters = new ParameterDefinition[0];
        }
        public override string ToString()
        {
            var baseString = base.ToString();
            var sb = new StringBuilder();
            sb.Append("");
            return baseString.Replace(PLACEHOLDER, sb.ToString());
        }
    }
}
