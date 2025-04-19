using System;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Intrinsics.Arm;

namespace SpooferHWID
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "HWID Spoofer | V1.0";

            while (true)
            {
                Console.Clear();
                UI.DisplayHeader();
                UI.DisplayModulesList();
                UI.DisplayMenuOptions();

                string input = Console.ReadLine()?.Trim().ToLower() ?? "";

                switch (input)
                {
                    case "1": // Spoof ALL
                        SpoofAllHardware();
                        break;
                    case "exit":
                    case "2":
                        Console.WriteLine("\nSaindo do programa...");
                        Thread.Sleep(1500);
                        return;
                    default:
                        Console.WriteLine("\n[X] Opção inválida. Tente novamente.");
                        Thread.Sleep(1500);
                        break;
                }
            }
        }

        private static void SpoofAllHardware()
        {
            Console.Clear();
            UI.DisplayHeader();
            Console.WriteLine("\nIniciando processo de spoofing de todos os componentes...\n");

            // Lista para armazenar resultados de cada operação
            var results = new List<(string Component, bool Success)>();

            // Disco
            results.Add(("Disco", SpoofComponent(() => {
                var before = Disk.GetDiskInfo();
                Console.WriteLine("[*] Spoofing Discos...");
                Disk.SpoofDisks();
                var after = Disk.GetDiskInfo();
                return CompareResults(before, after);
            })));

            // CPU
            results.Add(("CPU", SpoofComponent(() => {
                var before = CPU.GetCPUInfo();
                Console.WriteLine("[*] Spoofing CPU...");
                CPU.SpoofCPU();
                var after = CPU.GetCPUInfo();
                return CompareResults(before, after);
            })));

            // GPU
            results.Add(("GPU", SpoofComponent(() => {
                var before = GPU.GetGPUInfo();
                Console.WriteLine("[*] Spoofing GPU...");
                GPU.SpoofGPU();
                var after = GPU.GetGPUInfo();
                return CompareResults(before, after);
            })));

            // RAM
            results.Add(("RAM", SpoofComponent(() => {
                var before = RAM.GetRAMInfo();
                Console.WriteLine("[*] Spoofing RAM...");
                RAM.SpoofRAM();
                var after = RAM.GetRAMInfo();
                return CompareResults(before, after);
            })));

            // Motherboard
            results.Add(("Motherboard", SpoofComponent(() => {
                var before = Motherboard.GetMotherboardInfo();
                Console.WriteLine("[*] Spoofing Motherboard...");
                Motherboard.SpoofMotherboard();
                var after = Motherboard.GetMotherboardInfo();
                return CompareResults(before, after);
            })));

            // BIOS
            results.Add(("BIOS", SpoofComponent(() => {
                var before = Bios.GetBiosInfo();
                Console.WriteLine("[*] Spoofing BIOS...");
                Bios.SpoofBIOS();
                var after = Bios.GetBiosInfo();
                return CompareResults(before, after);
            })));

            // Network/MAC
            results.Add(("Network/MAC", SpoofComponent(() => {
                var before = Network.GetNetworkInfo();
                Console.WriteLine("[*] Spoofing Network/MAC...");
                Network.SpoofMAC();
                var after = Network.GetNetworkInfo();
                return CompareResults(before, after);
            })));

            // EFI
            results.Add(("EFI", SpoofComponent(() => {
                var before = EFI.GetEFIInfo();
                Console.WriteLine("[*] Spoofing EFI...");
                EFI.SpoofEFI();
                var after = EFI.GetEFIInfo();
                return CompareResults(before, after);
            })));

            // TPM
            results.Add(("TPM", SpoofComponent(() => {
                var before = TPM.GetTPMInfo();
                Console.WriteLine("[*] Spoofing TPM...");
                TPM.SpoofTPM();
                var after = TPM.GetTPMInfo();
                return CompareResults(before, after);
            })));

            // HVCI
            results.Add(("HVCI", SpoofComponent(() => {
                var before = HVCI.GetHVCIInfo();
                Console.WriteLine("[*] Configurando HVCI...");
                HVCI.ConfigureHVCI();
                var after = HVCI.GetHVCIInfo();
                return CompareResults(before, after);
            })));

            // Boot
            results.Add(("Boot", SpoofComponent(() => {
                var before = Boot.GetBootInfo();
                Console.WriteLine("[*] Spoofing Boot...");
                Boot.SpoofBoot();
                var after = Boot.GetBootInfo();
                return CompareResults(before, after);
            })));

            // Limpeza de Anti-Cheats
            results.Add(("Anti-Cheats", SpoofComponent(() => {
                Console.WriteLine("[*] Limpando Anti-Cheats...");
                return CleanAntiCheats.CleanAllAntiCheats();
            })));

            // Limpeza de Jogos
            results.Add(("Games", SpoofComponent(() => {
                Console.WriteLine("[*] Limpando rastros de jogos...");
                return CleanGames.CleanAllGames();
            })));

            // Limpeza de Sistema
            results.Add(("System", SpoofComponent(() => {
                Console.WriteLine("[*] Limpando rastros do sistema...");
                return CleanSystem.CleanSystemTraces();
            })));

            // Exibir resultados
            Console.WriteLine("\n\nResultados do processo de spoofing:");
            Console.WriteLine("=====================================");

            foreach (var result in results)
            {
                if (result.Success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[√] {result.Component}: Modificado com sucesso");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[X] {result.Component}: Falha na modificação");
                }
                Console.ResetColor();
            }

            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu principal...");
            Console.ReadKey();
        }

        private static bool SpoofComponent(Func<bool> spoofAction)
        {
            try
            {
                return spoofAction();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[X] Erro: {ex.Message}");
                Console.ResetColor();
                return false;
            }
        }

        private static bool CompareResults(Dictionary<string, string> before, Dictionary<string, string> after)
        {
            // Se os dicionários tiverem tamanhos diferentes, houve mudança
            if (before.Count != after.Count)
                return true;

            // Verifica se algum valor mudou
            foreach (var key in before.Keys)
            {
                // Se a chave não existir no "after" ou o valor for diferente, houve mudança
                if (!after.ContainsKey(key) || before[key] != after[key])
                    return true;
            }

            // Nenhuma mudança detectada
            return false;
        }
    }
}