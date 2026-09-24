// SPDX-License-Identifier: Apache-2.0
// © 2022-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.IoC.Enums;
using Depra.IoC.Exceptions;

namespace Depra.IoC.Description
{
	public sealed class TypeServiceDescription : ServiceDescription
	{
		public TypeServiceDescription(Type implementationType, Type type, LifetimeType lifetime, bool lazy = true) :
			base(type, lifetime, lazy)
		{
			Guard.AgainstNull(implementationType, nameof(implementationType));
			ImplementationType = implementationType;
		}

		public Type ImplementationType { get; }
	}
}