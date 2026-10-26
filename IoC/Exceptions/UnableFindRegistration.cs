// SPDX-License-Identifier: Apache-2.0
// © 2022-2026 Depra <n.melnikov@depra.org>

using System;

namespace Depra.IoC.Exceptions
{
	internal sealed class UnableFindRegistration : Exception
	{
		public UnableFindRegistration(Type service) : base($"Unable to find registration for {service.Name}") { }
	}
}