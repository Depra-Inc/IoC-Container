// SPDX-License-Identifier: Apache-2.0
// © 2022-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Depra.IoC.Scope;

namespace Depra.IoC.QoL.Scope
{
	public sealed class CombinedScope : IScope
	{
		private readonly IScope[] _scopes;
		private readonly IScope _rootScope;

		public CombinedScope(IScope root, params IScope[] scopes)
		{
			_scopes = scopes;
			_rootScope = root;

			foreach (var scope in _scopes)
			{
				SetParentScope(scope, root);
			}
		}

		bool IScope.CanResolve(Type service) =>
			_rootScope.CanResolve(service) ||
			_scopes.Any(scope => scope.CanResolve(service));

		object IScope.Resolve(Type service)
		{
			if (_rootScope.CanResolve(service))
			{
				return _rootScope.Resolve(service);
			}

			foreach (var scope in _scopes)
			{
				if (scope.CanResolve(service))
				{
					return scope.Resolve(service);
				}
			}

			throw new InvalidOperationException();
		}

		private static void SetParentScope(IScope scope, IScope parentScope)
		{
			if (scope == null || parentScope == null)
			{
				return;
			}

			var parentField = scope.GetType().GetField("_parentScope", BindingFlags.Instance | BindingFlags.NonPublic);
			if (parentField == null)
			{
				return;
			}

			parentField.SetValue(scope, parentScope);
		}

		void IDisposable.Dispose()
		{
			foreach (var scope in _scopes)
			{
				scope.Dispose();
			}
		}

		async ValueTask IAsyncDisposable.DisposeAsync()
		{
			foreach (var scope in _scopes)
			{
				await scope.DisposeAsync();
			}
		}
	}
}