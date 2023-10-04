using Microsoft.AspNetCore.DataProtection;

namespace LojaDeGames.Security
{
    public class Settings
    {
        private static string secret = "4d15596cab81d19a44346923f95b2191c3b5cfa2ee77dd8b2906f35540b0aeca";
        public static string Secret { get => secret; set => secret = value; }
    }
}
