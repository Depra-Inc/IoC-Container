// SPDX-License-Identifier: Apache-2.0
// © 2022-2026 Depra <n.melnikov@depra.org>

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Depra.IoC.Activation;
using Depra.IoC.Description;
using Depra.IoC.Enums;
using Depra.IoC.Exceptions;
using Depra.IoC.Scope;

namespace Depra.IoC
{
	public sealed class Container : IContainer
	{
		private readonly Scope _rootScope;
		private readonly IActivationBuilder _activationBuilder;
		private readonly ConcurrentDictionary<Type, ServiceDescription> _descriptors;
		private readonly ConcurrentDictionary<ServiceDescription, Func<IScope, object>> _buildActivators;

		public Container(IActivationBuilder activationBuilder, IEnumerable<ServiceDescription> descriptors)
		{
			Guard.AgainstNull(descriptors, nameof(descriptors));
			Guard.AgainstNull(activationBuilder, nameof(activationBuilder));

			_rootScope = new Scope(this);
			_activationBuilder = activationBuilder;
			_descriptors = new ConcurrentDictionary<Type, ServiceDescription>();
			_buildActivators = new ConcurrentDictionary<ServiceDescription, Func<IScope, object>>();

			var descriptorsArray = descriptors.ToArray();
			FillDescriptors(descriptorsArray);
			InitializeNonLazyDescriptors(descriptorsArray);
		}

		public void Dispose() => _rootScope.Dispose();

		public async ValueTask DisposeAsync() => await _rootScope
			.DisposeAsync()
			.ConfigureAwait(false);

		public IScope CreateScope() => new Scope(this, _rootScope);

