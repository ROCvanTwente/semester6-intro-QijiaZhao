using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace CreditCardEncryptionApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Stap 1: Laat de gebruiker het creditcardnummer invoeren
            Console.WriteLine("Voer het creditcardnummer in:");
            string creditCardNumber = Console.ReadLine();

            // Stap 2: Laat de gebruiker het wachtwoord invoeren voor versleuteling
            Console.WriteLine("Voer het wachtwoord in voor versleuteling:");
            string password = Console.ReadLine();

            // Stap 3: Genereer de sleutel van het wachtwoord
            string encryptionKey = GenerateKeyFromPassword(password);

            // Stap 4: Versleutel het creditcardnummer met de gegenereerde sleutel
            string encryptedData = EncryptCreditCardNumber(creditCardNumber, encryptionKey);

            // Stap 5: Toon het resultaat
            Console.WriteLine("Origineel CreditCard Nummer: " + creditCardNumber);
            Console.WriteLine("Versleuteld CreditCard Nummer (in Base64): " + encryptedData);
        }

        // Functie om een AES-sleutel te genereren van het wachtwoord
        public static string GenerateKeyFromPassword(string password)
        {
            // Gebruik SHA-256 om een sleutel te genereren van het wachtwoord
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(key); // Return de sleutel als een Base64-string
            }
        }

        // Functie om het creditcardnummer te versleutelen met AES
        public static string EncryptCreditCardNumber(string creditCardNumber, string encryptionKey)
        {
            try
            {
                // Creëer een nieuw AES-object
                using (Aes aesAlg = Aes.Create())
                {
                    // Zet de gegenereerde sleutel om naar bytes en stel deze in als de AES sleutel
                    aesAlg.Key = Convert.FromBase64String(encryptionKey);

                    // Genereer een willekeurige IV (Initialisatie Vector)
                    aesAlg.GenerateIV();

                    // Maak een encryptor object aan met de sleutel en IV
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Zet het creditcardnummer om naar bytes
                    byte[] creditCardBytes = Encoding.UTF8.GetBytes(creditCardNumber);

                    // Versleutel de data
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(creditCardBytes, 0, creditCardBytes.Length);

                    // Combineer de IV met de versleutelde data
                    byte[] result = new byte[aesAlg.IV.Length + encryptedBytes.Length];
                    aesAlg.IV.CopyTo(result, 0);
                    encryptedBytes.CopyTo(result, aesAlg.IV.Length);

                    // Converteer het resultaat naar Base64 en return dit als de versleutelde versie van het creditcardnummer
                    return Convert.ToBase64String(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Er is een fout opgetreden bij het versleutelen van de data: " + ex.Message);
                return null;
            }
        }
    }
}
