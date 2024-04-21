using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class JobSerializer
    {
        JobTimer _jobTimer = new JobTimer();   // 실행을 예약해두고 나중에 실행시키고 싶은 job
        Queue<IJob> _jobQueue = new Queue<IJob>();  // 바로 실행시켜줄 job
        object _lock = new object();

        // PushDealy도 push와 마찬가지로 헬퍼메소드 추가
        public void PushDelay(int tickDelay, Action action) { PushDelay(tickDelay, new Job(action)); }
        public void PushDelay<T1>(int tickDelay, Action<T1> action, T1 t1) { PushDelay(tickDelay, new Job<T1>(action, t1)); }
        public void PushDelay<T1, T2>(int tickDelay, Action<T1, T2> action, T1 t1, T2 t2) { PushDelay(tickDelay, new Job<T1, T2>(action, t1, t2)); }
        public void PushDelay<T1, T2, T3>(int tickDelay, Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) { PushDelay(tickDelay, new Job<T1, T2, T3>(action, t1, t2, t3)); }

        public void PushDelay(int tickDelay, IJob job)  // 바로실행시키는게아닌 tickDelay만큼 대기하고 실행시키고 싶은 job push
        {
            _jobTimer.Push(job, tickDelay);
        }

        // 사용하기 편하게 다른곳에서 일일히 IJob으로 만드는 대신 action으로 바로 넣어줄수 있게 해줌
        public void Push(Action action) { Push(new Job(action)); }
        public void Push<T1>(Action<T1> action, T1 t1) { Push(new Job<T1>(action, t1)); }
        public void Push<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2) { Push(new Job<T1, T2>(action, t1, t2)); }
        public void Push<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) { Push(new Job<T1, T2, T3>(action, t1, t2, t3)); }

        public void Push(IJob job)
        {
            lock (_lock)
            {
                _jobQueue.Enqueue(job);
            }
        }

        public void Flush()
        {
            _jobTimer.Flush();
            
            while (true)
            {
                IJob job = Pop();
                if (job == null)
                    return;

                job.Execute();
            }
        }

        IJob Pop()
        {
            lock (_lock)
            {
                if (_jobQueue.Count == 0)
                {
                    return null;
                }
                return _jobQueue.Dequeue();
            }
        }
    }
}
