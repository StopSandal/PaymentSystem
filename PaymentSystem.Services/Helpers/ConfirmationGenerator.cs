using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Services.Helpers
{
    /// <summary>
    /// Class that realizing interface <see cref="IConfirmationGenerator"/>
    /// </summary>
    public class ConfirmationGenerator : IConfirmationGenerator
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        /// <inheritdoc/>
        /// Generate code from uppercase letters and digits
        public async Task<string> GenerateConfirmationCodeAsync()
        {
            var _random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
