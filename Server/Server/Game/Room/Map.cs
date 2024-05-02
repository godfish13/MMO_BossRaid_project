using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;

namespace Server.Game
{
    public class Map
    {
        public string MapName;
        public float MinX { get; set; }
        public float MaxX { get; set; }
        public float MinY { get; set; }
        public float MaxY { get; set; }

        public void LoadMap(string mapName, string path = "../../../../../Common/MapData")
        {
            // Boundary 관련 파일 
            string text = File.ReadAllText($"{path}/{mapName}.txt");
            StringReader reader = new StringReader(text);

            MapName = mapName;
            MaxX = float.Parse(reader.ReadLine());    // txt의 맨 윗줄 읽어서 int로 파싱하여 MaxX에 저장
            MinX = float.Parse(reader.ReadLine());    // txt의 그 다음 줄 읽어서 int로 파싱하여 MinX에 저장
            MaxY = float.Parse(reader.ReadLine());    // ReadLine이 WriteLine처럼 한줄씩 읽고 넘겨줌
            MinY = float.Parse(reader.ReadLine());          
        }

        public PositionInfo InBoundary(PositionInfo movePositionInfo)     // 간단한 MovePacket 유효성 검증
        {
            PositionInfo ClampPositionInfo = new PositionInfo();
            float ClampX = Math.Clamp(movePositionInfo.PosX, MinX, MaxX);
            float ClampY = Math.Clamp(movePositionInfo.PosY, MinY, MaxY);

            ClampPositionInfo.State = movePositionInfo.State;
            ClampPositionInfo.PosX = ClampX;
            ClampPositionInfo.PosY = ClampY;
            ClampPositionInfo.LocalScaleX = movePositionInfo.LocalScaleX;

            return ClampPositionInfo;
        }
    }
}
