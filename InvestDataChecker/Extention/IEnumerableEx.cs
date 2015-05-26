using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
    public static partial class IEnumerableEx
    {
        //Distinctにラムダ式を直接渡せるように拡張
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
        {
            return source.Distinct(new CompareSelector<T, TKey>(selector));
        }

        internal class CompareSelector<T, TKey> : IEqualityComparer<T>
        {
            private Func<T, TKey> selector;

            public CompareSelector(Func<T, TKey> selector)
            {
                this.selector = selector;
            }

            public bool Equals(T x, T y)
            {
                return selector(x).Equals(selector(y));
            }

            public int GetHashCode(T obj)
            {
                return selector(obj).GetHashCode();
            }
        }

        //Listライクに直接ForEachを呼べるように拡張
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);
            }
        }
    }
}
