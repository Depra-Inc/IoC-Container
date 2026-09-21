using Depra.IoC.Activation;
using Depra.IoC.QoL.Builder;
using Depra.IoC.QoL.Scope;
using Depra.IoC.Scope;

namespace Depra.IoC.QoL.UnitTests;

internal sealed class CombinedScopeTests
{
	[Test]
	public void ResolveTypeFromRootScope_WhenTypeConstructorIsEmpty_ThenResolvedTypeEqualsToRegisteredType()
	{
		// Arrange:
		var activation = new LambdaBasedActivationBuilder();
		var combinedScope = new CombinedScope(new ContainerBuilder(activation)
				.RegisterSingleton<Mocks.TestService>()
				.Build().CreateScope(),
			new ContainerBuilder(activation)
				.RegisterSingleton<Mocks.TestServiceWithEmptyConstructor>()
				.Build().CreateScope());

		// Act:
		var resolved = combinedScope.Resolve<Mocks.TestService>();

		// Assert:
		resolved.Should().NotBeNull();
		resolved.Should().BeOfType<Mocks.TestService>();
	}

	[Test]
	public void ResolveTypeFromRootScope_WhenTypeConstructorIsNotEmpty_ThenResolvedTypeEqualsToRegisteredType()
	{
		// Arrange:
		var activation = new LambdaBasedActivationBuilder();
		var combinedScope = new CombinedScope(new ContainerBuilder(activation)
				.RegisterSingleton<Mocks.TestServiceWithConstructor.Token>()
				.Build().CreateScope(),
			new ContainerBuilder(activation)
				.RegisterSingleton<Mocks.TestService>()
				.RegisterScoped<Mocks.TestServiceWithConstructor>()
				.Build().CreateScope());

		// Act:
		var resolved = combinedScope.Resolve<Mocks.TestServiceWithConstructor>();

		// Assert:
		resolved.Should().NotBeNull();
		resolved.Should().BeOfType<Mocks.TestServiceWithConstructor>();
	}
}