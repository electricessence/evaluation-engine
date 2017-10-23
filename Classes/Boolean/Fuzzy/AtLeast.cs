/*!
 * @author electricessence / https://github.com/electricessence/
 * Licensing: MIT https://github.com/electricessence/Open.Evaluation/blob/master/LICENSE.txt
 */

using System;
using System.Collections.Generic;

namespace Open.Evaluation.BooleanOperators
{
	public class AtLeast : CountingBase
	{
		public const string PREFIX = "AtLeast";
		public AtLeast(int count, IEnumerable<IEvaluate<bool>> children = null)
			: base(PREFIX, count, children)
		{
			if (count < 1)
				throw new ArgumentOutOfRangeException("count", count, "Count must be at least 1.");
		}

		public override OperatorBase<IEvaluate<bool>, bool> CreateNewFrom(object param, IEnumerable<IEvaluate<bool>> children)
		{
			return new AtLeast((int)param, children);
		}

		protected override bool EvaluateInternal(object context)
		{
			int count = 0;
			foreach (var result in ChildResults(context))
			{
				if ((bool)result) count++;
				if (count == Count) return true;
			}

			return false;
		}

	}

}