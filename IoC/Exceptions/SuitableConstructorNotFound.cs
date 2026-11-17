// SPDX-License-Identifier: Apache-2.0
// © 2022-2026 Depra <n.melnikov@depra.org>

using System;

namespace Depra.IoC.Exceptions
{
	internal sealed class SuitableConstructorNotFound : DependencyResolutionException
	{
		private readonly Type _implementation;

		public SuitableConstructorNotFound(Type implementation) => _implementation = implementation;

		protected override string Reason => $"No public constructor in '{_implementation.ToReadableName()}'";
	}
}
