using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using ZCommander.Shared.Enums;
using ZCommander.Business.Factories;
using ZCommander.Core.Configuration;
using ZCommander.Core.Models;
using ZCommander.Core.Data;
using ZCommander.Business.Tasks;
using ZCommander.Models;
using ZCommander.Business.TaskVariables;
using ZCommander.Core.Tasks;
using ZCommander.Output;
using ZCommander.Business.Output;
using ZCommander.Exceptions;

namespace ZCommander.Business.Configuration
{
    [Serializable()]
    public class XMLConfig : IConfig
    {

        public string SourceConnectionString { get; set; }

        public bool LoggingEnabled { get; set; }

        public string LoggingTracePath { get; set; }

        public string LoggingConnectionString { get; set; }

        public string DataFactorySymbol { get; set; }

        public string DataFactoryPattern { get; set; }

        public string AssemblySymbol { get; set; }

        public string AssemblyPattern { get; set; }

        public string StaticValueSymbol { get; set; }

        public string StaticValuePattern { get; set; }

        public string TaskVariableSymbol { get; set; }

        public string TaskVariablePattern { get; set; }

        public List<Zombie> Zombies { get; set; }

        public Dictionary<string, IDataFactory> Factories { get; set; }

        public Dictionary<string, ExternalAssembly> Assemblies { get; set; }

        private XmlDocument ConfigXML { get; set; }

        private XMLConfig()
        {
        }

        public XMLConfig(FileInfo ConfigFile)
        {
            this.ConfigXML = new XmlDocument();
            this.ConfigXML.LoadXml(File.ReadAllText(ConfigFile.FullName));
            Init();
        }

        private void Init()
        {

            XmlNode Logging = this.ConfigXML.SelectSingleNode("//Logging");
            XmlNode Connections = this.ConfigXML.SelectSingleNode("//ConnectionStrings");
            XmlNode Patterns = this.ConfigXML.SelectSingleNode("/ZCommander/MacroPatterns");

            SourceConnectionString = Connections.Attributes["Source"].Value;

            LoggingEnabled = Convert.ToBoolean(GetAttributeValue(Logging, "Enabled"));
            LoggingTracePath = GetAttributeValue(Logging, "TracePath");
            LoggingConnectionString = GetAttributeValue(Logging, "ConnectionString");

            DataFactorySymbol = GetAttributeValue(Patterns, "DataFactorySymbol");
            DataFactoryPattern = GetAttributeValue(Patterns, "DataFactoryPattern");

            AssemblySymbol = GetAttributeValue(Patterns, "AssemblySymbol");
            AssemblyPattern = GetAttributeValue(Patterns, "AssemblyPattern");

            StaticValueSymbol = GetAttributeValue(Patterns, "StaticValueSymbol");
            StaticValuePattern = GetAttributeValue(Patterns, "StaticValuePattern");

            TaskVariableSymbol = GetAttributeValue(Patterns, "TaskVariableSymbol");
            TaskVariablePattern = GetAttributeValue(Patterns, "TaskVariablePattern");

            Zombies = GetZombies();
            Factories = GetFactories();
            Assemblies = GetAssemblies();
        }

