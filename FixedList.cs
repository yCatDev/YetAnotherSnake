using System;

namespace YetAnotherSnake
{
    public class FixedList<T>
    {
        private T[] _array;
        private int _counter;

        public T this[int index]
        {
            get
            {
                 if (index>=_counter)
                     throw new IndexOutOfRangeException();
                 return _array[index];
            }
        }
        
        public FixedList(int maxSize)
        {
            _array = new T[maxSize];
            _counter = 0;
        }

        public void Add(T item)
        {
            if (_counter>=_array.Length)
                throw new Exception("Fixed list is full");
            _array[_counter] = item;
            _counter++;
        }

        public int Length => _counter;

    }
}