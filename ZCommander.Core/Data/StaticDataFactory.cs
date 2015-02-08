using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCommander.Shared.Common;

namespace ZCommander.Core.Data
{
    public static class StaticDataFactory
    {
        private static Random _randomGenerator = new Random();

        public static string PullStaticValue(string key)
        {
            string[] values = key.Replace("[", "").Replace("]", "").Split(new Char[] { '.' });

            switch (values[0].ToLower())
            {
                case "datetime":
                    switch (values[1].ToLower())
                    {
                        case "now":
                            if (values[2].ToLower() == "none")
                            {
                                return DateTime.Now.ToString();
                            }
                            else
                            {
                                return string.Format(values[2], DateTime.Now);
                            }
                        case "nowutc":
                            string nowUTC = DateTime.UtcNow.ToString();
                            if (values[2].ToLower() == "none")
                            {
                                return DateTime.UtcNow.ToString();
                            }
                            else
                            {
                                return string.Format(values[2], DateTime.UtcNow);
                            }
                    }
                    break;
                case "datetimehelper":
                    if (values.Length > 2)
                    {
                        if (values[2].ToLower() == "none")
                        {
                            return Convert.ToString(DateTimeHelper.RelateiveDateToDateTime(Convert.ToString(values[1])));
                        }
                        else
                        {
                            return string.Format(values[2],DateTimeHelper.RelateiveDateToDateTime(values[1]));
                        }
                    }
                    else
                    {
                        return Convert.ToString(DateTimeHelper.RelateiveDateToDateTime(Convert.ToString(values[1])));
                    }

                case "newguid":
                    return Guid.NewGuid().ToString();
                case "string":
                    if (values.Length > 1)
                    {
                        switch (values[1].ToLower())
                        {
                            case "fill":
                                int fillCount = 0;

                                if (values[3].Contains("-"))
                                {
                                    var fillSplit = values[3].Split(new Char[] { '-' });
                                    fillCount = _randomGenerator.Next(Convert.ToInt32(fillSplit[0]), Convert.ToInt32(fillSplit[1]));
                                }
                                else
                                {
                                    fillCount = Convert.ToInt32(values[3]);
                                }
                                return new String(Convert.ToChar(values[2]), fillCount);
                        }
                    }
                    break;
                case "incrementor":

                    break;
            }
            return null;
        }
    }
}