		private ServiceDescription FindDescriptor(Type service)
		{
			if (_descriptors.TryGetValue(service, out var descriptor))
			{
				return descriptor;
			}

			if (service.IsAssignableFrom(typeof(IEnumerable)) && service.IsGenericType &&
			    service.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			{
				var items = FindDescriptor(service.GetGenericArguments().Single());
				return items != null
					? _descriptors.GetOrAdd(service, BuildUsingMultipleDescriptor(service, items))
					: null;
			}

			if (!service.IsConstructedGenericType)
			{
				return null;
			}

			var genericTypeDefinition = service.GetGenericTypeDefinition();
			var genericDescriptor = FindDescriptor(genericTypeDefinition);
			if (genericDescriptor is not TypeServiceDescription typeBased)
			{
				return null;
			}

			var genericArguments = service.GetGenericArguments();
			var genericType = typeBased.ImplementationType.MakeGenericType(genericArguments);
			var argumentsDescriptor = new TypeServiceDescription(genericType, service, typeBased.Lifetime);

			return _descriptors.GetOrAdd(genericType, argumentsDescriptor);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private object CreateInstance(IScope scope, ServiceDescription description) =>
			_buildActivators.GetOrAdd(description, _ => BuildActivation(description, _activationBuilder))(scope);

		private static Func<IScope, object> BuildActivation(ServiceDescription serviceDescription,
			IActivationBuilder activationBuilder) => serviceDescription switch
		{
			InstanceServiceDescription instanceBased => _ => instanceBased.Instance,
			FactoryServiceDescription factoryBased => factoryBased.Func,
			_ => activationBuilder.BuildActivation((TypeServiceDescription) serviceDescription)
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void FillDescriptors(IEnumerable<ServiceDescription> descriptors)
		{
			IDictionary<Type, ServiceDescription> descriptorsAsDictionary = _descriptors;
			var descriptorGroups = descriptors.GroupBy(x => x.Type);
			foreach (var descriptorsGroup in descriptorGroups)
			{
				var items = descriptorsGroup.ToArray();
				if (items.Length == 1)
				{
					descriptorsAsDictionary.Add(descriptorsGroup.Key, items[0]);
				}
				else
				{
					const LifetimeType LIFETIME = (LifetimeType) int.MaxValue;
					var multiple = new MultipleServicesDescription(descriptorsGroup.Key, LIFETIME, items);
					descriptorsAsDictionary.Add(descriptorsGroup.Key, multiple);
					var serviceType = typeof(IEnumerable<>).MakeGenericType(descriptorsGroup.Key);
					descriptorsAsDictionary.Add(serviceType, BuildUsingMultipleDescriptor(serviceType, multiple));
				}
			}

		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void InitializeNonLazyDescriptors(ServiceDescription[] descriptors)
		{
			for (int index = 0, count = descriptors.Length; index < count; index++)
			{
				var descriptor = descriptors[index];
				if (!descriptor.IsLazy)
				{
					_rootScope.ResolveInternal(descriptor);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ServiceDescription BuildUsingMultipleDescriptor(Type serviceType, ServiceDescription description) =>
			new FactoryServiceDescription(serviceType, LifetimeType.TRANSIENT, scope =>
			{
				var items = (description as MultipleServicesDescription)?.Descriptors ?? new[] { description };
				var scopeImpl = (Scope) scope;
				var array = Array.CreateInstance(description.Type, items.Length);
				for (var index = 0; index < array.Length; index++)
				{
					array.SetValue(scopeImpl.ResolveInternal(items[index]), index);
				}

				return array;
			});

		private sealed class Scope : IScope
		{
			private readonly Container _container;
			private readonly ConcurrentStack<object> _disposables;
			private readonly ConcurrentDictionary<ServiceDescription, object> _scopedInstances;
			private readonly IScope _parentScope;

			public Scope(Container container, IScope parentScope = null)
			{
				_container = container;
				_parentScope = parentScope;
				_disposables = new ConcurrentStack<object>();
				_scopedInstances = new ConcurrentDictionary<ServiceDescription, object>();
			}

			public void Dispose()
			{
				foreach (var disposable in _disposables)
				{
					if (disposable is IAsyncDisposable asyncDisposable)
					{
						Task.Run(async () => await asyncDisposable.DisposeAsync().ConfigureAwait(false))
							.ConfigureAwait(false)
							.GetAwaiter()
							.GetResult();
					}
					else
					{
						((IDisposable) disposable).Dispose();
					}
				}
			}

			public async ValueTask DisposeAsync()
			{
				foreach (var disposable in _disposables)
				{
					if (disposable is IAsyncDisposable asyncDisposable)
					{
						await asyncDisposable.DisposeAsync();
					}
					else
					{
						((IDisposable) disposable).Dispose();
					}
				}
			}

			public bool CanResolve(Type service) =>
				_container.FindDescriptor(service) != null ||
				(_parentScope != null && _parentScope.CanResolve(service));

			public object Resolve(Type service)
			{
				var descriptor = _container.FindDescriptor(service);
				if (descriptor == null && _parentScope != null)
				{
					return _parentScope.Resolve(service);
				}

				Guard.AgainstNotRegistered(descriptor, service);
				return ResolveInternal(descriptor);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal object ResolveInternal(ServiceDescription description)
			{
				if (description.Lifetime == LifetimeType.TRANSIENT)
				{
					return CreateInstance(description);
				}

				if (ShouldCacheInstanceInCurrentScope(description))
				{
					return _scopedInstances.GetOrAdd(description, _ => CreateInstance(description));
				}

				return _container._rootScope.ResolveInternal(description);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private bool ShouldCacheInstanceInCurrentScope(ServiceDescription description)
			{
				if (description.Lifetime == LifetimeType.SCOPED)
				{
					return true;
				}

				if (_container._rootScope == this)
				{
					return true;
				}

				return _parentScope != null && _parentScope != _container._rootScope;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private object CreateInstance(ServiceDescription description)
			{
				var result = _container.CreateInstance(this, description);
				if (result is IDisposable)
				{
					_disposables.Push(result);
				}

				if (result is IAsyncDisposable)
				{
					_disposables.Push(result);
				}

				return result;
			}
		}
	}
}