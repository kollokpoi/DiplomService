namespace DiplomService.Services
{
    public class PhoneWorker
    {

        public static string NormalizePhone(string phoneNumber)
        {

            string digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());


            if (digitsOnly.Length == 11 && digitsOnly[0] == '7')
            {
                return "8" + digitsOnly.Substring(1);
            }

            return digitsOnly;
        }
    }
}
