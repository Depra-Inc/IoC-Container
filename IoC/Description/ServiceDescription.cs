// SPDX-License-Identifier: Apache-2.0
// © 2022-2026 Depra <n.melnikov@depra.org>

using System;
using Depra.IoC.Enums;

namespace Depra.IoC.Description
{
    public abstract class ServiceDescription
    {
        protected ServiceDescription(Type type, LifetimeType lifetime)
        {
            Type = type;
            Lifetime = lifetime;
        }

        public Type Type { get; }
        public LifetimeType Lifetime { get; }
        public bool NonLazy { get; internal set; }
    }
}