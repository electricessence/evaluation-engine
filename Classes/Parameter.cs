/*!
 * @author electricessence / https://github.com/electricessence/
 * Licensing: MIT https://github.com/electricessence/Open.Evaluation/blob/master/LICENSE.txt
 */

using System;
using System.Collections.Generic;

namespace Open.Evaluation
{
    public class Parameter<TResult>
        : EvaluationBase<TResult>, IParameter<TResult>
        where TResult : IComparable
    {

        internal Parameter(ushort id, Func<object, ushort, TResult> evaluator) : base()
        {
			_evaluator = evaluator ?? throw new ArgumentNullException("evaluator");
			ID = id;
        }

        Func<object, ushort, TResult> _evaluator;

        public ushort ID
        {
            get;
            private set;
        }

		public static string ToStringRepresentation(ushort id)
		{
			return "{" + id + "}";
		}

        protected override string ToStringRepresentationInternal()
        {
            return ToStringRepresentation(ID);
        }

        protected override TResult EvaluateInternal(object context)
        {
            return _evaluator(context is ParameterContext p ? p.Context : context, ID);
        }

        protected override string ToStringInternal(object context)
        {
            return string.Empty + Evaluate(context);
        }
    }

    public class Parameter : Parameter<double>
    {
		internal Parameter(ushort id) : base(id, GetParamValueFrom)
        {
        }

        static double GetParamValueFrom(object source, ushort id)
        {
			if (source is IReadOnlyList<double> list) return list[id];
			if (source is IDictionary<ushort, double> d) return d[id];
			throw new ArgumentException("Unknown context type.");
        }
    }

	public static class ParameterExtensions
	{
		public static TParameter GetParameter<TParameter,TResult>(
			this ICatalog<IEvaluate<TResult>> catalog, ushort id, Func<ushort, TParameter> factory)
			where TParameter : IParameter<TResult>
		{
			return catalog.Register(Parameter.ToStringRepresentation(id), k => factory(id));
		}

		public static Parameter GetParameter(
			this ICatalog<IEvaluate<double>> catalog, ushort id, Func<ushort, Parameter> factory)
		{
			return GetParameter<Parameter, double>(catalog, id, factory);
		}

		public static Parameter GetParameter(
			this ICatalog<IEvaluate<double>> catalog, ushort id)
		{
			return GetParameter(catalog, id, i => new Parameter(id));
		}
	}

}