using CFlashcards.Models;
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
                _logger.LogError("[FlashcardRepository] ToListAsync() failed when GetAll() was called, error message:{e}", e.Message);
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
                _logger.LogError("[FlashcardRepository] FindAsync() failed when GetFlashcardById() was called for FlashcardId {FlashcardId:0000} error message:{e}", id, e.Message);
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
                _logger.LogError("[FlashcardRepository] flashcard creation failed for flashcard {@flashcard}, error message:{e}", e.Message);
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
                _logger.LogError("[FlashcardRepository] flashcard FindAsync(id) failed when updating the FlashcardId {FlashcardId:0000}, error message:{e}", flashcard.FlashcardId, e.Message); //Not same as lecture notes
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
                    _logger.LogError("[FlashcardRepository] flashcard not found for the FlashcardId {FlashcardId:0000}", id);
                    return false;
                }
                _db.Flashcards.Remove(flashcard);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[FlashcardRepository] flashcard deletion failed for the FlashcardId {FlashcardId:0000}, error message:{e}", id, e.Message); //Not same as lecture notes
                return false;
            }
        }
    }
}
