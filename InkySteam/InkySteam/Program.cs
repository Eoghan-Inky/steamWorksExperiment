using System;
using System.Diagnostics;
using Steamworks;
using System.Threading;
using System.Threading.Tasks;

namespace InkySteam
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Starts the app...
            SteamAPI.RestartAppIfNecessary((Steamworks.AppId_t)681220);

            //Console.WriteLine("Steam User ID: " + SteamUser.GetSteamID());

            Task.Run(SteamManager.Instance.CreateSessionTicket).Wait();
            
            Console.WriteLine("Session Ticket:\n" + SteamManager.Instance.getSessionTicketAsText());

            Console.WriteLine();
            Console.WriteLine("Press to end...");
            Console.ReadLine();

            SteamManager.Instance.Shutdown();
        }
    }
}
