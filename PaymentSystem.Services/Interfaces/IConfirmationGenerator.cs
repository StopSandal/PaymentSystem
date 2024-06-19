
namespace PaymentSystem.Services.Interfaces
{
    /// <summary>
    /// Interface for generating confirmation code.
    /// </summary>
    public interface IConfirmationGenerator
    {
        /// <summary>
        /// Generates a new confirmation code asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing the generated confirmation code as a string.</returns>
        Task<string> GenerateConfirmationCodeAsync();
    }
}
