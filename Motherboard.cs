using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Management;

namespace SpooferHWID
{
    public static class Motherboard
    {
        public static Dictionary<string, string> GetMotherboardInfo()
        {
            var result = new Dictionary<string, string>();

            try
            {
                // Usar WMI para obter informações detalhadas da placa-mãe
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        result["Manufacturer"] = obj["Manufacturer"]?.ToString() ?? "Unknown";
                        result["Product"] = obj["Product"]?.ToString() ?? "Unknown";
                        result["SerialNumber"] = obj["SerialNumber"]?.ToString() ?? "Unknown";
                        result["Version"] = obj["Version"]?.ToString() ?? "Unknown";

                        // Apenas a primeira placa-mãe
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao acessar WMI para placa-mãe: {ex.Message}");
                }

                // Obter informações do BIOS
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        result["BIOSVendor"] = obj["Manufacturer"]?.ToString() ?? "Unknown";
                        result["BIOSVersion"] = obj["SMBIOSBIOSVersion"]?.ToString() ?? "Unknown";
                        result["BIOSReleaseDate"] = obj["ReleaseDate"]?.ToString() ?? "Unknown";

                        // Apenas a primeira entrada
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao acessar WMI para BIOS: {ex.Message}");
                }

                // Obter informações do registro
                try
                {
                    using (RegistryKey biosKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS"))
                    {
                        if (biosKey != null)
                        {
                            result["Registry_BIOSVendor"] = biosKey.GetValue("BIOSVendor")?.ToString() ?? "Unknown";
                            result["Registry_BIOSVersion"] = biosKey.GetValue("BIOSVersion")?.ToString() ?? "Unknown";
                            result["Registry_BIOSReleaseDate"] = biosKey.GetValue("BIOSReleaseDate")?.ToString() ?? "Unknown";
                            result["Registry_SystemManufacturer"] = biosKey.GetValue("SystemManufacturer")?.ToString() ?? "Unknown";
                            result["Registry_SystemProductName"] = biosKey.GetValue("SystemProductName")?.ToString() ?? "Unknown";
                            result["Registry_BaseBoardManufacturer"] = biosKey.GetValue("BaseBoardManufacturer")?.ToString() ?? "Unknown";
                            result["Registry_BaseBoardProduct"] = biosKey.GetValue("BaseBoardProduct")?.ToString() ?? "Unknown";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao obter informações de placa-mãe do registro: {ex.Message}");
                }

                // Obter informações adicionais do SystemInformation
                try
                {
                    using (RegistryKey systemInfoKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SystemInformation"))
                    {
                        if (systemInfoKey != null)
                        {
                            result["SI_ComputerManufacturer"] = systemInfoKey.GetValue("ComputerManufacturer")?.ToString() ?? "Unknown";
                            result["SI_ComputerModel"] = systemInfoKey.GetValue("ComputerModel")?.ToString() ?? "Unknown";
                            result["SI_SystemManufacturer"] = systemInfoKey.GetValue("SystemManufacturer")?.ToString() ?? "Unknown";
                            result["SI_SystemProductName"] = systemInfoKey.GetValue("SystemProductName")?.ToString() ?? "Unknown";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao obter informações de SystemInformation: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter informações de placa-mãe: {ex.Message}");
            }

            return result;
        }

        public static bool SpoofMotherboard()
        {
            try
            {
                bool anySuccess = false;

                // Modificar informações da placa-mãe no registro
                try
                {
                    using (RegistryKey biosKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS", true))
                    {
                        if (biosKey != null)
                        {
                            // Gerar um fabricante aleatório de placa-mãe
                            string[] manufacturers = { "ASUS", "MSI", "Gigabyte", "ASRock", "EVGA", "Biostar", "Intel" };
                            Random rnd = new Random();
                            string manufacturer = manufacturers[rnd.Next(manufacturers.Length)];

                            // Gerar modelo de placa-mãe baseado no fabricante
                            string model = GenerateMotherboardModel(manufacturer);

                            // Gerar serial number aleatório
                            string serialNumber = Utils.RandomId(12);

                            // Gerar data de lançamento e versão da BIOS
                            int month = rnd.Next(1, 13);
                            int day = rnd.Next(1, 29); // Evitando problemas com meses curtos
                            int year = rnd.Next(2018, 2023);
                            string biosDate = $"{month:D2}/{day:D2}/{year}";
                            string biosVersion = $"{rnd.Next(1, 9)}.{rnd.Next(10, 99)}";

                            // Aplicar as mudanças
                            biosKey.SetValue("BaseBoardManufacturer", manufacturer);
                            biosKey.SetValue("BaseBoardProduct", model);
                            biosKey.SetValue("BaseBoardVersion", $"REV {rnd.Next(1, 9)}.{rnd.Next(0, 9)}");
                            biosKey.SetValue("BIOSVendor", manufacturer);
                            biosKey.SetValue("BIOSVersion", $"{manufacturer} - {rnd.Next(10000, 99999)}");
                            biosKey.SetValue("BIOSReleaseDate", biosDate);

                            anySuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao spoofar placa-mãe via registro: {ex.Message}");
                }

                // Modificar SystemInformation
                try
                {
                    using (RegistryKey systemInfoKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SystemInformation", true))
                    {
                        if (systemInfoKey != null)
                        {
                            // Usar as mesmas informações da modificação anterior
                            using (RegistryKey biosKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS"))
                            {
                                if (biosKey != null)
                                {
                                    string baseBoardManufacturer = biosKey.GetValue("BaseBoardManufacturer")?.ToString() ?? "Unknown";
                                    string baseBoardProduct = biosKey.GetValue("BaseBoardProduct")?.ToString() ?? "Unknown";
                                    string biosVersion = biosKey.GetValue("BIOSVersion")?.ToString() ?? "Unknown";
                                    string biosDate = biosKey.GetValue("BIOSReleaseDate")?.ToString() ?? "Unknown";

                                    // Aplicar no SystemInformation
                                    systemInfoKey.SetValue("ComputerManufacturer", baseBoardManufacturer);
                                    systemInfoKey.SetValue("ComputerModel", baseBoardProduct);
                                    systemInfoKey.SetValue("SystemManufacturer", baseBoardManufacturer);
                                    systemInfoKey.SetValue("SystemProductName", baseBoardProduct);
                                    systemInfoKey.SetValue("BIOSVersion", biosVersion);
                                    systemInfoKey.SetValue("BIOSReleaseDate", biosDate);

                                    anySuccess = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao spoofar SystemInformation: {ex.Message}");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao spoofar placa-mãe: {ex.Message}");
                return false;
            }
        }

        // Gera um modelo realista de placa-mãe baseado no fabricante
        private static string GenerateMotherboardModel(string manufacturer)
        {
            Random rnd = new Random();

            switch (manufacturer.ToUpper())
            {
                case "ASUS":
                    string[] asusPrefixes = { "ROG STRIX ", "TUF GAMING ", "PRIME ", "ProArt " };
                    string[] asusChipsets = { "Z690", "Z590", "B660", "B550", "X570", "H610" };
                    string[] asusSuffixes = { "-A", "-F", "-E", "-I", "-PLUS", " WIFI", " GAMING" };

                    return asusPrefixes[rnd.Next(asusPrefixes.Length)] + asusChipsets[rnd.Next(asusChipsets.Length)] + asusSuffixes[rnd.Next(asusSuffixes.Length)];

                case "MSI":
                    string[] msiPrefixes = { "MPG ", "MAG ", "MEG ", "PRO " };
                    string[] msiChipsets = { "Z690", "Z590", "B660", "B550", "X570", "H610" };
                    string[] msiSuffixes = { " GAMING", " EDGE", " TOMAHAWK", " TORPEDO", " MORTAR", " CARBON" };

                    return msiPrefixes[rnd.Next(msiPrefixes.Length)] + msiChipsets[rnd.Next(msiChipsets.Length)] + msiSuffixes[rnd.Next(msiSuffixes.Length)];

                case "GIGABYTE":
                    string[] gigabytePrefixes = { "AORUS ", "AERO ", "GAMING " };
                    string[] gigabyteChipsets = { "Z690", "Z590", "B660", "B550", "X570", "H610" };
                    string[] gigabyteSuffixes = { " ELITE", " MASTER", " PRO", " ULTRA", " EXTREME", " WIFI" };

                    return gigabytePrefixes[rnd.Next(gigabytePrefixes.Length)] + gigabyteChipsets[rnd.Next(gigabyteChipsets.Length)] + gigabyteSuffixes[rnd.Next(gigabyteSuffixes.Length)];

                case "ASROCK":
                    string[] asrockPrefixes = { "Phantom Gaming ", "Steel Legend ", "Pro ", "Extreme " };
                    string[] asrockChipsets = { "Z690", "Z590", "B660", "B550", "X570", "H610" };
                    string[] asrockSuffixes = { " Wifi", " Gaming", "-F", " Taichi", " Extreme" };

                    return asrockPrefixes[rnd.Next(asrockPrefixes.Length)] + asrockChipsets[rnd.Next(asrockChipsets.Length)] + asrockSuffixes[rnd.Next(asrockSuffixes.Length)];

                default:
                    return $"{manufacturer} MB-{Utils.RandomId(6)}";
            }
        }
    }
}