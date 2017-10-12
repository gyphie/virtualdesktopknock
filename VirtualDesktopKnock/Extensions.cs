using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualDesktopKnock
{
	public static class Extensions
	{
		public static T PopOrDefault<T>(this Stack<T> stack)
		{
			try
			{
				return stack.Pop();
			}
			catch { }

			return default(T);
		}

		public static T PeekOrDefault<T>(this Stack<T> stack)
		{
			try
			{
				return stack.Peek();
			}
			catch { }

			return default(T);
		}
	}
}
