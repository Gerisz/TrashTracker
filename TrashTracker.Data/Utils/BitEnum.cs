using System.Collections;
using System.Numerics;

namespace CleanTiszaMap.Data.Utils
{
    /// <summary>
    /// This is a wrapper class that provides an enumerable implementation for binary enums.
    /// </summary>
    public class BitEnum<T>(T Value = default) : IEnumerable<T>, ICollection<T>
        where T : struct, Enum
    {
        public int Count => BitOperations.PopCount(Convert.ToUInt32(Value));
        
        public bool IsReadOnly => false;

        public void Add(T item)
        {
            Value = (T)(object)(Convert.ToInt32(Value) | Convert.ToInt32(item));
        }

        public void Clear()
        {
            Value = default;
        }

        public bool Contains(T item)
            => Value.HasFlag(item);

        public void CopyTo(T[] array, int arrayIndex)
            => this.ToArray().CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var enumValue in Enum.GetValues<T>())
            {
                if (Convert.ToInt32(enumValue) != 0 && Contains(enumValue))
                    yield return enumValue;
            }
        }

        public bool Remove(T item)
        {
            if (!Contains(item))
                return false;

            Value = (T)(object)(Convert.ToInt32(Value) & ~Convert.ToInt32(item));
            return true;
        }

        public override string ToString()
            => Value.ToString();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
