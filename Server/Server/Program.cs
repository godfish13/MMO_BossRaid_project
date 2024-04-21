using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using ServerCore;
using Server.Game;

namespace Server
{
    internal class Program
    {
        static Listener _listener = new Listener();
        static List<System.Timers.Timer> _timers = new List<System.Timers.Timer>(); // Timer들 관리하기위해 들고있기

        static void TickRoom(GameRoom room, int tick = 100)
        {
            var timer = new System.Timers.Timer();
            timer.Interval = tick;  // 실행 간격
            //timer.Elapsed += (s, e) => { room.Update(); };  // 실행 간격마다 실행시키고 싶은 이벤트 등록
                                                            // s, e는 각각 sender(이벤트 발생시킨 객체, 보통 Timer자신)
                                                            // e는 이벤트와 관련된 정보가 담겨짐, 예를들어 e.SignalTime으로 타이머 만료된 시간 정보 획득 가능 (GPT 답변, word에 옮겨둠 참고)
            timer.AutoReset = true; // 리셋해주기
            timer.Enabled = true;   // 타이머 실행

            _timers.Add(timer);
            //timer.Stop();	// 타이머 정지
        }


        static void Main(string[] args)
        {         
            // DNS (Domain Name System) : 주소 이름으로 IP 찾는 방식
            string host = Dns.GetHostName();    // 내 컴퓨터 주소의 이름을 알아내고 host에 저장
            IPHostEntry ipHost = Dns.GetHostEntry(host);    // 알아낸 주소의 여러 정보가 담김 Dns가 알아서 해줌
            IPAddress ipAddr = ipHost.AddressList[0];   // ip는 여러개를 리스트로 묶어서 보내는 경우(구글이라던가)가 많음 => 일단 우리는 1개니깐 첫번째로만 사용
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // 뽑아낸 ip를 가공한 최종 주소, port == 입장문 번호

            // 손님의 문의가 오면 입장시키기
            _listener.init(endPoint, () => { return SessionManager.instance.Generate(); });
            Console.WriteLine("Listening...");


            while (true)
            {
                Thread.Sleep(10);
            }
        }
    }
}