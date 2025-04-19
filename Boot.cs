using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpooferHWID
{
    public static class Boot
    {
        public static Dictionary<string, string> GetBootInfo()
        {
            var result = new Dictionary<string, string>();

            try
            {
                // Obter informações de boot do registro
                try
                {
                    using (RegistryKey bootKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\BootVerificationProgram"))
                    {
                        if (bootKey != null)
                        {
                            foreach (string valueName in bootKey.GetValueNames())
                            {
                                result[$"Boot_{valueName}"] = bootKey.GetValue(valueName)?.ToString() ?? "Not Available";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao obter informações de boot do registro: {ex.Message}");
                }

                // Verificar configurações de inicialização
                try
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "bcdedit.exe";
                        process.StartInfo.Arguments = "/enum {current}";
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;

                        process.Start();
                        string output = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();

                        // Extrair informações relevantes do output
                        string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string line in lines)
                        {
                            string trimmedLine = line.Trim();

                            // Procurar por valores específicos
                            if (trimmedLine.StartsWith("identifier"))
                                result["BCD_Identifier"] = trimmedLine.Split(' ')[1];
                            else if (trimmedLine.StartsWith("device"))
                                result["BCD_Device"] = trimmedLine.Substring(trimmedLine.IndexOf(' ')).Trim();
                            else if (trimmedLine.StartsWith("path"))
                                result["BCD_Path"] = trimmedLine.Substring(trimmedLine.IndexOf(' ')).Trim();
                            else if (trimmedLine.StartsWith("description"))
                                result["BCD_Description"] = trimmedLine.Substring(trimmedLine.IndexOf(' ')).Trim();
                            else if (trimmedLine.StartsWith("locale"))
                                result["BCD_Locale"] = trimmedLine.Substring(trimmedLine.IndexOf(' ')).Trim();
                            else if (trimmedLine.StartsWith("inherit"))
                                result["BCD_Inherit"] = trimmedLine.Substring(trimmedLine.IndexOf(' ')).Trim();
                            else if (trimmedLine.StartsWith("bootmenupolicy"))
                                result["BCD_BootMenuPolicy"] = trimmedLine.Substring(trimmedLine.IndexOf(' ')).Trim();
                            else if (trimmedLine.StartsWith("recoveryenabled"))
                                result["BCD_RecoveryEnabled"] = trimmedLine.Substring(trimmedLine.IndexOf(' ')).Trim();
                            else if (trimmedLine.StartsWith("useplatformclock"))
                                result["BCD_UsePlatformClock"] = trimmedLine.Substring(trimmedLine.IndexOf(' ')).Trim();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao executar bcdedit: {ex.Message}");
                }

                // Verificar se o sistema está usando Fast Boot
                try
                {
                    using (RegistryKey powerKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power"))
                    {
                        if (powerKey != null && powerKey.GetValue("HiberbootEnabled") != null)
                        {
                            result["FastBoot"] = (int)powerKey.GetValue("HiberbootEnabled") == 1 ? "Enabled" : "Disabled";
                        }
                        else
                        {
                            result["FastBoot"] = "Not Found";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao verificar status do Fast Boot: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter informações de boot: {ex.Message}");
            }

            return result;
        }

        public static bool SpoofBoot()
        {
            try
            {
                bool anySuccess = false;

                // Modificar identificador de boot
                try
                {
                    using (RegistryKey bootKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\SpooferHWID\Boot"))
                    {
                        if (bootKey != null)
                        {
                            // Gerar identificadores aleatórios
                            string bootId = Guid.NewGuid().ToString();
                            bootKey.SetValue("BootIdentifier", bootId);
                            bootKey.SetValue("LastBootTime", DateTime.Now.AddDays(-Utils.RandomNumber(1, 7)).ToString("yyyy-MM-dd HH:mm:ss"));

                            anySuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao criar chave de simulação de boot: {ex.Message}");
                }

                // Desabilitar Fast Boot (pode ajudar a evitar detecção)
                try
                {
                    using (RegistryKey powerKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power", true))
                    {
                        if (powerKey != null)
                        {
                            powerKey.SetValue("HiberbootEnabled", 0, RegistryValueKind.DWord);
                            anySuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao desabilitar Fast Boot: {ex.Message}");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao spoofar boot: {ex.Message}");
                return false;
            }
        }

        // Verifica se o TestSigning está habilitado
        public static bool IsTestSigningEnabled()
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "bcdedit.exe";
                    process.StartInfo.Arguments = "/enum {current}";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    return output.Contains("testsigning") && output.Contains("Yes");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar status do TestSigning: {ex.Message}");
                return false;
            }
        }

        // Habilita o modo TestSigning (para carregamento de drivers não assinados)
        public static bool EnableTestSigning()
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "bcdedit.exe";
                    process.StartInfo.Arguments = "/set {current} testsigning on";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();
                    process.WaitForExit();

                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao habilitar TestSigning: {ex.Message}");
                return false;
            }
        }

        // Desabilita o modo TestSigning
        public static bool DisableTestSigning()
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "bcdedit.exe";
                    process.StartInfo.Arguments = "/set {current} testsigning off";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();
                    process.WaitForExit();

                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao desabilitar TestSigning: {ex.Message}");
                return false;
            }
        }
    }
}