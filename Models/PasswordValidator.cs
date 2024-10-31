using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SampleSecureWeb.Models
{
    public class PasswordValidator
    {
        // Memeriksa apakah karakter dapat dicetak menggunakan kategori Unicode
        public static bool IsPrintableUnicode(char character)
        {
            var category = Char.GetUnicodeCategory(character);
            return category != System.Globalization.UnicodeCategory.Control &&
                   category != System.Globalization.UnicodeCategory.Format;
        }

        // Memverifikasi bahwa setiap karakter dalam kata sandi dapat dicetak
        public static bool VerifyPassword(string password)
        {
            return password.All(IsPrintableUnicode);
        }
    }
}