        private List<Zombie> GetZombies()
        {
            List<Zombie> returnList = new List<Zombie>();
            XmlNodeList Zombies = this.ConfigXML.SelectNodes("//Zombies/Zombie[@Multiplier>0]");
            foreach (XmlNode zombie in Zombies)
            {
                Zombie z = new Zombie();

                z.Name = GetAttributeValue(zombie, "Name");
                z.Frequency = Convert.ToInt32(GetAttributeValue(zombie, "Frequency"));
                z.Multiplier = Convert.ToInt32(GetAttributeValue(zombie, "Multiplier"));

                XmlElement taskVariableList = zombie["TaskVariables"];

                if (taskVariableList != null)
                {
                    foreach (XmlNode variableNode in taskVariableList.ChildNodes)
                    {
                        switch (GetAttributeValue(variableNode, "Type").ToLower())
                        {
                            case "static":
                                StaticTaskVariable staticVar = new StaticTaskVariable();
                                staticVar.Name = GetNodeInnerText(variableNode, "Name");
                                staticVar.Value = GetNodeInnerText(variableNode, "Value");
                                staticVar.Reset = Convert.ToBoolean(GetAttributeValue(variableNode, "Reset"));
                                z.TaskVariables.Add(staticVar.Name, staticVar);
                                break;
                            case "sql":
                                SQLTaskVariable sqlVar = new SQLTaskVariable();
                                sqlVar.Name = GetNodeInnerText(variableNode, "Name");
                                sqlVar.Reset = Convert.ToBoolean(GetAttributeValue(variableNode, "Reset"));
                                sqlVar.ConnectionString = GetAttributeValue(variableNode, "ConnectionString");
                                sqlVar.ConnectionString = (sqlVar.ConnectionString.ToLower() == "{parent}") ? SourceConnectionString : sqlVar.ConnectionString;
                                sqlVar.Statement = GetAttributeValue(variableNode, "Statement");
                                z.TaskVariables.Add(sqlVar.Name, sqlVar);
                                break;
                        }
                    }
                }


                XmlElement taskNodeList = zombie["Tasks"];

                if (taskNodeList != null)
                {
                    foreach (XmlNode taskNode in taskNodeList.ChildNodes)
                    {
                        switch (taskNode.Attributes["Type"].Value.ToLower())
                        {
                            case "sql":
                                SQLTask task = new SQLTask(this);
                                task.Name = taskNode.Attributes["Name"].Value;
                                task.Sequence = Convert.ToInt32(taskNode.Attributes["Sequence"].Value);

                                XmlNodeList sqlStatements = taskNode["SQLStatements"].ChildNodes;

                                foreach (XmlNode statement in sqlStatements)
                                {
                                    SQLStatement newStatement = new SQLStatement();
                                    newStatement.Statement = statement["Text"].InnerText;

                                    if (statement.Attributes["OutputType"] != null)
                                    {
                                        newStatement.OutputType = (SqlStatementOutputType)Enum.Parse(typeof(SqlStatementOutputType), statement.Attributes["OutputType"].Value);
                                    }
                                    else
                                    {
                                        newStatement.OutputType = SqlStatementOutputType.None;
                                    }


                                    newStatement.ConnectionString = statement.Attributes["ConnectionString"].Value == "{PARENT}" ? SourceConnectionString : statement.Attributes["ConnectionString"].Value;


                                    XmlElement traceSQLOutput = statement["OutputList"];
                                    if (traceSQLOutput != null)
                                    {
                                        newStatement.OutputList = GetOutputValues(traceSQLOutput.ChildNodes);
                                    }

                                    task.QueryList.Add(newStatement);
                                }

                                XmlElement traceSQLReplacements = taskNode["Replacements"];
                                if (traceSQLReplacements != null)
                                {
                                    task.Replacements = GetReplacementValues(traceSQLReplacements.ChildNodes);
                                }



                                z.Tasks.Add(task);
                                break;
                            case "sqltrace":
                                SQLTraceTask trace = new SQLTraceTask(this);
                                trace.Name = taskNode.Attributes["Name"].Value;
                                trace.TraceSource = taskNode.Attributes["FilePath"].Value;
                                trace.ConnectionString = taskNode.Attributes["ConnectionString"].Value;

                                XmlNodeList traceTaskReplacements = taskNode["Replacements"].ChildNodes;

                                if (traceTaskReplacements != null)
                                {
                                    trace.Replacements = GetReplacementValues(traceTaskReplacements);
                                }

                                XmlNodeList traceEventList = taskNode["EventsToMonitor"].ChildNodes;
                                foreach (XmlNode sqlEvent in traceEventList)
                                {
                                    SQLEvent newEvent = new SQLEvent();
                                    newEvent.EventName = sqlEvent.Attributes["Text"].Value;

                                    trace.EventList.Add(newEvent);
                                }

                                z.Tasks.Add(trace);
                                break;
                            case "webservice":
                                WebServiceTask restTask = new WebServiceTask();
                                restTask.Endpoint = GetNodeInnerText(taskNode, "URL");
                                restTask.ContentType = GetChildNodeAttribute(taskNode, "URL", "ContentType");
                                restTask.ContentLength = Convert.ToInt32(GetChildNodeAttribute(taskNode, "URL", "ContentLength"));
                                restTask.Method = GetChildNodeAttribute(taskNode, "URL", "Verb");

                                if (taskNode["Authentication"] != null)
                                {
                                    switch (taskNode["Authentication"].Attributes["Type"].Value.ToLower())
                                    {
                                        case "basic":
                                            restTask.UserName = GetChildNodeAttribute(taskNode["Authentication"],"Credentials","UserName");
                                            restTask.Password = GetChildNodeAttribute(taskNode["Authentication"], "Credentials", "Password");
                                            break;
                                        case "none":
                                            break;
                                    }
                                }

                                if (taskNode["Cookies"] != null)
                                {
                                    foreach (XmlNode taskCookie in taskNode["Cookies"].ChildNodes)
                                    {
                                        HttpCookie cookie = new HttpCookie();
                                        cookie.Domain = GetAttributeValue(taskCookie,"Domain");
                                        cookie.Name = GetAttributeValue(taskCookie,"Name");
                                        cookie.Value = GetNodeInnerText(taskCookie,"Value");
                                        
                                        restTask.Cookies.Add(cookie);
                                    }
                                }

                                z.Tasks.Add(restTask);
                                break;
                        }
                    }
                }
                z.Tasks.Sort((task1, task2) => task1.Sequence.CompareTo(task2.Sequence));

                returnList.Add(z);
            }
            return returnList;
        }

