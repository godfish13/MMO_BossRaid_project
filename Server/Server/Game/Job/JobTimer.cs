using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using ServerCore;

namespace Server.Game
{
	struct JobTimerElem : IComparable<JobTimerElem>
	{
		public int execTick; // 실행 시간
		public IJob job;

		public int CompareTo(JobTimerElem other)
		{
			return other.execTick - execTick;
		}
	}

	public class JobTimer
	{
		PriorityQueue<JobTimerElem> _pq = new PriorityQueue<JobTimerElem>();
		object _lock = new object();

		public void Push(IJob job, int tickAfter = 0)
		{
			JobTimerElem jobelement;
			jobelement.execTick = System.Environment.TickCount + tickAfter;
			jobelement.job = job;

			lock (_lock)
			{
				_pq.Push(jobelement);
			}
		}

		public void Flush()
		{
			while (true)
			{
				int now = Environment.TickCount;

				JobTimerElem jobelement;

				lock (_lock)
				{
					if (_pq.Count == 0)
						break;

					jobelement = _pq.Peek();
					if (jobelement.execTick > now)
						break;
					_pq.Pop();
				}

				jobelement.job.Execute();
			}
		}
	}
}
