using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DiplomService.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class RussianPhoneNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return true; // Разрешаем пустое значение
            string? phoneNumber = value.ToString();

            // Удаляем все символы, кроме цифр
            string normalizedPhoneNumber = Regex.Replace(phoneNumber, @"[^\d]", "");

            // Проверяем, что номер состоит из 11 цифр и начинается с 7 или 8
            if (normalizedPhoneNumber.Length != 11) return false;
            if (normalizedPhoneNumber[0] != '7' && normalizedPhoneNumber[0] != '8') return false;

            // Нормализуем номер телефона в требуемый формат
            string formattedPhoneNumber = $"+7 ({normalizedPhoneNumber.Substring(1, 3)}) {normalizedPhoneNumber.Substring(4, 3)}-{normalizedPhoneNumber.Substring(7, 2)}-{normalizedPhoneNumber.Substring(9, 2)}";

            // Заменяем входное значение на отформатированный номер
            value = formattedPhoneNumber;

            return true;
        }
    }
}
