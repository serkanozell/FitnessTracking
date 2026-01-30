namespace Exercises.Application.Services
{

    /// <summary>
    /// Sadece Modüller arası iletişim için kullanılır, dışarıya açılmamalıdır.
    /// </summary>
    public interface IExerciseReadService
    {
        Task<IReadOnlyDictionary<Guid, string>> GetNamesByIdsAsync(CancellationToken cancellationToken = default);
    }
}