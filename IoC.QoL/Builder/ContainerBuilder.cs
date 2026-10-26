// SPDX-License-Identifier: Apache-2.0
// © 2022-2026 Depra <n.melnikov@depra.org>

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
		private ServiceDescription _lastRegistration;

		public ContainerBuilder(IActivationBuilder activationBuilder)
		{
			Guard.AgainstNull(activationBuilder, nameof(activationBuilder));

			_activationBuilder = activationBuilder;
			_descriptions = new List<ServiceDescription>();
		}

		ServiceDescription IContainerBuilder.LastRegistration => _lastRegistration;

		public IContainer Build() => new Container(_activationBuilder, _descriptions);

		void IContainerBuilder.Register(ServiceDescription description)
		{
			_lastRegistration = description;
			_descriptions.Add(description);
		}
	}
}