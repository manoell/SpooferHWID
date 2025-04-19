using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace SpooferHWID
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "HWID Spoofer | V1.2";

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

            // Disco
            ProcessHardwareComponent("Disco",
                () => Disk.GetDiskInfo(),
                () => { Disk.SpoofDisks(); return Disk.GetDiskInfo(); }
            );

            // CPU
            ProcessHardwareComponent("CPU",
                () => CPU.GetCPUInfo(),
                () => { CPU.SpoofCPU(); return CPU.GetCPUInfo(); }
            );

            // GPU
            ProcessHardwareComponent("GPU",
                () => GPU.GetGPUInfo(),
                () => { GPU.SpoofGPU(); return GPU.GetGPUInfo(); }
            );

            // RAM
            ProcessHardwareComponent("RAM",
                () => RAM.GetRAMInfo(),
                () => { RAM.SpoofRAM(); return RAM.GetRAMInfo(); }
            );

            // Motherboard
            ProcessHardwareComponent("Motherboard",
                () => Motherboard.GetMotherboardInfo(),
                () => { Motherboard.SpoofMotherboard(); return Motherboard.GetMotherboardInfo(); }
            );

            // BIOS
            ProcessHardwareComponent("BIOS",
                () => Bios.GetBiosInfo(),
                () => { Bios.SpoofBIOS(); return Bios.GetBiosInfo(); }
            );

            // Network/MAC
            ProcessHardwareComponent("Network/MAC",
                () => Network.GetNetworkInfo(),
                () => { Network.SpoofMAC(); return Network.GetNetworkInfo(); }
            );

            // EFI
            ProcessHardwareComponent("EFI",
                () => EFI.GetEFIInfo(),
                () => { EFI.SpoofEFI(); return EFI.GetEFIInfo(); }
            );

            // TPM
            ProcessHardwareComponent("TPM",
                () => TPM.GetTPMInfo(),
                () => { TPM.SpoofTPM(); return TPM.GetTPMInfo(); }
            );

            // HVCI
            ProcessHardwareComponent("HVCI",
                () => HVCI.GetHVCIInfo(),
                () => { HVCI.ConfigureHVCI(); return HVCI.GetHVCIInfo(); }
            );

            // Boot
            ProcessHardwareComponent("Boot",
                () => Boot.GetBootInfo(),
                () => { Boot.SpoofBoot(); return Boot.GetBootInfo(); }
            );

            // Anti-Cheats - Usando o novo método que retorna (nome, encontrado, sucesso)
            ProcessCleaningComponent("Anti-Cheats", () => {
                return CleanAntiCheats.CleanAllAntiCheats();
            });

            // Games - Usando o novo método que retorna (nome, encontrado, sucesso)
            ProcessCleaningComponent("Games", () => {
                return CleanGames.CleanAllGames();
            });

            // System - Usando o novo método que retorna (nome, encontrado, sucesso)
            ProcessCleaningComponent("Sistema", () => {
                return CleanSystem.CleanSystemTraces();
            });

            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu principal...");
            Console.ReadKey();
        }

        private static void ProcessHardwareComponent(string componentName, Func<Dictionary<string, string>> getInfo, Func<Dictionary<string, string>> spoofAndGetInfo)
        {
            try
            {
                UI.DisplayComponentHeader(componentName);
                Console.WriteLine($"[*] Spoofing {componentName}...");

                // Obter informações originais
                var before = getInfo();
                Console.WriteLine($"* Valores atuais ({componentName}):");
                DisplayValues(before, 3);

                // Executar spoofing e obter novas informações
                var after = spoofAndGetInfo();

                // Comparar e mostrar resultados
                Console.WriteLine($"\n* Resultados ({componentName}):");

                // Usar a extensão para comparar os dicionários
                var comparison = before.CompareWith(after);
                var changes = comparison.GetChangedValues();

                if (changes.Count > 0)
                {
                    // Mostrar valores alterados/adicionados/removidos
                    DisplayComparisonResults(changes, 3);
                }
                else
                {
                    // Nenhuma alteração
                    UI.DisplayNoChanges(3);
                }

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[X] Erro ao processar {componentName}: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static void ProcessCleaningComponent(string componentName, Func<List<(string, bool, bool)>> cleanAction)
        {
            try
            {
                UI.DisplayComponentHeader(componentName);

                // Executar limpeza e obter resultados
                // Formato do retorno: (nome, encontrado, limpo)
                var resultados = cleanAction();

                // Mostrar resultados
                Console.WriteLine($"\n* Resultados de Limpeza ({componentName}):");

                int encontrados = 0;
                int limpos = 0;

                foreach (var (item, encontrado, sucesso) in resultados)
                {
                    if (!encontrado)
                    {
                        // Não encontrado - cor cinza
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine($"  • {item}: Não encontrado");
                        Console.ResetColor();
                    }
                    else if (sucesso)
                    {
                        // Encontrado e limpo - cor verde
                        encontrados++;
                        limpos++;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"  ✓ {item}: Encontrado e limpo com sucesso");
                        Console.ResetColor();
                    }
                    else
                    {
                        // Encontrado mas falha na limpeza - cor vermelha
                        encontrados++;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"  ✗ {item}: Encontrado, mas falha na limpeza");
                        Console.ResetColor();
                    }
                }

                // Resumo
                Console.WriteLine($"\n  Resumo: {encontrados} componentes encontrados, {limpos} limpos com sucesso");

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[X] Erro ao processar {componentName}: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static void DisplayValues(Dictionary<string, string> values, int indent = 0)
        {
            string indentStr = new string(' ', indent);

            // Exibir no máximo 5 valores para não sobrecarregar a tela
            int count = 0;
            foreach (var pair in values.Take(5))
            {
                Console.WriteLine($"{indentStr}{pair.Key}: {pair.Value}");
                count++;
            }

            // Se houver mais valores, indicar a quantidade
            if (count < values.Count)
            {
                Console.WriteLine($"{indentStr}... e mais {values.Count - count} valores");
            }

            // Se não houver valores, informar
            if (values.Count == 0)
            {
                Console.WriteLine($"{indentStr}Nenhuma informação disponível");
            }
        }

        private static void DisplayComparisonResults(Dictionary<string, (bool Success, string BeforeValue, string AfterValue, ChangeType Type)> changes, int indent = 0)
        {
            string indentStr = new string(' ', indent);

            // Limitar o número de itens exibidos
            int maxDisplay = 10;
            int displayCount = 0;

            // Primeiro mostrar valores modificados
            foreach (var kvp in changes.Where(x => x.Value.Type == ChangeType.Modified).Take(maxDisplay))
            {
                DisplayModifiedValue(kvp.Key, kvp.Value.BeforeValue, kvp.Value.AfterValue, indent);
                displayCount++;
            }

            // Depois mostrar valores adicionados
            foreach (var kvp in changes.Where(x => x.Value.Type == ChangeType.Added).Take(maxDisplay - displayCount))
            {
                UI.DisplayNewValue(kvp.Key, kvp.Value.AfterValue, indent);
                displayCount++;
            }

            // Por fim mostrar valores removidos
            foreach (var kvp in changes.Where(x => x.Value.Type == ChangeType.Removed).Take(maxDisplay - displayCount))
            {
                UI.DisplayRemovedValue(kvp.Key, kvp.Value.BeforeValue, indent);
                displayCount++;
            }

            // Se houver mais alterações do que o exibido, indicar
            if (displayCount >= maxDisplay && changes.Count > maxDisplay)
            {
                Console.WriteLine($"{indentStr}... e mais {changes.Count - displayCount} alterações");
            }
        }

        private static void DisplayModifiedValue(string key, string beforeValue, string afterValue, int indent = 0)
        {
            string indentStr = new string(' ', indent);

            // Exibir a chave
            Console.WriteLine($"{indentStr}{key}:");

            // Exibir valor anterior
            Console.WriteLine($"{indentStr}  Atual: {beforeValue}");

            // Exibir valor alterado em verde
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{indentStr}  Alterado: {afterValue}");
            Console.ResetColor();
        }
    }
}