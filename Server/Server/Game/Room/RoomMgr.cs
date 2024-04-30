using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Server.Game
{
    public class RoomMgr
    {
        public static RoomMgr Instance { get; } = new RoomMgr();

        object _lock = new object();
        Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
        int _roomId = 1;    // 일단 한개만 하드코딩으로 생성

        public GameRoom Add()
        {
            GameRoom gameRoom = new GameRoom();
            //gameRoom.Push(gameRoom.Init, mapId);

            lock (_lock)
            {
                gameRoom.RoomId = _roomId;
                _rooms.Add(gameRoom.RoomId, gameRoom);
                _roomId++;
            }
            return gameRoom;
        }

        public bool Remove(int roomId)
        {
            lock (_lock)
            {
                return _rooms.Remove(roomId);
            }
        }

        public GameRoom Find(int roomId)
        {
            lock (_lock)
            {
                GameRoom gameRoom = null;
                if (_rooms.TryGetValue(roomId, out gameRoom))
                    return gameRoom;

                return null;
            }
        }
    }
}
