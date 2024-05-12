using System.Net;
using ServerCore;
using Server.Game;
using Server.Data;

namespace Server
{
    internal class Program
    {
        static Listener _listener = new Listener();
        static List<System.Timers.Timer> _timers = new List<System.Timers.Timer>(); // Timer들 관리하기위해 들고있기

        static void TickRoom(GameRoom room, int tick = 100) // ms 단위, 100 == 0.1초
        {
            var timer = new System.Timers.Timer();
            timer.Interval = tick;  // 실행 간격
            timer.Elapsed += (s, e) => { room.Update(); };  // 실행 간격마다 실행시키고 싶은 이벤트 등록
                                                            // s, e는 각각 sender(이벤트 발생시킨 객체, 보통 Timer자신) / 여기서는 사용 안함
                                                            // e는 이벤트와 관련된 정보가 담겨짐, 예를들어 e.SignalTime으로 타이머 만료된 시간 정보 획득 가능 (GPT 답변, word에 옮겨둠 참고)
            timer.AutoReset = true; // 리셋해주기
            timer.Enabled = true;   // 타이머 실행

            _timers.Add(timer);
            //timer.Stop();	// 타이머 정지
        }

        static void Main(string[] args)
        {
            // Load Json Data
            ConfigMgr.LoadConfig();
            DataMgr.LoadData();

            // GameRoom 생성 및 update
            GameRoom currentRoom = RoomMgr.Instance.Add("HighMountain");
            TickRoom(currentRoom, 10);  // 10ms 딜레이 주면서 currentRoom.Update 실행
            Console.WriteLine($"Current Room : {RoomMgr.Instance.Find(1).RoomId}");

            // DNS (Domain Name System) : 주소 이름으로 IP 찾는 방식
            string host = Dns.GetHostName();    // 내 컴퓨터 주소의 이름을 알아내고 host에 저장
            IPHostEntry ipHost = Dns.GetHostEntry(host);    // 알아낸 주소의 여러 정보가 담김 Dns가 알아서 해줌
            IPAddress ipAddr = ipHost.AddressList[0];   // ip는 여러개를 리스트로 묶어서 보내는 경우(구글이라던가)가 많음 => 일단 우리는 1개니깐 첫번째로만 사용           
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7185); // 뽑아낸 ip를 가공한 최종 주소, port == 입장문 번호

            ///////////////////////////////////////////
            ///외부 IP ServerConnect용 IPAddress 영역///
            ///////////////////////////////////////////

            // Client 문의 오면 입장
            _listener.init(endPoint, () => { return SessionManager.instance.Generate(); });
            Console.WriteLine("Listening...");

            // 프로그램 안꺼지게 유지
            while (true)
            {
                Thread.Sleep(10);
            }
        }
    }
}