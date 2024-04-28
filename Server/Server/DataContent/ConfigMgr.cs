using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.Data
{
    [Serializable]
    public class ServerConfig
    {
        public string dataPath;
    }

    internal class ConfigMgr
    {
        public static ServerConfig Config { get; private set; }

        public static void LoadConfig()
        {
            string text = File.ReadAllText("config.json");  // config파일을 Server.exe랑 같은 위치에 둘것임으로 이게 경로 끝
            Config = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerConfig>(text);
        }
    }
}
