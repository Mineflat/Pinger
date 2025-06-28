using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace Pinger
{
    internal static class Pinger
    {
        /// <summary>
        /// Получает список всех IPv4-адресов из подсети (включая .0 и broadcast).
        /// </summary>
        private static List<string>? GetHosts(string networkCIDR)
        {
            List<string> hosts = new List<string>();
            var parts = networkCIDR.Split('/');
            if (parts.Length != 2)
            {
                Program.OnPanic("Неверный формат CIDR. Ожидается x.x.x.x/y");
                throw new ArgumentException("Неверный формат CIDR. Ожидается x.x.x.x/y");
            }

            IPAddress baseIp = IPAddress.Parse(parts[0]);
            int prefixLength = int.Parse(parts[1]);
            if (prefixLength < 0 || prefixLength > 32)
            {
                Program.OnPanic("Префикс должен быть от 0 до 32");
                throw new ArgumentException("Префикс должен быть от 0 до 32");
            }

            // Преобразуем IP в UInt32 (сетевой порядок байт → хостовый порядок)
            uint ip = BitConverter.ToUInt32(baseIp.GetAddressBytes().Reverse().ToArray(), 0);
            // Генерируем маску
            uint mask = prefixLength == 0
                ? 0
                : 0xFFFFFFFFu << (32 - prefixLength);
            // Находим адрес сети и широковещательный адрес
            uint network = ip & mask;
            uint broadcast = network | ~mask;

            for (uint addr = network; addr <= broadcast; addr++)
            {
                // Обратно в байты в сетевом порядке
                byte[] bytes = BitConverter.GetBytes(addr)
                                          .Reverse()
                                          .ToArray();
                hosts.Add(new IPAddress(bytes).ToString());
            }

            return hosts;
        }
        /// <summary>
        /// Асинхронно пингует все адреса из указанного networkCIDR параллельно.
        /// </summary>
        /// <param name="networkCIDR">Строка вида "10.10.10.0/24"</param>
        /// <param name="timeoutMs">Таймаут для каждого пинга в миллисекундах</param>
        public static async Task<List<TargetHost>> UpdateHostList(string networkCIDR, int timeoutMs)
        {
            // Генерация полного списка адресов
            var allHosts = GetHosts(networkCIDR)
                ?? throw new Exception("В указанном CIDR нет ни одного адреса");

            // Запускаем пинг всех хостов параллельно
            var pingTasks = allHosts
                .Select(host => PingHostAsync(host, timeoutMs))
                .ToArray();

            // Ждём завершения всех задач
            TargetHost[] results = await Task.WhenAll(pingTasks);

            // Возвращаем в виде списка
            return results.ToList();
        }

        /// <summary>
        /// Пинг одного хоста с собственным таймаутом-методом.
        /// </summary>
        private static async Task<TargetHost> PingHostAsync(string host, int timeoutMs)
        {
            using var ping = new Ping();
            var pingTask = ping.SendPingAsync(host, timeoutMs);
            // Задача-таймаут
            var timeoutTask = Task.Delay(timeoutMs);

            var finished = await Task.WhenAny(pingTask, timeoutTask);
            if (finished == pingTask)
            {
                try
                {
                    var reply = await pingTask; // убедимся, что не упало с исключением
                    return new TargetHost
                    {
                        IP = host,
                        ISMP_OK = reply.Status == IPStatus.Success
                    };
                }
                catch (PingException)
                {
                    // Если SendPingAsync упал до таймаута
                    return new TargetHost
                    {
                        IP = host,
                        ISMP_OK = false
                    };
                }
            }
            else
            {
                // Превышен таймаут ожидания самого метода
                return new TargetHost
                {
                    IP = host,
                    ISMP_OK = false
                };
            }
        }



    }
}
