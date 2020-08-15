using System;
using System.Collections.Generic;
using System.Text;
using Steamworks;

namespace InkySteam
{
    class SteamManager
    {

        public SteamManager()
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
    }
}
