// SPDX-License-Identifier: Apache-2.0
// © 2022-2026 Depra <n.melnikov@depra.org>

using System;
using System.Text;

namespace Depra.IoC.Exceptions
{
	internal static class TypeNameFormatter
	{
		private const char GENERIC_ARITY_MARKER = '`';

		public static string ToReadableName(this Type type)
		{
			var builder = new StringBuilder();
			Append(builder, type);

			return builder.ToString();
		}

		private static void Append(StringBuilder builder, Type type)
		{
			if (type.IsArray)
			{
				Append(builder, type.GetElementType());
				builder.Append('[').Append(',', type.GetArrayRank() - 1).Append(']');
				return;
			}

			if (type.IsNested && type.IsGenericParameter == false)
			{
				Append(builder, type.DeclaringType);
				builder.Append('.');
			}

			var name = type.Name;
			var markerIndex = name.IndexOf(GENERIC_ARITY_MARKER);
			if (markerIndex < 0)
			{
				builder.Append(name);
				return;
			}

			builder.Append(name, 0, markerIndex);
			AppendOwnGenericArguments(builder, type, int.Parse(name.Substring(markerIndex + 1)));
		}

		private static void AppendOwnGenericArguments(StringBuilder builder, Type type, int ownArity)
		{
			var arguments = type.GetGenericArguments();
			var firstOwnIndex = arguments.Length - ownArity;

			builder.Append('<');
			for (var index = firstOwnIndex; index < arguments.Length; index++)
			{
				if (index > firstOwnIndex)
				{
					builder.Append(", ");
				}

				Append(builder, arguments[index]);
			}

			builder.Append('>');
		}
	}
}
