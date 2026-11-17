// SPDX-License-Identifier: Apache-2.0
// © 2022-2026 Depra <n.melnikov@depra.org>

using System;

namespace Depra.IoC.Exceptions
{
	internal sealed class UnableFindRegistration : DependencyResolutionException
	{
		private readonly Type _service;

		public UnableFindRegistration(Type service)
		{
			_service = service;
			PushFailed(service);
		}

		protected override string Reason => $"No registration for '{_service.ToReadableName()}'";
	}
}
