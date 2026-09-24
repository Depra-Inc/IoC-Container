// SPDX-License-Identifier: Apache-2.0
// © 2022-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.IoC.Enums;
using Depra.IoC.Exceptions;

namespace Depra.IoC.Description
{
	public sealed class InstanceServiceDescription : ServiceDescription
	{
		public InstanceServiceDescription(Type type, object instance, bool lazy = true) :
			base(type, LifetimeType.SINGLETON, lazy)
		{
			Guard.AgainstNull(instance, nameof(instance));
			Instance = instance;
		}

		public object Instance { get; }
	}
}