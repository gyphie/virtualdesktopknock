using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualDesktopKnock
{
	public class Knock
	{
		public long InTime { get; set; } = 0;
		public long OutTime { get; set; } = 0;

		public Knock(long inTime, long outTime)
		{
			this.InTime = inTime;
			this.OutTime = outTime;
		}

		public static Knock EmptyKnock()
		{
			return new Knock(0, 0);
		}
	}
}
