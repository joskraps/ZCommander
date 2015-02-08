using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ZCommander.Core.Configuration;
using ZCommander.Core.Data;
using ZCommander.Core.Models;
using ZCommander.Output;
using ZCommander.Shared.Common;
using ZCommander.Shared.Enums;
using ZCommander.Core.Tasks;

namespace ZCommander.Core.Managers
{
    public class DataManager
    {
        private Random _randomGenerator = new Random();
        private IConfig _config;

        public ConcurrentDictionary<string, ExternalAssembly> Assemblies { get; set; }
        public ConcurrentDictionary<string, IDataFactory> DataFactories { get; set; }

        public DataManager(IConfig config)
        {
            _config = config;
            Assemblies = new ConcurrentDictionary<string, ExternalAssembly>();
            DataFactories = new ConcurrentDictionary<string, IDataFactory>();
        }

        public string GetValueFromMacro(string text)
        {
            return PullMacroValues(text,null);
        }

        public string GetValueFromMacro(string text, ReplacementValue trace)
        {
            string returnValue = text;

            switch (trace.ReplaceMethod)
            {
                case ReplaceMethod.EditObjectFromBinary:
                    returnValue = EditAssemblyObject(trace.Value, returnValue, trace.Pattern, trace.Value);
                    break;
                case ReplaceMethod.Text:
                    var replacmentValue = GetValueFromMacro(trace.Value);
                    if (replacmentValue.Contains("!"))
                    {
                        returnValue = Regex.Replace(returnValue, trace.Pattern, GetValueFromMacro(replacmentValue));
                    }
                    else
                    {
                        returnValue = Regex.Replace(returnValue, trace.Pattern, replacmentValue);
                    }

                    break;
            }

            return returnValue;
        }

        public string GetValueFromMacro(string text, Dictionary<string, ITaskVariable> taskVariables)
        {
            return PullMacroValues(text, taskVariables);
        }

        public string GetValueFromMacro(string text, ReplacementValue strv, Dictionary<string, ITaskVariable> taskVariables)
        {
            string returnValue = text;

            switch (strv.ReplaceMethod)
            {
                case ReplaceMethod.EditObjectFromBinary:
                    returnValue = EditAssemblyObject(strv.Value, returnValue, strv.Pattern, strv.Value);
                    break;
                case ReplaceMethod.Text:
                    var replacmentValue = GetValueFromMacro(strv.Value);
                    if (replacmentValue.Contains("!"))
                    {
                        returnValue = Regex.Replace(returnValue, strv.Pattern, GetValueFromMacro(replacmentValue));
                    }
                    else
                    {
                        returnValue = Regex.Replace(returnValue, strv.Pattern, replacmentValue);
                    }

                    break;
            }

            return returnValue;
        }

        public void SetOutputValues(object output, List<ITaskOutput> OutputList,ref Dictionary<string, ITaskVariable> taskVariables)
        {
            foreach (ITaskOutput o in OutputList)
            {
                if (taskVariables.ContainsKey(o.Target))
                {
                    taskVariables[o.Target].Value = output.ToString();
                }
            }
        }

        private string PullMacroValues(string value, Dictionary<string, ITaskVariable> taskVariables)
        {
            var returnValue = value;

            if (returnValue.Contains(_config.DataFactorySymbol))
            {

                foreach (string result in Regex.Split(returnValue, _config.DataFactoryPattern, RegexOptions.IgnoreCase))
                {
                    if (result.Contains(_config.DataFactorySymbol))
                    {
                        var scrubbedResult = CommonFunctions.ScrubMacroIdentifiers(result);
                        returnValue = returnValue.Replace(result, Convert.ToString(PullDataFactoryValue(scrubbedResult)));
                    }

                }
            }

            if (returnValue.Contains(_config.AssemblySymbol))
            {
                foreach (string result in Regex.Split(returnValue, _config.AssemblyPattern, RegexOptions.IgnoreCase))
                {
                    if (result.Contains(_config.AssemblySymbol))
                    {
                        var scrubbedResult = CommonFunctions.ScrubMacroIdentifiers(result);
                        returnValue = returnValue.Replace(result, (string)CreateAssemblyObject(scrubbedResult));

                    }
                }
            }


            if (returnValue.Contains(_config.StaticValueSymbol))
            {
                foreach (string result in Regex.Split(returnValue, _config.StaticValuePattern, RegexOptions.IgnoreCase))
                {
                    if (result.Contains(_config.StaticValueSymbol))
                    {
                        var scrubbedResult = CommonFunctions.ScrubMacroIdentifiers(result);
                        returnValue = returnValue.Replace(result, (string)PullStaticValue(scrubbedResult));
                    }
                }
            }

            if (taskVariables != null)
            {
                if (returnValue.Contains(_config.TaskVariableSymbol))
                {
                    foreach (string result in Regex.Split(returnValue, _config.TaskVariablePattern, RegexOptions.IgnoreCase))
                    {
                        if (result.Contains(_config.TaskVariableSymbol))
                        {
                            var scrubbedResult = CommonFunctions.ScrubMacroIdentifiers(result);
                            returnValue = returnValue.Replace(result, PullTaskVariableValue(scrubbedResult,taskVariables));
                        }
                    }
                }
            }


            //returnValue = CommonFunctions.ScrubMacroIdentifiers(returnValue);

            return returnValue;
        }

