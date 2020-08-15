using System;
using System.Diagnostics;
using Steamworks;
using System.Threading;

namespace InkySteam
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Starts the app...
            SteamAPI.RestartAppIfNecessary((Steamworks.AppId_t)681220);

            //bool initialized = Steamworks.SteamAPI.Init();

            //Console.WriteLine("Initialized: " + initialized);

            //Console.WriteLine("Steam User ID: " + SteamUser.GetSteamID());

            SteamManager.Instance.createSessionTicket();
            Console.WriteLine("Session Ticket:\n" + SteamManager.Instance.getSessionTicketAsText());
            for(int i = 0; i <100; i++) {
                Thread.Sleep(100);
                SteamAPI.RunCallbacks();
                Console.Write(".");
            }
            SteamAPI.Shutdown();
        }
    }
}
