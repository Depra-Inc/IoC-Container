// SPDX-License-Identifier: Apache-2.0
// © 2022-2026 Depra <n.melnikov@depra.org>

using System;
using Depra.IoC.Scope;

namespace Depra.IoC
{
	public interface IContainer : IDisposable, IAsyncDisposable
	{
		IScope CreateScope();
		IScope CreateScope(IScope parentScope);
	}
}