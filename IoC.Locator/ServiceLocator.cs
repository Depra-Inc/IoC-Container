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

		public static void Initialize(IScope globalScope)
		{
			if (globalScope == null)
			{
				throw new ArgumentNullException(nameof(globalScope));
			}

			lock (LOCK)
			{
				if (Service.CurrentScope != null)
				{
					throw new InvalidOperationException(
						"ServiceLocator is already initialized. " +
						"Call 'ServiceLocator.Reset' to reset it.");
				}

				Service.SetScope(globalScope);
			}
		}

		public static void Reset() => Service.SetScope(null);
	}
}