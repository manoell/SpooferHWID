using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace SpooferHWID
{
    public static class CleanGames
    {
        public static bool CleanAllGames()
        {
            bool success = true;

            // Limpar rastros de jogos específicos
            success &= CleanValorant();
            success &= CleanFortnite();
            success &= CleanApex();
            success &= CleanRainbowSix();
            success &= CleanCSGO();
            success &= CleanPUBG();
            success &= CleanRocketLeague();
            success &= CleanUbisoft();

            return success;
        }

        // Limpa rastros do Valorant
        public static bool CleanValorant()
        {
            try
            {
                Console.WriteLine("[*] Limpando rastros do Valorant...");

                bool anySuccess = false;

                // Excluir arquivos de cache do Valorant
                string valorantLocalAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VALORANT");
                if (Directory.Exists(valorantLocalAppData))
                {
                    try
                    {
                        Directory.Delete(valorantLocalAppData, true);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir pasta do Valorant: {ex.Message}");
                    }
                }

                // Excluir pasta Riot Games (usuário)
                string riotGamesAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games");
                if (Directory.Exists(riotGamesAppData))
                {
                    try
                    {
                        Directory.Delete(riotGamesAppData, true);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir pasta Riot Games: {ex.Message}");
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao limpar registro do Valorant: {ex.Message}");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar Valorant: {ex.Message}");
                return false;
            }
        }

        // Limpa rastros do Fortnite
        public static bool CleanFortnite()
        {
            try
            {
                Console.WriteLine("[*] Limpando rastros do Fortnite...");

                bool anySuccess = false;

                // Excluir arquivos de cache do Fortnite
                string fortniteLocalAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FortniteGame");
                if (Directory.Exists(fortniteLocalAppData))
                {
                    try
                    {
                        Directory.Delete(fortniteLocalAppData, true);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir pasta do Fortnite: {ex.Message}");
                    }
                }

                // Excluir pasta Epic Games (usuário)
                string epicGamesAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Epic Games");
                if (Directory.Exists(epicGamesAppData))
                {
                    try
                    {
                        string[] dirs = Directory.GetDirectories(epicGamesAppData);
                        foreach (string dir in dirs)
                        {
                            if (Path.GetFileName(dir).Contains("Fortnite"))
                            {
                                Directory.Delete(dir, true);
                                anySuccess = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir pasta Epic Games: {ex.Message}");
                    }
                }

                // Limpar rastros no registro
                try
                {
                    using (RegistryKey epicKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Epic Games", true))
                    {
                        if (epicKey != null)
                        {
                            epicKey.DeleteSubKeyTree("Fortnite", false);
                            anySuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao limpar registro do Fortnite: {ex.Message}");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar Fortnite: {ex.Message}");
                return false;
            }
        }

        // Limpa rastros do Apex Legends
        public static bool CleanApex()
        {
            try
            {
                Console.WriteLine("[*] Limpando rastros do Apex Legends...");

                bool anySuccess = false;

                // Excluir arquivos de cache do Apex
                string apexSavedGames = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Saved Games", "Respawn", "Apex");
                if (Directory.Exists(apexSavedGames))
                {
                    try
                    {
                        Directory.Delete(apexSavedGames, true);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir pasta do Apex Legends: {ex.Message}");
                    }
                }

                // Excluir pasta EA (usuário)
                string eaAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Electronic Arts");
                if (Directory.Exists(eaAppData))
                {
                    try
                    {
                        string[] dirs = Directory.GetDirectories(eaAppData);
                        foreach (string dir in dirs)
                        {
                            if (Path.GetFileName(dir).Contains("Apex"))
                            {
                                Directory.Delete(dir, true);
                                anySuccess = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir pasta EA: {ex.Message}");
                    }
                }

                // Limpar rastros no registro
                try
                {
                    using (RegistryKey eaKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Electronic Arts", true))
                    {
                        if (eaKey != null)
                        {
                            string[] subKeys = eaKey.GetSubKeyNames();
                            foreach (string subKey in subKeys)
                            {
                                if (subKey.Contains("Apex"))
                                {
                                    eaKey.DeleteSubKeyTree(subKey, false);
                                    anySuccess = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao limpar registro do Apex Legends: {ex.Message}");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar Apex Legends: {ex.Message}");
                return false;
            }
        }

        // Limpa rastros do Rainbow Six Siege
        public static bool CleanRainbowSix()
        {
            try
            {
                Console.WriteLine("[*] Limpando rastros do Rainbow Six Siege...");

                bool anySuccess = false;

                // Excluir pasta de configurações
                string r6LocalAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ubisoft Game Launcher", "savegames");
                if (Directory.Exists(r6LocalAppData))
                {
                    try
                    {
                        Directory.Delete(r6LocalAppData, true);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir pasta de salvamentos do Rainbow Six: {ex.Message}");
                    }
                }

                // Excluir documentos do R6
                string r6Documents = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Rainbow Six - Siege");
                if (Directory.Exists(r6Documents))
                {
                    try
                    {
                        Directory.Delete(r6Documents, true);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir documentos do Rainbow Six: {ex.Message}");
                    }
                }

                // Limpar rastros no registro
                try
                {
                    using (RegistryKey ubiKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Ubisoft", true))
                    {
                        if (ubiKey != null)
                        {
                            string[] subKeys = ubiKey.GetSubKeyNames();
                            foreach (string subKey in subKeys)
                            {
                                if (subKey.Contains("Rainbow") || subKey.Contains("R6"))
                                {
                                    ubiKey.DeleteSubKeyTree(subKey, false);
                                    anySuccess = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao limpar registro do Rainbow Six: {ex.Message}");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar Rainbow Six Siege: {ex.Message}");
                return false;
            }
        }

        // Limpa rastros do CS:GO
        public static bool CleanCSGO()
        {
            try
            {
                Console.WriteLine("[*] Limpando rastros do CS:GO...");

                bool anySuccess = false;

                // Excluir pasta de configurações
                string steamUser = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "userdata");
                if (Directory.Exists(steamUser))
                {
                    try
                    {
                        string[] userDirs = Directory.GetDirectories(steamUser);
                        foreach (string userDir in userDirs)
                        {
                            string csgoDir = Path.Combine(userDir, "730");
                            if (Directory.Exists(csgoDir))
                            {
                                Directory.Delete(csgoDir, true);
                                anySuccess = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir pasta de usuário do CS:GO: {ex.Message}");
                    }
                }

                // Limpar rastros no registro
                try
                {
                    using (RegistryKey valveKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve", true))
                    {
                        if (valveKey != null)
                        {
                            string[] subKeys = valveKey.GetSubKeyNames();
                            foreach (string subKey in subKeys)
                            {
                                if (subKey.Contains("CSGO") || subKey.Contains("730"))
                                {
                                    valveKey.DeleteSubKeyTree(subKey, false);
                                    anySuccess = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao limpar registro do CS:GO: {ex.Message}");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar CS:GO: {ex.Message}");
                return false;
            }
        }

        // Limpa rastros do PUBG
        public static bool CleanPUBG()
        {
            try
            {
                Console.WriteLine("[*] Limpando rastros do PUBG...");

                bool anySuccess = false;

                // Excluir pasta de configurações
                string pubgLocalAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TslGame");
                if (Directory.Exists(pubgLocalAppData))
                {
                    try
                    {
                        Directory.Delete(pubgLocalAppData, true);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir pasta do PUBG: {ex.Message}");
                    }
                }

                // Excluir pasta AppData
                string pubgAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PUBG");
                if (Directory.Exists(pubgAppData))
                {
                    try
                    {
                        Directory.Delete(pubgAppData, true);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir pasta AppData do PUBG: {ex.Message}");
                    }
                }

                // Limpar rastros no registro
                try
                {
                    using (RegistryKey pubgKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\PUBG Corporation", true))
                    {
                        if (pubgKey != null)
                        {
                            foreach (string subKeyName in pubgKey.GetSubKeyNames())
                            {
                                pubgKey.DeleteSubKeyTree(subKeyName, false);
                                anySuccess = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao limpar registro do PUBG: {ex.Message}");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar PUBG: {ex.Message}");
                return false;
            }
        }

        // Limpa rastros do Rocket League
        public static bool CleanRocketLeague()
        {
            try
            {
                Console.WriteLine("[*] Limpando rastros do Rocket League...");

                bool anySuccess = false;

                // Excluir pasta de documentos
                string rlMyDocs = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Rocket League");
                if (Directory.Exists(rlMyDocs))
                {
                    try
                    {
                        Directory.Delete(rlMyDocs, true);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir pasta de documentos do Rocket League: {ex.Message}");
                    }
                }

                // Excluir pasta AppData
                string rlAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rocket League");
                if (Directory.Exists(rlAppData))
                {
                    try
                    {
                        Directory.Delete(rlAppData, true);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir pasta AppData do Rocket League: {ex.Message}");
                    }
                }

                // Limpar rastros no registro
                try
                {
                    using (RegistryKey epicKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Epic Games", true))
                    {
                        if (epicKey != null)
                        {
                            epicKey.DeleteSubKeyTree("RocketLeague", false);
                            anySuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao limpar registro do Rocket League: {ex.Message}");
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar Rocket League: {ex.Message}");
                return false;
            }
        }

        // Limpa rastros de jogos da Ubisoft
        public static bool CleanUbisoft()
        {
            try
            {
                Console.WriteLine("[*] Limpando rastros de jogos da Ubisoft...");

                bool anySuccess = false;

                // Limpar cache do Ubisoft Connect
                string ubiCachePath = Path.Combine("C:", "Program Files (x86)", "Ubisoft", "Ubisoft Game Launcher", "cache");
                if (Directory.Exists(ubiCachePath))
                {
                    try
                    {
                        Directory.Delete(ubiCachePath, true);
                        Directory.CreateDirectory(ubiCachePath);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao limpar cache da Ubisoft: {ex.Message}");
                    }
                }

                // Limpar logs
                string ubiLogsPath = Path.Combine("C:", "Program Files (x86)", "Ubisoft", "Ubisoft Game Launcher", "logs");
                if (Directory.Exists(ubiLogsPath))
                {
                    try
                    {
                        Directory.Delete(ubiLogsPath, true);
                        Directory.CreateDirectory(ubiLogsPath);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao limpar logs da Ubisoft: {ex.Message}");
                    }
                }

                // Limpar savegames
                string ubiSavesPath = Path.Combine("C:", "Program Files (x86)", "Ubisoft", "Ubisoft Game Launcher", "savegames");
                if (Directory.Exists(ubiSavesPath))
                {
                    try
                    {
                        Directory.Delete(ubiSavesPath, true);
                        Directory.CreateDirectory(ubiSavesPath);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao limpar savegames da Ubisoft: {ex.Message}");
                    }
                }

                // Limpar spool
                string ubiSpoolPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ubisoft Game Launcher", "spool");
                if (Directory.Exists(ubiSpoolPath))
                {
                    try
                    {
                        Directory.Delete(ubiSpoolPath, true);
                        Directory.CreateDirectory(ubiSpoolPath);
                        anySuccess = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao limpar spool da Ubisoft: {ex.Message}");
                    }
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar jogos da Ubisoft: {ex.Message}");
                return false;
            }
        }
    }
}