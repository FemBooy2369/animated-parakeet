using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp14.Tasks
{
    public class NumeroSiete
    {
        public static void Execute()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== МЕНЕДЖЕР ФОТОАРХИВА ===");
            Console.ResetColor();

            string defaultInputPath = GetDefaultInputPath();
            string korzinaPath = GetKorzinaPath();

            Console.Write($"Введите путь к папке с фото (Enter = DataTests\\task7_photos): ");
            string inputFolder = Console.ReadLine()?.Trim('\"', ' ');

            if (string.IsNullOrEmpty(inputFolder))
                inputFolder = defaultInputPath;

            if (!Directory.Exists(inputFolder))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Указанная папка не существует!");
                Console.ResetColor();
                return;
            }

            string[] extensions = { ".jpg", ".jpeg", ".png", ".bmp" };

            var filesToMove = new List<(string source, string destination)>();

            Console.WriteLine("\nСканирование папки...");
            ScanAndPlan(inputFolder, korzinaPath, extensions, filesToMove);

            if (filesToMove.Count == 0)
            {
                Console.WriteLine("Изображения для сортировки не найдены.");
                return;
            }

            Console.WriteLine($"\nНайдено файлов: {filesToMove.Count}");
            Console.WriteLine("\nПЛАН ДЕЙСТВИЙ (в Korzina):");
            foreach (var item in filesToMove)
            {
                Console.WriteLine($"  {Path.GetFileName(item.source)}  →  {GetRelativePath(korzinaPath, item.destination)}");
            }

            Console.Write("\nПодтверждаете перемещение? (да / нет): ");
            string confirm = Console.ReadLine()?.Trim().ToLower();

            if (confirm != "да" && confirm != "y" && confirm != "yes")
            {
                Console.WriteLine("Операция отменена.");
                return;
            }

            ExecuteMove(filesToMove);
        }

        private static void ScanAndPlan(string inputFolder, string korzinaPath, string[] extensions, List<(string, string)> filesToMove)
        {
            try
            {
                foreach (string file in Directory.GetFiles(inputFolder, "*.*", SearchOption.AllDirectories))
                {
                    string ext = Path.GetExtension(file).ToLowerInvariant();
                    if (Array.Exists(extensions, e => e == ext))
                    {
                        DateTime lastModified = File.GetLastWriteTime(file);
                        string yearMonth = lastModified.ToString("yyyy-MM");
                        string targetFolder = Path.Combine(korzinaPath, yearMonth);

                        if (!Directory.Exists(targetFolder))
                            Directory.CreateDirectory(targetFolder);

                        string fileName = Path.GetFileName(file);
                        string targetPath = Path.Combine(targetFolder, fileName);

                        filesToMove.Add((file, targetPath));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка сканирования: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static void ExecuteMove(List<(string source, string destination)> filesToMove)
        {
            int success = 0;
            int errors = 0;

            foreach (var item in filesToMove)
            {
                try
                {
                    string finalDest = item.destination;
                    int counter = 1;
                    while (File.Exists(finalDest))
                    {
                        string dir = Path.GetDirectoryName(item.destination) ?? "";
                        string name = Path.GetFileNameWithoutExtension(item.destination);
                        string ext = Path.GetExtension(item.destination);
                        finalDest = Path.Combine(dir, $"{name} ({counter}){ext}");
                        counter++;
                    }

                    File.Move(item.source, finalDest);
                    success++;
                    Console.WriteLine($"Перемещено: {Path.GetFileName(item.source)}");
                }
                catch (Exception)
                {
                    errors++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Ошибка: {Path.GetFileName(item.source)}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Готово! Успешно: {success} | Ошибок: {errors}");
            Console.ResetColor();
        }

        private static string GetDefaultInputPath()
        {
            return @"C:\Users\МКА-ученик\source\repos\ConsoleApp14\ConsoleApp14\DataTests\task7_photos";
        }

        private static string GetKorzinaPath()
        {
            return @"C:\Users\МКА-ученик\source\repos\ConsoleApp14\ConsoleApp14\Korzina";
        }

        private static string GetRelativePath(string root, string fullPath)
        {
            return fullPath.StartsWith(root) ? fullPath.Substring(root.Length + 1) : fullPath;
        }
    }
}