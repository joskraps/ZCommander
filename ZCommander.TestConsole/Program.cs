using System;
using System.IO;
using ZCommander.Business.Configuration;
using ZCommander.Business.Servers;

namespace ZCommander.TestConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Modified to be a Relative Path.
            XMLConfig config = new XMLConfig(new FileInfo(@"Configs\SimpleSQL.xml"));

            ZombieServer TestServer = new ZombieServer();
            LoggingServer LogServer = new LoggingServer(config);
            TestServer.Subscribe(LogServer);
            TestServer.Start(config);
            Console.WriteLine("Press \'q\' to quit the sample.");
            while (Console.Read() != 'q') ;
            //testing sandbox branch
        }
    }
}
