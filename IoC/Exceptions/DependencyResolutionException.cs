// SPDX-License-Identifier: Apache-2.0
// © 2022-2026 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Depra.IoC.Description;

namespace Depra.IoC.Exceptions
{
	internal abstract class DependencyResolutionException : Exception
	{
		private const int MIN_PATH_LENGTH_TO_PRINT = 3;
		private readonly List<ResolutionFrame> _path = new();

		protected DependencyResolutionException(Exception innerException = null) : base(null, innerException) { }

		public override string Message
		{
			get
			{
				var builder = new StringBuilder(Reason);
				AppendRequester(builder);
				AppendPath(builder);

				return builder.Append('.').ToString();
			}
		}

		protected abstract string Reason { get; }

		internal void PushFailed(Type service) => _path.Add(new ResolutionFrame(service, null));

		internal void PushConsumer(ServiceDescription description) =>
			_path.Insert(0, new ResolutionFrame(description.Type, GetImplementation(description)));

		private void AppendRequester(StringBuilder builder)
		{
			if (_path.Count < 2)
			{
				return;
			}

			var consumer = _path[^2];
			var failed = _path[^1];
			builder.Append(" required by '").Append(consumer.Owner.ToReadableName()).Append('\'');

			var parameter = FindParameterName(consumer.Implementation, failed.Service);
			if (parameter != null)
			{
				builder.Append(" parameter '").Append(parameter).Append('\'');
			}
		}

		private void AppendPath(StringBuilder builder)
		{
			if (_path.Count < MIN_PATH_LENGTH_TO_PRINT)
			{
				return;
			}

			builder.Append(". Path: ");
			builder.Append(string.Join(" -> ", _path.Select(x => x.Service.ToReadableName())));
		}

		private static string FindParameterName(Type consumer, Type dependency) => consumer?
			.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
			.FirstOrDefault()?
			.GetParameters()
			.FirstOrDefault(x => x.ParameterType == dependency)?
			.Name;

		private static Type GetImplementation(ServiceDescription description) => description switch
		{
			TypeBasedServiceDescription typeBased => typeBased.ImplementationType,
			InstanceBasedServiceDescription instanceBased => instanceBased.Instance.GetType(),
			_ => null
		};

		private readonly struct ResolutionFrame
		{
			public readonly Type Service;
			public readonly Type Implementation;

			public ResolutionFrame(Type service, Type implementation)
			{
				Service = service;
				Implementation = implementation;
			}

			public Type Owner => Implementation ?? Service;
		}
	}
}
