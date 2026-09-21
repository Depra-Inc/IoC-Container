// SPDX-License-Identifier: Apache-2.0
// © 2022-2026 Depra <n.melnikov@depra.org>

using Depra.IoC.Activation;
using Depra.IoC.QoL.Builder;
using Depra.IoC.QoL.Scope;
using Depra.IoC.Scope;

namespace Depra.IoC.QoL.UnitTests;

internal sealed class CombinedScopeTests
{
	[Test]
	public void ResolveFromRootScope_WhenEmptyCtor_ThenReturnsType()
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
	public void ResolveFromRootScope_WhenCtorHasDependency_ThenReturnsType()
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