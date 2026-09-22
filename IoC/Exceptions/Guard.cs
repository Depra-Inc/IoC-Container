// SPDX-License-Identifier: Apache-2.0
// © 2022-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Depra.IoC.Exceptions
{
	internal static class Guard
	{
		[Conditional(Conditional.ENSURE)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstNull(object value, string parameterName)
		{
			if (value == null)
			{
				throw new ArgumentNullException(parameterName);
			}
		}

		[Conditional(Conditional.ENSURE)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstNotRegistered(object value, Type serviceType)
		{
			if (value == null)
			{
				throw new UnableFindRegistration(serviceType);
			}
		}

		private static class Conditional
		{
			// ReSharper disable UnusedMember.Local
			private const string TRUE = "DEBUG";
			private const string FALSE = "THIS_IS_JUST_SOME_RANDOM_STRING_THAT_IS_NEVER_DEFINED";
			// ReSharper restore UnusedMember.Local

#if DEBUG || DEV_BUILD
			public const string ENSURE = TRUE;
#else
			public const string ENSURE = FALSE;
#endif
		}
	}
}