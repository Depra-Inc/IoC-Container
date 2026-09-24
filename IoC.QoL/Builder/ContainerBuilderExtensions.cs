// SPDX-License-Identifier: Apache-2.0
// © 2022-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;
using Depra.IoC.Description;
using Depra.IoC.Enums;
using Depra.IoC.Scope;

namespace Depra.IoC.QoL.Builder
{
	public static class ContainerBuilderExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IContainerBuilder RegisterTransient(this IContainerBuilder self, Type service, Type implementation)
			=> self.RegisterType(service, implementation, LifetimeType.TRANSIENT);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IContainerBuilder RegisterScoped(this IContainerBuilder self, Type service, Type implementation)
			=> self.RegisterType(service, implementation, LifetimeType.SCOPED);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IContainerBuilder RegisterSingleton(this IContainerBuilder self, Type service, Type implementation)
			=> self.RegisterType(service, implementation, LifetimeType.SINGLETON);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IContainerBuilder RegisterTransient(this IContainerBuilder self, Type service,
			Func<IScope, object> factory)
			=> self.RegisterFactory(service, factory, LifetimeType.TRANSIENT);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IContainerBuilder RegisterScoped(this IContainerBuilder self, Type service,
			Func<IScope, object> factory)
			=> self.RegisterFactory(service, factory, LifetimeType.SCOPED);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IContainerBuilder RegisterSingleton(this IContainerBuilder self, Type service,
			Func<IScope, object> factory)
			=> self.RegisterFactory(service, factory, LifetimeType.SINGLETON);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IContainerBuilder RegisterSingleton(this IContainerBuilder self, Type service, object instance)
			=> self.RegisterInstance(service, instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IContainerBuilder RegisterTransient<TImplementation>(this IContainerBuilder self)
			=> self.RegisterType(typeof(TImplementation), typeof(TImplementation), LifetimeType.TRANSIENT);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IContainerBuilder RegisterScoped<TImplementation>(this IContainerBuilder self)
			=> self.RegisterType(typeof(TImplementation), typeof(TImplementation), LifetimeType.SCOPED);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IContainerBuilder RegisterSingleton<TImplementation>(this IContainerBuilder self) =>
			self.RegisterType(typeof(TImplementation), typeof(TImplementation), LifetimeType.SINGLETON);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IContainerBuilder RegisterTransient<TService, TImplementation>(this IContainerBuilder self)
			where TImplementation : TService
			=> self.RegisterType(typeof(TService), typeof(TImplementation), LifetimeType.TRANSIENT);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IContainerBuilder RegisterScoped<TService, TImplementation>(this IContainerBuilder self)
			where TImplementation : TService
			=> self.RegisterType(typeof(TService), typeof(TImplementation), LifetimeType.SCOPED);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IContainerBuilder RegisterSingleton<TService, TImplementation>(this IContainerBuilder self)
			where TImplementation : TService
			=> self.RegisterType(typeof(TService), typeof(TImplementation), LifetimeType.SINGLETON);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IContainerBuilder RegisterSingleton<TService>(this IContainerBuilder self,
			TService instance) =>
			self.RegisterInstance(typeof(TService), instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IContainerBuilder RegisterSingleton<TService, TImplementation>(this IContainerBuilder self,
			TImplementation instance) where TImplementation : TService =>
			self.RegisterInstance(typeof(TService), instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IContainerBuilder RegisterType(this IContainerBuilder self, Type service, Type implementation,
			LifetimeType lifetime)
		{
			self.Register(new TypeServiceDescription(implementation, service, lifetime));
			return self;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static IContainerBuilder RegisterFactory(this IContainerBuilder self, Type service,
			Func<IScope, object> factory, LifetimeType lifetime)
		{
			self.Register(new FactoryServiceDescription(service, lifetime, factory));
			return self;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static IContainerBuilder RegisterInstance(this IContainerBuilder self, Type service, object instance)
		{
			self.Register(new InstanceServiceDescription(service, instance));
			return self;
		}
	}
}