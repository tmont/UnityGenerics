# UnityGenerics

Provides strongly typed generics support for `InjectionMember`s for Microsoft's Unity DI container.

Specifically:

- `InjectionProperty<T>`
- `InjectionMethod<T, TReturn>`
- `InjectionConstructor<T>`

## Examples

See unit tests for more examples.

```c#
class MyClass {
  public MyClass(string foo) {
    Foo = foo;
  }

  public MyClass() {
    Foo = "bar";
  }

  public string Foo { get; set; }

  public void MethodOfNoReturn(int arg1, string arg2) {
    Console.WriteLine("MethodOfNoReturn called");
  }
  public int MethodWithReturn() {
    Console.WriteLine("MethodWithReturn called");
  }
}

//container is an `IUnityContainer`

//property injection
container.RegisterType(new InjectionProperty<MyClass>(myClass => myClass.Foo), "custom");
Console.WriteLine(container.Resolve<MyClass>().Foo); //"custom"

//method injection
container.RegisterType(new InjectionMethod<MyClass>(myClass => myClass.MethodOfNoReturn(0, "foo")));
container.Resolve<MyClass>(); //console says: "MethodOfNoReturn called"

container.RegisterType(new InjectionMethod<MyClass>(myClass => myClass.MethodWithReturn()));
container.Resolve<MyClass>(); //console says: "MethodWithReturn called"

//constructor injection
container.RegisterType(new InjectionConstructor<MyClass>(() => new MyClass("foo"));
container.Resolve<MyClass>().Foo; //"foo"
