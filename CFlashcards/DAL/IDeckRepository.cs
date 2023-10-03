using CFlashcards.Models;

namespace CFlashcards.DAL
{
    public interface IDeckRepository
    {
        Task<IEnumerable<Deck>?> SearchDecksByTitle(string flashcardsUserId, string title);
        Task<IEnumerable<Deck>?> GetAll(string flashcardsUserId);
        Task<Deck?> GetDeckById(int id);
        Task<bool> Create(Deck deck);
        Task<bool> Update(Deck deck);
        Task<bool> Delete(int id);
    }
}
