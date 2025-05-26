// SPDX-License-Identifier: Apache-2.0
// © 2025 Depra <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;
using Depra.IoC.Scope;

[assembly: InternalsVisibleTo("Depra.IoC.ServiceLocator.UnitTests")]

namespace Depra.IoC.ServiceLocator
{
	/// <summary>
	/// Global entry point for initializing and disposing the service locator.
	/// </summary>
	public static class Locator
	{
		public static void Initialize(IScope globalScope)
		{
			if (globalScope == null)
			{
				throw new ArgumentNullException(nameof(globalScope));
			}

			if (Service.GlobalScope != null)
			{
				throw new AlreadyInitializedException();
			}

			Service.GlobalScope = globalScope;
		}

		public static void Dispose() => Service.GlobalScope = null!;

		internal sealed class AlreadyInitializedException : InvalidOperationException
		{
			public AlreadyInitializedException() : base("ServiceLocator is already initialized. " +
			                                            $"Call '{nameof(Locator)}.{nameof(Dispose)}' to reset it.") { }
		}
	}

	/// <summary>
	/// API for resolving services from the global scope.
	/// </summary>
	public static class Service
	{
		internal static IScope GlobalScope = null!;

		public static TService Resolve<TService>()
		{
			if (GlobalScope == null)
			{
				throw new InvalidOperationException("ServiceLocator is not initialized. " +
				                                    $"Call '{nameof(Locator)}.{nameof(Locator.Initialize)}' first.");
			}

			return GlobalScope.Resolve<TService>();
		}
	}
}