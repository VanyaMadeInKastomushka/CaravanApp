using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaravanAdventure
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            // Игровое поле
            int mapWidth = 25, mapHeight = 15;
            // Сущности
            int playerHealth = 100, caravanHealth = 1000, playerDamage = 25, banditHealth = 0, banditDamage = 14;
            // Координаты
            int playerX = 1, playerY = 1, caravanX = 2, caravanY = 2;
            // Ресурсы
            int collectedResources = 0, soldResources = 0, gold = 0, resourcePrice = 10, goalGold = 80;
            //Состояния
            bool isGameOver = false;
            bool isNewBattle = false;
            string gameResult = "";
            // Шансы
            int resourceSpawnChance = 20, banditAttackChance = 25;
            // Курсор
            int offsetY = 2;
            int hudCursorPosition = mapHeight + offsetY;
            int messageCursorPosition = mapHeight + offsetY + 1;
            int playerInputPosition = mapHeight + offsetY + 2;
            Random random = new Random();
            Console.Clear();
            Console.WriteLine("== КАРАВАН: путь через опасные земли ==");
            Console.WriteLine("И - игрок, @ - караван. Управление: W/A/S/D - ход, E - продать, Esc - выход");

            // Вывод игрового поля
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
                if (playerHealth < 0) playerHealth = 0;
                if (caravanHealth < 0) caravanHealth = 0;
                if (gold < 0) gold = 0;

                //Статус здоровья игрока
                string healthStatus;
                if (playerHealth <= 0)
                    healthStatus = "вы погибли";
                else if (playerHealth <= 30)
                    healthStatus = "мало здоровья";
                else if (playerHealth <= 60)
                    healthStatus = "ранен";
                else
                    healthStatus = "в порядке";

                //Статус здоровья каравана
                string caravanStatus;
                if (caravanHealth <= 0)
                    caravanStatus = "уничтожен";
                else if (caravanHealth <= 300)
                    caravanStatus = "сильно повреждён";
                else if (caravanHealth <= 600)
                    caravanStatus = "повреждён";
                else
                    caravanStatus = "в порядке";

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.SetCursorPosition(0, hudCursorPosition);
                string hudText = $"♥ Игрок: {playerHealth}/100 ({healthStatus}) | Караван: {caravanHealth}/1000 ({caravanStatus}) | " + $"Ресурсы: {collectedResources} (продано {soldResources}) | Золото: {gold}/{goalGold}";
                Console.Write($"{hudText,-80}");
                Console.ResetColor();

                // Бой
                if (banditHealth > 0)
                {
                    if (isNewBattle)
                    {
                        isNewBattle = false;
                        string battleStart = $"Разбойник атакует! Его здоровье: {banditHealth}. R - атаковать, F - бежать";
                        Console.SetCursorPosition(0, messageCursorPosition);
                        Console.Write($"{battleStart,-80}");
                    }

                    Console.SetCursorPosition(0, playerInputPosition);
                    Console.Write("Ваш ход: ");
                    var battleKey = Console.ReadKey(true).Key;

                    switch (battleKey)
                    {
                        case ConsoleKey.R:
                            int playerHit = random.Next(20, 34);
                            banditHealth -= playerHit;
                            if (banditHealth < 0) banditHealth = 0;

                            if (banditHealth == 0)
                            {
                                int loot = random.Next(1, 4);
                                collectedResources += loot;
                                string killMessage = $"Вы нанесли {playerHit} урона. Разбойник убит! Нашли {loot} ресурсов";
                                Console.SetCursorPosition(0, messageCursorPosition);
                                Console.Write($"{killMessage,-80}");
                            }
                            else
                            {
                                int banditHit = random.Next(10, 21);
                                playerHealth -= banditHit;
                                caravanHealth -= banditHit * 6;
                                string fightMessage = $"Вы ударили на {playerHit}. Ответный удар: -{banditHit} игроку, -{banditHit * 6} каравану. У разбойника {banditHealth} HP";
                                Console.SetCursorPosition(0, messageCursorPosition);
                                Console.Write($"{fightMessage,-80}");
                            }
                            break;

                        case ConsoleKey.F:
                            int runHit = random.Next(10, 21);
                            playerHealth -= runHit * 2;
                            caravanHealth -= runHit * 12;
                            banditHealth = 0;
                            string fleeMessage = $"Вы отступили, но получили -{runHit * 2} HP игроку, -{runHit * 12} HP каравану";
                            Console.SetCursorPosition(0, messageCursorPosition);
                            Console.Write($"{fleeMessage,-80}");
                            break;

                        default:
                            string unknownBattle = "Неизвестная команда, бой продолжается. R - атаковать, F - бежать";
                            Console.SetCursorPosition(0, messageCursorPosition);
                            Console.Write($"{unknownBattle,-80}");
                            break;
                    }

                    Console.SetCursorPosition(0, playerInputPosition);
                    Console.Write(new string(' ', 80));
                }
                else
                {
                    Console.SetCursorPosition(0, playerInputPosition);
                    Console.Write("Ваш ход (W/A/S/D - движение, E - продать, Esc - выход): ");
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
                                // Стирает старое рисует по новому
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

                                // Сценарий - атака разбойников
                                if (random.Next(100) < banditAttackChance)
                                {
                                    banditHealth = 100;
                                    isNewBattle = true;
                                    Console.SetCursorPosition(0, messageCursorPosition);
                                    Console.Write($"{"На караван напали разбойники!",-80}");
                                }
                                // Сценарий — подбор ресурсов
                                else if (random.Next(100) < resourceSpawnChance)
                                {
                                    int found = random.Next(1, 4);
                                    collectedResources += found;
                                    Console.SetCursorPosition(0, messageCursorPosition);
                                    Console.Write($"{"Вы нашли ресурсы (+" + found + ")",-80}");
                                }
                            }
                            else
                            {
                                Console.SetCursorPosition(0, messageCursorPosition);
                                Console.Write($"{"Там край, дальше нельзя",-80}");
                            }
                            break;

                        case ConsoleKey.E:
                            if (collectedResources == 0)
                            {
                                Console.SetCursorPosition(0, messageCursorPosition);
                                Console.Write($"{"Нечего продавать",-80}");
                            }
                            else
                            {
                                int earned = collectedResources * resourcePrice;
                                gold += earned;
                                soldResources += collectedResources;
                                string soldMessage = $"Продано {collectedResources} ресурсов за {earned} золота";
                                Console.SetCursorPosition(0, messageCursorPosition);
                                Console.Write($"{soldMessage,-80}");
                                collectedResources = 0;
                            }
                            break;

                        case ConsoleKey.Escape:
                            isGameOver = true;
                            gameResult = "Вы покинули игру.";
                            break;

                        default:
                            Console.SetCursorPosition(0, messageCursorPosition);
                            Console.Write($"{"Неизвестная команда. Доступно: W, A, S, D, E (продать), Esc",-80}");
                            break;
                    }

                    Console.SetCursorPosition(0, playerInputPosition);
                    Console.Write(new string(' ', 80));
                }

                // Проверка победы и поражений
                if ((gold >= goalGold) && playerHealth > 0 && caravanHealth > 0)
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
