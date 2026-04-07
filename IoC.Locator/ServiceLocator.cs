// SPDX-License-Identifier: Apache-2.0
// © 2025-2026 Depra <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;
using Depra.IoC.Scope;

[assembly: InternalsVisibleTo("Depra.IoC.Locator.UnitTests")]

namespace Depra.IoC.Locator
{
	/// <summary>
	/// Global entry point for initializing and disposing the service locator.
	/// </summary>
	public static class ServiceLocator
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
			                                            $"Call '{nameof(ServiceLocator)}.{nameof(Dispose)}' to reset it.") { }
		}
	}
}