using MainSms;

namespace DiplomService.Services
{
    public class SmsService
    {
        private static readonly string project_id = "diplom_proj";
        private static readonly string api_key = "739eca854e9c8a9128776bb4577ecd91";

        public static void SendSms(string toNumber, string message)
        {
            SmsMessage sms = new SmsMessage(project_id, api_key);
            ResponseBalance responseBalance = sms.getBalance();
            if (responseBalance.status == "success") Console.WriteLine(responseBalance.balance);
            else Console.WriteLine("Error - " + responseBalance.message);

            ResponsePrice responsePrice = sms.getMessagesPrice("PrograMath ", toNumber, message);
            if (responsePrice.status == "success") Console.WriteLine($"Частей в одной смс {responsePrice.parts}, всего частей {responsePrice.count}, стоимость отправки {responsePrice.price}");
            else Console.WriteLine("Error - " + responsePrice.message);

            ResponseSend responseSend = sms.sendSms("PrograMath ", toNumber, message);
            if (responseSend.status == "success") Console.WriteLine($"Частей в одной смс {responseSend.parts}, всего частей {responseSend.count}, стоимость отправки {responseSend.price}");
            else Console.WriteLine("Error - " + responseSend.message);
        }
    }
}
