// To make this file compile, ensure the test project has:
// 1. A <ProjectReference> to the main Contract_Monthly_Claim_System project.
// 2. NuGet packages for xUnit and Moq.

using Xunit;
using Microsoft.AspNetCore.Mvc;
using Contract_Monthly_Claim_System.Controllers;
using Contract_Monthly_Claim_System.Models;
using System;
using System.Linq;

namespace Contract_Monthly_Claim_System.Tests
{
    public class CoordinatorControllerTests
    {
        [Fact]
        public void Approve_Post_ChangesStatusToApproved()
        {
            // Arrange
            var controller = new CoordinatorController();
            var claim = new Claim { Status = ClaimStatus.Submitted };
            ClaimsController.Claims.TryAdd(claim.ClaimId, claim);

            // Act
            var result = controller.Approve(claim.ClaimId, "Looks good.");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            var approvedClaim = ClaimsController.Claims[claim.ClaimId];
            Assert.Equal(ClaimStatus.Approved, approvedClaim.Status);
            Assert.Single(approvedClaim.Approvals);
            Assert.Equal("Looks good.", approvedClaim.Approvals.First().Comments);
        }

        [Fact]
        public void Reject_Post_ChangesStatusToRejected()
        {
            // Arrange
            var controller = new CoordinatorController();
            var claim = new Claim { Status = ClaimStatus.Submitted };
            ClaimsController.Claims.TryAdd(claim.ClaimId, claim);

            // Act
            var result = controller.Reject(claim.ClaimId, "Missing document.");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            var rejectedClaim = ClaimsController.Claims[claim.ClaimId];
            Assert.Equal(ClaimStatus.Rejected, rejectedClaim.Status);
            Assert.Single(rejectedClaim.Approvals);
            Assert.False(rejectedClaim.Approvals.First().IsApproved);
        }

        [Fact]
        public void Details_Get_ReturnsNotFoundForInvalidId()
        {
            // Arrange
            var controller = new CoordinatorController();
            var invalidGuid = Guid.NewGuid();

            // Act
            var result = controller.Details(invalidGuid);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

