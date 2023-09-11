using CFlashcards.Models;

namespace CFlashcards.DAL
{
    public interface IFlashcardRepository
    {
        Task<IEnumerable<Flashcard>?> GetAll();
        Task<Flashcard?> GetFlashcardById(int id);
        Task<bool> Create(Flashcard flashcard);
        Task<bool> Update(Flashcard flashcard);
        Task<bool> Delete(int id);
    }
}
