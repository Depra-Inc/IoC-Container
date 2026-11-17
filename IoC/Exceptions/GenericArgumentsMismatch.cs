// SPDX-License-Identifier: Apache-2.0
// © 2022-2026 Depra <n.melnikov@depra.org>

using System;

namespace Depra.IoC.Exceptions
{
	internal sealed class GenericArgumentsMismatch : DependencyResolutionException
	{
		private readonly Type _service;
		private readonly Type _openImplementation;

		public GenericArgumentsMismatch(Type service, Type openImplementation, Exception innerException) :
			base(innerException)
		{
			_service = service;
			_openImplementation = openImplementation;
			PushFailed(service);
		}

		protected override string Reason =>
			$"'{_openImplementation.ToReadableName()}' can't be closed as '{_service.ToReadableName()}'";
	}
}
