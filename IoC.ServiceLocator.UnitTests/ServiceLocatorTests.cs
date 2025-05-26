// SPDX-License-Identifier: Apache-2.0
// © 2025 Depra <n.melnikov@depra.org>

using Depra.IoC.Scope;
using NSubstitute;

namespace Depra.IoC.ServiceLocator.UnitTests;

public sealed class ServiceLocatorTests
{
	[Fact]
	public void Should_Throw_When_ServiceLocator_Is_Not_Initialized() =>
		Assert.Throws<InvalidOperationException>(Service.Resolve<IService>);

	[Fact]
	public void Should_Throw_When_ServiceLocator_Is_Already_Initialized()
	{
		// Arrange:
		var scope = Substitute.For<IScope>();
		Locator.Initialize(scope);

		// Act:
		var act = () => Locator.Initialize(scope);

		// Assert:
		Assert.Throws<Locator.AlreadyInitializedException>(act);

		// Cleanup:
		Locator.Dispose();
	}
	
	[Fact]
	public void Should_Resolve_Service_From_Global_Scope()
	{
		// Arrange:
		var scope = Substitute.For<IScope>();
		var service = Substitute.For<IService>();
		scope.Resolve<IService>().Returns(service);
		Locator.Initialize(scope);

		// Act:
		var resolvedService = Service.Resolve<IService>();

		// Assert:
		Assert.Equal(service, resolvedService);

		// Cleanup:
		Locator.Dispose();
	}

	public interface IService { }
}