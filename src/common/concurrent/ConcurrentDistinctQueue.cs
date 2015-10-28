using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace rift.common.concurrent
{
    public class ConcurrentDistinctQueue<T> : IProducerConsumerCollection<T>
    {
        private readonly ConcurrentHashSet<T> _elements = new ConcurrentHashSet<T>();

        private IProducerConsumerCollection<T> _queue = new ConcurrentQueue<T>();

        public void CopyTo(T[] array, int index)
        {
            _queue.CopyTo(array, index);
        }

        public T[] ToArray()
        {
            return _queue.ToArray();
        }

        public bool TryAdd(T item)
        {
            if (_elements.Add(item))
            {
                _queue.TryAdd(item);

                return true;
            }
            return false;
        }

        public bool TryTake(out T item)
        {
            if (_queue.TryTake(out item))
            {
                _elements.Remove(item);

                return true;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            _queue.CopyTo(array, index);
        }

        public int Count
        {
            get { return _queue.Count; }
        }

        public bool IsSynchronized
        {
            get { return _queue.IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return _queue.SyncRoot; }
        }
    }
}
