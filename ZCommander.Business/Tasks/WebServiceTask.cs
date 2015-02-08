using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Security;
using ZCommander.Core.Logging;
using ZCommander.Core.Managers;
using ZCommander.Core.Models;
using ZCommander.Core.Tasks;
using ZCommander.Models;
using ZCommander.Output;
using ZCommander.Shared.Enums;


namespace ZCommander.Business.Tasks
{
    public class WebServiceTask : ITask
    {
        public string Name { get; set; }
        public int Sequence { get; set; }
        public List<ITaskOutput> OutputList { get; set; }
        public string Endpoint { get; set; }
        public string Method { get; set; }
        public string ContentType { get; set; }
        public int ContentLength { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<HttpCookie> Cookies { get; set; }

        private HttpWebRequest request { get; set; }

        public WebServiceTask()
        {
            UserName = "";
            Password = "";
            Cookies = new List<HttpCookie>();
        }

        public TaskType Type
        {
            get { return TaskType.WebService; }
        }

        public void Prepare(ref Log returnLog)
        {
            returnLog.Type = LogType.Message;
            returnLog.EndTime = DateTime.Now;
            returnLog.Message = "Prepare successful";
        }

        public List<Log> Execute(DataManager dm, Dictionary<string, ITaskVariable> taskVariables, Log returnLog)
        {
            List<Log> returnLogs = new List<Log>();
            var tempEndpoint = Endpoint;
            tempEndpoint = dm.GetValueFromMacro(tempEndpoint);
            returnLog.Statement = tempEndpoint;
            request = CreateWebRequest(tempEndpoint);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
                    returnLog.Successful = false;
                    returnLog.EndTime = DateTime.Now;
                    returnLog.Message = message;
                }

                // grab the response
                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        responseValue = reader.ReadToEnd();
                    }
                }
                returnLog.EndTime = DateTime.Now;
                returnLog.Message = responseValue;
                returnLogs.Add(returnLog);
            }

            return returnLogs;
        }


        private HttpWebRequest CreateWebRequest(string endpoint)
        {
            var request = (HttpWebRequest)WebRequest.Create(endpoint);

            request.Method = Method;
            request.ContentLength = ContentLength;
            request.ContentType = ContentType;

            if (UserName != "" && Password != "")
            {
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(UserName + ":" + Password));
                request.Headers.Add("Authorization", "Basic " + encoded);
            }

            if (Cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                
                foreach (HttpCookie cookie in Cookies)
                {
                    Uri target = new Uri(cookie.Domain);
                    request.CookieContainer.Add(new Cookie(cookie.Name, cookie.Value) { Domain = target.Host });
                }
            }

            return request;
        }
    }
}
