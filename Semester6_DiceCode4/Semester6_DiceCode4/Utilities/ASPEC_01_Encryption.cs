using System;
using System.Text;

namespace Semester6_DiceCode4.Utilities
{
    public static class ASPEC_01_Encryption
    {
        private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 !@#$%^&*()_+-=[]{}|;:',.<>?/\"\\";
        private const int MaxKeyLength = 100; // Beveiliging tegen te grote keys

        public static string Encrypt(string text, int key)
        {
            ValidateInput(text, key);

            var encryptedText = new StringBuilder(text.Length);
            int effectiveKey = key % Characters.Length;

            foreach (var character in text)
            {
                encryptedText.Append(ProcessCharacter(character, effectiveKey, encrypt: true));
            }

            return encryptedText.ToString();
        }

        public static string Decrypt(string encryptedText, int key)
        {
            ValidateInput(encryptedText, key);

            var decryptedText = new StringBuilder(encryptedText.Length);
            int effectiveKey = key % Characters.Length;

            foreach (var character in encryptedText)
            {
                decryptedText.Append(ProcessCharacter(character, effectiveKey, encrypt: false));
            }

            return decryptedText.ToString();
        }

        private static char ProcessCharacter(char character, int key, bool encrypt)
        {
            int index = Characters.IndexOf(character);
            if (index < 0) return character;

            int newIndex = encrypt
                ? (index + key) % Characters.Length
                : (index - key + Characters.Length) % Characters.Length;

            return Characters[newIndex];
        }

        private static void ValidateInput(string text, int key)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text), "Text cannot be null or empty");

            if (key <= 0 || key > MaxKeyLength)
                throw new ArgumentOutOfRangeException(nameof(key), $"Key must be between 1 and {MaxKeyLength}");
        }
    }
}