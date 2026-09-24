// SPDX-License-Identifier: Apache-2.0
// © 2022-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.IoC.Description;

namespace Depra.IoC.QoL.Builder
{
	public interface IContainerBuilder
	{
		IContainer Build();

		bool Exists(Type service);

		void Register(ServiceDescription description);

		IContainerBuilder NonLazy();
	}
}