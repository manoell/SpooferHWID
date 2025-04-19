using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SpooferHWID
{
    public static class CleanAntiCheats
    {
        public static List<(string, bool, bool)> CleanAllAntiCheats()
        {
            var resultados = new List<(string, bool, bool)>();

            // Vanguard (Valorant)
            bool vanguardEncontrado = EstaInstalado("Vanguard") ||
                                    Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VALORANT")) ||
                                    Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Riot Vanguard"));
            bool vanguardSucesso = vanguardEncontrado ? CleanVanguard() : false;
            resultados.Add(("Vanguard", vanguardEncontrado, vanguardSucesso));

            // EasyAntiCheat
            bool eacEncontrado = EstaInstalado("EasyAntiCheat") ||
                                Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "EasyAntiCheat"));
            bool eacSucesso = eacEncontrado ? CleanEasyAntiCheat() : false;
            resultados.Add(("EasyAntiCheat", eacEncontrado, eacSucesso));

            // BattlEye
            bool beEncontrado = EstaInstalado("BattlEye") ||
                              Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "BattlEye"));
            bool beSucesso = beEncontrado ? CleanBattlEye() : false;
            resultados.Add(("BattlEye", beEncontrado, beSucesso));

            // FaceIT
            bool faceitEncontrado = EstaInstalado("FACEIT") ||
                                  Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FACEIT"));
            bool faceitSucesso = faceitEncontrado ? CleanFaceIT() : false;
            resultados.Add(("FaceIT", faceitEncontrado, faceitSucesso));

            // ESEA
            bool eseaEncontrado = EstaInstalado("ESEA") ||
                                Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ESEA"));
            bool eseaSucesso = eseaEncontrado ? CleanESEA() : false;
            resultados.Add(("ESEA", eseaEncontrado, eseaSucesso));

            return resultados;
        }

        // Método auxiliar para verificar se um software está instalado
        public static bool EstaInstalado(string nomeSoftware)
        {
            try
            {
                // Verificar em registros de instalação do Windows
                using (RegistryKey uninstallKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
                {
                    if (uninstallKey != null)
                    {
                        foreach (string subKeyName in uninstallKey.GetSubKeyNames())
                        {
                            using (RegistryKey subKey = uninstallKey.OpenSubKey(subKeyName))
                            {
                                string displayName = subKey.GetValue("DisplayName")?.ToString() ?? "";
                                if (displayName.Contains(nomeSoftware))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

                // Verificar também em registros de 32 bits em sistemas 64 bits
                using (RegistryKey uninstallKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"))
                {
                    if (uninstallKey != null)
                    {
                        foreach (string subKeyName in uninstallKey.GetSubKeyNames())
                        {
                            using (RegistryKey subKey = uninstallKey.OpenSubKey(subKeyName))
                            {
                                string displayName = subKey.GetValue("DisplayName")?.ToString() ?? "";
                                if (displayName.Contains(nomeSoftware))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignora erros na verificação
            }

            return false;
        }

        // Limpa o Vanguard (Valorant)
        public static bool CleanVanguard()
        {
            try
            {
                Console.WriteLine("[*] Limpando Vanguard...");

                bool anySuccess = false;

                // Parar serviços do Vanguard
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "sc",
                        Arguments = "stop vgc",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    })?.WaitForExit();

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "sc",
                        Arguments = "stop vgk",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    })?.WaitForExit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao parar serviços do Vanguard: {ex.Message}");
                }

                // Excluir arquivos de cache do Valorant
                string valorantLocalAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VALORANT");
                if (Directory.Exists(valorantLocalAppData))
                {
                    try
                    {
                        string savedPath = Path.Combine(valorantLocalAppData, "saved");
                        if (Directory.Exists(savedPath))
                        {
                            Directory.Delete(savedPath, true);
                            anySuccess = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir cache do Valorant: {ex.Message}");
                    }
                }

                // Excluir arquivos de log do Vanguard
                string vanguardLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Riot Vanguard", "Logs");
                if (Directory.Exists(vanguardLogPath))
                {
                    try
                    {
                        Directory.Delete(vanguardLogPath, true);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir logs do Vanguard: {ex.Message}");
                    }
                }

                // Limpar rastros no registro
                try
                {
                    using (RegistryKey riotKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Riot Games", true))
                    {
                        if (riotKey != null)
                        {
                            riotKey.DeleteSubKeyTree("VALORANT", false);
                            anySuccess = true;
                        }
                    }

                    using (RegistryKey riotKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\vgk", true))
                    {
                        if (riotKey != null)
                        {
                            foreach (string valueName in riotKey.GetValueNames())
                            {
                                if (valueName.Contains("Instance"))
                                {
                                    riotKey.DeleteValue(valueName, false);
                                    anySuccess = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao limpar registro do Vanguard: {ex.Message}");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar Vanguard: {ex.Message}");
                return false;
            }
        }

        // Limpa o EasyAntiCheat
        public static bool CleanEasyAntiCheat()
        {
            try
            {
                Console.WriteLine("[*] Limpando EasyAntiCheat...");

                bool anySuccess = false;

                // Excluir arquivos de cache do EAC
                string eacPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "EasyAntiCheat");
                if (Directory.Exists(eacPath))
                {
                    try
                    {
                        Directory.Delete(eacPath, true);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir cache do EasyAntiCheat: {ex.Message}");
                    }
                }

                // Limpar logs
                string eacLogsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EasyAntiCheat");
                if (Directory.Exists(eacLogsPath))
                {
                    try
                    {
                        Directory.Delete(eacLogsPath, true);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir logs do EasyAntiCheat: {ex.Message}");
                    }
                }

                // Limpar registro
                try
                {
                    using (RegistryKey eacKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\EasyAntiCheat", true))
                    {
                        if (eacKey != null)
                        {
                            foreach (string subKeyName in eacKey.GetSubKeyNames())
                            {
                                eacKey.DeleteSubKeyTree(subKeyName, false);
                                anySuccess = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao limpar registro do EasyAntiCheat: {ex.Message}");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar EasyAntiCheat: {ex.Message}");
                return false;
            }
        }

        // Limpa o BattlEye
        public static bool CleanBattlEye()
        {
            try
            {
                Console.WriteLine("[*] Limpando BattlEye...");

                bool anySuccess = false;

                // Excluir logs do BattlEye
                string battleEyePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "BattlEye");
                if (Directory.Exists(battleEyePath))
                {
                    try
                    {
                        Directory.Delete(battleEyePath, true);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir logs do BattlEye: {ex.Message}");
                    }
                }

                // BattlEye pode estar em pastas de jogos específicos
                // Procurar pastas comuns de jogos
                string[] gameFolders = {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steamapps", "common"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Epic Games")
                };

                foreach (string gameFolder in gameFolders)
                {
                    if (Directory.Exists(gameFolder))
                    {
                        foreach (string dir in Directory.GetDirectories(gameFolder))
                        {
                            string bePath = Path.Combine(dir, "BattlEye");
                            if (Directory.Exists(bePath))
                            {
                                try
                                {
                                    string[] logFiles = Directory.GetFiles(bePath, "*.log");
                                    foreach (string logFile in logFiles)
                                    {
                                        File.Delete(logFile);
                                        anySuccess = true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Erro ao excluir logs do BattlEye em {dir}: {ex.Message}");
                                }
                            }
                        }
                    }
                }

                // Limpar registro
                try
                {
                    using (RegistryKey beKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\BattlEye", true))
                    {
                        if (beKey != null)
                        {
                            foreach (string subKeyName in beKey.GetSubKeyNames())
                            {
                                beKey.DeleteSubKeyTree(subKeyName, false);
                                anySuccess = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao limpar registro do BattlEye: {ex.Message}");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar BattlEye: {ex.Message}");
                return false;
            }
        }

        // Limpa o FaceIT
        public static bool CleanFaceIT()
        {
            try
            {
                Console.WriteLine("[*] Limpando FaceIT...");

                bool anySuccess = false;

                // Excluir arquivos do FaceIT
                string faceitPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FACEIT");
                if (Directory.Exists(faceitPath))
                {
                    try
                    {
                        Directory.Delete(faceitPath, true);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir arquivos do FACEIT: {ex.Message}");
                    }
                }

                // Limpar logs do FaceIT Anti-Cheat
                string faceitACPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "FACEIT AC");
                if (Directory.Exists(faceitACPath))
                {
                    try
                    {
                        string[] logFiles = Directory.GetFiles(faceitACPath, "*.log");
                        foreach (string logFile in logFiles)
                        {
                            File.Delete(logFile);
                            anySuccess = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir logs do FACEIT AC: {ex.Message}");
                    }
                }

                // Limpar registro
                try
                {
                    using (RegistryKey faceitKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\FACEIT", true))
                    {
                        if (faceitKey != null)
                        {
                            foreach (string subKeyName in faceitKey.GetSubKeyNames())
                            {
                                faceitKey.DeleteSubKeyTree(subKeyName, false);
                                anySuccess = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao limpar registro do FACEIT: {ex.Message}");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar FACEIT: {ex.Message}");
                return false;
            }
        }

        // Limpa o ESEA
        public static bool CleanESEA()
        {
            try
            {
                Console.WriteLine("[*] Limpando ESEA...");

                bool anySuccess = false;

                // Excluir arquivos do ESEA
                string eseaPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ESEA");
                if (Directory.Exists(eseaPath))
                {
                    try
                    {
                        Directory.Delete(eseaPath, true);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir arquivos do ESEA: {ex.Message}");
                    }
                }

                // Limpar logs do ESEA Client
                string eseaClientPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "ESEA Client");
                if (Directory.Exists(eseaClientPath))
                {
                    try
                    {
                        string[] logFiles = Directory.GetFiles(eseaClientPath, "*.log");
                        foreach (string logFile in logFiles)
                        {
                            File.Delete(logFile);
                            anySuccess = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir logs do ESEA Client: {ex.Message}");
                    }
                }

                // Limpar registro
                try
                {
                    using (RegistryKey eseaKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\ESEA", true))
                    {
                        if (eseaKey != null)
                        {
                            foreach (string subKeyName in eseaKey.GetSubKeyNames())
                            {
                                eseaKey.DeleteSubKeyTree(subKeyName, false);
                                anySuccess = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao limpar registro do ESEA: {ex.Message}");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar ESEA: {ex.Message}");
                return false;
            }
        }
    }
}