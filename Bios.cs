using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Management;

namespace SpooferHWID
{
    public static class Bios
    {
        public static Dictionary<string, string> GetBiosInfo()
        {
            var result = new Dictionary<string, string>();

            try
            {
                // Obter informações da BIOS através do WMI
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        result["SerialNumber"] = obj["SerialNumber"]?.ToString() ?? "Not Available";
                        result["Manufacturer"] = obj["Manufacturer"]?.ToString() ?? "Not Available";
                        result["Version"] = obj["Version"]?.ToString() ?? "Not Available";
                        result["ReleaseDate"] = obj["ReleaseDate"]?.ToString() ?? "Not Available";

                        // Apenas a primeira entrada
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao acessar WMI para BIOS: {ex.Message}");
                }

                // Obter informações do registro também
                using (RegistryKey biosKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS"))
                {
                    if (biosKey != null)
                    {
                        result["Registry_BIOSVendor"] = biosKey.GetValue("BIOSVendor")?.ToString() ?? "Not Available";
                        result["Registry_BIOSVersion"] = biosKey.GetValue("BIOSVersion")?.ToString() ?? "Not Available";
                        result["Registry_SystemManufacturer"] = biosKey.GetValue("SystemManufacturer")?.ToString() ?? "Not Available";
                        result["Registry_SystemProductName"] = biosKey.GetValue("SystemProductName")?.ToString() ?? "Not Available";
                        result["Registry_SystemFamily"] = biosKey.GetValue("SystemFamily")?.ToString() ?? "Not Available";
                        result["Registry_SystemVersion"] = biosKey.GetValue("SystemVersion")?.ToString() ?? "Not Available";
                        result["Registry_SystemSKU"] = biosKey.GetValue("SystemSKU")?.ToString() ?? "Not Available";
                        result["Registry_BaseBoardManufacturer"] = biosKey.GetValue("BaseBoardManufacturer")?.ToString() ?? "Not Available";
                        result["Registry_BaseBoardProduct"] = biosKey.GetValue("BaseBoardProduct")?.ToString() ?? "Not Available";
                        result["Registry_BaseBoardVersion"] = biosKey.GetValue("BaseBoardVersion")?.ToString() ?? "Not Available";
                    }
                }

                // Verificar informações do SystemInformation
                using (RegistryKey systemInfoKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SystemInformation"))
                {
                    if (systemInfoKey != null)
                    {
                        result["BIOSReleaseDate"] = systemInfoKey.GetValue("BIOSReleaseDate")?.ToString() ?? "Not Available";
                        result["BIOSVersion"] = systemInfoKey.GetValue("BIOSVersion")?.ToString() ?? "Not Available";
                        result["ComputerHardwareId"] = systemInfoKey.GetValue("ComputerHardwareId")?.ToString() ?? "Not Available";
                        result["ComputerManufacturer"] = systemInfoKey.GetValue("ComputerManufacturer")?.ToString() ?? "Not Available";
                        result["ComputerModel"] = systemInfoKey.GetValue("ComputerModel")?.ToString() ?? "Not Available";
                        result["SystemManufacturer"] = systemInfoKey.GetValue("SystemManufacturer")?.ToString() ?? "Not Available";
                        result["SystemProductName"] = systemInfoKey.GetValue("SystemProductName")?.ToString() ?? "Not Available";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter informações da BIOS: {ex.Message}");
            }

            return result;
        }

        public static bool SpoofBIOS()
        {
            try
            {
                bool anySuccess = false;

                // Modificar informações da BIOS no registro
                try
                {
                    using (RegistryKey biosKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS", true))
                    {
                        if (biosKey != null)
                        {
                            // Gerar um número de série aleatório
                            string serialNumber = Utils.RandomId(10);

                            // Tentar modificar SystemSerialNumber
                            if (biosKey.GetValue("SystemSerialNumber") != null)
                            {
                                biosKey.SetValue("SystemSerialNumber", serialNumber);
                                anySuccess = true;
                            }

                            // Gerar outros valores aleatórios
                            string manufacturer = Utils.RandomManufacturer();
                            string productName = Utils.RandomProductName();

                            // Modificar outros valores
                            biosKey.SetValue("BIOSVendor", manufacturer);
                            biosKey.SetValue("SystemManufacturer", manufacturer);
                            biosKey.SetValue("SystemProductName", productName);
                            biosKey.SetValue("BaseBoardManufacturer", manufacturer);
                            biosKey.SetValue("BaseBoardProduct", Utils.RandomId(8));

                            anySuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao spoofar BIOS via registro: {ex.Message}");
                }

                // Modificar informações do SystemInformation
                try
                {
                    using (RegistryKey systemInfoKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SystemInformation", true))
                    {
                        if (systemInfoKey != null)
                        {
                            // Gerar data aleatória para BIOS
                            Random rnd = new Random();
                            int day = rnd.Next(1, 29); // Evitando 29-31 para lidar com fevereiro
                            string dayStr = day < 10 ? $"0{day}" : day.ToString();

                            int month = rnd.Next(1, 13);
                            string monthStr = month < 10 ? $"0{month}" : month.ToString();

                            int year = rnd.Next(2015, 2022);

                            // Modificar data de lançamento da BIOS
                            systemInfoKey.SetValue("BIOSReleaseDate", $"{monthStr}/{dayStr}/{year}");

                            // Modificar versão da BIOS
                            systemInfoKey.SetValue("BIOSVersion", Utils.RandomId(10));

                            // Modificar Hardware ID
                            systemInfoKey.SetValue("ComputerHardwareId", $"{{{Guid.NewGuid()}}}");

                            // Modificar Hardware IDs
                            string hardwareIds = $"{{{Guid.NewGuid()}}}\n{{{Guid.NewGuid()}}}\n{{{Guid.NewGuid()}}}\n";
                            systemInfoKey.SetValue("ComputerHardwareIds", hardwareIds);

                            // Modificar fabricante
                            string manufacturer = Utils.RandomManufacturer();
                            systemInfoKey.SetValue("ComputerManufacturer", manufacturer);
                            systemInfoKey.SetValue("SystemManufacturer", manufacturer);

                            // Modificar modelo
                            string productName = Utils.RandomProductName();
                            systemInfoKey.SetValue("ComputerModel", productName);
                            systemInfoKey.SetValue("SystemProductName", productName);

                            anySuccess = true;
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
                Console.WriteLine($"Erro ao spoofar BIOS: {ex.Message}");
                return false;
            }
        }
    }
}