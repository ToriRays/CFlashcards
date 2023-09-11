using CFlashcards.Models;

namespace CFlashcards.DAL
{
    public interface IDeckRepository
    {
        Task<IEnumerable<Deck>?> GetAll();
        Task<Deck?> GetDeckById(int id);
        Task<bool> Create(Deck deck);
        Task<bool> Update(Deck deck);
        Task<bool> Delete(int id);
    }
}
