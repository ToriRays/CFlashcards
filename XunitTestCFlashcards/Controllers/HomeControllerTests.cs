using Castle.Core.Logging;
using CFlashcards.Controllers;
using CFlashcards.DAL;
using CFlashcards.Models;
using CFlashcards.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace XunitTestCFlashcards.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly Mock<SignInManager<FlashcardsUser>> _mockSignInManager;
        private readonly Mock<UserManager<FlashcardsUser>> _mockUserManager;
        private readonly HomeController _homeController;
        
        public HomeControllerTests()
            // Below we initialize mock object to prevent reuse of code in the unit tests.
        {
            // Create a mock logger
            _mockLogger = new Mock<ILogger<HomeController>>();
            // Create a mock UserStore that is needed to create a mock UserManager.
            var mockUserStore = new Mock<IUserStore<FlashcardsUser>>();
            // Create a mock UserManager.
            _mockUserManager = new Mock<UserManager<FlashcardsUser>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);
            // Create a mock ContextAccessor needed for SignInManager
            var mockContextAccessor = new Mock<IHttpContextAccessor>();
            // Create a mock ClaimFactory needed for SignInManager
            var mockClaimsFactory = new Mock<IUserClaimsPrincipalFactory<FlashcardsUser>>();
            // Create a mock SignInManager
            _mockSignInManager = new Mock<SignInManager<FlashcardsUser>>(
                _mockUserManager.Object, mockContextAccessor.Object, mockClaimsFactory.Object, null, null, null, null);
            _homeController = new HomeController(_mockLogger.Object, _mockUserManager.Object, _mockSignInManager.Object);
        }

        [Fact]
        public void TestIndexLoginCheckPasses()
        {
            // Arrange
            _mockSignInManager.Setup(m => m.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(true);

            // Act
            var result = _homeController.Index();

            // Assert
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            var resultAction = Assert.IsAssignableFrom<IActionResult>(result);
            Assert.Equal("Index1", redirectToAction.ActionName);
        }

        [Fact]
        public void TestIndexLoginCheckFails()
    {
            // Arrange
            _mockSignInManager.Setup(m => m.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(false);

            // Act
            var result = _homeController.Index();

            // Assert
            var resultAction = Assert.IsAssignableFrom<IActionResult>(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
        }

        [Fact]
        public void TestIndex1()
        {
            // Act
            var result = _homeController.Index1();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index1", viewResult.ViewName);
        }
    }
}
