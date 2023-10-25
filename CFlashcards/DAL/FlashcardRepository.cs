﻿using CFlashcards.Models;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

namespace CFlashcards.DAL
{
    public class FlashcardRepository : IFlashcardRepository
    {
        private readonly AuthDbContext _db;
        private readonly ILogger<FlashcardRepository> _logger;

        public FlashcardRepository(AuthDbContext db, ILogger<FlashcardRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Flashcard>?> GetAll()
        {
            try
            {
                return await _db.Flashcards.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("[FlashcardRepository] ToListAsync() failed when GetAll() was called, error message:{@e.Message}", e.Message);
                return null;
            }
        }
        
        public async Task<Flashcard?> GetFlashcardById(int id)
        {
            try
            {
                return await _db.Flashcards.FindAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError("[FlashcardRepository] FindAsync() failed when GetFlashcardById() was called for FlashcardId {@id} error message:{@e.Message}", id, e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<Flashcard>?> GetFlashcardsByDeckId(int deckId)
        {
            try
            {
                var flashcards = await _db.Flashcards.Where(flashcard => flashcard.DeckId == deckId).ToListAsync();
                if (flashcards == null)
                {
                    _logger.LogError("The flashcard list is null in GetFlashcardsByDeckId");
                }
                return flashcards;
            }
            catch (Exception e)
            {
                _logger.LogError("[FlashcardRepository] Where() failed when GetFlashcardsByDeckId() was called for DeckId {@deckId}, error message: {@e}", deckId, e.Message);
                return null;
            }
        }

        public async Task<bool> Create(Flashcard flashcard)
        {
            try
            {
                _db.Flashcards.Add(flashcard);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[FlashcardRepository] Add() failed for flashcard {@flashcard}, error message:{@e.Message}", flashcard, e.Message);
                return false;
            }
        }

        public async Task<bool> Update(Flashcard flashcard)
        {
            try
            {
                _db.Flashcards.Update(flashcard);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[FlashcardRepository] Update() failed when updating the FlashcardId {@flashcard.FlashcardId}, error message:{@e.Message}", flashcard.FlashcardId, e.Message);
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var flashcard = await _db.Flashcards.FindAsync(id);
                if (flashcard == null)
                {
                    _logger.LogError("[FlashcardRepository] flashcard not found for the FlashcardId {@id}", id);
                    return false;
                }
                _db.Flashcards.Remove(flashcard);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[FlashcardRepository] Remove() failed for the FlashcardId {@id}, error message:{@e.Message}", id, e.Message);
                return false;
            }
        }
    }
}
