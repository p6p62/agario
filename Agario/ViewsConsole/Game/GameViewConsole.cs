﻿using AgarioModels.Game;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Views.Game;
using ViewsConsole.Menu;
using static System.Net.Mime.MediaTypeNames;

namespace ViewsConsole.Game
{
  /// <summary>
  /// Представление игры в консоли
  /// </summary>
  public class GameViewConsole : GameView
  {
    /// <summary>
    /// Ширина игрового окна консоли
    /// </summary>
    public const int GAME_CONSOLE_WIDTH = 110;
    /// <summary>
    /// Высота игрового окна консоли
    /// </summary>
    public const int GAME_CONSOLE_HEIGHT = 35;

    /// <summary>
    /// Длина буфера для консоли
    /// </summary>
    private const int CONSOLE_BUFFER_LENGTH = GAME_CONSOLE_WIDTH * GAME_CONSOLE_HEIGHT;

    /// <summary>
    /// Смещение для битового сдвига влево, чтобы настроить цвет фона консоли
    /// </summary>
    private const int LEFT_BYTE_SHIFT_OFFSET_FOR_BACKGROUND_COLOR = 4;

    /// <summary>
    /// Дескриптор вывода консоли
    /// </summary>
    private readonly SafeFileHandle _handleConsoleOut;

    /// <summary>
    /// Буфер для содержимого консоли
    /// </summary>
    private readonly ConsoleHelperUtilite.CharInfo[] _consoleBuffer;

    /// <summary>
    /// Игровое поле
    /// </summary>
    private readonly GameField _gameField;

    /// <summary>
    /// Генератор случайных чисел
    /// </summary>
    private readonly Random _random = new();

    /// <summary>
    /// Цвета игроков
    /// </summary>
    private readonly Dictionary<Player, ConsoleColor> _playerColors = new();

    /// <summary>
    /// Цвета еды
    /// </summary>
    private readonly Dictionary<Cell, ConsoleColor> _eatColors = new();

    /// <summary>
    /// Инициализация
    /// </summary>
    public GameViewConsole()
    {
      _handleConsoleOut = ConsoleHelperUtilite.GetConsoleOutputHandle();
      _consoleBuffer = new ConsoleHelperUtilite.CharInfo[CONSOLE_BUFFER_LENGTH];
      _gameField = GameInstance.GameField;

      GameInstance.GameField.Players.ForEach(OnPlayerCreated);
      GameInstance.GameField.Food.ForEach(OnEatCreated);

      GameInstance.GameField.PlayerCreated += OnPlayerCreated;
      GameInstance.GameField.EatCreated += OnEatCreated;
      GameInstance.GameField.FoodEaten += OnFoodEaten;
      GameInstance.CanRender += Render;
    }

    /// <summary>
    /// Создание камеры
    /// </summary>
    /// <returns>Камера</returns>
    protected override Camera CreateCamera()
    {
      Camera camera = new CameraConsole(GameInstance)
      {
        ScreenWidth = GAME_CONSOLE_WIDTH,
        ScreenHeight = GAME_CONSOLE_HEIGHT
      };
      camera.UpdateScreenWidthToHeightRatio();
      return camera;
    }

    /// <summary>
    /// Вывод символа в буфер консоли
    /// </summary>
    /// <param name="parX">Позиция по горизонтали</param>
    /// <param name="parY">Позиция по вертикали</param>
    /// <param name="parColor">Цвет</param>
    /// <param name="parChar">Символ</param>
    private void PlaceCharToBuffer(int parX, int parY, ConsoleColor parColor, char parChar)
    {
      if (parX < 0 || parX >= GAME_CONSOLE_WIDTH || parY < 0 || parY >= GAME_CONSOLE_HEIGHT)
        return;

      int offset = parY * GAME_CONSOLE_WIDTH + parX;
      _consoleBuffer[offset].Attributes = (short)(((short)parColor) | ((short)ViewsProperties.GAME_BACKGROUND_COLOR) << LEFT_BYTE_SHIFT_OFFSET_FOR_BACKGROUND_COLOR);
      _consoleBuffer[offset].Char.UnicodeChar = parChar;
    }

