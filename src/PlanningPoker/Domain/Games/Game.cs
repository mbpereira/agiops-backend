#region

using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Common.Extensions;

#endregion

namespace PlanningPoker.Domain.Games;

public sealed class Game : TenantableAggregateRoot
{
    private Game(
        string id,
        string tenantId,
        string name,
        string userId,
        VotingSystem votingSystem,
        string? password = null,
        string? teamId = null)
        : base(id, tenantId)
    {
        SetName(name);
        SetOwner(userId);
        SetPassword(password);
        SetVotingSystem(votingSystem);
        SetTeamId(teamId);
    }

    public string Name { get; private set; } = string.Empty;
    public EntityId UserId { get; private set; } = EntityId.Empty;
    public GameCredentials? Credentials { get; private set; }
    public GradeDetails GradeDetails { get; private set; } = GradeDetails.Empty();
    public EntityId? TeamId { get; private set; }

    public void SetVotingSystem(VotingSystem votingSystem)
    {
        if (votingSystem.IsNull() || !votingSystem.IsValid)
        {
            AddError(GameErrors.InvalidVotingSystem);
            return;
        }

        GradeDetails = votingSystem.GradeDetails;
    }

    public void SetOwner(string userId)
    {
        if (UserId.Value.IsPresent())
        {
            AddError(GameErrors.OwnerAlreadySet);
            return;
        }

        if (!userId.IsPresent())
        {
            AddError(GameErrors.InvalidUserId);
            return;
        }

        UserId = userId;
    }

    public void SetName(string name)
    {
        if (!name.HasMinLength(1))
        {
            AddError(GameErrors.InvalidName);
            return;
        }

        Name = name;
    }

    public void SetPassword(string? password = null)
    {
        if (password.IsNullOrEmpty())
        {
            Credentials = null;
            return;
        }

        if (!password.HasMinLength(6))
        {
            AddError(GameErrors.InvalidPassword);
            return;
        }

        Credentials = new GameCredentials(password!);
    }

    public void SetTeamId(string? teamId = null)
    {
        if (teamId.IsPresent()) TeamId = teamId!;
    }

    public static Game New(
        string tenantId,
        string name,
        string userId,
        VotingSystem votingSystem,
        string? password = null,
        string? teamId = null)
    {
        return new Game(EntityId.Generate(), tenantId, name, userId, votingSystem, password, teamId);
    }
}