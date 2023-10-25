using Castle.Core.Logging;
using CFlashcards.Controllers;
using CFlashcards.DAL;
using CFlashcards.Models;
using CFlashcards.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XunitTestCFlashcards.Controllers
{
    public class DeckControllerTests
    {
        // Declaring decks used for testing
        private readonly Deck deck1 = new()
        {
            DeckId = 1,
            Title = "Test1",
            Description = "This is the first test deck.",
            Flashcards = null,
            FlashcardUserId = ""
        };

        private readonly Deck deck2 = new()
        {
            DeckId = 2,
            Title = "Test2",
            Description = "This is the second test deck.",
            Flashcards = null,
            FlashcardUserId = ""
        };

        private static DeckController CreateDeckController(Mock<IDeckRepository> mockDeckRepository)
        // This function creates a DeckController instance and is used to prevent
        // reuse of code in the unit tests.
        {
            // Create a mock UserStore that is needed to create a mock UserManager.
            var mockUserStore = new Mock<IUserStore<FlashcardsUser>>();
            // Create a mock UserManager.
            var mockUserManager = new Mock<UserManager<FlashcardsUser>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);
            // Create a mock logger
            var mockLogger = new Mock<ILogger<DeckController>>();
            return new DeckController(mockDeckRepository.Object, mockLogger.Object, mockUserManager.Object);
        }

        [Fact]
        public async Task TestBrowseDecksWithoutSearch()
        {
            // Arrange
            var deckList = new List<Deck>()
            {
                deck1, deck2
            };
            PaginatedList<Deck>? paginatedDeckList = PaginatedList<Deck>.Create(deckList, 1, 6);

            var mockDeckRepository = new Mock<IDeckRepository>();
            // The flashcardUserId passed in to the GetAll function in the controller will be an empty string "",
            // so for the test to work, an empty string needs to be passed in here as well.
            mockDeckRepository.Setup(repo => repo.GetAll("")).ReturnsAsync(deckList);
            var deckController = CreateDeckController(mockDeckRepository);

            // Act
            var result = await deckController.BrowseDecks("", null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewPaginatedDeckList = Assert.IsAssignableFrom<PaginatedList<Deck>>(viewResult.ViewData.Model);
            Assert.Equal(2, viewPaginatedDeckList.Count);
            if (paginatedDeckList != null) // Avoid null exception.
            {
                Assert.Equal(paginatedDeckList, viewPaginatedDeckList);
            }
        }

        [Fact]
        public async Task TestBrowseDecksWithSearch()
        {
            // Arrange
            var deckList = new List<Deck>()
            {
                deck1, deck2
            };
            PaginatedList<Deck>? paginatedDeckList = PaginatedList<Deck>.Create(deckList, 1, 6);

            var searchString = "test";

            var mockDeckRepository = new Mock<IDeckRepository>();
            // The flashcardUserId passed in to the GetAll function in the controller will be an empty string "",
            // so for the test to work, an empty string needs to be passed in here as well.
            mockDeckRepository.Setup(repo => repo.SearchDecksByTitle("",searchString)).ReturnsAsync(deckList);
            var deckController = CreateDeckController(mockDeckRepository);

            // Act
            var result = await deckController.BrowseDecks(searchString, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewPaginatedDeckList = Assert.IsAssignableFrom<PaginatedList<Deck>>(viewResult.ViewData.Model);
            Assert.Equal(2, viewPaginatedDeckList.Count);
            Assert.Equal(paginatedDeckList, viewPaginatedDeckList);
        }


        [Fact]
        public async Task TestCreateDeckFunctionFails()
        {
            // Arrange
            var mockDeckRepository = new Mock<IDeckRepository>();
            mockDeckRepository.Setup(repo => repo.Create(deck1)).ReturnsAsync(false);
            var deckController = CreateDeckController(mockDeckRepository);

            // Act
            var result = await deckController.CreateDeck(deck1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewDeck = Assert.IsAssignableFrom<Deck>(viewResult.ViewData.Model);
            Assert.Equal(deck1, viewDeck);
        }

        [Fact]
        public async Task TestCreateDeckFunctionPasses()
        {
            // Arrange
            var mockDeckRepository = new Mock<IDeckRepository>();
            mockDeckRepository.Setup(repo => repo.Create(deck1)).ReturnsAsync(true);
            var deckController = CreateDeckController(mockDeckRepository);

            // Act
            var result = await deckController.CreateDeck(deck1);

            // Assert
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            var resultAction = Assert.IsAssignableFrom<IActionResult>(result);
            Assert.Equal("BrowseDecks", redirectToAction.ActionName);
        }

        [Fact]
        public async Task TestUpdateDeckFunctionFails()
        {
            // Arrange
            var mockDeckRepository = new Mock<IDeckRepository>();
            mockDeckRepository.Setup(repo => repo.Update(deck1)).ReturnsAsync(false);
            var deckController = CreateDeckController(mockDeckRepository);

            // Act
            var result = await deckController.UpdateDeck(deck1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewDeck = Assert.IsAssignableFrom<Deck>(viewResult.ViewData.Model);
            // Check that the deck remains the same
            Assert.Equal(deck1, viewDeck);
        }

        [Fact]
        public async Task TestUpdateDeckFunctionPasses()
        {
            // Arrange
            var mockDeckRepository = new Mock<IDeckRepository>();
            mockDeckRepository.Setup(repo => repo.Update(deck1)).ReturnsAsync(true);
            var deckController = CreateDeckController(mockDeckRepository);

            // Act
            var result = await deckController.UpdateDeck(deck1);

            // Assert
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            var resultAction = Assert.IsAssignableFrom<IActionResult>(result);
            Assert.Equal("BrowseDecks", redirectToAction.ActionName);
        }

        [Fact]
        public async Task TestDeleteDeckConfirmedFunctionFails()
        {
            // Arrange
            var testId = 1;

            var mockDeckRepository = new Mock<IDeckRepository>();
            mockDeckRepository.Setup(repo => repo.Delete(testId)).ReturnsAsync(false);
            var deckController = CreateDeckController(mockDeckRepository);

            // Act
            var result = await deckController.DeleteDeckConfirmed(testId);

            // Assert
            var viewResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task TestDeleteDeckConfirmedFunctionPasses()
        {
            // Arrange
            var testId = 1;

            var mockDeckRepository = new Mock<IDeckRepository>();
            mockDeckRepository.Setup(repo => repo.Delete(testId)).ReturnsAsync(true);
            var deckController = CreateDeckController(mockDeckRepository);

            // Act
            var result = await deckController.DeleteDeckConfirmed(testId);

            // Assert
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            var resultAction = Assert.IsAssignableFrom<IActionResult>(result);
            Assert.Equal("BrowseDecks", redirectToAction.ActionName);
        }
    }
}
