using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FileFinder
{
    public class CommonQueue<T>
    {
        private Queue<T> _commonQueue;
        private object _locker;

        public CommonQueue()
        {
            _commonQueue = new Queue<T>();
            _locker = new object();
            Count = 0;
        }
        public int Count { get; set; }
        public void Enqueue(T item)
        {
            lock (_locker)
            {
                _commonQueue.Enqueue(item);
                Count++;
                Monitor.Pulse(_locker);
            }
        }
        public T Dequeue()
        {
            lock (_locker)
            {
                if (_commonQueue.Count == 0)
                {
                    Monitor.Wait(_locker);
                }
                Count--;
                return _commonQueue.Dequeue();
            }
        }
    }
}
