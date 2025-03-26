using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Semester6_DiceCode4.Controllers
{
    public class ASPER_02Controller : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly byte[] encryptionKey = Encoding.UTF8.GetBytes("12345678901234567890123456789012"); // 32-byte sleutel voor AES

        public ASPER_02Controller(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View(new PersonViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPersonData(PersonViewModel person)
        {
            try
            {
                if (string.IsNullOrEmpty(person.CreditCardNumber))
                {
                    ViewBag.ErrorMessage = "Creditcardnummer is vereist!";
                    return View("Index");
                }

                // Encrypt het creditcardnummer
                byte[] encryptedCreditCard = EncryptCreditCardNumber(person.CreditCardNumber);

                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
                        INSERT INTO Person (FirstName, LastName, Street, HouseNumber, PostalCode, City, EncryptedCreditCardNumber)
                        VALUES (@FirstName, @LastName, @Street, @HouseNumber, @PostalCode, @City, @EncryptedCreditCardNumber)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", person.FirstName);
                        command.Parameters.AddWithValue("@LastName", person.LastName);
                        command.Parameters.AddWithValue("@Street", person.Street);
                        command.Parameters.AddWithValue("@HouseNumber", person.HouseNumber);
                        command.Parameters.AddWithValue("@PostalCode", person.PostalCode);
                        command.Parameters.AddWithValue("@City", person.City);
                        command.Parameters.AddWithValue("@EncryptedCreditCardNumber", encryptedCreditCard);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Er is een fout opgetreden: " + ex.Message;
                return View("Error");
            }
        }

        private byte[] EncryptCreditCardNumber(string creditCardNumber)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = encryptionKey;
                aes.GenerateIV(); // Unieke IV voor elke encryptie
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    byte[] creditCardBytes = Encoding.UTF8.GetBytes(creditCardNumber);
                    byte[] encryptedData = encryptor.TransformFinalBlock(creditCardBytes, 0, creditCardBytes.Length);

                    // Combineer IV + versleutelde data voor opslag
                    byte[] result = new byte[aes.IV.Length + encryptedData.Length];
                    Array.Copy(aes.IV, 0, result, 0, aes.IV.Length);
                    Array.Copy(encryptedData, 0, result, aes.IV.Length, encryptedData.Length);

                    return result;
                }
            }
        }
    }
}