    /// <summary>
    /// Рисование окружности по алгоритму Брезенхема
    /// </summary>
    /// <param name="parCell">Клетка</param>
    /// <param name="parColor">Цвет</param>
    private void DrawCircle(Cell parCell, ConsoleColor parColor, char parChar)
    {
      Vector2 cellCenterOnScreen = Camera.CalculatePointPositionInScreen(parCell.Position);
      int xC = (int)cellCenterOnScreen.X;
      int yC = (int)cellCenterOnScreen.Y;
      float cellRadiusOnScreen = Camera.CalculateLineLengthInScreen(parCell.Radius);

      int x = 0;
      int y = (int)cellRadiusOnScreen;
      int delta = (int)(3 - 2 * y);
      while (y >= x)
      {
        PlaceCharToBuffer(xC + x, yC + y, parColor, parChar);
        PlaceCharToBuffer(xC + x, yC - y, parColor, parChar);
        PlaceCharToBuffer(xC - x, yC + y, parColor, parChar);
        PlaceCharToBuffer(xC - x, yC - y, parColor, parChar);
        PlaceCharToBuffer(xC + y, yC + x, parColor, parChar);
        PlaceCharToBuffer(xC + y, yC - x, parColor, parChar);
        PlaceCharToBuffer(xC - y, yC + x, parColor, parChar);
        PlaceCharToBuffer(xC - y, yC - x, parColor, parChar);

        delta += (delta < 0) ? 4 * x + 6 : 4 * (x - y--) + 10;
        ++x;
      }
    }

    /// <summary>
    /// Рисование еды
    /// </summary>
    private void DrawEat()
    {
      List<Cell> drawedEat = Camera.GetEatInViewport();
      foreach (Cell elEat in drawedEat)
        DrawCircle(elEat, _eatColors[elEat], 'x');
    }

    /// <summary>
    /// Рисование игрока
    /// </summary>
    /// <param name="parPlayer">Игрок</param>
    private void DrawPlayer(Player parPlayer)
    {
      ConsoleColor cellColor = _playerColors[parPlayer];
      List<MovingCell> cells = parPlayer.Cells;
      for (int i = 0; i < cells.Count; i++)
        DrawCircle(cells[i], cellColor, 'o');

      // имя и счёт
      Cell firstCell = parPlayer.Cells[0];
      Vector2 playerTextPosition = Camera.CalculatePointPositionInScreen(firstCell.Position);
      float halfRadiusOnScreen = Camera.CalculateLineLengthInScreen(firstCell.Radius) / 2;

      string label = $"{parPlayer.Name} - {parPlayer.Score}";
      int x = (int)(playerTextPosition.X - halfRadiusOnScreen);
      int y = (int)(playerTextPosition.Y - halfRadiusOnScreen);
      foreach (char elChar in label)
        PlaceCharToBuffer(x++, y, ViewsProperties.GAME_TEXT_COLOR, elChar);
    }

    /// <summary>
    /// Рисование игроков
    /// </summary>
    private void DrawPlayers()
    {
      // TODO
      List<Player> drawedPlayers = Camera.GetPlayersInViewport();
      foreach (Player elPlayer in drawedPlayers)
        DrawPlayer(elPlayer);
    }

