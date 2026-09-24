// SPDX-License-Identifier: Apache-2.0
// © 2022-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using Depra.IoC.Activation;
using Depra.IoC.Description;
using Depra.IoC.Exceptions;

namespace Depra.IoC.QoL.Builder
{
	public sealed class ContainerBuilder : IContainerBuilder
	{
		private readonly IActivationBuilder _activationBuilder;
		private readonly List<ServiceDescription> _descriptions;

		public ContainerBuilder(IActivationBuilder activationBuilder)
		{
			Guard.AgainstNull(activationBuilder, nameof(activationBuilder));

			_activationBuilder = activationBuilder;
			_descriptions = new List<ServiceDescription>();
		}

		public bool Exists(Type service) => _descriptions.Exists(x => x.Type == service);

		public IContainer Build() => new Container(_activationBuilder, _descriptions);

		public void Register(ServiceDescription description) => _descriptions.Add(description);

		public IContainerBuilder NonLazy()
		{
			if (_descriptions.Count == 0)
			{
				throw new InvalidOperationException("No registration is available to mark as non-lazy.");
			}

			_descriptions[^1].IsLazy = false;
			return this;
		}
	}
}