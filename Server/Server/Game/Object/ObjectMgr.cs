using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class ObjectMgr
    {
        public static ObjectMgr Instance { get; } = new ObjectMgr();

        object _lock = new object();

        // 플레이어 아이디 하나 딸랑 담기에 int는 너무 큼 그러므로 비트 분할해서 사용
        // int = 4 byte = 32 bit 이므로 나눠서 사용
        // [unused 1-bit] [Type 7-bit] [Id 24-bit]  //포트폴리오 제작 과정 수업 기록 2 - server연동 8)화살 파트 참고
        int _counter = 0;

        public T Add<T>() where T : GameObject, new()     // 최상위 GameObject 일반화 ver, 오브젝트 생성 및 Id 부여
        {
            T gameObject = new T();
            lock (_lock) 
            {
                gameObject.GameObjectId = GenerateId(gameObject.ObjectType);    // Object Id 생성
            }
            return gameObject;
        }

        int GenerateId(GameObjectType type)
        {
            lock (_lock)
            {
                return (int)type << 24 | (_counter++);  // 비트플래그 형태
            }   // 0 type 0000 0000 0000 000n 의 형태로 return // n == _counter
        }       // ex) type = Player, _counter = 5 ==> 0000 0001 0000 0000 0000 0000 0000 0101
                                                    // /--type-- ------------ID---------------
        public static GameObjectType GetObjectTypebyId(int id)
        {
            int type = (id >> 24) & 0x7F;   // 0x7F == 127 == 0000 0000 0000 0000 0000 0000 0111 1111
                                            // 24칸 뒤로 밀려서 저장되어있는 type 10진수형태로 보게 제일 뒤로 끌고옴
                                            // 이후 0x7F 곱해서 앞쪽 싹 청소
            return (GameObjectType)type;
        }

        public static int GetDecimalId(int id)
        {
            int DecimalId = (id) & 0xFFFFFF;    // 0000 0000 1111 1111 1111 1111 1111 1111
            return DecimalId;                   // 제일 앞 nonuse와 Type 부분 싹 날려서 10진수 숫자 get
        }
    }
}
