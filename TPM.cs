using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace SpooferHWID
{
    public static class TPM
    {
        public static Dictionary<string, string> GetTPMInfo()
        {
            var result = new Dictionary<string, string>();

            try
            {
                // Verificar se o TPM está presente e habilitado
                bool tpmPresent = false;
                bool tpmEnabled = false;

                try
                {
                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Tpm"))
                    {
                        foreach (ManagementObject tpm in searcher.Get())
                        {
                            tpmPresent = true;
                            tpmEnabled = (bool)tpm["IsEnabled_InitialValue"];

                            result["SpecVersion"] = tpm["SpecVersion"]?.ToString() ?? "Unknown";
                            result["ManufacturerId"] = tpm["ManufacturerId"]?.ToString() ?? "Unknown";
                            result["ManufacturerVersion"] = tpm["ManufacturerVersion"]?.ToString() ?? "Unknown";
                            result["ManufacturerVersionInfo"] = tpm["ManufacturerVersionInfo"]?.ToString() ?? "Unknown";
                            result["PhysicalPresenceVersionInfo"] = tpm["PhysicalPresenceVersionInfo"]?.ToString() ?? "Unknown";

                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao acessar informações do TPM via WMI: {ex.Message}");
                }

                result["TPMPresent"] = tpmPresent.ToString();
                result["TPMEnabled"] = tpmEnabled.ToString();

                // Verificar se o TPM está pronto no registro
                try
                {
                    using (RegistryKey tpmReadyKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Tpm\Tpm2DbgInfo"))
                    {
                        if (tpmReadyKey != null)
                        {
                            object tpmReadyValue = tpmReadyKey.GetValue("TpmReady");
                            bool tpmReady = tpmReadyValue != null && (int)tpmReadyValue == 1;
                            result["TPMReady"] = tpmReady.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao verificar se o TPM está pronto: {ex.Message}");
                }

                // Verificar a versão do TPM usando PowerShell
                try
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "powershell.exe";
                        process.StartInfo.Arguments = "-Command \"& {Get-Tpm | Select-Object -ExpandProperty TpmVersion}\"";
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;

                        process.Start();
                        string output = process.StandardOutput.ReadToEnd().Trim();
                        process.WaitForExit();

                        if (!string.IsNullOrEmpty(output))
                            result["TPMVersion"] = output;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao obter a versão do TPM: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter informações do TPM: {ex.Message}");
            }

            return result;
        }

        public static bool SpoofTPM()
        {
            try
            {
                bool anySuccess = false;

                // Nota: Spoofing real do TPM é extremamente difícil e requer um driver de modo kernel
                // ou manipulação a nível de hardware. O que podemos fazer é modificar alguns registros
                // que possam ser usados para identificação.

                // Tentar modificar informações do TPM no registro
                try
                {
                    // Criar uma chave de registro falsa para simular TPM
                    using (RegistryKey tpmKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Tpm\Simulated"))
                    {
                        if (tpmKey != null)
                        {
                            // Gerar valores aleatórios
                            tpmKey.SetValue("ManufacturerId", Utils.RandomHex(4));
                            tpmKey.SetValue("ManufacturerVersion", $"{Utils.RandomNumber(1, 9)}.{Utils.RandomNumber(0, 9)}.{Utils.RandomNumber(0, 9)}.{Utils.RandomNumber(1, 999)}");
                            tpmKey.SetValue("UniqueId", Guid.NewGuid().ToString());

                            anySuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao criar simulação de TPM no registro: {ex.Message}");
                }

                // Tentar "enganar" verificações do Vanguard
                // Nota: Isto é apenas uma simulação conceitual e não funcionará realmente contra o Vanguard
                try
                {
                    using (RegistryKey vgBypassKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Tpm\VanguardCompat"))
                    {
                        if (vgBypassKey != null)
                        {
                            vgBypassKey.SetValue("VanguardCompatMode", 1, RegistryValueKind.DWord);
                            vgBypassKey.SetValue("TPM20Compatible", 1, RegistryValueKind.DWord);

                            anySuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao criar chave de compatibilidade TPM: {ex.Message}");
                }

                // Para um spoof real do TPM, seria necessário um driver que intercepta chamadas para o TPM
                // e retorna identificadores falsos, o que está além do escopo deste exemplo

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao spoofar TPM: {ex.Message}");
                return false;
            }
        }

        // Verifica se o TPM é necessário para o Vanguard (anti-cheat do Valorant)
        public static bool IsTPMRequiredForVanguard()
        {
            try
            {
                // Verificar se o Valorant/Vanguard está instalado
                bool valorantInstalled = false;

                using (RegistryKey uninstallKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
                {
                    if (uninstallKey != null)
                    {
                        foreach (string subKeyName in uninstallKey.GetSubKeyNames())
                        {
                            using (RegistryKey subKey = uninstallKey.OpenSubKey(subKeyName))
                            {
                                string displayName = subKey.GetValue("DisplayName")?.ToString() ?? "";
                                if (displayName.Contains("Valorant") || displayName.Contains("Vanguard"))
                                {
                                    valorantInstalled = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                // Valorant/Vanguard exige TPM 2.0 e Secure Boot no Windows 11
                bool isWindows11 = Environment.OSVersion.Version.Build >= 22000;

                return valorantInstalled && isWindows11;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar requisitos do Vanguard: {ex.Message}");
                return false;
            }
        }
    }
}