using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;

namespace UnityGenerics {
	/// <summary>
	/// Configures injection for a constructor
	/// </summary>
	/// <typeparam name="T">The type on which to perform injection</typeparam>
	public class InjectionConstructor<T> : InjectionConstructor, IGenericInjectionProperty<T> {
		/// <param name="constructorCall">Expression identifying the constructor to perform injection for e.g. <c>() => new Foo(param1, param2)</c></param>
		public InjectionConstructor(Expression<Func<T>> constructorCall) : base(GetConstructorArguments(constructorCall)) { }

		private static object[] GetConstructorArguments(LambdaExpression expression) {
			var newExpression = expression.Body as NewExpression;
			if (newExpression == null) {
				throw new ArgumentException("Expected lambda expression like: () => new Foo(param1, param2)");
			}

			return newExpression
				.Arguments
				.Select(argumentExpression => Expression.Lambda(argumentExpression).Compile().DynamicInvoke())
				.ToArray();
		}
	}
}