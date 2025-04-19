using System;
using System.Collections.Generic;

namespace SpooferHWID
{
    public static class UI
    {
        private static readonly List<string> Modules = new List<string>
        {
            "Disco/HD - Serial Number, Product ID, Volume ID",
            "CPU - CPU ID, Microcode, Serial Number",
            "GPU - Device ID, Vendor ID, Serial Number, ROM Version",
            "RAM - Serial Number, Fabricante, Timings",
            "Motherboard - Serial, BIOS Version, BIOS Date",
            "BIOS - System UUID, Manufacturer, Serial Number",
            "Network/MAC - MAC Address, Network Adapter Information",
            "EFI - EFI Variables, Secure Boot",
            "TPM - Identificadores TPM, Bypass verificações",
            "HVCI - Configurações de proteção de integridade",
            "Boot - Configurações de inicialização",
            "Anti-Cheats - Logs, arquivos de persistência, registros",
            "Games - Valorant, Fortnite/EAC, outros jogos populares",
            "Sistema - Event Logs, Prefetch, USN Journal, Restore Points"
        };

        public static void DisplayHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
 ██████╗██████╗  ██████╗  ██████╗ ███████╗███████╗██████╗     ██╗  ██╗██╗    ██╗██╗██████╗ 
██╔════╝██╔══██╗██╔═══██╗██╔═══██╗██╔════╝██╔════╝██╔══██╗    ██║  ██║██║    ██║██║██╔══██╗
╚█████╗ ██████╔╝██║   ██║██║   ██║█████╗  █████╗  ██████╔╝    ███████║██║ █╗ ██║██║██║  ██║
 ╚═══██╗██╔═══╝ ██║   ██║██║   ██║██╔══╝  ██╔══╝  ██╔══██╗    ██╔══██║██║███╗██║██║██║  ██║
██████╔╝██║     ╚██████╔╝╚██████╔╝██║     ███████╗██║  ██║    ██║  ██║╚███╔███╔╝██║██████╔╝
╚═════╝ ╚═╝      ╚═════╝  ╚═════╝ ╚═╝     ╚══════╝╚═╝  ╚═╝    ╚═╝  ╚═╝ ╚══╝╚══╝ ╚═╝╚═════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nHWID Spoofer - Sistema de Spoofing Completo para Anti-Cheats");
            Console.WriteLine("=====================================================");
            Console.ResetColor();
        }

        public static void DisplayModulesList()
        {
            Console.WriteLine("\nMódulos que serão alterados:");
            Console.WriteLine("===========================");

            for (int i = 0; i < Modules.Count; i++)
            {
                Console.WriteLine($"[*] {Modules[i]}");
            }
        }

        public static void DisplayMenuOptions()
        {
            Console.WriteLine("\n===========================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Escolha uma opção:");
            Console.WriteLine("[1] Spoof ALL - Modificar todos os componentes");
            Console.WriteLine("[2] Exit - Sair do programa");
            Console.Write("\nSua escolha: ");
            Console.ResetColor();
        }

        public static void DisplayComponentResult(string component, bool success)
        {
            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[√] {component}: Modificado com sucesso");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[X] {component}: Falha na modificação");
            }
            Console.ResetColor();
        }
    }
}