        public List<string> PullMacrosFromString(string value)
        {
            List<string> returnList = new List<string>();


            if (value.Contains(_config.DataFactorySymbol))
            {

                foreach (string result in Regex.Split(value, _config.DataFactoryPattern, RegexOptions.IgnoreCase))
                {
                    if (result.Contains(_config.DataFactorySymbol))
                    {
                        returnList.Add(result);
                    }

                }
            }

            if (value.Contains(_config.AssemblySymbol))
            {
                foreach (string result in Regex.Split(value, _config.AssemblyPattern, RegexOptions.IgnoreCase))
                {
                    if (result.Contains(_config.AssemblySymbol))
                    {
                        returnList.Add(result);
                    }
                }
            }


            if (value.Contains(_config.StaticValueSymbol))
            {
                foreach (string result in Regex.Split(value, _config.StaticValuePattern, RegexOptions.IgnoreCase))
                {
                    if (result.Contains(_config.StaticValueSymbol))
                    {
                        returnList.Add(result);
                    }
                }
            }

            return returnList;
        }

        private string PullStaticValue(string key)
        {
            return StaticDataFactory.PullStaticValue(key);
        }

        private object PullDataFactoryValue(string key)
        {
            string[] values = key.Replace("[", "").Replace("]", "").Split(new Char[] { '.' });
            if (DataFactories.ContainsKey(values[0]))
            {
                return DataFactories[values[0]].GetValue(values[1], values[2], values[3]);
            }
            else
            {
                throw new Exception(string.Format("Data factory request but has not been created: {0}", values[0]));
            }
        }

        private string PullTaskVariableValue(string key, Dictionary<string, ITaskVariable> taskVariables)
        {
            string[] values = key.Replace("[", "").Replace("]", "").Split(new Char[] { '.' });
            if (taskVariables.ContainsKey(values[0]))
            {
                return taskVariables[values[0]].Value;
            }
            else
            {
                return values[1];
            }
        }

        private string CreateAssemblyObject(string key)
        {
            string[] values = key.Replace("[", "").Replace("]", "").Split(new Char[] { '.' });
            object item = new object();

            item = Activator.CreateInstance(Assemblies[values[0]].ObjectType);

            SetObjectProperties(key, ref item);

            string returnString = "";

            returnString = GetReturnValue(item, values[1]);

            return returnString;
        }