    /// <summary>
    /// Рисование границ игрового поля
    /// </summary>
    private void DrawGameFieldBorders()
    {
      Camera camera = Camera;
      Vector2 leftUpGameFieldCornerOnScreen = camera.CalculatePointPositionInScreen(new(0, 0));
      Vector2 rightUpGameFieldCornerOnScreen = camera.CalculatePointPositionInScreen(new(_gameField.Width, 0));
      Vector2 leftDownGameFieldCornerOnScreen = camera.CalculatePointPositionInScreen(new(0, _gameField.Height));
      Vector2 rightDownGameFieldCornerOnScreen = camera.CalculatePointPositionInScreen(new(_gameField.Width, _gameField.Height));

      int xLeft = (int)leftUpGameFieldCornerOnScreen.X;
      int xRight = (int)rightUpGameFieldCornerOnScreen.X;
      for (int y = (int)leftUpGameFieldCornerOnScreen.Y; y <= (int)leftDownGameFieldCornerOnScreen.Y; y++)
      {
        PlaceCharToBuffer(xLeft, y, ViewsProperties.GAME_TEXT_COLOR, '\xB3');
        PlaceCharToBuffer(xRight, y, ViewsProperties.GAME_TEXT_COLOR, '\xB3');
      }

      int yUp = (int)leftUpGameFieldCornerOnScreen.Y;
      int yDown = (int)leftDownGameFieldCornerOnScreen.Y;
      for (int x = (int)leftUpGameFieldCornerOnScreen.X; x <= (int)rightUpGameFieldCornerOnScreen.X; x++)
      {
        PlaceCharToBuffer(x, yUp, ViewsProperties.GAME_TEXT_COLOR, '\xC4');
        PlaceCharToBuffer(x, yDown, ViewsProperties.GAME_TEXT_COLOR, '\xC4');
      }

      PlaceCharToBuffer((int)leftUpGameFieldCornerOnScreen.X, (int)leftUpGameFieldCornerOnScreen.Y, ViewsProperties.GAME_TEXT_COLOR, '\xDA');
      PlaceCharToBuffer((int)leftDownGameFieldCornerOnScreen.X, (int)leftDownGameFieldCornerOnScreen.Y, ViewsProperties.GAME_TEXT_COLOR, '\xC0');
      PlaceCharToBuffer((int)rightUpGameFieldCornerOnScreen.X, (int)rightUpGameFieldCornerOnScreen.Y, ViewsProperties.GAME_TEXT_COLOR, '\xBF');
      PlaceCharToBuffer((int)rightDownGameFieldCornerOnScreen.X, (int)rightDownGameFieldCornerOnScreen.Y, ViewsProperties.GAME_TEXT_COLOR, '\xD9');
    }

    /// <summary>
    /// Очистка буфера консоли
    /// </summary>
    private void ClearConsoleBuffer()
    {
      Array.Fill(_consoleBuffer, new() { Char = new() { UnicodeChar = ' ' }, Attributes = (short)ViewsProperties.GAME_BACKGROUND_COLOR << LEFT_BYTE_SHIFT_OFFSET_FOR_BACKGROUND_COLOR });
    }

    /// <summary>
    /// Функция отрисовки
    /// </summary>
    private void Render()
    {
      // TODO
      Camera.Update();
      ClearConsoleBuffer();
      DrawGameField();
      ConsoleHelperUtilite.PrintToConsoleFast(_handleConsoleOut, _consoleBuffer, GAME_CONSOLE_WIDTH, GAME_CONSOLE_HEIGHT);
    }

    /// <summary>
    /// Получение случайного цвета переднего плана, не совпадающего с <paramref name="parBackgroundColor"/>
    /// </summary>
    /// <param name="parBackgroundColor">Цвет заднего плана</param>
    /// <returns></returns>
    private ConsoleColor GetRandomForegroundConsoleColor(ConsoleColor parBackgroundColor)
    {
      const int CONSOLE_COLORS_COUNT = 16;
      ConsoleColor result = (ConsoleColor)_random.Next(CONSOLE_COLORS_COUNT);
      while (parBackgroundColor == result)
        result = (ConsoleColor)_random.Next(CONSOLE_COLORS_COUNT);
      return result;
    }

    /// <summary>
    /// Обработка добавления игрока на поле
    /// </summary>
    /// <param name="parPlayer">Добавленный игрок</param>
    private void OnPlayerCreated(Player parPlayer)
    {
      if (!_playerColors.ContainsKey(parPlayer))
        _playerColors.Add(parPlayer, GetRandomForegroundConsoleColor(ViewsProperties.GAME_BACKGROUND_COLOR));
    }

    /// <summary>
    /// Обработка появления еды на поле
    /// </summary>
    /// <param name="parEat">Добавленная еда</param>
    private void OnEatCreated(Cell parEat)
    {
      if (!_eatColors.ContainsKey(parEat))
        _eatColors.Add(parEat, GetRandomForegroundConsoleColor(ViewsProperties.GAME_BACKGROUND_COLOR));
    }

    /// <summary>
    /// Обработка съедания еды
    /// </summary>
    /// <param name="parEat"></param>
    /// <param name="_"></param>
    private void OnFoodEaten(Cell parEat, Player? _)
    {
      _eatColors.Remove(parEat);
    }

