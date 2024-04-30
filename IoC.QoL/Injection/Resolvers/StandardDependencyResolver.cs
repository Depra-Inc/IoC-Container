﻿// SPDX-License-Identifier: Apache-2.0
// © 2022-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Reflection;
using Depra.IoC.QoL.Attributes;
using Depra.IoC.QoL.Injection.Factory;
using Depra.IoC.Scope;

namespace Depra.IoC.QoL.Injection.Resolvers
{
	public sealed class StandardDependencyResolver : IDependencyResolver
	{
		private static readonly Type DEPENDENCY_TYPE = typeof(DependencyAttribute);

		private readonly IInjectionFactory _factory;

		public StandardDependencyResolver(IInjectionFactory factory) => _factory = factory;

		void IDependencyResolver.Resolve(IScope scope, object target)
		{
			const BindingFlags BINDING_ATTR = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			var fields = target.GetType().GetFields(BINDING_ATTR);
			foreach (var field in fields)
			{
				if (field.IsStatic == false && Attribute.IsDefined(field, DEPENDENCY_TYPE))
				{
					_factory.Create(field).Invoke(target, scope.Resolve(field.FieldType));
				}
			}
		}
	}
}