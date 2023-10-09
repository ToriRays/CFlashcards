﻿using CFlashcards.Controllers;
using CFlashcards.DAL;
using CFlashcards.Models;
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


        // A function that will prevent reuse of code in the unit tests.
        private FlashcardController CreateFlashcardController(Mock<IFlashcardRepository> mockFlashcardRepository)
        {
            var testDeckId = 1;
            // Create a mock UserStore that is needed to create a mock UserManager.
            var mockUserStore = new Mock<IUserStore<FlashcardsUser>>();
            // Create a deck repository.
            var mockDeckRepository = new Mock<IDeckRepository>();
            mockDeckRepository.Setup(repo => repo.GetDeckById(testDeckId)).ReturnsAsync(deck1);
            // Create a mock logger
            var loggerMock = new Mock<ILogger<FlashcardController>>();
            return new FlashcardController(mockFlashcardRepository.Object, mockDeckRepository.Object, loggerMock.Object);
        }

        [Fact]
        public async Task TestDetails()
        {
            var testId = 1;
            // Arrange
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
            Assert.Equal("Deck", redirectToAction.ControllerName);
            Assert.Equal("Carousel", redirectToAction.ActionName);
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
            var testFlashcard = flashcard1;
            testFlashcard.Deck = deck1;
            // Arrange
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
            var flashcard = flashcard1;
            // Arrange
            var mockFlashcardRepository = new Mock<IFlashcardRepository>();
            mockFlashcardRepository.Setup(repo => repo.Update(flashcard1)).ReturnsAsync(true);
            var flashcardController = CreateFlashcardController(mockFlashcardRepository);

            // Act
            var result = await flashcardController.UpdateFlashcard(flashcard1);

            // Assert
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            var resultAction = Assert.IsAssignableFrom<IActionResult>(result);
            Assert.Equal("Details", redirectToAction.ActionName);
            //Assert.Equal(flashcard1.FlashcardId, redirectToAction.RouteValues);
        }

        [Fact]
        public async Task TestDeleteFlashcardConfirmedFunctionFails()
        {
            var testFlashcardId = 1;
            var testDeckId = 1;
            // Arrange
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
            var testFlashcardId = 1;
            var testDeckId = 1;
            // Arrange
            var mockFlashcardRepository = new Mock<IFlashcardRepository>();
            mockFlashcardRepository.Setup(repo => repo.Delete(testFlashcardId)).ReturnsAsync(true);
            var flashcardController = CreateFlashcardController(mockFlashcardRepository);

            // Act
            var result = await flashcardController.DeleteFlashcardConfirmed(testFlashcardId, testDeckId);

            // Assert
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            var resultAction = Assert.IsAssignableFrom<IActionResult>(result);
            Assert.Equal("Deck", redirectToAction.ControllerName);
            Assert.Equal("Carousel", redirectToAction.ActionName);
        }
    }
}