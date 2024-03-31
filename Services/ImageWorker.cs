namespace DiplomService.Services
{
    public class ImageWorker
    {
        public static string GetImageMimeType(byte[] PriviewImage)
        {
            if (PriviewImage == null) return "application/octet-stream";
            if (PriviewImage.Length < 4)
            {
                return "application/octet-stream";
            }

            if (PriviewImage[0] == 0xFF && PriviewImage[1] == 0xD8 && PriviewImage[2] == 0xFF)
            {
                return "image/jpeg";
            }
            else if (PriviewImage[0] == 0x89 && PriviewImage[1] == 0x50 && PriviewImage[2] == 0x4E && PriviewImage[3] == 0x47)
            {
                return "image/png";
            }

            return "application/octet-stream"; 
        }
    }
}
