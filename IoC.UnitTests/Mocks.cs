// SPDX-License-Identifier: Apache-2.0
// © 2022-2024 Nikolay Melnikov <n.melnikov@depra.org>
// ReSharper disable All

namespace Depra.IoC.UnitTests;

internal static class Mocks
{
	internal class EmptyGeneric;

	internal interface ITestService;

	internal class TestService : ITestService;

	internal sealed class GenericTestService<T> : ITestService where T : EmptyGeneric
	{
		public GenericTestService(T value)
		{
			if (value == null)
			{
				throw new NullReferenceException();
			}
		}
	}

	internal sealed class EnumerableTestService : ITestService, IEnumerable<EmptyGeneric>
	{
		public IEnumerator<EmptyGeneric> GetEnumerator() => throw new NotImplementedException();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	internal sealed class TestServiceWithConstructor : ITestService
	{
		public sealed class Token
		{
			private readonly Guid _guid = Guid.NewGuid();

			public override string ToString() => _guid.ToString();
		}

		public TestServiceWithConstructor(Token token)
		{
			if (token == null)
			{
				throw new NullReferenceException();
			}
		}
	}

	internal sealed class TestServiceWithEmptyConstructor : ITestService
	{
		public TestServiceWithEmptyConstructor() { }
	}

	internal sealed class TestServiceWithEnumerableConstructor : ITestService
	{
		public TestServiceWithEnumerableConstructor(IEnumerable<EmptyGeneric> items)
		{
			if (items == null)
			{
				throw new NullReferenceException();
			}
		}
	}
}