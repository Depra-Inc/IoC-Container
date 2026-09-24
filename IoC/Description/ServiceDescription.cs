// SPDX-License-Identifier: Apache-2.0
// © 2022-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.IoC.Enums;

namespace Depra.IoC.Description
{
    public abstract class ServiceDescription
    {
        protected ServiceDescription(Type type, LifetimeType lifetime, bool lazy = true)
        {
            Type = type;
            Lifetime = lifetime;
            IsLazy = lazy;
        }

        public Type Type { get; }
        public LifetimeType Lifetime { get; }
        public bool IsLazy { get; internal set; }
    }
}