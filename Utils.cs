using System;
using System.Text;

namespace SpooferHWID
{
    public static class Utils
    {
        private static readonly Random random = new Random();

        // Gera um ID aleatório com o comprimento especificado
        public static string RandomId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }

        // Gera um número hexadecimal aleatório com o comprimento especificado
        public static string RandomHex(int length)
        {
            const string chars = "0123456789ABCDEF";
            StringBuilder result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }

        // Gera um endereço MAC aleatório
        public static string RandomMac()
        {
            // Caracteres permitidos para MAC
            const string chars = "0123456789ABCDEF";

            // Caracteres permitidos para o segundo dígito (garante um MAC não-multicast e localmente administrado)
            const string secondChars = "26AE";

            StringBuilder result = new StringBuilder(17); // 12 caracteres + 5 separadores

            // Primeiro octeto (XX:xx:xx:xx:xx:xx)
            result.Append(chars[random.Next(chars.Length)]);
            result.Append(secondChars[random.Next(secondChars.Length)]);

            // Adicionar os outros 5 octetos (xx:XX:XX:XX:XX:XX)
            for (int i = 0; i < 5; i++)
            {
                result.Append('-');
                result.Append(chars[random.Next(chars.Length)]);
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }

        // Gera um ProcessorId aleatório que pareça realista
        public static string GenerateProcessorId()
        {
            // Formato comum de ProcessorId: "BFEBFBFF000906EA"
            // Os primeiros 8 caracteres geralmente representam a família/modelo/stepping
            // Os últimos 8 caracteres são variáveis

            return $"BFEBFBFF{RandomHex(8)}";
        }

        // Gera um número aleatório dentro de um intervalo
        public static int RandomNumber(int min, int max)
        {
            return random.Next(min, max + 1);
        }

        // Gera um fabricante aleatório para hardware
        public static string RandomManufacturer()
        {
            string[] manufacturers = {
                "ASUSTeK COMPUTER INC.",
                "Dell Inc.",
                "Gigabyte Technology Co., Ltd.",
                "Micro-Star International Co., Ltd.",
                "HP Inc.",
                "Lenovo",
                "Acer Inc.",
                "ASRock Inc.",
                "Intel Corporation",
                "Samsung Electronics Co., Ltd.",
                "Apple Inc."
            };

            return manufacturers[random.Next(manufacturers.Length)];
        }

        // Gera um nome de produto aleatório
        public static string RandomProductName()
        {
            string[] products = {
                "ROG STRIX B550-F GAMING",
                "TUF GAMING X570-PLUS",
                "Z690 AORUS ELITE",
                "B550M MORTAR",
                "X570 GAMING X",
                "ROG MAXIMUS XIII HERO",
                "B660 TORPEDO",
                "PRIME Z590-A",
                "MPG Z590 GAMING EDGE",
                "X570 PHANTOM GAMING 4"
            };

            return products[random.Next(products.Length)];
        }

        // Gera um UUID aleatório (formato RFC4122)
        public static string RandomUUID()
        {
            return Guid.NewGuid().ToString();
        }

        // Verifica se o processo está sendo executado como administrador
        public static bool IsRunningAsAdmin()
        {
            using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
        }

        // Converte bytes para uma string hexadecimal
        public static string BytesToHex(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:X2}", b);
            }
            return hex.ToString();
        }

        // Converte uma string hexadecimal para bytes
        public static byte[] HexToBytes(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
    }
}