using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SpooferHWID
{
    public static class CleanSystem
    {
        public static List<(string, bool, bool)> CleanSystemTraces()
        {
            var resultados = new List<(string, bool, bool)>();

            // Windows Event Logs
            bool logsEncontrados = true; // Sempre disponível no Windows
            bool logsSucesso = CleanWindowsEventLogs();
            resultados.Add(("Logs de Eventos", logsEncontrados, logsSucesso));

            // Prefetch Files
            string prefetchPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch");
            bool prefetchEncontrado = Directory.Exists(prefetchPath);
            bool prefetchSucesso = prefetchEncontrado ? CleanPrefetchFiles() : false;
            resultados.Add(("Arquivos Prefetch", prefetchEncontrado, prefetchSucesso));

            // Temp Files
            string tempPath = Path.GetTempPath();
            bool tempEncontrado = Directory.Exists(tempPath);
            bool tempSucesso = tempEncontrado ? CleanTempFiles() : false;
            resultados.Add(("Arquivos Temporários", tempEncontrado, tempSucesso));

            // USN Journal
            bool usnEncontrado = true; // Sempre disponível em sistemas NTFS
            bool usnSucesso = CleanUSNJournal();
            resultados.Add(("USN Journal", usnEncontrado, usnSucesso));

            // Recent Items
            string recentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Recent));
            bool recentEncontrado = Directory.Exists(recentPath);
            bool recentSucesso = recentEncontrado ? CleanRecentItems() : false;
            resultados.Add(("Itens Recentes", recentEncontrado, recentSucesso));

            // Windows Logs
            string windowsLogsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Logs");
            bool windowsLogsEncontrado = Directory.Exists(windowsLogsPath);
            bool windowsLogsSucesso = windowsLogsEncontrado ? CleanWindowsLogs() : false;
            resultados.Add(("Logs do Windows", windowsLogsEncontrado, windowsLogsSucesso));

            return resultados;
        }

        // Limpa os logs de eventos do Windows
        public static bool CleanWindowsEventLogs()
        {
            try
            {
                Console.WriteLine("[*] Limpando logs de eventos do Windows...");

                bool anySuccess = false;

                // Lista de logs comuns para limpar
                string[] logs = { "Application", "Security", "System", "Setup" };

                // Usar o comando wevtutil para limpar cada log
                foreach (string log in logs)
                {
                    try
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = "wevtutil.exe";
                        process.StartInfo.Arguments = $"cl {log}";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;
                        process.Start();
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                            anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao limpar log {log}: {ex.Message}");
                    }
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar logs de eventos do Windows: {ex.Message}");
                return false;
            }
        }

        // Limpa os arquivos prefetch do Windows
        public static bool CleanPrefetchFiles()
        {
            try
            {
                Console.WriteLine("[*] Limpando arquivos prefetch...");

                bool anySuccess = false;

                // Caminho para a pasta prefetch
                string prefetchPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch");

                if (Directory.Exists(prefetchPath))
                {
                    try
                    {
                        string[] files = Directory.GetFiles(prefetchPath, "*.pf");
                        foreach (string file in files)
                        {
                            try
                            {
                                File.Delete(file);
                                anySuccess = true;
                            }
                            catch
                            {
                                // Alguns arquivos podem estar em uso, ignorar esses erros
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao limpar arquivos prefetch: {ex.Message}");
                    }
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar arquivos prefetch: {ex.Message}");
                return false;
            }
        }

        // Limpa arquivos temporários
        public static bool CleanTempFiles()
        {
            try
            {
                Console.WriteLine("[*] Limpando arquivos temporários...");

                bool anySuccess = false;

                // Limpar pasta Temp do usuário
                string userTempPath = Path.GetTempPath();
                if (Directory.Exists(userTempPath))
                {
                    try
                    {
                        string[] files = Directory.GetFiles(userTempPath);
                        foreach (string file in files)
                        {
                            try
                            {
                                File.Delete(file);
                                anySuccess = true;
                            }
                            catch
                            {
                                // Alguns arquivos podem estar em uso, ignorar esses erros
                            }
                        }

                        string[] dirs = Directory.GetDirectories(userTempPath);
                        foreach (string dir in dirs)
                        {
                            try
                            {
                                Directory.Delete(dir, true);
                                anySuccess = true;
                            }
                            catch
                            {
                                // Algumas pastas podem estar em uso, ignorar esses erros
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao limpar pasta Temp do usuário: {ex.Message}");
                    }
                }

                // Limpar pasta Temp do Windows
                string winTempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");
                if (Directory.Exists(winTempPath))
                {
                    try
                    {
                        string[] files = Directory.GetFiles(winTempPath);
                        foreach (string file in files)
                        {
                            try
                            {
                                File.Delete(file);
                                anySuccess = true;
                            }
                            catch
                            {
                                // Alguns arquivos podem estar em uso, ignorar esses erros
                            }
                        }

                        string[] dirs = Directory.GetDirectories(winTempPath);
                        foreach (string dir in dirs)
                        {
                            try
                            {
                                Directory.Delete(dir, true);
                                anySuccess = true;
                            }
                            catch
                            {
                                // Algumas pastas podem estar em uso, ignorar esses erros
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao limpar pasta Temp do Windows: {ex.Message}");
                    }
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar arquivos temporários: {ex.Message}");
                return false;
            }
        }

        // Limpa USN Journal
        public static bool CleanUSNJournal()
        {
            try
            {
                Console.WriteLine("[*] Limpando USN Journal...");

                // Usar o fsutil para limpar o USN Journal
                Process process = new Process();
                process.StartInfo.FileName = "fsutil.exe";
                process.StartInfo.Arguments = "usn deletejournal /d C:";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();

                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar USN Journal: {ex.Message}");
                return false;
            }
        }

        // Limpa itens recentes
        public static bool CleanRecentItems()
        {
            try
            {
                Console.WriteLine("[*] Limpando itens recentes...");

                bool anySuccess = false;

                // Limpar pasta Recent do usuário
                string recentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Recent));
                if (Directory.Exists(recentPath))
                {
                    try
                    {
                        string[] files = Directory.GetFiles(recentPath);
                        foreach (string file in files)
                        {
                            try
                            {
                                File.Delete(file);
                                anySuccess = true;
                            }
                            catch
                            {
                                // Alguns arquivos podem estar em uso, ignorar esses erros
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao limpar pasta Recent: {ex.Message}");
                    }
                }

                // Limpar histórico de FileExplorer
                try
                {
                    using (RegistryKey recentDocs = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\RecentDocs", true))
                    {
                        if (recentDocs != null)
                        {
                            foreach (string subKeyName in recentDocs.GetSubKeyNames())
                            {
                                using (RegistryKey subKey = recentDocs.OpenSubKey(subKeyName, true))
                                {
                                    if (subKey != null)
                                    {
                                        foreach (string valueName in subKey.GetValueNames())
                                        {
                                            subKey.DeleteValue(valueName);
                                            anySuccess = true;
                                        }
                                    }
                                }
                            }

                            foreach (string valueName in recentDocs.GetValueNames())
                            {
                                recentDocs.DeleteValue(valueName);
                                anySuccess = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao limpar histórico do Explorer: {ex.Message}");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar itens recentes: {ex.Message}");
                return false;
            }
        }

        // Limpa logs do Windows
        public static bool CleanWindowsLogs()
        {
            try
            {
                Console.WriteLine("[*] Limpando logs do Windows...");

                bool anySuccess = false;

                // Caminho para a pasta de logs
                string logsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Logs");

                if (Directory.Exists(logsPath))
                {
                    try
                    {
                        // Limpar arquivos de log (mantendo as pastas para evitar problemas)
                        string[] files = Directory.GetFiles(logsPath, "*.log", SearchOption.AllDirectories);
                        foreach (string file in files)
                        {
                            try
                            {
                                // Criar um arquivo vazio no lugar de excluir
                                File.WriteAllText(file, string.Empty);
                                anySuccess = true;
                            }
                            catch
                            {
                                // Alguns arquivos podem estar em uso, ignorar esses erros
                            }
                        }

                        // Limpar arquivos .etl
                        files = Directory.GetFiles(logsPath, "*.etl", SearchOption.AllDirectories);
                        foreach (string file in files)
                        {
                            try
                            {
                                File.Delete(file);
                                anySuccess = true;
                            }
                            catch
                            {
                                // Alguns arquivos podem estar em uso, ignorar esses erros
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao limpar pasta de logs do Windows: {ex.Message}");
                    }
                }

                // Limpar logs do DirectX
                string dxdiagPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "dxdiag.log");
                if (File.Exists(dxdiagPath))
                {
                    try
                    {
                        File.Delete(dxdiagPath);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir log do DirectX: {ex.Message}");
                    }
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar logs do Windows: {ex.Message}");
                return false;
            }
        }

        // Desativa pontos de restauração
        public static bool DisableRestorePoints()
        {
            try
            {
                Console.WriteLine("[*] Desativando pontos de restauração...");

                // Desativar proteção do sistema
                Process process = new Process();
                process.StartInfo.FileName = "vssadmin.exe";
                process.StartInfo.Arguments = "delete shadows /all /quiet";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();

                bool success = process.ExitCode == 0;

                // Desativar proteção do sistema no registro
                try
                {
                    using (RegistryKey computerKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", true))
                    {
                        if (computerKey != null)
                        {
                            computerKey.SetValue("DisableSR", 1, RegistryValueKind.DWord);
                            success = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao desativar proteção do sistema no registro: {ex.Message}");
                }

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao desativar pontos de restauração: {ex.Message}");
                return false;
            }
        }
    }
}