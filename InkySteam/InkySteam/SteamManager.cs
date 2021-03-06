﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Steamworks;

namespace InkySteam
{
    public class SteamManager
    {

        private static SteamManager s_steamMgr;
        private static readonly object s_padlock = new object();

        static ManualResetEvent s_resetEvent = new ManualResetEvent(false);

        readonly CancellationTokenSource cts = new CancellationTokenSource();

        protected Callback<GetAuthSessionTicketResponse_t> m_getAuthSessionTicketResponse;
        
        private byte[] m_ticket;
        private UInt32 m_pcbTicket;
        private HAuthTicket m_HAuthTicket;

        /// <summary>
        /// On first call, should call 
        /// Double lock pattern.
        /// </summary>
        public static SteamManager Instance
        {
            get
            {
                if (s_steamMgr == null)
                { 
                    lock (s_padlock)
                    {
                        if (s_steamMgr == null)
                        {
                            s_steamMgr = new SteamManager();
                        }
                    }
                }

                return s_steamMgr;
            }
        }

        private SteamManager()
        {
            Console.WriteLine("constructor");
            if (!Packsize.Test())
            {
                throw new Exception("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
            }

            if (!DllCheck.Test())
            {
                throw new Exception("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
            }


            //Initialize SteamAPI.
            if(!Steamworks.SteamAPI.Init())
            {
                throw new Exception("Failed to instantiate SteamAPI");
            }

            //Enable Responses
            m_getAuthSessionTicketResponse = Callback<GetAuthSessionTicketResponse_t>.Create(OnGetAuthSessionTicketResponse);
            Task.Run(StartListeningForCallBacks);
        }

        public async Task CreateSessionTicket()
        {

            m_ticket = new byte[1024];
            m_HAuthTicket = SteamUser.GetAuthSessionTicket(m_ticket, 1024, out m_pcbTicket);
            
            Console.WriteLine("SteamUser.GetAuthSessionTicket(Ticket, 1024, out pcbTicket) - " + m_HAuthTicket + " -- " + m_pcbTicket);

            //Wait for callback.
            Task t = WaitForAuthSessionTicketResponse();
            if(await Task.WhenAny(t, Task.Delay(20000)) != t)
            {
                throw new Exception("Timed out!");
            }

            Console.WriteLine("Ended.");
        }

        private async Task WaitForAuthSessionTicketResponse()
        {
            s_resetEvent.WaitOne();
            Console.WriteLine("Done!");
        }

        //Never seems to get called!!! 
        void OnGetAuthSessionTicketResponse(GetAuthSessionTicketResponse_t pCallback)
        {
            Console.WriteLine("[" + GetAuthSessionTicketResponse_t.k_iCallback + " - GetAuthSessionTicketResponse] - " + pCallback.m_hAuthTicket + " -- " + pCallback.m_eResult);
            s_resetEvent.Set(); //Tell that callback is finished.
        }

        public string getSessionTicketAsText()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < (int) m_pcbTicket; i++)
            {
                sb.AppendFormat("{0:x2}", m_ticket[i]);
            }

            return sb.ToString();
        }

        public void Shutdown()
        {
            //What happens if this gets called multiple times?
            Console.WriteLine("Shutdown...");
            StopListeningForCallBacks();
            SteamAPI.Shutdown();
        }

        ~SteamManager()
        {
            Shutdown();
        }

        private void Awake()
        {
            try
            {
                if (SteamAPI.RestartAppIfNecessary((Steamworks.AppId_t)681220))
                {
                    //Application.Quit();
                    return;
                }
            }
            catch (System.DllNotFoundException e)
            {
                //Application.Quit();
                throw new Exception("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.", e);
            }
        }


        private async Task StartListeningForCallBacks()
        {
            while (!cts.IsCancellationRequested)
            {
                Thread.Sleep(100);
                SteamAPI.RunCallbacks();
            }
        }

        private void StopListeningForCallBacks()
        {
            cts.Cancel();
        }
    }
}
