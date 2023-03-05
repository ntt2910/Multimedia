using System;
using System.Collections.Generic;

namespace UniLinq
{
	public static partial class Enumerable
	{
		public static bool IsIntersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.Intersect(second).Any();
		}

		public static TSource FirstOrValue<TSource>(this IEnumerable<TSource> source, TSource @value)
		{
			Check.Source(source);

			foreach (var element in source)
				return element;

			return @value;
		}

		public static int FindIndex<TSource>(this IEnumerable<TSource> source, Predicate<TSource> predicate)
		{
			Check.SourceAndPredicate(source, predicate);

			int index = 0;
			foreach (TSource v in source) {
				if (predicate(v)) return index;
				index++;
			}
			return -1;
		}

		public static TSource FirstOrValue<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource @value)
		{
			Check.SourceAndPredicate(source, predicate);

			return First(source, predicate, Fallback.Default, @value);
		}

		public static TSource ElementAtOrValue<TSource>(this IEnumerable<TSource> source, int index, TSource @value)
		{
			Check.Source(source);

			if (index < 0)
				return @value;

			var list = source as IList<TSource>;
			if (list != null)
				return index < list.Count ? list[index] : @value;

			return ElementAt(source, index, Fallback.Default, @value);
		}

		public static TSource FindMax<TSource, TKey>(this IEnumerable<TSource> source,
			Func<TSource, TKey> selector)
		{
			return source.FindMax(selector, Comparer<TKey>.Default);
		}

		public static TSource FindMax<TSource, TKey>(this IEnumerable<TSource> source,
			Func<TSource, TKey> selector, IComparer<TKey> comparer)
		{
			Check.SourceAndSelector(source, selector);

			if (comparer == null)
				comparer = Comparer<TKey>.Default;

			using (var sourceIterator = source.GetEnumerator())
			{
				if (!sourceIterator.MoveNext())
				{
					throw new InvalidOperationException("Sequence contains no elements");
				}
				var max = sourceIterator.Current;
				var maxKey = selector(max);
				while (sourceIterator.MoveNext())
				{
					var candidate = sourceIterator.Current;
					var candidateProjected = selector(candidate);
					if (comparer.Compare(candidateProjected, maxKey) > 0)
					{
						max = candidate;
						maxKey = candidateProjected;
					}
				}
				return max;
			}
		}

		public static TSource FindMin<TSource, TKey>(this IEnumerable<TSource> source,
			Func<TSource, TKey> selector)
		{
			return source.FindMin(selector, Comparer<TKey>.Default);
		}

		public static TSource FindMin<TSource, TKey>(this IEnumerable<TSource> source,
			Func<TSource, TKey> selector, IComparer<TKey> comparer)
		{
			Check.SourceAndSelector(source, selector);

			if (comparer == null)
				comparer = Comparer<TKey>.Default;

			using (var sourceIterator = source.GetEnumerator())
			{
				if (!sourceIterator.MoveNext())
				{
					throw new InvalidOperationException("Sequence contains no elements");
				}
				var min = sourceIterator.Current;
				var minKey = selector(min);
				while (sourceIterator.MoveNext())
				{
					var candidate = sourceIterator.Current;
					var candidateProjected = selector(candidate);
					if (comparer.Compare(candidateProjected, minKey) < 0)
					{
						min = candidate;
						minKey = candidateProjected;
					}
				}
				return min;
			}
		}
	}
}