        private string EditAssemblyObject(string key, string source, string pattern, string replacementText)
        {
            int startIndex;
            int endIndex;
            string keys = key;
            string returnValue = source;

            if (replacementText.Contains("#!"))
            {
                startIndex = replacementText.IndexOf("#!");
                endIndex = replacementText.IndexOf("!#");
                keys = CommonFunctions.ScrubMacroIdentifiers(replacementText.Substring(startIndex, endIndex - 2));
            }
            else if (replacementText.Contains("{!"))
            {
                startIndex = replacementText.IndexOf("{!");
                endIndex = replacementText.IndexOf("!}");
                keys = CommonFunctions.ScrubMacroIdentifiers(replacementText.Substring(startIndex, endIndex - 2));
            }
            else if (replacementText.Contains("^!"))
            {
                startIndex = replacementText.IndexOf("^!");
                endIndex = replacementText.IndexOf("!^");
                keys = CommonFunctions.ScrubMacroIdentifiers(replacementText.Substring(startIndex, endIndex - 2));
            }

            object EditObject = new Object();

            foreach (Match oText in Regex.Matches(source, pattern))
            {
                string matchText = oText.ToString();

                string Hex = matchText.Replace("0x", "");
                int NumberChars = Hex.Length;
                Byte[] Data = new Byte[NumberChars / 2];

                for (int i = 0; i <= NumberChars - 1; i += 2)
                {
                    var index = i / 2;
                    var holder = Hex.Substring(i, 2);
                    Data[index] = Convert.ToByte(holder, 16);
                }

                LoadObjectFromBinary(Data, ref EditObject, false);
                SetObjectProperties(keys, ref EditObject);

                var temp = replacementText;
                string targetType = keys.Replace("[", "").Replace("]", "").Split(new Char[] { '.' })[1];
                var newValue = GetReturnValue(EditObject, targetType);

                if (replacementText.Contains("#!"))
                {
                    var pattern1 = @"#!|!#";
                    foreach (string result in Regex.Split(temp, pattern1, RegexOptions.IgnoreCase))
                    {
                        if (result.Contains("].["))
                        {
                            temp = CommonFunctions.ScrubMacroIdentifiers(temp.Replace(result, newValue));
                        }
                    }
                }


                returnValue = returnValue.Replace(matchText, temp);
            }
            return returnValue;
        }

        private void LoadObjectFromBinary(Byte[] Data, ref object obj, bool IsCompressed)
        {
            CommonFunctions.BinaryToObject(Data, ref obj, IsCompressed);
        }

        private void SetObjectProperties(string key, ref object target)
        {
            string[] values = key.Replace("[", "").Replace("]", "").Split(new Char[] { '.' });
            foreach (ExternalAssemblyObjectProperty prop in Assemblies[values[0]].Properties)
            {
                switch (prop.Type.ToLower())
                {
                    case "int32":
                        if (prop.Value.Contains("!"))
                        {
                            target.GetType().GetProperty(prop.Name).SetValue(target, Convert.ToInt32(GetValueFromMacro(prop.Value)), null);
                        }
                        else
                        {
                            target.GetType().GetProperty(prop.Name).SetValue(target, Convert.ToInt32(prop.Value), null);
                        }

                        break;
                    case "datetime":
                        if (prop.Value.Contains("!"))
                        {
                            target.GetType().GetProperty(prop.Name).SetValue(target, Convert.ToDateTime(GetValueFromMacro(prop.Value)), null);
                        }
                        else
                        {
                            switch (prop.Value)
                            {
                                case "NOW":
                                    target.GetType().GetProperty(prop.Name).SetValue(target, Convert.ToDateTime(DateTime.Now), null);
                                    break;
                                case "NOWUTC":
                                    target.GetType().GetProperty(prop.Name).SetValue(target, Convert.ToDateTime(DateTime.UtcNow), null);
                                    break;
                                default:
                                    target.GetType().GetProperty(prop.Name).SetValue(target, Convert.ToDateTime(prop.Value), null);
                                    break;
                            }
                        }
                        break;
                    case "bool":
                        if (prop.Value.Contains("!"))
                        {
                            target.GetType().GetProperty(prop.Name).SetValue(target, Convert.ToDateTime(GetValueFromMacro(prop.Value)), null);
                        }
                        else
                        {
                            target.GetType().GetProperty(prop.Name).SetValue(target, Convert.ToBoolean(prop.Value), null);
                        }
                        break;
                    case "string":
                        if (prop.Value.Contains("!"))
                        {
                            target.GetType().GetProperty(prop.Name).SetValue(target, Convert.ToString(GetValueFromMacro(prop.Value)), null);
                        }
                        else
                        {
                            target.GetType().GetProperty(prop.Name).SetValue(target, Convert.ToString(prop.Value), null);
                        }
                        break;
                    case "datetimehelper":
                        target.GetType().GetProperty(prop.Name).SetValue(target, Convert.ToDateTime(DateTimeHelper.RelateiveDateToDateTime(prop.Value)), null);
                        break;


                }

            }
        }

        private string GetReturnValue(object source, string method)
        {
            switch (method.ToLower())
            {
                case "tobinary":
                    string holder = CommonFunctions.ByteArrayToString(CommonFunctions.ObjectToBinary(source, false));
                    return holder;
                case "tojson":
                    return null;
                case "toxml":
                    return null;
                default:
                    return source.ToString();
            }
        }
    }


}
