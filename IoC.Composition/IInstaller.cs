// SPDX-License-Identifier: Apache-2.0
// © 2022-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.IoC.QoL.Builder;

namespace Depra.IoC.Composition
{
	public interface IInstaller
	{
		void Install(IContainerBuilder builder);
	}
}