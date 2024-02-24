namespace PlanningPoker.UnitTests.Domain.Invitations;

public class InvitationFixture
{
    public static IEnumerable<object[]> GetAcceptedOrCancelledInvitations()
    {
        var cancelledInvitation = FakerInstance.NewValidInvitation();
        cancelledInvitation.Cancel();
        yield return [cancelledInvitation];

        var acceptedInvitation = FakerInstance.NewValidInvitation();
        acceptedInvitation.Accept();
        yield return [acceptedInvitation];
    }
}