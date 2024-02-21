﻿using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Domain.Games;
using PlanningPoker.UnitTests.Domain.Common.Extensions;

namespace PlanningPoker.UnitTests.Domain.Games
{
    public class VotingSystemTests
    {
        private readonly Faker _faker = new();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("te")]
        public void New_ShouldReturnExpectedErrors(string invalidDescription)
        {
            var expectedErrors = new[]
            {
                new { Code = "TenantId", Message = "Provided value must be greater than 0." },
                new
                {
                    Code = "VotingSystem.Name",
                    Message = "The provided string does not meet the minimum length requirement. Min length: 3."
                },
                new { Code = "VotingSystem.Grades", Message = "The list cannot be empty." },
                new { Code = "VotingSystem.UserId", Message = "Provided value must be greater than 0." },
            };

            var votingSystem = VotingSystem.New(tenantId: 0, invalidDescription, userId: 0, new List<string>());

            votingSystem.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Theory]
        [InlineData(SharingStatus.Unshared)]
        [InlineData(SharingStatus.Approved)]
        public void SetSharingStatus_ShouldReturnErrorWhenTryingToApproveSharingOfNonValidVotingSystem(
            SharingStatus invalidApprovalStatus)
        {
            var votingSystem = GetValidVotingSystem(invalidApprovalStatus);

            votingSystem.SetSharingStatus(SharingStatus.Approved);

            votingSystem.Errors.Should().BeEquivalentTo(new[]
            {
                new
                {
                    Code = "VotingSystem.SharingStatus",
                    Message = "Only the statuses 'requested' and 'rejected' can be approved."
                }
            });
        }

        [Theory]
        [InlineData(SharingStatus.Unshared)]
        [InlineData(SharingStatus.Approved)]
        [InlineData(SharingStatus.Rejected)]
        public void SetSharingStatus_ShouldReturnErrorWhenTryingToRejectSharingOfNonValidVotingSystem(
            SharingStatus invalidApprovalStatus)
        {
            var votingSystem = GetValidVotingSystem(invalidApprovalStatus);

            votingSystem.SetSharingStatus(SharingStatus.Rejected);

            votingSystem.Errors.Should().BeEquivalentTo(new[]
            {
                new { Code = "VotingSystem.SharingStatus", Message = "Only the status 'requested' can be rejected." }
            });
        }

        [Theory]
        [InlineData(SharingStatus.Requested)]
        [InlineData(SharingStatus.Approved)]
        public void SetSharingStatus_ShouldReturnErrorWhenTryingToRequestSharingOfNonValidVotingSystem(
            SharingStatus invalidApprovalStatus)
        {
            var votingSystem = GetValidVotingSystem(invalidApprovalStatus);

            votingSystem.SetSharingStatus(SharingStatus.Requested);

            votingSystem.Errors.Should().BeEquivalentTo(new[]
            {
                new
                {
                    Code = "VotingSystem.SharingStatus",
                    Message = "Only the statuses 'rejected' and 'unshared' can made a request sharing."
                }
            });
        }

        [Theory]
        [InlineData(SharingStatus.Approved, SharingStatus.Requested)]
        [InlineData(SharingStatus.Approved, SharingStatus.Rejected)]
        [InlineData(SharingStatus.Rejected, SharingStatus.Requested)]
        [InlineData(SharingStatus.Requested, SharingStatus.Unshared)]
        [InlineData(SharingStatus.Requested, SharingStatus.Rejected)]
        public void SetSharingStatus_ShouldReturnErrorWhenTryingToSetUndefinedAsSharingStatus(
            SharingStatus nextSharingStatus, SharingStatus oldSharingStatus)
        {
            var votingSystem = GetValidVotingSystem(oldSharingStatus);

            votingSystem.SetSharingStatus(nextSharingStatus);

            using var _ = new AssertionScope();
            votingSystem.IsValid.Should().BeTrue();
            votingSystem.SharingStatus.Should().Be(nextSharingStatus);
        }

        [Fact]
        public void SetName_ShouldChangeName()
        {
            var newDescription = _faker.Random.String2(length: 10);
            var votingSystem = GetValidVotingSystem();

            votingSystem.SetName(newDescription);

            votingSystem.Name.Should().Be(newDescription);
        }

        [Fact]
        public void GradeDetails_ShouldReturnIsQuantifiableAsFalseWhenExistsAnyNonNumericGrade()
        {
            var votingSystem = GetValidVotingSystem();
            votingSystem.SetPossibleGrades(new[] { "P", "M", "G" });

            var gradeDetails = votingSystem.GradeDetails;

            gradeDetails.IsQuantifiable.Should().BeFalse();
        }

        [Fact]
        public void GradeDetails_ShouldReturnIsQuantifiableAsTrueWhenAllGradesAreNumeric()
        {
            var votingSystem = GetValidVotingSystem();
            votingSystem.SetPossibleGrades(new[] { "1", "2", "3" });

            var gradeDetails = votingSystem.GradeDetails;

            gradeDetails.IsQuantifiable.Should().BeTrue();
        }

        private VotingSystem GetValidVotingSystem(SharingStatus sharingStatus = SharingStatus.Unshared)
            => _faker.NewValidVotingSystem(sharingStatus);
    }
}