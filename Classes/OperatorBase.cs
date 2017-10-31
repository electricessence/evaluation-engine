/*!
 * @author electricessence / https://github.com/electricessence/
 * Licensing: MIT https://github.com/electricessence/Open.Evaluation/blob/master/LICENSE.txt
 */

using Open.Hierarchy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Open.Evaluation
{
	public abstract class OperatorBase<TChild, TResult>
		: OperationBase<TResult>, IOperator<TChild, TResult>, IReproducable<IEnumerable<TChild>>

		where TChild : class, IEvaluate
		where TResult : IComparable
	{

		protected OperatorBase(char symbol, string separator, IEnumerable<TChild> children = null, bool reorderChildren = false) : base(symbol, separator)
		{
			ChildrenInternal = children == null ? new List<TChild>() : new List<TChild>(children);
			if (reorderChildren) ChildrenInternal.Sort(Compare);
			Children = ChildrenInternal.AsReadOnly();
		}

		protected readonly List<TChild> ChildrenInternal;

		public IReadOnlyList<TChild> Children
		{
			get;
			private set;
		}

		IReadOnlyList<object> IParent.Children => Children;

		protected override string ToStringInternal(object contents)
		{
			var collection = contents as IEnumerable;
			if (contents == null) return base.ToStringInternal(contents);
			var result = new StringBuilder('(');
			int index = -1;
			foreach (var o in collection)
			{
				if (++index != 0) result.Append(SymbolString);
				result.Append(o);
			}
			result.Append(')');
			return result.ToString();
		}

		protected IEnumerable<object> ChildResults(object context)
		{
			foreach (var child in ChildrenInternal)
				yield return child.Evaluate(context);
		}

		protected IEnumerable<string> ChildRepresentations()
		{
			foreach (var child in ChildrenInternal)
				yield return child.ToStringRepresentation();
		}

		protected override string ToStringRepresentationInternal()
		{
			return ToStringInternal(ChildRepresentations());
		}


		// Need a standardized way to order so that comparisons are easier.
		protected static int Compare(TChild a, TChild b)
		{

			if (a is Constant<TResult> && !(b is Constant<TResult>))
				return 1;

			if (b is Constant<TResult> && !(a is Constant<TResult>))
				return -1;

			var aC = a as Constant<TResult>;
			var bC = b as Constant<TResult>;
			if (aC != null && bC != null)
				return bC.Value.CompareTo(aC.Value); // Descending...

			if (a is Parameter<TResult> && !(b is Parameter<TResult>))
				return 1;

			if (b is Parameter<TResult> && !(a is Parameter<TResult>))
				return -1;

			var aP = a as Parameter<TResult>;
			var bP = b as Parameter<TResult>;
			if (aP != null && bP != null)
				return aP.ID.CompareTo(bP.ID);

			var ats = a.ToStringRepresentation();
			var bts = b.ToStringRepresentation();

			return String.Compare(ats, bts);

		}

		public abstract IEvaluate NewUsing(ICatalog<IEvaluate> catalog, IEnumerable<TChild> param);

		public IEvaluate NewUsing(ICatalog<IEvaluate> catalog, TChild child, params TChild[] rest)
		{
			return NewUsing(catalog, Enumerable.Repeat(child,1).Concat(rest));
		}

		public IEvaluate NewWithIndexRemoved(ICatalog<IEvaluate> catalog, int index)
		{
			return NewUsing(catalog, ChildrenInternal.SkipAt(index));
		}

		public IEvaluate NewWithIndexReplaced(ICatalog<IEvaluate> catalog, int index, TChild repacement)
		{
			return NewUsing(catalog, ChildrenInternal.ReplaceAt(index,repacement));
		}

		public IEvaluate NewWithAppended(ICatalog<IEvaluate> catalog, IEnumerable<TChild> appended)
		{
			return NewUsing(catalog, ChildrenInternal.Concat(appended));
		}

		public IEvaluate NewWithAppended(ICatalog<IEvaluate> catalog, TChild child, params TChild[] rest)
		{
			return NewWithAppended(catalog, Enumerable.Repeat(child, 1).Concat(rest));
		}

	}

	public abstract class OperatorBase<TResult> : OperatorBase<IEvaluate<TResult>,TResult>
		where TResult : IComparable
	{
		protected OperatorBase(char symbol, string separator, IEnumerable<IEvaluate<TResult>> children = null, bool reorderChildren = false) : base(symbol, separator)
		{
		}

	}

}