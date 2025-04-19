using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpooferHWID
{
    public static class HVCI
    {
        public static Dictionary<string, string> GetHVCIInfo()
        {
            var result = new Dictionary<string, string>();

            try
            {
                // Obter status do HVCI (Hypervisor-protected Code Integrity)
                try
                {
                    using (RegistryKey hvciKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity"))
                    {
                        if (hvciKey != null)
                        {
                            object enabled = hvciKey.GetValue("Enabled");
                            object running = hvciKey.GetValue("Running");

                            result["HVCIEnabled"] = enabled != null ? enabled.ToString() : "0";
                            result["HVCIRunning"] = running != null ? running.ToString() : "0";
                        }
                        else
                        {
                            result["HVCIEnabled"] = "0";
                            result["HVCIRunning"] = "0";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao obter status do HVCI do registro: {ex.Message}");
                }

                // Obter status usando PowerShell
                try
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "powershell.exe";
                        process.StartInfo.Arguments = "-Command \"& {Get-CimInstance -ClassName Win32_DeviceGuard -Namespace root\\Microsoft\\Windows\\DeviceGuard | Select-Object -ExpandProperty SecurityServicesRunning}\"";
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;

                        process.Start();
                        string output = process.StandardOutput.ReadToEnd().Trim();
                        process.WaitForExit();

                        if (!string.IsNullOrEmpty(output))
                        {
                            // SecurityServicesRunning = 1 significa que Virtualization-based security está ativo
                            // SecurityServicesRunning = 2 significa que HVCI está ativo
                            result["SecurityServicesRunning"] = output;

                            bool vbsRunning = output.Contains("1");
                            bool hvciRunning = output.Contains("2");

                            result["VBSRunning"] = vbsRunning.ToString();
                            result["HVCIRunningSecurity"] = hvciRunning.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao obter status de segurança do HVCI: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter informações do HVCI: {ex.Message}");
            }

            return result;
        }

        public static bool ConfigureHVCI()
        {
            try
            {
                bool anySuccess = false;

                // Tentar desativar o HVCI
                try
                {
                    using (RegistryKey hvciKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", true))
                    {
                        if (hvciKey != null)
                        {
                            // Desativar o HVCI (requer reinicialização para ter efeito)
                            hvciKey.SetValue("Enabled", 0, RegistryValueKind.DWord);

                            anySuccess = true;
                        }
                        else
                        {
                            // Se a chave não existir, vamos criá-la
                            using (RegistryKey newHvciKey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity"))
                            {
                                if (newHvciKey != null)
                                {
                                    newHvciKey.SetValue("Enabled", 0, RegistryValueKind.DWord);
                                    anySuccess = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao configurar HVCI no registro: {ex.Message}");
                }

                // Adicionar configurações adicionais
                try
                {
                    using (RegistryKey deviceGuardKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\DeviceGuard", true))
                    {
                        if (deviceGuardKey != null)
                        {
                            // Desabilitar características que podem interferir com o spoofing
                            deviceGuardKey.SetValue("EnableVirtualizationBasedSecurity", 0, RegistryValueKind.DWord);
                            deviceGuardKey.SetValue("RequirePlatformSecurityFeatures", 0, RegistryValueKind.DWord);

                            anySuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao configurar DeviceGuard: {ex.Message}");
                }

                // Nota: Para algumas destas alterações terem efeito, é necessário reiniciar o sistema

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao configurar HVCI: {ex.Message}");
                return false;
            }
        }

        // Verifica se o HVCI está ativo
        public static bool IsHVCIActive()
        {
            try
            {
                using (RegistryKey hvciKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity"))
                {
                    if (hvciKey != null)
                    {
                        object running = hvciKey.GetValue("Running");
                        return running != null && (int)running == 1;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar se o HVCI está ativo: {ex.Message}");
                return false;
            }
        }

        // Retorna instruções sobre como desativar HVCI via GUI do Windows
        public static string GetHVCIDisableInstructions()
        {
            return @"Instruções para desativar o HVCI (Hypervisor-protected Code Integrity):

1. Abra o menu Iniciar e digite 'segurança do windows'
2. Abra o aplicativo Segurança do Windows
3. Clique em 'Segurança do dispositivo'
4. Em 'Integridade do núcleo', clique em 'Detalhes da integridade do núcleo'
5. Desative a opção 'Integridade de memória'
6. Reinicie o computador

Alternativamente, através do PowerShell (como Administrador):
1. Execute o comando: Set-ItemProperty -Path 'HKLM:\SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity' -Name 'Enabled' -Value 0
2. Reinicie o computador

Atenção: Desativar o HVCI pode comprometer a segurança do seu sistema!";
        }
    }
}