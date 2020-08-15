using System;
using System.Diagnostics;
using Steamworks;


namespace InkySteam
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Starts the app...
            SteamAPI.RestartAppIfNecessary((Steamworks.AppId_t)681220);

            bool initialized = Steamworks.SteamAPI.Init();

            Console.WriteLine("Initialized: " + initialized);

            //Console.WriteLine("Steam User ID: " + SteamUser.GetSteamID());

            SteamManager sm = new SteamManager();

            SteamAPI.Shutdown();
        }
    }
}
