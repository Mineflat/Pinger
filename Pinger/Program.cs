namespace Pinger
{
    internal class Program
    {
        /// <summary>
        /// Метод используется чтобы красиво завершить приложение с ошибкой
        /// </summary>
        public static void OnPanic(string errorMessage = "")
        {
            Console.WriteLine($"Usage:\n\t{AppDomain.CurrentDomain.FriendlyName}  [x.x.x.x/y (CIDR)] [--only-up (hides down hosts)] [MaxRowsPerBlock (as POSITIVE DIGIT, means table height)] [--no-hostnames (hides hostnames in table)]\n");
            if (!string.IsNullOrEmpty(errorMessage)) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"Commom errors: {errorMessage}"); Console.ResetColor(); }
            Environment.Exit(1);
        }
        /// <summary>
        /// Точка входа в приложение
        /// </summary>
        /// <param name="args">Передаваемые аргументы</param>
        static async Task Main(string[] args)
        {
            // Проверяем общее число аргументов
            if (args.Length < 1 || args.Length > 3)
                OnPanic();
            // Первый аргумент — CIDR
            string cidr = args[0];
            // Значения по умолчанию
            bool onlyUP = false;
            bool showHostnames = true;
            int maxRowsPerBlock = 30;
            // Разбираем остальные аргументы в любом порядке
            for (int i = 1; i < args.Length; i++)
            {
                var a = args[i];
                if (a.Equals("--only-up", StringComparison.OrdinalIgnoreCase))
                {
                    onlyUP = true;
                }
                else if (int.TryParse(a, out var rows) && rows > 0)
                {
                    maxRowsPerBlock = rows;
                }
                else if (a.Equals("--no-hostnames", StringComparison.OrdinalIgnoreCase))
                {
                    showHostnames = false;
                }
                else
                        {
                    OnPanic($"Unknown or invalid argument: {a}");
                }
            }
            // Делаем сам пинг
            var hostList = await Pinger.UpdateHostList(cidr, timeoutMs: 1000);
            // Рендерим таблицу
            ScreenRenderer.RenderScreen(hostList, onlyUP, maxRowsPerBlock, showHostnames);
        }
    }
}
