using System.Collections;
using System.Collections.Generic;

namespace Util
{
    public class ConcactableArray<T> : IEnumerable<T>, IPseudoArray<T>
    {
        public T this[int index] => inner[index];
        public int Length => inner.Count;
        List<T> inner = new List<T>();
        public IEnumerator<T> GetEnumerator() => inner.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();
        public void Add(T element) => inner.Add(element);
        public void Add(ConcactableArray<T> elements) => inner.AddRange(elements);
        public void Add(params T[] elements) => inner.AddRange(elements);
    }

    public interface IPseudoArray<T>
    {
        T this[int index] { get; }
        int Length { get; }
    }
}