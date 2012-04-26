using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;

namespace UnityGenerics {
	/// <summary>
	/// Configures injection for a method
	/// </summary>
	/// <typeparam name="T">The type to perform injection on</typeparam>
	/// <typeparam name="TReturn">The return type of the method</typeparam>
	public class InjectionMethod<T, TReturn> : InjectionMethod<T> {
		/// <param name="methodExpression">Expression identifying the method to perform injection for e.g. <c>foo => foo.Bar("arg", 3)</c></param>
		public InjectionMethod(Expression<Func<T, TReturn>> methodExpression) : base(ExtractMethodData(methodExpression)) { }
	}

	/// <summary>
	/// Configures injection for a method
	/// </summary>
	/// <typeparam name="T">The type to perform injection on</typeparam>
	public class InjectionMethod<T> : InjectionMethod, IGenericInjectionProperty<T> {
		private const string ErrorMessage = "Expected lambda expression like: foo => foo.Bar(param1, param2), where Bar is the name of the method to perform injection on";

		/// <param name="methodExpression">Expression identifying the method to perform injection for e.g. <c>foo => foo.Bar()</c></param>
		public InjectionMethod(Expression<Action<T>> methodExpression) : this(ExtractMethodData(methodExpression)) { }

		protected InjectionMethod(MethodData methodData) : base(methodData.Name, methodData.Parameters) { }

		protected static MethodData ExtractMethodData(LambdaExpression expression) {
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression == null) {
				throw new ArgumentException(ErrorMessage);
			}

			var parameter = expression.Parameters[0];
			var leftSide = methodCallExpression.Object as ParameterExpression;
			if (leftSide == null || leftSide.Name != parameter.Name) {
				throw new ArgumentException(ErrorMessage);
			}

			return new MethodData {
				Name = methodCallExpression.Method.Name,
				Parameters = methodCallExpression
					.Arguments
					.Select(expr => Expression.Lambda(expr).Compile().DynamicInvoke())
					.ToArray()
			};
		}

		protected struct MethodData {
			public string Name { get; set; }
			public object[] Parameters { get; set; }
		}
	}
}