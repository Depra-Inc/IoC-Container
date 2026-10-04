// SPDX-License-Identifier: Apache-2.0
// © 2022-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.IoC.Description;

namespace Depra.IoC.QoL.Builder
{
	public interface IContainerBuilder
	{
		internal ServiceDescription LastRegistration { get; }

		IContainer Build();

		void Register(ServiceDescription description);
	}
}