        private Dictionary<string, IDataFactory> GetFactories()
        {
            Dictionary<string, IDataFactory> returnList = new Dictionary<string, IDataFactory>();
            XmlNodeList Factories = this.ConfigXML.SelectNodes("//Factories/Factory");

            foreach (XmlNode factory in Factories)
            {
                switch (factory.Attributes["Type"].Value.ToLower())
                {
                    case "sql":
                        SQLDataFactory sqlf = new SQLDataFactory();
                        sqlf.Name = factory.Attributes["Name"].Value;
                        sqlf.RefreshValues = Convert.ToBoolean(factory.Attributes["Refresh"].Value);
                        sqlf.RefreshRate = Convert.ToInt32(factory.Attributes["RefreshRate"].Value);
                        sqlf.ConnectionString = factory["ConnectionString"].InnerText;
                        sqlf.FillQuery = factory["FillQuery"].InnerText;
                        returnList.Add(sqlf.Name, sqlf);
                        break;
                    case "json":
                        JSONDataFactory jsonf = new JSONDataFactory();
                        jsonf.Name = factory.Attributes["Name"].Value;
                        jsonf.RefreshValues = Convert.ToBoolean(factory.Attributes["Refresh"].Value);
                        jsonf.RefreshRate = Convert.ToInt32(factory.Attributes["RefreshRate"].Value);
                        jsonf.LoadPath = factory["LoadPath"].InnerText;
                        returnList.Add(jsonf.Name, jsonf);
                        break;
                }
            }

            return returnList;
        }

