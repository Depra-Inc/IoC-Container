﻿// SPDX-License-Identifier: Apache-2.0
// © 2022-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.IoC.Enums;
using Depra.IoC.Exceptions;
using Depra.IoC.Scope;

namespace Depra.IoC.Description
{
	public sealed class FactoryBasedServiceDescription : ServiceDescription
	{
		public FactoryBasedServiceDescription(Type type, LifetimeType lifetime, Func<IScope, object> func) : base(type, lifetime)
		{
			Guard.AgainstNull(func, nameof(func));
			Func = func;
		}

		public Func<IScope, object> Func { get; }
	}
}