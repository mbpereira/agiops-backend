#region

using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Common.Extensions;

#endregion

namespace PlanningPoker.Domain.Users;

public sealed class User : AggregateRoot
{
    private User(string id, string name) : base(id)
    {
        SetName(name);
        Guest = Guest.New();
    }

    private User(string id, string name, string email) : base(id)
    {
        SetName(name);
        SetEmail(email);
    }

    public string Name { get; private set; } = string.Empty;
    public Email? Email { get; private set; }
    public Guest? Guest { get; private set; }

    public bool IsGuest => Guest is not null;


    public void SetName(string name)
    {
        if (!name.HasMinLength(3))
        {
            AddError(UserErrors.InvalidName);
            return;
        }

        Name = name;
    }

    public void SetEmail(string? email)
    {
        if (email.IsEmail())
        {
            Email = new Email(email!);
            Guest = null;
            return;
        }

        AddError(UserErrors.InvalidEmail);
    }

    public static User New(string name, string email)
    {
        return new User(EntityId.Generate(), name, email);
    }

    public static User NewGuest(string name)
    {
        return new User(EntityId.Generate(), name);
    }
}