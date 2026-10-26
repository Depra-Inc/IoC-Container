// SPDX-License-Identifier: Apache-2.0
// © 2022-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.IoC.Activation;
using Depra.IoC.Enums;
using Depra.IoC.Exceptions;
using Depra.IoC.QoL.Builder;
using Depra.IoC.Scope;

namespace Depra.IoC.QoL.UnitTests;

internal sealed class ContainerBuilderTests
{
	private static IEnumerable<LifetimeType> GetLifetime()
	{
		yield return LifetimeType.SCOPED;
		yield return LifetimeType.TRANSIENT;
		yield return LifetimeType.SINGLETON;
	}

	private static IEnumerable<IActivationBuilder> GetActivationBuilders()
	{
		yield return new LambdaBasedActivationBuilder();
		yield return new ReflectionBasedActivationBuilder();
	}

	[Test]
	public void ResolveByImplType_WhenRegisterByImplType_ThenResolvedTypeEqualsToRegisteredType(
		[ValueSource(nameof(GetLifetime))] LifetimeType lifetime,
		[ValueSource(nameof(GetActivationBuilders))]
		IActivationBuilder activationBuilder)
	{
		// Arrange:
		var implementationType = typeof(Mocks.TestService);
		using var container = new ContainerBuilder(activationBuilder)
			.RegisterType(implementationType, implementationType, lifetime)
			.Build();
		var scope = container.CreateScope();

		// Act:
		var service = scope.Resolve<Mocks.TestService>();

		// Assert:
		service.Should().BeOfType(implementationType);
	}

	[Test]
	public void ResolveByInterface_WhenRegisteredByInterface_ThenResolvedTypeEqualsToRegisteredType(
		[ValueSource(nameof(GetLifetime))] LifetimeType lifetime,
		[ValueSource(nameof(GetActivationBuilders))]
		IActivationBuilder activationBuilder)
	{
		// Arrange:
		var interfaceType = typeof(Mocks.ITestService);
		var implementationType = typeof(Mocks.TestService);
		using var container = new ContainerBuilder(activationBuilder)
			.RegisterType(interfaceType, implementationType, lifetime)
			.Build();
		var scope = container.CreateScope();

		// Act:
		var service = scope.Resolve<Mocks.ITestService>();

		// Assert:
		service.Should().BeOfType(implementationType);
	}
#if DEBUG
	[Test]
	public void ResolveByType_WhenTypeNotRegisteredInContainer_ThrowsInvalidOperationException(
		[ValueSource(nameof(GetActivationBuilders))]
		IActivationBuilder activationBuilder)
	{
		// Arrange:
		using var container = new ContainerBuilder(activationBuilder).Build();
		var scope = container.CreateScope();

		// Act:
		var act = () => scope.Resolve<Mocks.TestService>();

		// Assert:
		act.Should().Throw<UnableFindRegistration>();
	}
#endif
	[Test]
	public void ResolveType_WhenTypeConstructorIsEmpty_ThenResolvedTypeEqualsToRegisteredType(
		[ValueSource(nameof(GetActivationBuilders))]
		IActivationBuilder activationBuilder)
	{
		// Arrange:
		using var container = new ContainerBuilder(activationBuilder)
			.RegisterTransient<Mocks.ITestService, Mocks.TestServiceWithEmptyConstructor>()
			.Build();
		var scope = container.CreateScope();

		// Act:
		var service = scope.Resolve<Mocks.ITestService>();

		// Assert:
		service.Should().BeOfType<Mocks.TestServiceWithEmptyConstructor>();
	}

	[Test]
	public void ResolveType_WhenTypeConstructorIsNotEmpty_ThenResolvedTypeEqualsToRegisteredType(
		[ValueSource(nameof(GetActivationBuilders))]
		IActivationBuilder activationBuilder)
	{
		// Arrange:
		using var container = new ContainerBuilder(activationBuilder)
			.RegisterSingleton<Mocks.TestServiceWithConstructor.Token>()
			.RegisterTransient<Mocks.ITestService, Mocks.TestServiceWithConstructor>()
			.Build();
		var scope = container.CreateScope();

		// Act:
		var service = scope.Resolve<Mocks.ITestService>();

		// Assert:
		service.Should().BeOfType<Mocks.TestServiceWithConstructor>();
	}

	[Test]
	public void ResolveType_WhenTypeIsGeneric_ThenResolvedTypeEqualsToRegisteredType(
		[ValueSource(nameof(GetActivationBuilders))]
		IActivationBuilder activationBuilder)
	{
		// Arrange:
		using var container = new ContainerBuilder(activationBuilder)
			.RegisterTransient<Mocks.EmptyGeneric>()
			.RegisterTransient<Mocks.GenericTestService<Mocks.EmptyGeneric>>()
			.Build();
		var scope = container.CreateScope();

		// Act:
		var service = scope.Resolve<Mocks.GenericTestService<Mocks.EmptyGeneric>>();

		// Assert:
		service.Should().BeOfType<Mocks.GenericTestService<Mocks.EmptyGeneric>>();
	}

	[Test]
	public void ResolveType_WhenTypeIsEnumerable_ThenResolvedTypeEqualsToRegisteredType(
		[ValueSource(nameof(GetActivationBuilders))]
		IActivationBuilder activationBuilder)
	{
		// Arrange:
		using var container = new ContainerBuilder(activationBuilder)
			.RegisterTransient<Mocks.EmptyGeneric>()
			.RegisterTransient<Mocks.EnumerableTestService>()
			.Build();
		var scope = container.CreateScope();

		// Act:
		var service = scope.Resolve<Mocks.EnumerableTestService>();

		// Assert:
		service.Should().BeOfType<Mocks.EnumerableTestService>();
	}
}