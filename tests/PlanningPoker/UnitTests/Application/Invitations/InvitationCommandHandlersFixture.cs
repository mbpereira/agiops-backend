#region

using PlanningPoker.Application.Tenants;
using PlanningPoker.Application.Users;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Abstractions.Clock;
using PlanningPoker.Domain.Invitations;
using PlanningPoker.Domain.Users;

#endregion

namespace PlanningPoker.UnitTests.Application.Invitations;

public class InvitationCommandHandlersFixture
{
    public readonly IDateTimeProvider DateTimeProvider;
    public readonly IAccessGrantsRepository Grants;
    public readonly IInvitationsRepository Invitations;
    public readonly ITenantContext TenantContext;
    public readonly TenantInformation TenantInformation;
    public readonly IUnitOfWork Uow;
    public readonly IUserContext UserContext;
    public readonly UserInformation UserInformation;

    public InvitationCommandHandlersFixture()
    {
        TenantContext = Substitute.For<ITenantContext>();
        TenantInformation = new TenantInformation(FakerInstance.ValidId());
        TenantContext.GetCurrentTenantAsync().Returns(TenantInformation);
        DateTimeProvider = Substitute.For<IDateTimeProvider>();
        Grants = Substitute.For<IAccessGrantsRepository>();
        Invitations = Substitute.For<IInvitationsRepository>();
        Uow = Substitute.For<IUnitOfWork>();
        Uow.Invitations.Returns(Invitations);
        Uow.AccessGrants.Returns(Grants);
        UserContext = Substitute.For<IUserContext>();
        UserInformation = new UserInformation(FakerInstance.ValidId());
        UserContext.GetCurrentUserAsync().Returns(UserInformation);
    }
}