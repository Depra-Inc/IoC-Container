// SPDX-License-Identifier: Apache-2.0
// © 2025-2026 Depra <n.melnikov@depra.org>

using System;
using Depra.IoC.Scope;

namespace Depra.IoC.Locator
{
	/// <summary>
	/// Consumer API: resolves services from the global scope.
	/// </summary>
	public static class Service
	{
		private static IScope _globalScope;
		internal static IScope CurrentScope => _globalScope;

		public static TService Get<TService>()
		{
			var scope = _globalScope;
			if (scope == null)
			{
				throw new InvalidOperationException(
					"ServiceLocator is not initialized. " +
					"Call 'ServiceLocator.Initialize' first.");
			}

			return scope.Resolve<TService>();
		}

		[Obsolete("Use Get<T>() instead.")]
		public static TService Resolve<TService>() => Get<TService>();

		internal static void SetScope(IScope scope) => _globalScope = scope;
	}
}