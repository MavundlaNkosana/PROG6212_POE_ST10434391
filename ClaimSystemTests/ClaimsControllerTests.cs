// To make this file compile, ensure the test project has:
// 1. A <ProjectReference> to the main Contract_Monthly_Claim_System project.
// 2. NuGet packages for xUnit, Moq, and Microsoft.AspNetCore.Mvc.Core.

using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Contract_Monthly_Claim_System.Controllers;
using Contract_Monthly_Claim_System.Models;
using System;
using System.IO;

namespace Contract_Monthly_Claim_System.Tests
{
    public class ClaimsControllerTests
    {
        [Fact]
        public void Create_Post_AddsClaimAndRedirects()
        {
            // Arrange
            var controller = new ClaimsController();
            var newClaim = new Claim { Month = 10, Year = 2025, LecturerId = Guid.NewGuid() };

            // Act
            var result = controller.Create(newClaim);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues["id"]);
        }

        [Fact]
        public void Submit_Post_ChangesStatusToSubmitted()
        {
            // Arrange
            var controller = new ClaimsController();
            var claim = new Claim { Status = ClaimStatus.Draft };
            ClaimsController.Claims.TryAdd(claim.ClaimId, claim);

            // Act
            var result = controller.Submit(claim.ClaimId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal(ClaimStatus.Submitted, claim.Status);
            Assert.NotNull(claim.SubmittedAt);
        }

        [Fact]
        public void Edit_Get_ReturnsNotFoundForInvalidId()
        {
            // Arrange
            var controller = new ClaimsController();
            var invalidGuid = Guid.NewGuid();

            // Act
            var result = controller.Edit(invalidGuid);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Upload_Post_RejectsInvalidFileType()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var controller = new ClaimsController { TempData = tempData };

            var claim = new Claim();
            ClaimsController.Claims.TryAdd(claim.ClaimId, claim);

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("This is a dummy file");
            writer.Flush();
            stream.Position = 0;
            IFormFile mockFile = new FormFile(stream, 0, stream.Length, "file", "test.txt"); // .txt is not allowed

            // Act
            var result = controller.Upload(claim.ClaimId, mockFile);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirectToActionResult.ActionName);
            Assert.True(controller.TempData.ContainsKey("ErrorMessage"));
            Assert.Equal("Invalid file type. Only .pdf, .docx, and .xlsx are allowed.", controller.TempData["ErrorMessage"]);
        }
    }
}

