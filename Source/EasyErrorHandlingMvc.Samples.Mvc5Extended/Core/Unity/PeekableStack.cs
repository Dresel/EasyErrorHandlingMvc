// See https://github.com/roblevine/UnityLog4NetExtension

namespace EasyErrorHandlingMvc.Samples.Mvc5Extended.Core.Unity
{
	using System.Collections.Generic;

	public class PeekableStack<T>
	{
		private readonly List<T> list;

		public PeekableStack()
		{
			this.list = new List<T>();
		}

		public PeekableStack(IEnumerable<T> initialItems)
		{
			this.list = new List<T>(initialItems);
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public IEnumerable<T> Items
		{
			get
			{
				return this.list.ToArray();
			}
		}

		public T Peek(int depth)
		{
			int index = this.list.Count - 1 - depth;
			return this.list[index];
		}

		public T Pop()
		{
			int index = this.list.Count - 1;
			T ret = this.list[index];
			this.list.RemoveAt(index);
			return ret;
		}

		public void Push(T obj)
		{
			this.list.Add(obj);
		}
	}
}