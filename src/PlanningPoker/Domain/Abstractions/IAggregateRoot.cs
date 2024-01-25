﻿namespace Domain.Abstractions
{

    public interface IAggregateRoot
    {
        IReadOnlyList<IDomainEvent> GetDomainEvents();

        void ClearDomainEvents();
    }
}