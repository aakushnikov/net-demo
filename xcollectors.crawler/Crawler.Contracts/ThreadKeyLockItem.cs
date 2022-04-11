using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler.Contracts
{
	internal sealed class ThreadKeyLockItem
	{
		private readonly string _key;
		private readonly int _maxCount;
		private readonly object _sync = new object();

		public ThreadKeyLockItem(string key, int maxCount)
		{
			_key = key;
			_maxCount = maxCount;
			_count = 1;
		}

		private int _count;

		internal int GetCount()
		{
			return _count;
		}
		internal int Increment()
		{
			if (_maxCount < _count)
			{
				throw new InvalidOperationException($"Key is already used:{_key}; count;{_count}");
			}
			lock (_sync)
			{
				return Interlocked.Increment(ref _count);
			}
		}

		internal int Decrement()
		{
			lock (_sync)
			{
				return Interlocked.Decrement(ref _count);
			}

		}
	}

	public sealed class ThreadKeyLockInstance : IDisposable
	{
		private readonly string _name;

		private ThreadKeyLockInstance(string name, int maxCount)
		{
			_name = name;
			ThreadKeyLock.Instance.Register(_name, maxCount);
		}

		public static ThreadKeyLockInstance Create(string name, int maxCount = 12)
		{
			
			return new ThreadKeyLockInstance(name, maxCount);
		}

		public void Dispose()
		{
			ThreadKeyLock.Instance.Release(_name);
		}
	}

	public sealed class ThreadKeyLock
	{
		private ThreadKeyLock()
		{
		}
		private static readonly object _sync = new object();

		private static ThreadKeyLock _instance;
		public static ThreadKeyLock Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_sync)
					{
						if (_instance == null)
						{
							_instance = new ThreadKeyLock();
						}
					}
				}
				return _instance;
			}
		}



		public void Register(string key, int maxCount = 12)
		{
			ThreadKeyLockItem threadKeyLockItem;
			if (!_process.ContainsKey(key))
			{
				lock (_syncInstance)
				{
					if (!_process.ContainsKey(key))
					{
						threadKeyLockItem = new ThreadKeyLockItem(key, maxCount);
						_process.Add(key, threadKeyLockItem);
					}
					else
					{
						threadKeyLockItem = _process[key];
					}
				}
			}
			else
			{
				threadKeyLockItem = _process[key];
			}
			threadKeyLockItem.Increment();
		}

		
		public void Release(string key)
		{
			if (_process.ContainsKey(key))
			{
				lock (_syncInstance)
				{
					if (_process.ContainsKey(key))
					{
						Console.WriteLine($"Release:{key}");
						var threadKeyLockItem = _process[key];
						threadKeyLockItem.Decrement();
					}
				}
			}
		}



		private readonly object _syncInstance = new object();

		private readonly Dictionary<string, ThreadKeyLockItem> _process = new Dictionary<string, ThreadKeyLockItem>();

	}
}
