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

        public static void DisplayValueDetails(string key, string beforeValue, string afterValue, int indent = 0)
        {
            string indentStr = new string(' ', indent);

            // Exibir a chave
            Console.WriteLine($"{indentStr}{key}:");

            // Exibir valor anterior
            Console.Write($"{indentStr}  Antes: {beforeValue}");
            Console.WriteLine();

            // Verificar se o valor mudou
            if (beforeValue != afterValue)
            {
                // Valor alterado com sucesso - verde
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{indentStr}  Depois: {afterValue}");
            }
            else
            {
                // Valor não alterado - vermelho
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{indentStr}  Não alterado: {afterValue}");
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        public static void DisplayNewValue(string key, string value, int indent = 0)
        {
            string indentStr = new string(' ', indent);

            // Novo valor adicionado - verde
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{indentStr}+ {key}: {value} (Novo)");
            Console.ResetColor();
        }

        public static void DisplayRemovedValue(string key, string value, int indent = 0)
        {
            string indentStr = new string(' ', indent);

            // Valor removido - amarelo (alerta)
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{indentStr}- {key}: {value} (Removido)");
            Console.ResetColor();
        }

        public static void DisplayCleaningResult(string component, bool success, int indent = 0)
        {
            string indentStr = new string(' ', indent);

            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{indentStr}✓ {component}: Limpo com sucesso");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{indentStr}✗ {component}: Falha na limpeza");
            }

            Console.ResetColor();
        }

        public static void DisplayNoChanges(int indent = 0)
        {
            string indentStr = new string(' ', indent);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{indentStr}Nenhum valor foi alterado");
            Console.ResetColor();
        }

        public static void DisplayComponentHeader(string componentName)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"===== {componentName} =====");
            Console.ResetColor();
        }
    }
}