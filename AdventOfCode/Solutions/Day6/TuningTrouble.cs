using System.Collections;
using AdventOfCode.Cli.Infrastructure;

namespace AdventOfCode.Cli.Solutions;

internal class TuningTrouble : ISolution
{
    private class Buffer<T> : IEnumerable<T>
    {
        private readonly int _capacity;
        private readonly Queue<T> _queue;

        public int Count => _queue.Count;
        public bool IsFull => _queue.Count == _capacity;

        public Buffer(int capacity)
        {
            _capacity = capacity;
            _queue = new Queue<T>(capacity);
        }

        public void Enqueue(T element)
        {
            if (_queue.Count + 1 > _capacity)
            {
                _queue.Dequeue();
            }

            _queue.Enqueue(element);
        }

        public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _queue.GetEnumerator();
    }

    public void Run(EntryPoint entryPoint)
    {
        using var file = File.OpenRead(entryPoint.InputPath);
        using var reader = new StreamReader(file);
        var line = reader.ReadLine() ?? string.Empty;

        Buffer<char> smallBuffer = new(4);
        Buffer<char> largeBuffer = new(14);
        HashSet<char> hashSet = new(14);

        int? initialSequenceIndex = null;
        int? messageSequenceIndex = null;

        for (int i = 0; i < line.Length; i++)
        {
            smallBuffer.Enqueue(line[i]);
            largeBuffer.Enqueue(line[i]);

            initialSequenceIndex ??= IsSequenceOfUniqueElements(smallBuffer) ? i + 1 : null;
            messageSequenceIndex ??= IsSequenceOfUniqueElements(largeBuffer) ? i + 1 : null;

            if (messageSequenceIndex.HasValue && initialSequenceIndex.HasValue) break;

            hashSet.Clear();
        }

        Console.WriteLine($"The elvish protocol starting sequence comes after {initialSequenceIndex} characters.");
        Console.WriteLine($"The elvish protocol message sequence comes after {messageSequenceIndex} characters.");

        bool IsSequenceOfUniqueElements(Buffer<char> buffer)
        {
            if (buffer.IsFull)
            {
                hashSet.UnionWith(buffer);
            }

            return hashSet.Count == buffer.Count;
        }
    }
}