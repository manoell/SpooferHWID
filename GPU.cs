using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Management;

namespace SpooferHWID
{
    public static class GPU
    {
        public static Dictionary<string, string> GetGPUInfo()
        {
            var result = new Dictionary<string, string>();

            try
            {
                // Usar WMI para obter informações da GPU
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        result["Name"] = obj["Name"]?.ToString() ?? "Not Available";
                        result["DeviceID"] = obj["DeviceID"]?.ToString() ?? "Not Available";
                        result["DriverVersion"] = obj["DriverVersion"]?.ToString() ?? "Not Available";
                        result["VideoProcessor"] = obj["VideoProcessor"]?.ToString() ?? "Not Available";
                        result["AdapterRAM"] = obj["AdapterRAM"]?.ToString() ?? "Not Available";

                        // Interrompe após a primeira GPU para simplicidade
                        // Para múltiplas GPUs, você poderia adicionar um índice ao nome da chave
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao acessar WMI para GPU: {ex.Message}");
                }

                // Tentar obter informações específicas para NVIDIA
                try
                {
                    string nvRegPath = @"SYSTEM\CurrentControlSet\Enum\PCI";
                    using (RegistryKey pciKey = Registry.LocalMachine.OpenSubKey(nvRegPath))
                    {
                        if (pciKey != null)
                        {
                            foreach (string subKeyName in pciKey.GetSubKeyNames())
                            {
                                if (subKeyName.StartsWith("VEN_10DE")) // 10DE é o Vendor ID da NVIDIA
                                {
                                    using (RegistryKey vendorKey = pciKey.OpenSubKey(subKeyName))
                                    {
                                        if (vendorKey != null)
                                        {
                                            foreach (string deviceKey in vendorKey.GetSubKeyNames())
                                            {
                                                using (RegistryKey gpuKey = vendorKey.OpenSubKey(deviceKey))
                                                {
                                                    if (gpuKey != null && gpuKey.GetValue("HardwareID") != null)
                                                    {
                                                        result["NVIDIA_HardwareID"] = gpuKey.GetValue("HardwareID").ToString();
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                // Verificar também para AMD (1002)
                                if (subKeyName.StartsWith("VEN_1002"))
                                {
                                    using (RegistryKey vendorKey = pciKey.OpenSubKey(subKeyName))
                                    {
                                        if (vendorKey != null)
                                        {
                                            foreach (string deviceKey in vendorKey.GetSubKeyNames())
                                            {
                                                using (RegistryKey gpuKey = vendorKey.OpenSubKey(deviceKey))
                                                {
                                                    if (gpuKey != null && gpuKey.GetValue("HardwareID") != null)
                                                    {
                                                        result["AMD_HardwareID"] = gpuKey.GetValue("HardwareID").ToString();
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                // Verificar também para Intel (8086)
                                if (subKeyName.StartsWith("VEN_8086"))
                                {
                                    using (RegistryKey vendorKey = pciKey.OpenSubKey(subKeyName))
                                    {
                                        if (vendorKey != null)
                                        {
                                            foreach (string deviceKey in vendorKey.GetSubKeyNames())
                                            {
                                                using (RegistryKey gpuKey = vendorKey.OpenSubKey(deviceKey))
                                                {
                                                    if (gpuKey != null && gpuKey.GetValue("HardwareID") != null)
                                                    {
                                                        result["Intel_HardwareID"] = gpuKey.GetValue("HardwareID").ToString();
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao obter informações de GPU do registro: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter informações de GPU: {ex.Message}");
            }

            return result;
        }

        public static bool SpoofGPU()
        {
            try
            {
                bool anySuccess = false;

                // Tentar spoofar HardwareID para placas NVIDIA
                try
                {
                    string nvRegPath = @"SYSTEM\CurrentControlSet\Enum\PCI";
                    using (RegistryKey pciKey = Registry.LocalMachine.OpenSubKey(nvRegPath))
                    {
                        if (pciKey != null)
                        {
                            foreach (string subKeyName in pciKey.GetSubKeyNames())
                            {
                                if (subKeyName.StartsWith("VEN_10DE")) // 10DE é o Vendor ID da NVIDIA
                                {
                                    using (RegistryKey vendorKey = pciKey.OpenSubKey(subKeyName))
                                    {
                                        if (vendorKey != null)
                                        {
                                            foreach (string deviceKey in vendorKey.GetSubKeyNames())
                                            {
                                                using (RegistryKey gpuKey = vendorKey.OpenSubKey(deviceKey, true))
                                                {
                                                    if (gpuKey != null)
                                                    {
                                                        // Gerar um novo HardwareID aleatório (mantendo o formato NVIDIA)
                                                        string newHardwareID = $"PCI\\VEN_10DE&DEV_{Utils.RandomHex(4)}&SUBSYS_{Utils.RandomHex(8)}&REV_{Utils.RandomHex(2)}";

                                                        // Tentar modificar o HardwareID
                                                        gpuKey.SetValue("HardwareID", newHardwareID);
                                                        gpuKey.SetValue("CompatibleIDs", new string[] { newHardwareID });

                                                        anySuccess = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                // Fazer o mesmo para AMD (1002)
                                if (subKeyName.StartsWith("VEN_1002"))
                                {
                                    using (RegistryKey vendorKey = pciKey.OpenSubKey(subKeyName))
                                    {
                                        if (vendorKey != null)
                                        {
                                            foreach (string deviceKey in vendorKey.GetSubKeyNames())
                                            {
                                                using (RegistryKey gpuKey = vendorKey.OpenSubKey(deviceKey, true))
                                                {
                                                    if (gpuKey != null)
                                                    {
                                                        // Gerar um novo HardwareID aleatório (mantendo o formato AMD)
                                                        string newHardwareID = $"PCI\\VEN_1002&DEV_{Utils.RandomHex(4)}&SUBSYS_{Utils.RandomHex(8)}&REV_{Utils.RandomHex(2)}";

                                                        // Tentar modificar o HardwareID
                                                        gpuKey.SetValue("HardwareID", newHardwareID);
                                                        gpuKey.SetValue("CompatibleIDs", new string[] { newHardwareID });

                                                        anySuccess = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                // Fazer o mesmo para Intel (8086)
                                if (subKeyName.StartsWith("VEN_8086"))
                                {
                                    using (RegistryKey vendorKey = pciKey.OpenSubKey(subKeyName))
                                    {
                                        if (vendorKey != null)
                                        {
                                            foreach (string deviceKey in vendorKey.GetSubKeyNames())
                                            {
                                                using (RegistryKey gpuKey = vendorKey.OpenSubKey(deviceKey, true))
                                                {
                                                    if (gpuKey != null)
                                                    {
                                                        // Gerar um novo HardwareID aleatório (mantendo o formato Intel)
                                                        string newHardwareID = $"PCI\\VEN_8086&DEV_{Utils.RandomHex(4)}&SUBSYS_{Utils.RandomHex(8)}&REV_{Utils.RandomHex(2)}";

                                                        // Tentar modificar o HardwareID
                                                        gpuKey.SetValue("HardwareID", newHardwareID);
                                                        gpuKey.SetValue("CompatibleIDs", new string[] { newHardwareID });

                                                        anySuccess = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao spoofar GPU via registro: {ex.Message}");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao spoofar GPU: {ex.Message}");
                return false;
            }
        }
    }
}