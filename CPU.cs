using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Management;

namespace SpooferHWID
{
    public static class CPU
    {
        public static Dictionary<string, string> GetCPUInfo()
        {
            var result = new Dictionary<string, string>();

            try
            {
                // Coletar informações do CPU do Registry
                using (RegistryKey processorKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0"))
                {
                    if (processorKey != null)
                    {
                        if (processorKey.GetValue("ProcessorNameString") != null)
                            result["ProcessorName"] = processorKey.GetValue("ProcessorNameString").ToString();

                        if (processorKey.GetValue("Identifier") != null)
                            result["Identifier"] = processorKey.GetValue("Identifier").ToString();

                        if (processorKey.GetValue("VendorIdentifier") != null)
                            result["VendorIdentifier"] = processorKey.GetValue("VendorIdentifier").ToString();

                        if (processorKey.GetValue("ProcessorId") != null)
                            result["ProcessorId"] = processorKey.GetValue("ProcessorId").ToString();
                    }
                }

                // Usar WMI para obter informações adicionais do CPU
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        result["SerialNumber"] = obj["ProcessorId"]?.ToString() ?? "Not Available";
                        result["Manufacturer"] = obj["Manufacturer"]?.ToString() ?? "Not Available";
                        result["MaxClockSpeed"] = obj["MaxClockSpeed"]?.ToString() ?? "Not Available";
                        result["NumberOfCores"] = obj["NumberOfCores"]?.ToString() ?? "Not Available";

                        // Apenas a primeira CPU (no caso de múltiplos CPUs)
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao acessar WMI para CPU: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter informações de CPU: {ex.Message}");
            }

            return result;
        }

        public static bool SpoofCPU()
        {
            try
            {
                // Spoofing do ProcessorId (vai funcionar apenas em nível de registro)
                using (RegistryKey processorKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0", true))
                {
                    if (processorKey != null)
                    {
                        // Gerar um novo ProcessorId aleatório que pareça realista
                        string newProcessorId = Utils.GenerateProcessorId();

                        // Modificar o ProcessorId no registro
                        // Nota: Isso não vai alterar o verdadeiro ID de CPU no nível de hardware
                        processorKey.SetValue("ProcessorId", newProcessorId);

                        // Em sistemas reais, o ProcessorId é protegio e normalmente não pode ser modificado
                        // Este método demonstra a técnica, mas é mais para fins educacionais
                    }
                }

                // Para spoofing de CPU mais avançado, seria necessário usar um driver em kernel mode
                // que intercepta chamadas de sistema relevantes

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao modificar CPU: {ex.Message}");
                return false;
            }
        }
    }
}