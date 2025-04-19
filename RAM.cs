using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Management;

namespace SpooferHWID
{
    public static class RAM
    {
        public static Dictionary<string, string> GetRAMInfo()
        {
            var result = new Dictionary<string, string>();

            try
            {
                // Usar WMI para obter informações detalhadas da RAM
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
                    int index = 0;

                    foreach (ManagementObject obj in searcher.Get())
                    {
                        result[$"Manufacturer_{index}"] = obj["Manufacturer"]?.ToString() ?? "Unknown";
                        result[$"PartNumber_{index}"] = obj["PartNumber"]?.ToString() ?? "Unknown";
                        result[$"SerialNumber_{index}"] = obj["SerialNumber"]?.ToString() ?? "Unknown";
                        result[$"Capacity_{index}"] = obj["Capacity"]?.ToString() ?? "Unknown";
                        result[$"Speed_{index}"] = obj["Speed"]?.ToString() ?? "Unknown";
                        result[$"DeviceLocator_{index}"] = obj["DeviceLocator"]?.ToString() ?? "Unknown";
                        result[$"ConfiguredClockSpeed_{index}"] = obj["ConfiguredClockSpeed"]?.ToString() ?? "Unknown";

                        index++;
                    }

                    result["ModuleCount"] = index.ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao acessar WMI para RAM: {ex.Message}");
                }

                // Obter informações do registro
                try
                {
                    using (RegistryKey memKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0"))
                    {
                        if (memKey != null)
                        {
                            // ~MHz é a velocidade da RAM em MHz
                            if (memKey.GetValue("~MHz") != null)
                                result["Registry_MHz"] = memKey.GetValue("~MHz").ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao obter informações de RAM do registro: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter informações de RAM: {ex.Message}");
            }

            return result;
        }

        public static bool SpoofRAM()
        {
            try
            {
                bool anySuccess = false;

                // Modificar informações de RAM no registro
                // Nota: A modificação real de identificadores de RAM é muito limitada a partir do modo usuário
                // e muitas informações são lidas diretamente do hardware. O código abaixo é principalmente
                // para demonstração conceitual.

                // Criar uma chave de registro personalizada para simular informações de RAM modificadas
                try
                {
                    using (RegistryKey spooferKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\SpooferHWID\RAM"))
                    {
                        if (spooferKey != null)
                        {
                            // Gerar fabricantes comuns de RAM
                            string[] manufacturers = { "Kingston", "Corsair", "G.Skill", "Crucial", "Samsung", "HyperX", "ADATA", "TeamGroup" };
                            Random rnd = new Random();

                            int moduleCount = rnd.Next(2, 5); // 2 a 4 módulos de RAM

                            for (int i = 0; i < moduleCount; i++)
                            {
                                string manufacturer = manufacturers[rnd.Next(0, manufacturers.Length)];
                                string serialNumber = Utils.RandomId(16);
                                int capacity = 8 * (int)Math.Pow(2, rnd.Next(0, 3)); // 8, 16, 32 GB
                                int speed = (rnd.Next(24, 37) * 100) + (rnd.Next(0, 2) * 66); // 2400, 2666, 3000, 3200, 3600 MHz

                                spooferKey.SetValue($"Module_{i}_Manufacturer", manufacturer);
                                spooferKey.SetValue($"Module_{i}_SerialNumber", serialNumber);
                                spooferKey.SetValue($"Module_{i}_Capacity", capacity.ToString());
                                spooferKey.SetValue($"Module_{i}_Speed", speed.ToString());
                            }

                            spooferKey.SetValue("ModuleCount", moduleCount.ToString());
                            anySuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao criar chave de simulação de RAM: {ex.Message}");
                }

                // Nota: Para um verdadeiro spoofing de RAM, seria necessário um driver de modo kernel
                // que intercepta chamadas de sistema e WMI para retornar identificadores falsos

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao spoofar RAM: {ex.Message}");
                return false;
            }
        }

        // Retorna fabricantes comuns de RAM para simulação
        public static string[] GetCommonRAMManufacturers()
        {
            return new string[] {
                "Kingston", "Corsair", "G.Skill", "Crucial", "Samsung",
                "HyperX", "ADATA", "TeamGroup", "Patriot", "Mushkin",
                "PNY", "Micron", "Hynix", "Apacer", "Transcend"
            };
        }

        // Gera um número de parte realista de RAM
        public static string GenerateRealisticPartNumber(string manufacturer)
        {
            Random rnd = new Random();

            switch (manufacturer.ToLower())
            {
                case "kingston":
                    return $"KHX{Utils.RandomHex(4)}C{rnd.Next(9, 18)}/{rnd.Next(8, 33)}";

                case "corsair":
                    return $"CMK{rnd.Next(8, 33)}GX{rnd.Next(3, 5)}M{Utils.RandomHex(1)}";

                case "g.skill":
                    return $"F4-{rnd.Next(2400, 4000)}-{Utils.RandomHex(4)}-{rnd.Next(8, 33)}G";

                case "crucial":
                    return $"CT{rnd.Next(8, 33)}G{rnd.Next(3, 5)}M{Utils.RandomHex(2)}";

                case "samsung":
                    return $"M{Utils.RandomHex(2)}{Utils.RandomHex(2)}-{Utils.RandomHex(3)}-{rnd.Next(8, 33)}G";

                default:
                    return $"RAM-{Utils.RandomHex(6)}-{rnd.Next(8, 33)}GB";
            }
        }
    }
}