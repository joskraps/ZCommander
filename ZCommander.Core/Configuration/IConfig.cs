using System.Collections.Generic;
using ZCommander.Core.Data;
using ZCommander.Core.Models;


namespace ZCommander.Core.Configuration
{
    public interface IConfig
    {

        string SourceConnectionString {get;set;}
        bool LoggingEnabled { get; set; }
        string LoggingTracePath { get; set; }
        string LoggingConnectionString { get; set; }
        string DataFactorySymbol { get; set; }
        string DataFactoryPattern { get; set; }
        string AssemblySymbol { get; set; }
        string AssemblyPattern { get; set; }
        string StaticValueSymbol { get; set; }
        string StaticValuePattern { get; set; }
        string TaskVariableSymbol { get; set; }
        string TaskVariablePattern { get; set; }
        List<Zombie> Zombies { get; set; }
        Dictionary<string, IDataFactory> Factories { get; set; }
        Dictionary<string, ExternalAssembly> Assemblies { get; set; }
    }
}
