using System.Linq;
using Microsoft.Practices.Unity;

namespace UnityGenerics {
	public static class ContainerExtensions {
		/// <summary>
		/// Registers a type with specified injection members with the container
		/// </summary>
		/// <typeparam name="T">The type to register</typeparam>
		/// <param name="properties">Properties to perform injection on</param>
		public static IUnityContainer RegisterType<T>(this IUnityContainer container, params IGenericInjectionProperty<T>[] properties) {
			return container.RegisterType(typeof(T), properties.Cast<InjectionMember>().ToArray());
		}
	}
}