    /// <summary>
    /// Отображение игрового поля
    /// </summary>
    protected override void DrawGameField()
    {
      if (Camera.IsGameFieldBordersInViewPort())
        DrawGameFieldBorders();

      DrawEat();
      DrawPlayers();
      DrawLeaderboard();
      DrawGameInfo();
    }

    /// <summary>
    /// Отображение игровой информации
    /// </summary>
    protected override void DrawGameInfo()
    {
      const int RIGHT_OFFSET = 20;
      const int BOTTOM_OFFSET = 5;

      int x = GAME_CONSOLE_WIDTH - RIGHT_OFFSET - 1;
      int y = GAME_CONSOLE_HEIGHT - BOTTOM_OFFSET - 1;
      PrintString("Space - Divide", ViewsProperties.GAME_TEXT_COLOR, x, y++);
      PrintString("P(en) - Pause", ViewsProperties.GAME_TEXT_COLOR, x, y++);
      PrintString("Enter - Resume", ViewsProperties.GAME_TEXT_COLOR, x, y++);
      PrintString("Escape - Back to menu", ViewsProperties.GAME_TEXT_COLOR, x, y++);
    }

    /// <summary>
    /// Выводит строку <paramref name="parString"/> с началом в точке (<paramref name="parX"/>, <paramref name="parY"/>)
    /// цветом <paramref name="parColor"/>. Без переносов, при нехватке места строка отсечётся
    /// </summary>
    /// <param name="parString"></param>
    /// <param name="parColor"></param>
    /// <param name="parX"></param>
    /// <param name="parY"></param>
    private void PrintString(string parString, ConsoleColor parColor, int parX, int parY)
    {
      int l = parString.Length;
      for (int i = 0; i < l; ++i)
        PlaceCharToBuffer(parX++, parY, parColor, parString[i]);
    }

    /// <summary>
    /// Отображение таблицы лидеров
    /// </summary>
    protected override void DrawLeaderboard()
    {
      const int RIGHT_OFFSET = 20;

      // отбор
      List<Player> livePlayers = new();
      List<Player> deadPlayers = new();
      foreach (Player elPlayer in GameInstance.GameField.Players)
        if (elPlayer.IsAlive)
          livePlayers.Add(elPlayer);
        else
          deadPlayers.Add(elPlayer);
      livePlayers.Sort((p1, p2) => p2.Score - p1.Score);

      // вывод
      int x = GAME_CONSOLE_WIDTH - RIGHT_OFFSET - 1;
      int y = 0;
      PrintString("Leaderboard", ViewsProperties.GAME_TEXT_COLOR, x, y);

      foreach (Player elPlayer in livePlayers)
        PrintString($"{elPlayer.Name} [{elPlayer.Score}]", _playerColors[elPlayer], x, ++y);

      foreach (Player elPlayer in deadPlayers)
        PrintString($"{elPlayer.Name} [DEAD]", _playerColors[elPlayer], x, ++y);
    }

    /// <summary>
    /// Обновление представления при установке паузы
    /// </summary>
    protected override void OnPause()
    {
      Debug.WriteLine("Paused");
    }

    /// <summary>
    /// Обновление представления при возобновлении игры
    /// </summary>
    protected override void OnResume()
    {
      Debug.WriteLine("Resumed");
    }

    /// <summary>
    /// Настройка экрана при запуске игры
    /// </summary>
    protected override void OnStartGame()
    {
      if (OperatingSystem.IsWindows())
      {
        Console.WindowWidth = GAME_CONSOLE_WIDTH;
        Console.WindowHeight = GAME_CONSOLE_HEIGHT;
        Console.BufferWidth = GAME_CONSOLE_WIDTH;
        Console.BufferHeight = GAME_CONSOLE_HEIGHT;
      }
      Console.BackgroundColor = ViewsProperties.GAME_BACKGROUND_COLOR;
      Console.ForegroundColor = ViewsProperties.GAME_TEXT_COLOR;
    }

    /// <summary>
    /// Обновление представления при остановке игры
    /// </summary>
    protected override void OnStopGame()
    {
      Debug.WriteLine("Exit from game");
    }
  }
}
