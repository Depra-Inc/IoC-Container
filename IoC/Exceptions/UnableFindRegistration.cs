﻿// SPDX-License-Identifier: Apache-2.0
// © 2022-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Reflection;

namespace Depra.IoC.Exceptions
{
	internal sealed class UnableFindRegistration : Exception
	{
		public UnableFindRegistration(MemberInfo service) : base($"Unable to find registration for {service.Name}") { }
	}
}