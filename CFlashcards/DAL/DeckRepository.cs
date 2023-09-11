using CFlashcards.Models;
using Microsoft.EntityFrameworkCore;

namespace CFlashcards.DAL
{
    public class DeckRepository
    {
        private readonly AuthDbContext _db;
        private readonly ILogger<DeckRepository> _logger;

        public DeckRepository(AuthDbContext db, ILogger<DeckRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Deck>?> GetAll()
        {
            try
            {
                return await _db.Decks.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("[DeckRepository] ToListAsync() failed when GetAll() was called, error message:{e}", e.Message);
                return null;
            }
        }

        public async Task<Deck?> GetDeckById(int id)
        {
            try
            {
                return await _db.Decks.FindAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError("[DeckRepository] FindAsync() failed when GetDeckById() was called for DeckId {DeckId:0000} error message:{e}", id, e.Message);
                return null;
            }
        }

        public async Task<bool> Create(Deck deck)
        {
            try
            {
                _db.Decks.Add(deck);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[DeckRepository] deck creation failed for deck {@deck}, error message:{e}", e.Message);
                return false;
            }
        }

        public async Task<bool> Update(Deck deck)
        {
            try
            {
                _db.Decks.Update(deck);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[DeckRepository] deck FindAsync(id) failed when updating the DeckId {DeckId:0000}, error message:{e}", deck.DeckId, e.Message); //Not same as lecture notes
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var deck = await _db.Decks.FindAsync(id);
                if (deck == null)
                {
                    _logger.LogError("[DeckRepository] deck not found for the DeckId {DeckId:0000}", id);
                    return false;
                }
                _db.Decks.Remove(deck);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[DeckRepository] deck deletion failed for the DeckId {DeckId:0000}, error message:{e}", id, e.Message); //Not same as lecture notes
                return false;
            }
        }
    }
}
