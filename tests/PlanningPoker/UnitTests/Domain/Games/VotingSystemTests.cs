﻿#region

using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;

#endregion

namespace PlanningPoker.UnitTests.Domain.Games;

public class VotingSystemTests
{
    private readonly Faker _faker = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("te")]
    public void New_InvalidData_ReturnsValidationErrors(string invalidDescription)
    {
        var expectedErrors = new[]
        {
            new { Code = "TenantId", Message = "Provided value cannot be null, empty or white space." },
            new
            {
                Code = "VotingSystem.Name",
                Message = "The provided string does not meet the minimum length requirement. Min length: 3."
            },
            new { Code = "VotingSystem.Grades", Message = "The list cannot be empty." },
            new { Code = "VotingSystem.UserId", Message = "Provided value cannot be null, empty or white space." }
        };

        var votingSystem = VotingSystem.New(EntityId.Empty, invalidDescription, EntityId.Empty,
            []);

        votingSystem.Errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Theory]
    [InlineData(SharingStatus.Unshared)]
    [InlineData(SharingStatus.Approved)]
    public void SetSharingStatus_ApprovalAttemptOfInvalidStatus_ShouldReturnsApprovalError(
        SharingStatus invalidApprovalStatus)
    {
        var votingSystem = GetValidVotingSystem(invalidApprovalStatus);

        votingSystem.SetSharingStatus(SharingStatus.Approved);

        votingSystem.Errors.Should().BeEquivalentTo([
            new
            {
                Code = "VotingSystem.SharingStatus",
                Message = "Only the statuses 'requested' and 'rejected' can be approved."
            }
        ]);
    }

    [Theory]
    [InlineData(SharingStatus.Unshared)]
    [InlineData(SharingStatus.Approved)]
    [InlineData(SharingStatus.Rejected)]
    public void SetSharingStatus_RejectAttemptOfInvalidStatus_ReturnsRejectError(
        SharingStatus invalidStatus)
    {
        var votingSystem = GetValidVotingSystem(invalidStatus);

        votingSystem.SetSharingStatus(SharingStatus.Rejected);

        votingSystem.Errors.Should().BeEquivalentTo([
            new { Code = "VotingSystem.SharingStatus", Message = "Only the status 'requested' can be rejected." }
        ]);
    }

    [Theory]
    [InlineData(SharingStatus.Requested)]
    [InlineData(SharingStatus.Approved)]
    public void SetSharingStatus_SharingAttemptOfInvalidStatus_ReturnsSharingError(
        SharingStatus invalidStatus)
    {
        var votingSystem = GetValidVotingSystem(invalidStatus);

        votingSystem.SetSharingStatus(SharingStatus.Requested);

        votingSystem.Errors.Should().BeEquivalentTo([
            new
            {
                Code = "VotingSystem.SharingStatus",
                Message = "Only the statuses 'rejected' and 'unshared' can made a request sharing."
            }
        ]);
    }

    [Theory]
    [InlineData(SharingStatus.Approved, SharingStatus.Requested)]
    [InlineData(SharingStatus.Approved, SharingStatus.Rejected)]
    [InlineData(SharingStatus.Rejected, SharingStatus.Requested)]
    [InlineData(SharingStatus.Requested, SharingStatus.Unshared)]
    [InlineData(SharingStatus.Requested, SharingStatus.Rejected)]
    public void SetSharingStatus_ValidStatusChange_ReturnsIsValidTrue(
        SharingStatus nextStatus, SharingStatus oldStatus)
    {
        var votingSystem = GetValidVotingSystem(oldStatus);

        votingSystem.SetSharingStatus(nextStatus);

        using var _ = new AssertionScope();
        votingSystem.IsValid.Should().BeTrue();
        votingSystem.SharingStatus.Should().Be(nextStatus);
    }

    [Fact]
    public void SetName_ValidName_ChangeName()
    {
        var newDescription = _faker.Random.String2(10);
        var votingSystem = GetValidVotingSystem();

        votingSystem.SetName(newDescription);

        votingSystem.Name.Should().Be(newDescription);
    }

    [Fact]
    public void GradeDetails_NonNumericGrades_ReturnsIsQuantifiableFalse()
    {
        var votingSystem = GetValidVotingSystem();
        votingSystem.SetPossibleGrades(["P", "M", "G"]);

        var gradeDetails = votingSystem.GradeDetails;

        gradeDetails.IsQuantifiable.Should().BeFalse();
    }

    [Fact]
    public void GradeDetails_NumericGrades_ReturnsIsQuantifiableTrue()
    {
        var votingSystem = GetValidVotingSystem();
        votingSystem.SetPossibleGrades(["1", "2", "3"]);

        var gradeDetails = votingSystem.GradeDetails;

        gradeDetails.IsQuantifiable.Should().BeTrue();
    }

    private VotingSystem GetValidVotingSystem(SharingStatus sharingStatus = SharingStatus.Unshared)
    {
        return _faker.NewValidVotingSystem(sharingStatus);
    }
}