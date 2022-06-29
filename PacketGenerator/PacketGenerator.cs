using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using PacketGenerator.Definitions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Configuration;

namespace PacketGenerator
{
    [Generator]
    public class PacketGenerator : ISourceGenerator
    {
        private const string path = @"C:\Users\it_ah\source\repos\UnifyFramework\Definitions\";

        private List<ClassDefinition> _classDefinitions;
        private List<PacketClass> _serverPackets;
        private List<PacketClass> _clientPackets;
        private List<EnumDefinition> _enumDefinitions;
        private Dictionary<string, List<string>> _categories;

        private GeneratorExecutionContext _context;
        public void Execute(GeneratorExecutionContext context)
        {
            _context = context;

            foreach (var enumDef in _enumDefinitions)
            {
                GenerateEnumClass(enumDef);
            }
            foreach (var classDefinition in _classDefinitions)
            {
                GenerateClass(classDefinition);
            }
            var partialClient = new UnifyClientClass(_serverPackets.ToArray());
            GenerateClass(partialClient);

        }

        public void Initialize(GeneratorInitializationContext context)
        {
//#if DEBUG
//            if (!Debugger.IsAttached)
//            {
//                Debugger.Launch();
//            }
//#endif
            Debug.WriteLine("Initalize packet generator");
            _classDefinitions = new List<ClassDefinition>();
            _enumDefinitions = new List<EnumDefinition>();
            _serverPackets = new List<PacketClass>();
            _clientPackets = new List<PacketClass>();
            _categories = new Dictionary<string, List<string>>();
            LoadPacketDefinitions();
            LoadDataDefinitions();
            LoadEnumDefinitions();
        }
        private void LoadPacketDefinitions()
        {
            var lines = File.ReadAllLines($"{path}PacketDefinitions.txt");
            Debug.WriteLine($"{lines.Length} packets found!");

            foreach (var line in lines)
            {
                _classDefinitions.Add(ParsePacketDefinition(line));
            }
        }
        private void LoadDataDefinitions()
        {
            var lines = File.ReadAllLines($"{path}DataDefinitions.txt");
            Debug.WriteLine($"{lines.Length} data packets found!");

            foreach (var line in lines)
            {
                _classDefinitions.Add(ParseDataClass(line));
            }
        }
        private void LoadEnumDefinitions()
        {
            foreach (var category in _categories)
            {
                _enumDefinitions.Add(new EnumDefinition($"{category.Key}ID", category.Value.ToArray()));
            }
            string[] keys = new string[_categories.Keys.Count];
            _categories.Keys.CopyTo(keys, 0);
            _enumDefinitions.Add(new EnumDefinition("PacketCategory", keys));
        }
        private DataClass ParseDataClass(string line)
        {
            var split = line.Split(';');
            var name = split[0];
            var props = split[1].Split(',');

            var properties = ParseProperties(props);

            return new DataClass(name, properties);
        }
        private PacketClass ParsePacketDefinition(string line)
        {
            var split = line.Split(';');
            var name = split[0];
            var type = (PacketType) int.Parse(split[1]);
            var category = split[2];
            var props = split[3].Split(',');

            var properties = ParseProperties(props);

            ParseCategory(category, name);
            var packetClass = new PacketClass(name, type, category, properties);
            if (type == PacketType.ServerPacket) _serverPackets.Add(packetClass);
            else if (type == PacketType.ClientPacket) _clientPackets.Add(packetClass);
            return packetClass;
        }
        private PropertyDefinition[] ParseProperties(string[] props)
        {
            var properties = new PropertyDefinition[props.Length];

            for (int i = 0; i < props.Length; i++)
            {
                string prop = props[i];
                var propDef = new PropertyDefinition();
                propDef.Name = prop.Split(':')[0];
                propDef.Type = prop.Split(':')[1];

                properties[i] = propDef;
            }

            return properties;
        }
        private void ParseCategory(string category,string name)
        {
            if (!_categories.ContainsKey(category))
            {
                _categories[category] = new List<string>(byte.MaxValue);
        
            }
            _categories[category].Add(name);
        }
        private void GenerateClass(ClassDefinition classDefinition)
        {
            Debug.WriteLine($"Generating Packet[{classDefinition.Name}]");
            _context.AddSource($"{classDefinition.Name}.g.cs", SourceText.From(classDefinition.ToString(), Encoding.UTF8));
        }
        private void GenerateEnumClass(EnumDefinition enumDefinition)
        {
            _context.AddSource($"{enumDefinition.Name}.g.cs", SourceText.From(enumDefinition.ToString(), Encoding.UTF8));

        }
    }

}
