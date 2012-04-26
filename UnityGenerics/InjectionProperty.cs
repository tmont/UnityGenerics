using System;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;

namespace UnityGenerics {
	/// <summary>
	/// Configures injection for a property
	/// </summary>
	/// <typeparam name="T">The type on which to perform property injection</typeparam>
	/// <typeparam name="TReturn">The type that the injected property returns</typeparam>
	public class InjectionProperty<T, TReturn> : InjectionProperty, IGenericInjectionProperty<T> {
		/// <param name="propertyAccessor">Expression representing the property to inject (i.e. foo => foo.Bar)</param>
		public InjectionProperty(Expression<Func<T, TReturn>> propertyAccessor) : base(GetPropertyName(propertyAccessor)) { }

		/// <param name="propertyAccessor">Expression representing the property to inject (i.e. foo => foo.Bar)</param>
		/// <param name="propertyValue">The value to inject</param>
		public InjectionProperty(Expression<Func<T, TReturn>> propertyAccessor, TReturn propertyValue) : base(GetPropertyName(propertyAccessor), propertyValue) { }

		internal static string GetPropertyName(LambdaExpression expression) {
			const string errorMessage = "Expected lambda expression like: foo => foo.Bar, where Bar is the name of the property to be injected";

			var parameterName = expression.Parameters[0].Name;
			var memberExpression = expression.Body as MemberExpression;

			if (memberExpression == null) {
				throw new ArgumentException(errorMessage);
			}

			var leftSide = memberExpression.Expression as ParameterExpression;
			if (leftSide == null || leftSide.Name != parameterName) {
				throw new ArgumentException(errorMessage);
			}

			return memberExpression.Member.Name;
		}
	}

	/// <summary>
	/// Configures injection for a class property
	/// </summary>
	/// <typeparam name="T">The type on which to perform property injection</typeparam>
	public class InjectionProperty<T> : InjectionProperty<T, object> {
		/// <param name="propertyAccessor">Expression representing the property to inject (i.e. foo => foo.Bar)</param>
		public InjectionProperty(Expression<Func<T, object>> propertyAccessor) : base(propertyAccessor) { }

		/// <param name="propertyAccessor">Expression representing the property to inject (i.e. foo => foo.Bar)</param>
		/// <param name="propertyValue">The value to inject</param>
		public InjectionProperty(Expression<Func<T, object>> propertyAccessor, object propertyValue) : base(propertyAccessor, propertyValue) { }
	}
}
