using CFlashcards.Controllers;
using CFlashcards.DAL;
using CFlashcards.Models;
using CFlashcards.ViewModels;
using Microsoft.AspNetCore.Identity;
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
    public class FlashcardControllerTests
    {
        // Declaring flashcard and deck used for testing
        private readonly Deck deck1 = new()
        {
            DeckId = 1,
            Title = "Test1",
            Description = "This is the first test deck.",
            Flashcards = null,
            FlashcardUserId = ""
        };

        private readonly Flashcard flashcard1 = new()
        {
            FlashcardId = 1,
            Question = "Test1",
            Answer = "This is the first test card.",
            Notes = "Test notes.",
            DeckId = 1,
        };

        private readonly Flashcard flashcard2 = new()
        {
            FlashcardId = 2,
            Question = "Test2",
            Answer = "This is the second test card.",
            Notes = "Test notes.",
            DeckId = 1,
        };


        // A function that will prevent reuse of code in the unit tests.
        private FlashcardController CreateFlashcardController(Mock<IFlashcardRepository> mockFlashcardRepository)
        // This function created a FlashcardController instance and is used to prevent
        // reuse of code in the unit tests.
        {
            var testDeckId = 1;
            // Create a mock UserStore that is needed to create a mock UserManager.
            var mockUserStore = new Mock<IUserStore<FlashcardsUser>>();
            // Create a mock UserManager.
            var mockUserManager = new Mock<UserManager<FlashcardsUser>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);
            // Create and setup a deck repository for testing.
            var mockDeckRepository = new Mock<IDeckRepository>();
            mockDeckRepository.Setup(repo => repo.GetDeckById(testDeckId)).ReturnsAsync(deck1); // Setup the deck repository such that deck1 is returned given the testDeckId.
            // Create a mock logger
            var mockLogger = new Mock<ILogger<FlashcardController>>();
            return new FlashcardController(mockFlashcardRepository.Object, mockDeckRepository.Object, mockUserManager.Object, mockLogger.Object);
        }

        [Fact]
        public async Task TestBrowseFlashcards()
        {
            // Arrange
            var deckId = 1;
            var flashcardList = new List<Flashcard>()
            {
                flashcard1,flashcard1
            };
            PaginatedList<Flashcard>? paginatedFlashcards = PaginatedList<Flashcard>.Create(flashcardList, 1, 4) 
                ?? new PaginatedList<Flashcard>(flashcardList, 2, 1, 4);
            PaginatedFlashcardsViewModel paginatedFlashcardsViewModel = new(paginatedFlashcards, deckId, "");
            
            var mockFlashcardRepository = new Mock<IFlashcardRepository>();
            mockFlashcardRepository.Setup(repo => repo.GetFlashcardsByDeckId(deckId)).ReturnsAsync(flashcardList);
            var deckController = CreateFlashcardController(mockFlashcardRepository);

            // Act
            var result = await deckController.BrowseFlashcards(deckId, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewPaginatedFlashcardsViewModel = Assert.IsAssignableFrom<PaginatedFlashcardsViewModel>(viewResult.ViewData.Model);
            // Below we check that the Flashcards inside the model are equal because the two models will always be pointers
            // to two different objects and the assert will fail, meanwhile the Flashcards should be the same object.
            Assert.Equal(paginatedFlashcardsViewModel.Flashcards, viewPaginatedFlashcardsViewModel.Flashcards);
        }

        [Fact]
        public async Task TestDetails()
        {
            // Arrange
            var testId = 1;

            var mockFlashcardRepository = new Mock<IFlashcardRepository>();
            mockFlashcardRepository.Setup(repo => repo.GetFlashcardById(testId)).ReturnsAsync(flashcard1);
            var flashcardController = CreateFlashcardController(mockFlashcardRepository);

            // Act
            var result = await flashcardController.Details(testId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewFlashcard = Assert.IsAssignableFrom<Flashcard>(viewResult.ViewData.Model);
            Assert.Equal(flashcard1, viewFlashcard);
        }

        [Fact]
        public async Task TestCreateFlashcardFunctionFails()
        {
            // Arrange
            var testFlashcard = flashcard1;
            testFlashcard.Deck = deck1;

            var mockFlashcardRepository = new Mock<IFlashcardRepository>();
            mockFlashcardRepository.Setup(repo => repo.Create(testFlashcard)).ReturnsAsync(false);
            var flashcardController = CreateFlashcardController(mockFlashcardRepository);

            // Act
            var result = await flashcardController.CreateFlashcard(flashcard1);

            // Assert
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            var resultAction = Assert.IsAssignableFrom<IActionResult>(result);
            Assert.Equal("BrowseFlashcards", redirectToAction.ActionName);
        }

        [Fact]
        public async Task TestCreateFlashcardFunctionPasses()
        {
            // Arrange
            var testFlashcard = flashcard1;
            testFlashcard.Deck = deck1;
            var mockFlashcardRepository = new Mock<IFlashcardRepository>();
            mockFlashcardRepository.Setup(repo => repo.Create(testFlashcard)).ReturnsAsync(true);
            var flashcardController = CreateFlashcardController(mockFlashcardRepository);

            // Act
            var result = await flashcardController.CreateFlashcard(flashcard1);

            // Assert
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            var resultAction = Assert.IsAssignableFrom<IActionResult>(result);
            Assert.Equal("Details", redirectToAction.ActionName);
        }

        [Fact]
        public async Task TestUpdateFlashcardFunctionFails()
        {
            // Arrange
            var testFlashcard = flashcard1;
            testFlashcard.Deck = deck1;
            
            var mockFlashcardRepository = new Mock<IFlashcardRepository>();
            mockFlashcardRepository.Setup(repo => repo.Update(testFlashcard)).ReturnsAsync(false);
            var flashcardController = CreateFlashcardController(mockFlashcardRepository);

            // Act
            var result = await flashcardController.UpdateFlashcard(flashcard1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewFlashcard = Assert.IsAssignableFrom<Flashcard>(viewResult.ViewData.Model);
            // Check that the card remains the same
            Assert.Equal(flashcard1, viewFlashcard);
        }

        [Fact]
        public async Task TestUpdateFlashcardFunctionPasses()
        {
            // Arrange
            var flashcard = flashcard1;
            
            var mockFlashcardRepository = new Mock<IFlashcardRepository>();
            mockFlashcardRepository.Setup(repo => repo.Update(flashcard1)).ReturnsAsync(true);
            var flashcardController = CreateFlashcardController(mockFlashcardRepository);

            // Act
            var result = await flashcardController.UpdateFlashcard(flashcard1);

            // Assert
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            var resultAction = Assert.IsAssignableFrom<IActionResult>(result);
            Assert.Equal("Details", redirectToAction.ActionName);
        }

        [Fact]
        public async Task TestDeleteFlashcardConfirmedFunctionFails()
        {
            // Arrange
            var testFlashcardId = 1;
            var testDeckId = 1;
            
            var mockFlashcardRepository = new Mock<IFlashcardRepository>();
            mockFlashcardRepository.Setup(repo => repo.Delete(testFlashcardId)).ReturnsAsync(false);
            var flashcardController = CreateFlashcardController(mockFlashcardRepository);

            // Act
            var result = await flashcardController.DeleteFlashcardConfirmed(testFlashcardId, testDeckId);

            // Assert
            var viewResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task TestDeleteFlashcardConfirmedFunctionPasses()
        {
            // Arrange
            var testFlashcardId = 1;
            var testDeckId = 1;
            
            var mockFlashcardRepository = new Mock<IFlashcardRepository>();
            mockFlashcardRepository.Setup(repo => repo.Delete(testFlashcardId)).ReturnsAsync(true);
            var flashcardController = CreateFlashcardController(mockFlashcardRepository);

            // Act
            var result = await flashcardController.DeleteFlashcardConfirmed(testFlashcardId, testDeckId);

            // Assert
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            var resultAction = Assert.IsAssignableFrom<IActionResult>(result);
            Assert.Equal("BrowseFlashcards", redirectToAction.ActionName);
        }
    }
}
