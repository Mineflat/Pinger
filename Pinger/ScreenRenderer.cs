using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace Pinger
{
    internal class ScreenRenderer
    {
        public static void RenderScreen(List<TargetHost> hosts, bool onlyUP = false, int MaxRowsPerBlock = 30, bool showHostnames = true)
        {
            // Фильтруем, если нужно только «UP»-хосты
            var filtered = onlyUP
                ? hosts.Where(h => h.ISMP_OK).ToList()
                : hosts;

            if (filtered == null || filtered.Count == 0)
            {
                Program.OnPanic("Не найдено ни одного хоста для вывода информации");
                return;
            }
            // Разбиваем на чанки по MaxRowsPerBlock хостов
            var chunks = filtered
                .Select((host, idx) => new { host, idx })
                .GroupBy(x => x.idx / MaxRowsPerBlock)
                .Select(g => g.Select(x => x.host).ToList())
                .ToList();

            // Строим таблицу
            var table = new Table()
                .Border(TableBorder.Rounded)
                .Expand()
                .Caption($"ALL HOST STATUSES IN THIS NETWORK (total {filtered.Count})");

            // Для каждого чанка добавляем две колонки
            foreach (var chunk in chunks)
            {
                table.AddColumn(new TableColumn("IP ADDRESS").Centered());
                table.AddColumn(new TableColumn("STATUS").Centered());
            }

            // Определяем, сколько строк нужно — не больше 15
            int rows = chunks.Max(c => c.Count);

            // Заполняем строки
            for (int row = 0; row < rows; row++)
            {
                var cells = new List<string>();

                foreach (var chunk in chunks)
                {
                    if (row < chunk.Count)
                    {
                        var h = chunk[row];
                        string ipText = string.Empty;
                        // Логика подставление hostname
                        if (showHostnames) ipText = h.ISMP_OK ? $"[green]{h.Hostname ?? h.IP}[/]" : $"[grey]{h.Hostname ?? h.IP}[/]";
                        else  ipText = h.ISMP_OK ? $"[green]{h.IP}[/]" : $"[grey]{h.IP}[/]";

                        var statusText = h.ISMP_OK ? "[green]UP[/]" : "[grey]DOWN[/]";
                        cells.Add(ipText);
                        cells.Add(statusText);
                    }
                    else
                    {
                        // Если в этом чанке нет хоста на этой строке — пустые ячейки
                        cells.Add("");
                        cells.Add("");
                    }
                }

                table.AddRow(cells.ToArray());
            }

            AnsiConsole.Write(table);
        }
    }
}
