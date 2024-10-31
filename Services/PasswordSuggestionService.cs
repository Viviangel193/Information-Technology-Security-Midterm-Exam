using System;
using System.Text;

namespace SampleSecureWeb.Services
{
    public class PasswordSuggestionService
    {
        private static readonly char[] AllowedChars = 
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-+=<>?{}[]~".ToCharArray();

        private static readonly string[] Emojis = 
        {
            "😊", "😂", "😍", "🤔", "😎", "😜", "🤩", "🥳", "😇", "🤯", "🥺", "💪"
        };

        public string GenerateStrongPassword(int length = 12)
        {
            var random = new Random();
            var builder = new StringBuilder();

            // Tambahkan karakter acak untuk memenuhi panjang password yang diinginkan
            for (int i = 0; i < length - 2; i++) // -2 untuk menyisakan tempat bagi emoji
            {
                var randomChar = AllowedChars[random.Next(AllowedChars.Length)];
                builder.Append(randomChar);
            }

            // Tambahkan dua emoji acak
            builder.Append(Emojis[random.Next(Emojis.Length)]);
            builder.Append(Emojis[random.Next(Emojis.Length)]);

            return builder.ToString();
        }
    }
}
