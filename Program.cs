using System;
using System.Text;

namespace CaravanAdventure
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.Unicode;
            // Игровое поле
            int mapWidth = 25, mapHeight = 15;
            int offsetY = 2;
            // Сущности
            int playerHealth = 100, caravanHealth = 1000, playerDamage = 25,banditHealth = 0, banditDamage = 14;
            // Координаты
            int playerX = 1, playerY = 1;
            int caravanX = 2, caravanY = 2;
            // Ресурсы
            int collectedResources = 0, soldResources = 0, gold = 0, resourcePrice = 10, goalGold = 80;
            bool isGameOver = false;
            string gameResult = "";
            // Шансы
            int resourceSpawnChance = 20, banditAttackChance = 25;
            int hudCursorPosition = mapHeight + offsetY, msgCursorPosition = mapHeight + offsetY + 1, playerInputPosition = mapHeight + offsetY + 2;
            Random random = new Random();
            Console.Clear();
            Console.WriteLine("== КАРАВАН: путь через опасные земли ==");
            Console.WriteLine("И - игрок, @ - караван. Управление: W/A/S/D - ход, E - продать, Esc - выход");

            // вывод игрового поля
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                    Console.Write('*');
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(caravanX, caravanY + offsetY);
            Console.Write('@');
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(playerX, playerY + offsetY);
            Console.Write('И');
            Console.ResetColor();

            // Игровой цикл
            while (!isGameOver)
            {
                // вывод строки состояния
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.SetCursorPosition(0, hudCursorPosition);
                Console.Write($"♥ Игрок: {playerHealth}/100 | Караван: {caravanHealth}/1000 | " +
                    $"Ресурсы: {collectedResources} (продано {soldResources}) | Золото: {gold}   ");
                Console.ResetColor();
                // Бой
                if (banditHealth > 0)
                {
                    Console.SetCursorPosition(0, msgCursorPosition);
                    Console.Write($"Разбойник атакует. Его здоровье: {banditHealth}  ");

                    Console.SetCursorPosition(0, playerInputPosition);
                    Console.Write("Ваш ход (R - атаковать, F - бежать): ");
                    var battleKey = Console.ReadKey(true).Key;

                    switch (battleKey)
                    {
                        case ConsoleKey.R:
                            int playerHit = random.Next(20, 34);
                            banditHealth -= playerHit;
                            if (banditHealth <= 0)
                            {
                                int loot = random.Next(1, 4);
                                collectedResources += loot;
                                Console.SetCursorPosition(0, msgCursorPosition);
                                Console.Write($"Вы нанесли {playerHit} урона. Разбойник убит! Нашли {loot} ресурсов        ");
                            }
                            else
                            {
                                int banditHit = random.Next(10, 21);
                                playerHealth -= banditHit;
                                caravanHealth -= banditHit * 6;
                                Console.SetCursorPosition(0, msgCursorPosition);
                                Console.Write($"Вы ударили на {playerHit}. Разбойник в ответ: -{banditHit} HP игроку, -{banditHit * 6} HP каравану  ");
                            }
                            break;

                        case ConsoleKey.F:
                            int runHit = random.Next(10, 21);
                            playerHealth -= runHit * 2;
                            caravanHealth -= runHit * 12;
                            banditHealth = 0;
                            Console.SetCursorPosition(0, msgCursorPosition);
                            Console.Write($"Вы отступили, но получили -{runHit * 2} HP игроку, -{runHit * 12} HP каравану          ");
                            break;

                        default:
                            Console.SetCursorPosition(0, msgCursorPosition);
                            Console.Write("Неизвестная команда, бой продолжается        ");
                            break;
                    }
                    Console.SetCursorPosition(0, playerInputPosition);
                    Console.Write(new string(' ', 80));
                }
                else
                {
                    Console.SetCursorPosition(0, playerInputPosition);
                    Console.Write("Ваш ход (W/A/S/D): ");
                    var key = Console.ReadKey(true).Key;

                    switch (key)
                    {
                        case ConsoleKey.W:
                        case ConsoleKey.A:
                        case ConsoleKey.S:
                        case ConsoleKey.D:
                            int stepX = 0, stepY = 0;
                            if (key == ConsoleKey.A) stepX = -1;
                            if (key == ConsoleKey.D) stepX = 1;
                            if (key == ConsoleKey.W) stepY = -1;
                            if (key == ConsoleKey.S) stepY = 1;
                            if (playerX + stepX >= 0 && playerX + stepX < mapWidth &&
                                playerY + stepY >= 0 && playerY + stepY < mapHeight)
                            {
                                Console.SetCursorPosition(caravanX, caravanY + offsetY);
                                Console.Write('*');
                                Console.SetCursorPosition(playerX, playerY + offsetY);
                                Console.Write('*');
                                caravanX = playerX;
                                caravanY = playerY;
                                playerX += stepX;
                                playerY += stepY;
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.SetCursorPosition(caravanX, caravanY + offsetY);
                                Console.Write('@');
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.SetCursorPosition(playerX, playerY + offsetY);
                                Console.Write('И');
                                Console.ResetColor();

                                // Случайный разбойник
                                if (random.Next(100) < banditAttackChance)
                                {
                                    banditHealth = 100;
                                    Console.SetCursorPosition(0, msgCursorPosition);
                                    Console.Write("На караван напали разбойники   ");
                                }
                                // Случайный ресурс
                                else if (random.Next(100) < resourceSpawnChance)
                                {
                                    int found = random.Next(1, 4);
                                    collectedResources += found;
                                    Console.SetCursorPosition(0, msgCursorPosition);
                                    Console.Write($"Вы нашли ресурсы (+{found})              ");
                                }
                            }
                            break;

                        case ConsoleKey.E:
                            if (collectedResources == 0)
                            {
                                Console.SetCursorPosition(0, msgCursorPosition);
                                Console.Write("Нечего продавать   ");
                            }
                            else
                            {
                                int earned = collectedResources * resourcePrice;
                                gold += earned;
                                soldResources += collectedResources;
                                Console.SetCursorPosition(0, msgCursorPosition);
                                Console.Write($"Продано {collectedResources} ресурсов за {earned} золота            ");
                                collectedResources = 0;
                            }
                            break;

                        case ConsoleKey.Escape:
                            isGameOver = true;
                            gameResult = "Вы покинули игру.";
                            break;

                        default:
                            Console.SetCursorPosition(0, msgCursorPosition);
                            Console.Write("Неизвестная команда. Доступно: W, A, S, D, E(продать)        ");
                            break;
                    }
                    Console.SetCursorPosition(0, playerInputPosition);
                    Console.Write(new string(' ', 80));
                }

                // Проверка условий победы и поражения
                if ((gold >= goalGold) &&
                    playerHealth > 0 && caravanHealth > 0)
                {
                    isGameOver = true;
                    gameResult = "ПОБЕДА! Караван спасён и ресурсы проданы";
                }
                if (playerHealth <= 0)
                {
                    isGameOver = true;
                    gameResult = "ПОРАЖЕНИЕ! Игрок погиб";
                }
                if (caravanHealth <= 0)
                {
                    isGameOver = true;
                    gameResult = "ПОРАЖЕНИЕ! Караван уничтожен разбойниками";
                }
            }

            Console.SetCursorPosition(0, playerInputPosition);
            Console.WriteLine(gameResult);
            Console.WriteLine($"Итог: золото {gold}, продано ресурсов {soldResources}");
        }
    }
}
