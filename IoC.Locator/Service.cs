// SPDX-License-Identifier: Apache-2.0
// © 2025-2026 Depra <n.melnikov@depra.org>

using System;
using Depra.IoC.Scope;

namespace Depra.IoC.Locator
{
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
				                                    $"Call '{nameof(ServiceLocator)}.{nameof(ServiceLocator.Initialize)}' first.");
			}

			return GlobalScope.Resolve<TService>();
		}
	}
}