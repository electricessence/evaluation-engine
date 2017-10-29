/*!
 * @author electricessence / https://github.com/electricessence/
 * Licensing: MIT https://github.com/electricessence/Open.Evaluation/blob/master/LICENSE.txt
 */

using Open.Hierarchy;
using System;

namespace Open.Evaluation
{
	public interface IOperator<out TChild, out TResult>
		: IFunction<TResult>, IParent<TChild>, IReproducable
		where TChild : class, IEvaluate
		where TResult : IComparable
	{

	}
}