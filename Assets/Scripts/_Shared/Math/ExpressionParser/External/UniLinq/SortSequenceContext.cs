//
// SortSequenceContext.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//
// (C) 2008 Novell, Inc. (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Collections.Generic;

namespace UniLinq
{
	class SortSequenceContext<TElement, TKey> : SortContext<TElement>
	{

		Func<TElement, TKey> selector;
		IComparer<TKey> comparer;
		TKey[] keys;

		public SortSequenceContext (Func<TElement, TKey> selector, IComparer<TKey> comparer, SortDirection direction, SortContext<TElement> child_context)
			: base (direction, child_context)
		{
			this.selector = selector;
			this.comparer = comparer;
		}

		public override void Initialize (TElement[] elements)
		{
			if (this.child_context != null) this.child_context.Initialize (elements);

			this.keys = new TKey [elements.Length];
			for (int i = 0; i < this.keys.Length; i++) this.keys [i] = this.selector (elements [i]);
		}

		public override int Compare (int first_index, int second_index)
		{
			int comparison = this.comparer.Compare (this.keys [first_index], this.keys [second_index]);

			if (comparison == 0) {
				if (this.child_context != null)
					return this.child_context.Compare (first_index, second_index);

				comparison = this.direction == SortDirection.Descending
					? second_index - first_index
					: first_index - second_index;
			}

			return this.direction == SortDirection.Descending ? -comparison : comparison;
		}
	}
}
