// SPDX-License-Identifier: Apache-2.0
// © 2025-2026 Depra <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;
using Depra.IoC.Scope;

[assembly: InternalsVisibleTo("Depra.IoC.Locator.UnitTests")]

namespace Depra.IoC.Locator
{
	/// <summary>
	/// Setup API: initializes and frees the global scope.
	/// </summary>
	public static class ServiceLocator
	{
		private static readonly object LOCK = new();

		public static void Initialize(IScope scope)
		{
			if (scope == null)
			{
				throw new ArgumentNullException(nameof(scope));
			}

			lock (LOCK)
			{
				Service.SetScope(scope);
			}
		}

		public static void Reset()
		{
			lock (LOCK)
			{
				Service.SetScope(null);
			}
		}
	}
}