public class PersonViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }

    public string CreditCardNumber { get; set; }
    public byte[] EncryptedCreditCardNumber { get; set; }
}