        private Dictionary<string, ExternalAssembly> GetAssemblies()
        {
            Dictionary<string, ExternalAssembly> returnList = new Dictionary<string, ExternalAssembly>();
            XmlNodeList Asses = this.ConfigXML.SelectNodes("//Assemblies/Assembly");
            foreach (XmlNode ass in Asses)
            {
                ExternalAssembly tempAss = new ExternalAssembly();
                List<ExternalAssemblyObjectProperty> tempAssProps = new List<ExternalAssemblyObjectProperty>();
                tempAss.Name = ass.Attributes["Name"].Value;
                tempAss.ObjectName = ass.Attributes["Object"].Value;
                tempAss.Path = ass.Attributes["Path"].Value;

                XmlNodeList AssProperties = ass.SelectNodes("//Properties/Property");
                foreach (XmlNode prop in AssProperties)
                {
                    ExternalAssemblyObjectProperty assProp = new ExternalAssemblyObjectProperty();
                    assProp.Name = prop["Name"].InnerText;
                    assProp.Type = prop["Type"].InnerText;
                    assProp.Value = prop["Value"].InnerText;

                    tempAssProps.Add(assProp);
                }
                tempAss.Properties = tempAssProps;

                var asm = Assembly.LoadFile(tempAss.Path);
                tempAss.ObjectType = asm.GetType(tempAss.ObjectName);

                returnList.Add(tempAss.Name, tempAss);
            }
            return returnList;
        }

        private List<ReplacementValue> GetReplacementValues(XmlNodeList nodes)
        {
            List<ReplacementValue> returnList = new List<ReplacementValue>();
            foreach (XmlNode replacement in nodes)
            {
                ReplacementValue newReplacement = new ReplacementValue();
                newReplacement.Pattern = replacement["MatchPattern"].InnerText;
                newReplacement.Value = replacement["ReplaceValue"].InnerText;
                newReplacement.ReplaceMethod = (ReplaceMethod)Enum.Parse(typeof(ReplaceMethod), replacement["ReplaceMethod"].InnerText);
                newReplacement.Global = Convert.ToBoolean(replacement.Attributes["Global"].Value);
                returnList.Add(newReplacement);
            }

            return returnList;
        }

        private List<ITaskOutput> GetOutputValues(XmlNodeList nodes)
        {
            List<ITaskOutput> returnList = new List<ITaskOutput>();

            foreach (XmlNode output in nodes)
            {
                switch (output.Attributes["Type"].Value)
                {
                    case "TaskVariable":
                        TaskVariableOutput outputVar = new TaskVariableOutput();
                        outputVar.Target = output.Attributes["Target"].Value;
                        returnList.Add(outputVar);
                        break;
                }
            }

            return returnList;
        }

        private string GetChildNodeAttribute(XmlNode parentNode, string childNode, string attribute)
        {
            try
            {
                return parentNode[childNode].Attributes[attribute].Value;
            }
            catch (Exception ex)
            {
                throw new InvalidConfigurationException(string.Format("Error pulling child node attribute. Parent Node:{0}. Child Node: {1}. Attribute:{2}. Node Text:{3}. Exception:{4}", parentNode.Name, childNode, attribute, parentNode.OuterXml, ex.Message));
            }
        }

        private string GetAttributeValue(XmlNode node, string attribute)
        {
            try
            {
                return node.Attributes[attribute].Value;
            }
            catch (Exception ex)
            {
                throw new InvalidConfigurationException(string.Format("Error pulling node attribute. Node:{0}. Attribute:{1}. Node Text:{2}. Exception:{3}", node.Name, attribute, node.OuterXml, ex.Message));
            }

        }

        private string GetNodeInnerText(XmlNode parentNode, string nodeText)
        {
            try
            {
                return parentNode[nodeText].InnerText;
            }
            catch (Exception ex)
            {
                throw new InvalidConfigurationException(string.Format("Error pulling node inner text. Parent Node:{0}. Node:{1}. Node Text:{2}. Exception:{3}", parentNode.Name, nodeText, parentNode.OuterXml, ex.Message));
            }
        }

        private string GetNodeInnerText(XmlNode node)
        {
            try
            {
                return node.InnerText;
            }
            catch (Exception ex)
            {
                throw new InvalidConfigurationException(string.Format("Error pulling node inner text. Node:{0}. Node Text:{1}. Exception:{2}", node.Name, node.OuterXml, ex.Message));
            }
        }
    }
}
