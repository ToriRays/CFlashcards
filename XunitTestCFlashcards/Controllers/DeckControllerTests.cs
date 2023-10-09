﻿using Castle.Core.Logging;
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

        // A function that will prevent reuse of code in the unit tests.
        private static DeckController CreateDeckController(Mock<IDeckRepository> mockDeckRepository)
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
        public async Task TestBrowse()
        {
            // Arrange
            var deckList = new List<Deck>()
            {
                deck1, deck2
            };

            var mockDeckRepository = new Mock<IDeckRepository>();
            // The flashcardUserId passed in to the GetAll function in the controller will be an empty string "",
            // so for the test to work, an empty string needs to be passed in here as well.
            mockDeckRepository.Setup(repo => repo.GetAll("")).ReturnsAsync(deckList);
            var deckController = CreateDeckController(mockDeckRepository);

            // Act
            var result = await deckController.Browse();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewDeckList = Assert.IsAssignableFrom<IEnumerable<Deck>>(viewResult.ViewData.Model);
            Assert.Equal(2, viewDeckList.Count());
            Assert.Equal(deckList, viewDeckList);
        }

        [Fact]
        public async Task TestCarousel()
        {
            var testId = 1;
            // Arrange
            var mockDeckRepository = new Mock<IDeckRepository>();
            mockDeckRepository.Setup(repo => repo.GetDeckById(testId)).ReturnsAsync(deck1);
            var deckController = CreateDeckController(mockDeckRepository);

            // Act
            var result = await deckController.Carousel(testId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewDeck = Assert.IsAssignableFrom<Deck>(viewResult.ViewData.Model);
            Assert.Equal(deck1, viewDeck);
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
            Assert.Equal("Browse", redirectToAction.ActionName);
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
            Assert.Equal("Browse", redirectToAction.ActionName);
        }

        [Fact]
        public async Task TestDeleteDeckConfirmedFunctionFails()
        {
            var testId = 1;
            // Arrange
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
            var testId = 1;
            // Arrange
            var mockDeckRepository = new Mock<IDeckRepository>();
            mockDeckRepository.Setup(repo => repo.Delete(testId)).ReturnsAsync(true);
            var deckController = CreateDeckController(mockDeckRepository);

            // Act
            var result = await deckController.DeleteDeckConfirmed(testId);

            // Assert
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            var resultAction = Assert.IsAssignableFrom<IActionResult>(result);
            Assert.Equal("Browse", redirectToAction.ActionName);
        }
    }
}