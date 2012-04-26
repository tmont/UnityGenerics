using System;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace UnityGenerics.Tests {

	[TestFixture]
	public class GenericInjectionMemberTests {

		private IUnityContainer container;

		[SetUp]
		public void SetUp() {
			container = new UnityContainer();
		}

		#region properties
		[Test]
		public void Should_inject_property() {
			container.RegisterType(new InjectionProperty<Foo>(foo => foo.Bar, "yay"));
			Assert.That(container.Resolve<Foo>().Bar, Is.EqualTo("yay"));
		}

		[Test]
		public void Should_inject_property_with_typed_return_value() {
			container.RegisterType(new InjectionProperty<Foo, string>(foo => foo.Bar, "yay"));
			Assert.That(container.Resolve<Foo>().Bar, Is.EqualTo("yay"));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Should_not_allow_non_member_expression_for_injection_property() {
			container.RegisterType(new InjectionProperty<Foo>(foo => new Foo()));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Should_require_the_use_of_given_parameter_for_injection_property() {
			container.RegisterType(new InjectionProperty<Foo>(foo => new Foo().Bar));
		}
		#endregion

		#region methods
		[Test]
		public void Should_inject_method_with_no_arguments() {
			container.RegisterType(new InjectionMethod<Foo>(foo => foo.SetBar()));
			Assert.That(container.Resolve<Foo>().Bar, Is.EqualTo(Foo.DefaultBar));
		}

		[Test]
		public void Should_inject_method_with_arguments() {
			const string barValue = "my bar";
			container.RegisterType(new InjectionMethod<Foo>(foo => foo.SetBar(barValue)));

			Assert.That(container.Resolve<Foo>().Bar, Is.EqualTo(barValue));
		}

		[Test]
		public void Should_inject_method_with_return_value_with_no_arguments() {
			container.RegisterType(new InjectionMethod<Foo, int>(foo => foo.MethodThatReturns()));
			Assert.That(container.Resolve<Foo>().Value, Is.EqualTo(Foo.DefaultValue));
		}

		[Test]
		public void Should_inject_method_with_return_value_with_arguments() {
			container.RegisterType(new InjectionMethod<Foo>(foo => foo.MethodThatReturns(7)));
			Assert.That(container.Resolve<Foo>().Value, Is.EqualTo(7));
		}

		[Test]
		public void Should_inject_method_with_return_value_without_specifying_return_type() {
			container.RegisterType(new InjectionMethod<Foo>(foo => foo.MethodThatReturns()));
			Assert.That(container.Resolve<Foo>().Value, Is.EqualTo(Foo.DefaultValue));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Should_require_parameter_to_match_object_for_injection_method() {
			container.RegisterType(new InjectionMethod<Foo>(foo => new Foo().SetBar()));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Should_not_allow_static_method_calls_for_injection_method() {
			container.RegisterType(new InjectionMethod<Foo>(foo => Foo.StaticMethod()));
		}
		#endregion

		#region constructors
		[Test]
		public void Should_inject_constructor_with_no_arguments() {
			container.RegisterType(new InjectionConstructor<ConstructorClass>(() => new ConstructorClass()));
			Assert.That(container.Resolve<ConstructorClass>().Bar, Is.EqualTo(ConstructorClass.DefaultBar));
		}

		[Test]
		public void Should_inject_constructor_with_arguments() {
			const string barValue = "my bar";
			container.RegisterType(new InjectionConstructor<ConstructorClass>(() => new ConstructorClass(barValue, new Foo())));

			var value = container.Resolve<ConstructorClass>();
			Assert.That(value.Bar, Is.EqualTo(barValue));
			Assert.That(value.FooProperty, Is.Not.Null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Should_only_allow_new_expressions_for_injection_constructors() {
			var obj = new ConstructorClass();
			container.RegisterType(new InjectionConstructor<ConstructorClass>(() => obj));
		}
		#endregion

		#region mocks
		private class Foo {
			public static string DefaultBar { get { return "bar"; } }
			public static int DefaultValue { get { return 2; } }
			public string Bar { get; set; }
			public int Value { get; private set; }

			public void SetBar() {
				Bar = DefaultBar;
			}

			public void SetBar(string newBar) {
				Bar = newBar;
			}

			public int MethodThatReturns(int value) {
				Value = value;
				return Value;
			}

			public int MethodThatReturns() {
				Value = DefaultValue;
				return Value;
			}

			public static void StaticMethod() { }
		}

		private class ConstructorClass {
			public static string DefaultBar { get { return "bar"; } }
			public string Bar { get; set; }
			public Foo FooProperty { get; set; }

			public ConstructorClass() : this(DefaultBar, null) { }

			public ConstructorClass(string bar, Foo foo) {
				Bar = bar;
				FooProperty = foo;
			}
		}
		#endregion
	}
}
