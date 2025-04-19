using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SpooferHWID
{
    public static class EFI
    {
        public static Dictionary<string, string> GetEFIInfo()
        {
            var result = new Dictionary<string, string>();

            try
            {
                // Verificar se o sistema está usando UEFI
                bool isUefi = false;
                try
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "powershell.exe";
                        process.StartInfo.Arguments = "-Command \"& {if (Test-Path variable:global:firmware) { $true } else { Get-ItemProperty -Path HKLM:\\System\\CurrentControlSet\\Control\\SecureBoot\\State | Select-Object -ExpandProperty UEFISecureBootEnabled -ErrorAction SilentlyContinue }}\"";
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;
                        process.Start();
                        string output = process.StandardOutput.ReadToEnd().Trim();
                        process.WaitForExit();

                        isUefi = output.Equals("True", StringComparison.OrdinalIgnoreCase) || output.Equals("1");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao determinar se o sistema usa UEFI: {ex.Message}");
                }

                result["IsUEFI"] = isUefi.ToString();

                // Verificar status do Secure Boot
                bool secureBootEnabled = false;
                try
                {
                    using (RegistryKey secureBootKey = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\SecureBoot\State"))
                    {
                        if (secureBootKey != null)
                        {
                            object secureBootValue = secureBootKey.GetValue("UEFISecureBootEnabled");
                            secureBootEnabled = secureBootValue != null && (int)secureBootValue == 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao determinar o status do Secure Boot: {ex.Message}");
                }

                result["SecureBootEnabled"] = secureBootEnabled.ToString();

                // Obter informações de variáveis EFI do registro
                try
                {
                    using (RegistryKey efiVariables = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Nsi\{eb004a03-9b1a-11d4-9123-0050047759bc}\26"))
                    {
                        if (efiVariables != null)
                        {
                            if (efiVariables.GetValue("VariableId") != null)
                                result["VariableId"] = efiVariables.GetValue("VariableId").ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao obter ID de variável EFI: {ex.Message}");
                }

                // Obter GUIDs do EFI
                try
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "powershell.exe";
                        process.StartInfo.Arguments = "-Command \"& {Get-CimInstance -ClassName Win32_ComputerSystemProduct | Select-Object -ExpandProperty UUID}\"";
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;
                        process.Start();
                        string output = process.StandardOutput.ReadToEnd().Trim();
                        process.WaitForExit();

                        if (!string.IsNullOrEmpty(output))
                            result["SystemUUID"] = output;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao obter UUID do sistema: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter informações EFI: {ex.Message}");
            }

            return result;
        }

        public static bool SpoofEFI()
        {
            try
            {
                bool anySuccess = false;

                // Tentar modificar VariableId do EFI
                try
                {
                    using (RegistryKey efiVariables = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Nsi\{eb004a03-9b1a-11d4-9123-0050047759bc}\26", true))
                    {
                        if (efiVariables != null)
                        {
                            string efiVariableId = Guid.NewGuid().ToString();
                            efiVariables.SetValue("VariableId", efiVariableId);
                            anySuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao spoofar ID de variável EFI: {ex.Message}");
                }

                // Para alterações mais profundas do EFI, seria necessário um driver de modo kernel
                // e interação direta com o firmware UEFI, o que está além do escopo deste exemplo

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao spoofar EFI: {ex.Message}");
                return false;
            }
        }

        public static bool IsSecureBootEnabled()
        {
            try
            {
                using (RegistryKey secureBootKey = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\SecureBoot\State"))
                {
                    if (secureBootKey != null)
                    {
                        object secureBootValue = secureBootKey.GetValue("UEFISecureBootEnabled");
                        return secureBootValue != null && (int)secureBootValue == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar status do Secure Boot: {ex.Message}");
            }

            return false;
        }

        // Nota: Manipular diretamente o Secure Boot não é possível a partir do Windows sem interação do usuário na UEFI
        // Esta função apenas fornece informações sobre como alguém poderia desativar o Secure Boot
        public static string GetSecureBootDisableInstructions()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("1. Reinicie o computador");
            sb.AppendLine("2. Durante a inicialização, pressione a tecla para acessar a UEFI/BIOS (geralmente F2, DEL, F10, F12, dependendo do fabricante)");
            sb.AppendLine("3. Navegue até as configurações de segurança/boot");
            sb.AppendLine("4. Encontre a opção 'Secure Boot' e altere para 'Disabled'");
            sb.AppendLine("5. Salve as alterações e saia");
            sb.AppendLine("6. Reinicie o computador");
            sb.AppendLine("\nOBS: A desativação do Secure Boot pode comprometer a segurança do sistema e pode ser detectada por anti-cheats modernos.");

            return sb.ToString();
        }
    }
}