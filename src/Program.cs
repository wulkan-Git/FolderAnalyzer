using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace FolderAnalyzer
{
    internal static class Program
    {
        /// <summary>
        /// Получение иконки приложения
        /// </summary>
        public static Icon AppIcon { get; private set; }

        [STAThread]
        static void Main()
        {
            // Загружаем иконку при запуске
            LoadApplicationIcon();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void LoadApplicationIcon()
        {
            try
            {
                // Способ 1: Ищем файл иконки рядом с exe
                string exePath = Assembly.GetExecutingAssembly().Location;
                string exeDirectory = Path.GetDirectoryName(exePath);
                string[] possibleIconPaths =
                {
                    Path.Combine(exeDirectory, "App.ico"),
                    Path.Combine(exeDirectory, "FolderAnalyzer.ico"),
                    Path.Combine(exeDirectory, "icon.ico"),
                    "App.ico",
                    "FolderAnalyzer.ico",
                    "icon.ico"
                };

                foreach (string path in possibleIconPaths)
                {
                    if (File.Exists(path))
                    {
                        AppIcon = new Icon(path);
                        return;
                    }
                }

                // Способ 2: Извлекаем иконку из exe файла
                AppIcon = Icon.ExtractAssociatedIcon(exePath);
            }
            catch (Exception ex)
            {
                // Если не удалось загрузить иконку, оставляем null
                Console.WriteLine($"Ошибка загрузки иконки: {ex.Message}");
                AppIcon = null;
            }
        }
    }
}