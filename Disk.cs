using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpooferHWID
{
    public static class Disk
    {
        public static Dictionary<string, string> GetDiskInfo()
        {
            var result = new Dictionary<string, string>();

            try
            {
                // Capturar informações dos discos via SCSI
                using (RegistryKey ScsiPorts = Registry.LocalMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\Scsi"))
                {
                    if (ScsiPorts != null)
                    {
                        foreach (string port in ScsiPorts.GetSubKeyNames())
                        {
                            using (RegistryKey ScsiBuses = Registry.LocalMachine.OpenSubKey($"HARDWARE\\DEVICEMAP\\Scsi\\{port}"))
                            {
                                if (ScsiBuses != null)
                                {
                                    foreach (string bus in ScsiBuses.GetSubKeyNames())
                                    {
                                        using (RegistryKey ScsuiBus = Registry.LocalMachine.OpenSubKey($"HARDWARE\\DEVICEMAP\\Scsi\\{port}\\{bus}\\Target Id 0\\Logical Unit Id 0"))
                                        {
                                            if (ScsuiBus != null && ScsuiBus.GetValue("DeviceType")?.ToString() == "DiskPeripheral")
                                            {
                                                string keyPath = $"SCSI Port {port} Bus {bus}";

                                                if (ScsuiBus.GetValue("Identifier") != null)
                                                    result[$"{keyPath} Identifier"] = ScsuiBus.GetValue("Identifier").ToString();

                                                if (ScsuiBus.GetValue("SerialNumber") != null)
                                                    result[$"{keyPath} SerialNumber"] = ScsuiBus.GetValue("SerialNumber").ToString();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Capturar informações via DiskPeripherals
                using (RegistryKey DiskPeripherals = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\MultifunctionAdapter\\0\\DiskController\\0\\DiskPeripheral"))
                {
                    if (DiskPeripherals != null)
                    {
                        foreach (string disk in DiskPeripherals.GetSubKeyNames())
                        {
                            using (RegistryKey DiskPeripheral = Registry.LocalMachine.OpenSubKey($"HARDWARE\\DESCRIPTION\\System\\MultifunctionAdapter\\0\\DiskController\\0\\DiskPeripheral\\{disk}"))
                            {
                                if (DiskPeripheral != null && DiskPeripheral.GetValue("Identifier") != null)
                                {
                                    result[$"DiskPeripheral {disk} Identifier"] = DiskPeripheral.GetValue("Identifier").ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter informações de disco: {ex.Message}");
            }

            return result;
        }

        public static bool SpoofDisks()
        {
            try
            {
                // Spoofing via SCSI
                using (RegistryKey ScsiPorts = Registry.LocalMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\Scsi"))
                {
                    if (ScsiPorts != null)
                    {
                        foreach (string port in ScsiPorts.GetSubKeyNames())
                        {
                            using (RegistryKey ScsiBuses = Registry.LocalMachine.OpenSubKey($"HARDWARE\\DEVICEMAP\\Scsi\\{port}"))
                            {
                                if (ScsiBuses != null)
                                {
                                    foreach (string bus in ScsiBuses.GetSubKeyNames())
                                    {
                                        using (RegistryKey ScsuiBus = Registry.LocalMachine.OpenSubKey($"HARDWARE\\DEVICEMAP\\Scsi\\{port}\\{bus}\\Target Id 0\\Logical Unit Id 0", true))
                                        {
                                            if (ScsuiBus != null && ScsuiBus.GetValue("DeviceType")?.ToString() == "DiskPeripheral")
                                            {
                                                string identifier = Utils.RandomId(14);
                                                string serialNumber = Utils.RandomId(14);

                                                ScsuiBus.SetValue("DeviceIdentifierPage", Encoding.UTF8.GetBytes(serialNumber));
                                                ScsuiBus.SetValue("Identifier", identifier);
                                                ScsuiBus.SetValue("InquiryData", Encoding.UTF8.GetBytes(identifier));
                                                ScsuiBus.SetValue("SerialNumber", serialNumber);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Spoofing via DiskPeripherals
                using (RegistryKey DiskPeripherals = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\MultifunctionAdapter\\0\\DiskController\\0\\DiskPeripheral"))
                {
                    if (DiskPeripherals != null)
                    {
                        foreach (string disk in DiskPeripherals.GetSubKeyNames())
                        {
                            using (RegistryKey DiskPeripheral = Registry.LocalMachine.OpenSubKey($"HARDWARE\\DESCRIPTION\\System\\MultifunctionAdapter\\0\\DiskController\\0\\DiskPeripheral\\{disk}", true))
                            {
                                if (DiskPeripheral != null)
                                {
                                    DiskPeripheral.SetValue("Identifier", $"{Utils.RandomId(8)}-{Utils.RandomId(8)}-A");
                                }
                            }
                        }
                    }
                }

                // Spoofing Volume IDs (opcional se necessário)
                // Código para spoofar IDs de volume aqui

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao modificar discos: {ex.Message}");
                return false;
            }
        }
    }
}