using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZCommander.Shared.Common;
using ZCommander.Core.Data;

namespace ZCommander.DataFactories.Tests
{
    [TestClass]
    public class StaticDataFactoryTests
    {

        public StaticDataFactoryTests()
        {
        }

        [TestMethod]
        public void TestScrub()
        {
            string key = "^![DATETIME].[NOW].[NONE]!^";
            key = CommonFunctions.ScrubMacroIdentifiers(key);
            Assert.AreEqual(key, "[DATETIME].[NOW].[NONE]");
        }

        [TestMethod]
        public void TestDateTimeNowNoFormat()
        {
            string key = "[DATETIME].[NOW].[NONE]";
            string holder = DateTime.Now.ToString();
            string value = StaticDataFactory.PullStaticValue(key);
            DateTime tempDateTime;

            Assert.IsTrue(DateTime.TryParse(value, out tempDateTime));            
        }

        [TestMethod]
        public void TestDateTimeNowFormat()
        {
            string key = "[DATETIME].[NOW].[{0:MM/dd/yy H:mm}]";
            string formattedDate = string.Format("{0:MM/dd/yy H:mm}",DateTime.Now);
            string value = StaticDataFactory.PullStaticValue(key);

            Assert.AreEqual(value,formattedDate);
        }

        [TestMethod]
        public void TestDateUTCTimeScrub()
        {
            string key = "^![DATETIME].[NOWUTC].[NONE]!^";
            key = CommonFunctions.ScrubMacroIdentifiers(key);
            Assert.AreEqual(key, "[DATETIME].[NOWUTC].[NONE]");
        }

        [TestMethod]
        public void TestDateTimeUTCNowNoFormat()
        {
            string key = "[DATETIME].[NOWUTC].[NONE]";
            string holder = DateTime.UtcNow.ToString();
            string value = StaticDataFactory.PullStaticValue(key);
            DateTime tempDateTime;

            Assert.IsTrue(DateTime.TryParse(value, out tempDateTime));
        }

        [TestMethod]
        public void TestDateTimeUTCNowFormat()
        {
            string key = "[DATETIME].[NOWUTC].[{0:MM/dd/yy H:mm}]";
            string formattedDate = string.Format("{0:MM/dd/yy H:mm}", DateTime.UtcNow);
            string value = StaticDataFactory.PullStaticValue(key);

            Assert.AreEqual(value, formattedDate);
        }

        [TestMethod]
        public void TestGuid()
        {
            string key = "[NEWGUID]";
            key = StaticDataFactory.PullStaticValue(key);
            Guid newGuid;
            Assert.IsTrue(Guid.TryParse(key, out newGuid));
        }

        [TestMethod]
        public void TestStringRandomFill()
        {
            string key = "[STRING].[FILL].[X].[50]";
            key = StaticDataFactory.PullStaticValue(key);
            Assert.IsTrue(key.Length == 50);
        }

        [TestMethod]
        public void TestStringFill()
        {
            string key = "[STRING].[FILL].[X].[1-50]";
            key = StaticDataFactory.PullStaticValue(key);
            Assert.IsTrue(key.Length >= 1 && key.Length <= 50);

        }

        [TestMethod]
        public void TestDateTimeHelperNoFormat()
        {
            string holder;
            string key = "[DATETIMEHELPER].[1 day from now].[NONE]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = DateTime.Today.AddDays(1).ToString();
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[1 day ago].[NONE]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = DateTime.Today.AddDays(-1).ToString();
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[2 days from now].[NONE]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = DateTime.Today.AddDays(2).ToString();
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[2 days ago].[NONE]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = DateTime.Today.AddDays(-2).ToString();
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[1 week from now].[NONE]";
            key= StaticDataFactory.PullStaticValue(key);
            holder = DateTime.Today.AddDays(7).ToString();
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[1 week ago].[NONE]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = DateTime.Today.AddDays(-7).ToString();
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[2 weeks from now].[NONE]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = DateTime.Today.AddDays(14).ToString();
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[2 weeks ago].[NONE]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = DateTime.Today.AddDays(-14).ToString();
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[1 month from now].[NONE]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = DateTime.Today.AddMonths(1).ToString();
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[1 month ago].[NONE]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = DateTime.Today.AddMonths(-1).ToString();
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[2 months from now].[NONE]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = DateTime.Today.AddMonths(2).ToString();
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[2 months ago].[NONE]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = DateTime.Today.AddMonths(-2).ToString();
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[1 year from now].[NONE]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = DateTime.Today.AddYears(1).ToString();
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[1 year ago].[NONE]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = DateTime.Today.AddYears(-1).ToString();
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[2 years from now].[NONE]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = DateTime.Today.AddYears(2).ToString();
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[2 years ago].[NONE]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = DateTime.Today.AddYears(-2).ToString();
            Assert.AreEqual(holder, key);
        }

        [TestMethod]
        public void TestDateTimeHelperFormat()
        {
            string holder;
            string key = "[DATETIMEHELPER].[1 day from now].[{0:MM/dd/yy}]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = string.Format("{0:MM/dd/yy}",DateTime.Today.AddDays(1));
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[1 day ago].[{0:MM/dd/yy}]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = string.Format("{0:MM/dd/yy}", DateTime.Today.AddDays(-1));
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[2 days from now].[{0:MM/dd/yy}]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = string.Format("{0:MM/dd/yy}", DateTime.Today.AddDays(2));
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[2 days ago].[{0:MM/dd/yy}]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = string.Format("{0:MM/dd/yy}", DateTime.Today.AddDays(-2));
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[1 week from now].[{0:MM/dd/yy}]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = string.Format("{0:MM/dd/yy}", DateTime.Today.AddDays(7));
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[1 week ago].[{0:MM/dd/yy}]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = string.Format("{0:MM/dd/yy}", DateTime.Today.AddDays(-7));
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[2 weeks from now].[{0:MM/dd/yy}]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = string.Format("{0:MM/dd/yy}", DateTime.Today.AddDays(14));
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[2 weeks ago].[{0:MM/dd/yy}]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = string.Format("{0:MM/dd/yy}", DateTime.Today.AddDays(-14));
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[1 month from now].[{0:MM/dd/yy}]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = string.Format("{0:MM/dd/yy}", DateTime.Today.AddMonths(1));
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[1 month ago].[{0:MM/dd/yy}]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = string.Format("{0:MM/dd/yy}", DateTime.Today.AddMonths(-1));
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[2 months from now].[{0:MM/dd/yy}]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = string.Format("{0:MM/dd/yy}", DateTime.Today.AddMonths(2));
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[2 months ago].[{0:MM/dd/yy}]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = string.Format("{0:MM/dd/yy}", DateTime.Today.AddMonths(-2));
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[1 year from now].[{0:MM/dd/yy}]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = string.Format("{0:MM/dd/yy}", DateTime.Today.AddYears(1));
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[1 year ago].[{0:MM/dd/yy}]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = string.Format("{0:MM/dd/yy}", DateTime.Today.AddYears(-1));
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[2 years from now].[{0:MM/dd/yy}]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = string.Format("{0:MM/dd/yy}", DateTime.Today.AddYears(2));
            Assert.AreEqual(holder, key);

            key = "[DATETIMEHELPER].[2 years ago].[{0:MM/dd/yy}]";
            key = StaticDataFactory.PullStaticValue(key);
            holder = string.Format("{0:MM/dd/yy}", DateTime.Today.AddYears(-2));
            Assert.AreEqual(holder, key);
        }
    }
}
