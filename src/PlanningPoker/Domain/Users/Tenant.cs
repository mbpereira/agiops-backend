﻿using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users
{
    public static class TenantErrors
    {
        public static Error InvalidName = Error.MinLength(nameof(Tenant), nameof(Tenant.Name), minLength: 3);
    }

    public class Tenant : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;

        private Tenant(int id, string name)
            : base(id)
        {
            SetName(name);
        }

        public void SetName(string name)
        {
            if (!name.HasMinLength(minLength: 3))
            {
                AddError(TenantErrors.InvalidName);
                return;
            }

            Name = name;
        }

        public static Tenant Load(int id, string name) => new(id, name);
        public static Tenant New(string name) => new(EntityId.AutoIncrement(), name);
    }
}
