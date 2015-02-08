using System;

namespace ZCommander.Shared.Common
{
    public static class DateTimeHelper
    {
        public static DateTime RelateiveDateToDateTime(string DateTimeDescription)
        {
            var descriptionHolder = DateTimeDescription.ToLower();
            switch (descriptionHolder)
            {
                case "now":
                case "today":
                    return DateTime.Today;
                case "none":
                    return new DateTime(3000, 01, 01);
                case "tomorrow":
                    return DateTime.Today.AddDays(1);
                case "yesterday":
                    return DateTime.Today.AddDays(-1);
            }

            var type = "";
            var daycount = 0;
            var weekcount = 0;
            var monthcount = 0;
            var yearcount = 0;


            if (descriptionHolder.Contains("days"))
            {
                type = "day";

                if (descriptionHolder.Contains("ago"))
                {
                    daycount = -Int16.Parse(descriptionHolder.Replace(" days ago", ""));
                }
                else if (descriptionHolder.Contains("from now"))
                {
                    daycount = Int16.Parse(descriptionHolder.Replace(" days from now", ""));
                }
            }
            else if (descriptionHolder.Contains("day"))
            {
                type = "day";
                if (descriptionHolder.Contains("ago"))
                {
                    daycount = -Int16.Parse(descriptionHolder.Replace(" day ago", ""));
                }
                else if (descriptionHolder.Contains("from now"))
                {
                    daycount = Int16.Parse(descriptionHolder.Replace(" day from now", ""));
                }
            }
            else if (descriptionHolder.Contains("weeks"))
            {
                type = "week";
                if (descriptionHolder.Contains("ago"))
                {
                    weekcount = -Int16.Parse(descriptionHolder.Replace(" weeks ago", ""));
                }
                else if (descriptionHolder.Contains("from now"))
                {
                    weekcount = Int16.Parse(descriptionHolder.Replace(" weeks from now", ""));
                }
            }
            else if (descriptionHolder.Contains("week"))
            {
                type = "week";
                if (descriptionHolder.Contains("ago"))
                {
                    weekcount = -Int16.Parse(descriptionHolder.Replace(" week ago", ""));
                }
                else if (descriptionHolder.Contains("from now"))
                {
                    weekcount = Int16.Parse(descriptionHolder.Replace(" week from now", ""));
                }
            }
            else if (descriptionHolder.Contains("months"))
            {
                type = "month";
                if (descriptionHolder.Contains("ago"))
                {
                    monthcount = -Int16.Parse(descriptionHolder.Replace(" months ago", ""));
                }
                else if (descriptionHolder.Contains("from now"))
                {
                    monthcount = Int16.Parse(descriptionHolder.Replace(" months from now", ""));
                }
            }
            else if (descriptionHolder.Contains("month"))
            {
                type = "month";
                if (descriptionHolder.Contains("ago"))
                {
                    monthcount = -Int16.Parse(descriptionHolder.Replace(" month ago", ""));
                    return DateTime.Today.AddMonths(monthcount);
                }
                else if (descriptionHolder.Contains("from now"))
                {
                    monthcount = Int16.Parse(descriptionHolder.Replace(" month from now", ""));
                }
            }
            else if (descriptionHolder.Contains("years"))
            {
                type = "year";
                if (descriptionHolder.Contains("ago"))
                {
                    yearcount = -Int16.Parse(descriptionHolder.Replace(" years ago", ""));
                }
                else if (descriptionHolder.Contains("from now"))
                {
                    yearcount = Int16.Parse(descriptionHolder.Replace(" years from now", ""));
                }
            }
            else if (descriptionHolder.Contains("year"))
            {
                type = "year";
                if (descriptionHolder.Contains("ago"))
                {
                    yearcount = -Int16.Parse(descriptionHolder.Replace(" year ago", ""));

                }
                else if (descriptionHolder.Contains("from now"))
                {
                    yearcount = Int16.Parse(descriptionHolder.Replace(" year from now", ""));
                }

            }

            switch (type)
            {
                case "day":
                     return DateTime.Today.AddDays(daycount); 
                case "week":
                     return DateTime.Today.AddDays(weekcount * 7); 
                case "month":
                    return DateTime.Today.AddMonths(monthcount); 
                case "year":
                   return DateTime.Today.AddYears(yearcount); 
                default:
                    //if its made it this far, then it is most likely just a datetime
                    return Convert.ToDateTime(descriptionHolder);
            }
        }
    }
}
