// SPDX-License-Identifier: Apache-2.0
// © 2022-2025 Depra <n.melnikov@depra.org>

using System.Linq;
using Depra.IoC.Activation;
using Depra.IoC.Description;
using Depra.IoC.Enums;
using Depra.IoC.Exceptions;
using Depra.IoC.Scope;

namespace Depra.IoC.UnitTests;

internal sealed class ContainerTests
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
	public void ResolveByImplType_WhenRegisteredByImplType_ThenResolvedTypeEqualsToRegisteredType(
		[ValueSource(nameof(GetLifetime))] LifetimeType lifetime,
		[ValueSource(nameof(GetActivationBuilders))]
		IActivationBuilder activationBuilder)
	{
		// Arrange:
		var implementationType = typeof(Mocks.TestService);
		var descriptors = new ServiceDescription[]
			{ new TypeServiceDescription(implementationType, implementationType, lifetime) };
		using var container = new Container(activationBuilder, descriptors);
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
		var descriptors = new ServiceDescription[]
			{ new TypeServiceDescription(implementationType, interfaceType, lifetime) };
		using var container = new Container(activationBuilder, descriptors);
		var scope = container.CreateScope();

		// Act:
		var service = scope.Resolve<Mocks.ITestService>();

		// Assert:
		service.Should().BeOfType(implementationType);
	}

	[Test]
	public void Build_WhenTypeDescriptionIsNonLazy_ThenServiceIsCreatedImmediately(
		[ValueSource(nameof(GetActivationBuilders))]
		IActivationBuilder activationBuilder)
	{
		// Arrange:
		Mocks.NonLazyTestService.CreationCount = 0;
		var descriptors = new ServiceDescription[]
		{
			new TypeServiceDescription(typeof(Mocks.NonLazyTestService),
				typeof(Mocks.NonLazyTestService), LifetimeType.SINGLETON, lazy: false)
		};

		// Act:
		using var container = new Container(activationBuilder, descriptors);

		// Assert:
		Mocks.NonLazyTestService.CreationCount.Should().Be(1);
	}
#if DEBUG
	[Test]
	public void ResolveByType_WhenTypeNotRegisteredInContainer_ThenInvalidOperationExceptionIsThrown(
		[ValueSource(nameof(GetActivationBuilders))]
		IActivationBuilder activationBuilder)
	{
		// Arrange:
		var descriptions = Array.Empty<ServiceDescription>();
		using var container = new Container(activationBuilder, descriptions);
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
		const LifetimeType LIFETIME = LifetimeType.TRANSIENT;
		var interfaceType = typeof(Mocks.ITestService);
		var implementationType = typeof(Mocks.TestServiceWithEmptyConstructor);
		var descriptors = new ServiceDescription[]
			{ new TypeServiceDescription(implementationType, interfaceType, LIFETIME) };
		using var container = new Container(activationBuilder, descriptors);
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
		var descriptors = new ServiceDescription[]
		{
			new TypeServiceDescription(lifetime: LifetimeType.SINGLETON,
				type: typeof(Mocks.TestServiceWithConstructor.Token),
				implementationType: typeof(Mocks.TestServiceWithConstructor.Token)),
			new TypeServiceDescription(lifetime: LifetimeType.TRANSIENT,
				type: typeof(Mocks.ITestService),
				implementationType: typeof(Mocks.TestServiceWithConstructor))
		};
		using var container = new Container(activationBuilder, descriptors);
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
		var descriptors = new ServiceDescription[]
		{
			new TypeServiceDescription(lifetime: LifetimeType.TRANSIENT,
				type: typeof(Mocks.EmptyGeneric), implementationType: typeof(Mocks.EmptyGeneric)),
			new TypeServiceDescription(lifetime: LifetimeType.TRANSIENT,
				type: typeof(Mocks.GenericTestService<Mocks.EmptyGeneric>),
				implementationType: typeof(Mocks.GenericTestService<Mocks.EmptyGeneric>))
		};
		using var container = new Container(activationBuilder, descriptors);
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
		var descriptors = new ServiceDescription[]
		{
			new TypeServiceDescription(lifetime: LifetimeType.TRANSIENT,
				type: typeof(Mocks.EmptyGeneric), implementationType: typeof(Mocks.EmptyGeneric)),
			new TypeServiceDescription(lifetime: LifetimeType.TRANSIENT,
				type: typeof(Mocks.EnumerableTestService), implementationType: typeof(Mocks.EnumerableTestService))
		};
		using var container = new Container(activationBuilder, descriptors);
		var scope = container.CreateScope();

		// Act:
		var service = scope.Resolve<Mocks.EnumerableTestService>();

		// Assert:
		service.Should().BeOfType<Mocks.EnumerableTestService>();
	}

	[Test]
	public void ResolveType_WhenTypeConstructorHasEnumerableParameter_ThenResolvedTypeEqualsToRegisteredType(
		[ValueSource(nameof(GetActivationBuilders))]
		IActivationBuilder activationBuilder)
	{
		// Arrange:
		var descriptors = new ServiceDescription[]
		{
			new TypeServiceDescription(lifetime: LifetimeType.TRANSIENT,
				type: typeof(IEnumerable<Mocks.EmptyGeneric>), implementationType: typeof(Mocks.EnumerableTestService)),
			new TypeServiceDescription(lifetime: LifetimeType.TRANSIENT,
				type: typeof(Mocks.TestServiceWithEnumerableConstructor),
				implementationType: typeof(Mocks.TestServiceWithEnumerableConstructor))
		};
		using var container = new Container(activationBuilder, descriptors);
		var scope = container.CreateScope();

		// Act:
		var service = scope.Resolve<Mocks.TestServiceWithEnumerableConstructor>();

		// Assert:
		service.Should().BeOfType<Mocks.TestServiceWithEnumerableConstructor>();
	}

	[Test]
	public void ResolveMultiple_WhenTypeIsEnumerable_ThenResolvedTypesEqualsToRegisteredTypes(
		[ValueSource(nameof(GetActivationBuilders))]
		IActivationBuilder activationBuilder)
	{
		// Arrange:
		var descriptor = new MultipleServicesDescription(lifetime: LifetimeType.TRANSIENT,
			type: typeof(IEnumerable<Mocks.ITestService>), descriptors:
			[
				new TypeServiceDescription(lifetime: LifetimeType.TRANSIENT,
					type: typeof(Mocks.ITestService), implementationType: typeof(Mocks.TestService)),
				new TypeServiceDescription(lifetime: LifetimeType.TRANSIENT,
					type: typeof(Mocks.ITestService), implementationType: typeof(Mocks.TestServiceWithEmptyConstructor))
			]);

		using var container = new Container(activationBuilder, [descriptor]);
		var scope = container.CreateScope();

		// Act:
		var services = scope.Resolve<IEnumerable<Mocks.ITestService>>().ToArray();

		// Assert:
		services.Length.Should().Be(2);
		services[0].Should().BeOfType<Mocks.TestService>();
		services[1].Should().BeOfType<Mocks.TestServiceWithEmptyConstructor>();
	}
}