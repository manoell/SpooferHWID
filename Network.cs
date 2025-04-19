using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace SpooferHWID
{
    public static class Network
    {
        public static Dictionary<string, string> GetNetworkInfo()
        {
            var result = new Dictionary<string, string>();

            try
            {
                // Obter informações sobre adaptadores de rede
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    // Obter endereço MAC físico
                    string mac = BitConverter.ToString(nic.GetPhysicalAddress().GetAddressBytes());
                    result[$"MAC_{nic.Name}"] = mac;

                    // Obter informações adicionais
                    result[$"ID_{nic.Name}"] = nic.Id;
                    result[$"Type_{nic.Name}"] = nic.NetworkInterfaceType.ToString();
                    result[$"Status_{nic.Name}"] = nic.OperationalStatus.ToString();

                    // Verificar informações de configuração IPv4
                    var ipProps = nic.GetIPProperties();
                    int ipv4Count = 0;
                    foreach (var ipAddr in ipProps.UnicastAddresses)
                    {
                        if (ipAddr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            result[$"IPv4_{nic.Name}_{ipv4Count}"] = ipAddr.Address.ToString();
                            ipv4Count++;
                        }
                    }
                }

                // Tentar obter informações do registro sobre os adaptadores de rede
                using (RegistryKey NetworkAdapters = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}"))
                {
                    if (NetworkAdapters != null)
                    {
                        foreach (string adapter in NetworkAdapters.GetSubKeyNames())
                        {
                            if (adapter != "Properties")
                            {
                                using (RegistryKey NetworkAdapter = NetworkAdapters.OpenSubKey(adapter))
                                {
                                    if (NetworkAdapter != null)
                                    {
                                        string adapterName = NetworkAdapter.GetValue("DriverDesc")?.ToString() ?? "Unknown";

                                        string networkAddress = NetworkAdapter.GetValue("NetworkAddress")?.ToString() ?? "Not Set";
                                        result[$"Registry_MAC_{adapterName}"] = networkAddress;

                                        string netCfgInstanceId = NetworkAdapter.GetValue("NetCfgInstanceId")?.ToString() ?? "Unknown";
                                        result[$"Registry_ID_{adapterName}"] = netCfgInstanceId;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter informações de rede: {ex.Message}");
            }

            return result;
        }

        private static bool EnableDisableNetworkAdapter(string adapterId, bool enable)
        {
            try
            {
                string interfaceName = "Ethernet";
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.Id == adapterId)
                    {
                        interfaceName = nic.Name;
                        break;
                    }
                }

                string control = enable ? "enable" : "disable";

                ProcessStartInfo psi = new ProcessStartInfo("netsh", $"interface set interface \"{interfaceName}\" {control}");
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;

                using (Process process = new Process())
                {
                    process.StartInfo = psi;
                    process.Start();
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao {(enable ? "ativar" : "desativar")} adaptador de rede: {ex.Message}");
                return false;
            }
        }

        public static bool SpoofMAC()
        {
            try
            {
                bool anySuccess = false;

                using (RegistryKey NetworkAdapters = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}"))
                {
                    if (NetworkAdapters != null)
                    {
                        foreach (string adapter in NetworkAdapters.GetSubKeyNames())
                        {
                            if (adapter != "Properties")
                            {
                                try
                                {
                                    using (RegistryKey NetworkAdapter = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Control\Class\{{4d36e972-e325-11ce-bfc1-08002be10318}}\{adapter}", true))
                                    {
                                        if (NetworkAdapter != null && NetworkAdapter.GetValue("BusType") != null)
                                        {
                                            // Gerar um novo endereço MAC aleatório
                                            string newMac = Utils.RandomMac();

                                            // Salvar o endereço MAC atual para possível restauração posterior
                                            string currentMac = NetworkAdapter.GetValue("NetworkAddress")?.ToString() ?? "";

                                            // Definir o novo endereço MAC
                                            NetworkAdapter.SetValue("NetworkAddress", newMac);

                                            // Obter o ID da instância do adaptador
                                            string adapterId = NetworkAdapter.GetValue("NetCfgInstanceId")?.ToString() ?? "";

                                            if (!string.IsNullOrEmpty(adapterId))
                                            {
                                                // Desativar e reativar o adaptador para aplicar a alteração
                                                EnableDisableNetworkAdapter(adapterId, false);
                                                Thread.Sleep(1000); // Esperar um momento
                                                EnableDisableNetworkAdapter(adapterId, true);
                                            }

                                            anySuccess = true;
                                        }
                                    }
                                }
                                catch (System.Security.SecurityException)
                                {
                                    Console.WriteLine("\n[X] Inicie o spoofer no modo administrador para spoofar endereços MAC!");
                                    return false;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Erro ao modificar MAC para adaptador {adapter}: {ex.Message}");
                                }
                            }
                        }
                    }
                }

                return anySuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao spoofar MAC: {ex.Message}");
                return false;
            }
        }

        // Métodos adicionais para limpar tabelas ARP, DHCP, etc.
        public static bool ClearARPCache()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("netsh", "interface ip delete arpcache");
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;

                using (Process process = new Process())
                {
                    process.StartInfo = psi;
                    process.Start();
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar cache ARP: {ex.Message}");
                return false;
            }
        }

        public static bool FlushDNS()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("ipconfig", "/flushdns");
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;

                using (Process process = new Process())
                {
                    process.StartInfo = psi;
                    process.Start();
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar cache DNS: {ex.Message}");
                return false;
            }
        }

        public static bool RenewDHCP()
        {
            try
            {
                // Liberar endereços DHCP
                ProcessStartInfo psiRelease = new ProcessStartInfo("ipconfig", "/release");
                psiRelease.CreateNoWindow = true;
                psiRelease.UseShellExecute = false;
                psiRelease.RedirectStandardOutput = true;
                psiRelease.RedirectStandardError = true;

                using (Process processRelease = new Process())
                {
                    processRelease.StartInfo = psiRelease;
                    processRelease.Start();
                    processRelease.WaitForExit();
                }

                // Renovar endereços DHCP
                ProcessStartInfo psiRenew = new ProcessStartInfo("ipconfig", "/renew");
                psiRenew.CreateNoWindow = true;
                psiRenew.UseShellExecute = false;
                psiRenew.RedirectStandardOutput = true;
                psiRenew.RedirectStandardError = true;

                using (Process processRenew = new Process())
                {
                    processRenew.StartInfo = psiRenew;
                    processRenew.Start();
                    processRenew.WaitForExit();
                    return processRenew.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao renovar DHCP: {ex.Message}");
                return false;
            }
        }
    }
}