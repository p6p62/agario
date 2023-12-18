using AgarioModels.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Views.Game;

namespace ViewsWPF.Game
{
  /// <summary>
  /// Представление игры в WPF
  /// </summary>
  public class GameViewWPF : GameView
  {
    /// <summary>
    /// Фигура игрока для отрисовки
    /// </summary>
    private class PlayerFigure
    {
      /// <summary>
      /// Игрок
      /// </summary>
      public Player Player { get; private set; }

      /// <summary>
      /// Кисть для заполнения клеток 
      /// </summary>
      public Brush CellFillingBrush { get; set; }
      /// <summary>
      /// Фигуры клеток игрока
      /// </summary>
      public List<Ellipse> CellFigures { get; private set; } = new();
      /// <summary>
      /// Текст на игроке
      /// </summary>
      public TextBlock PlayerText { get; set; }

      /// <summary>
      /// Инициализация
      /// </summary>
      /// <param name="parPlayer">Игрок</param>
      /// <param name="parCellFillingBrush">Кисть для заполнения клеток игрока</param>
      /// <param name="parCellFigures">Фигуры для клеток игрока</param>
      public PlayerFigure(Player parPlayer, Brush parCellFillingBrush, List<Ellipse> parCellFigures, TextBlock parPlayerText)
      {
        Player = parPlayer;
        CellFillingBrush = parCellFillingBrush;
        CellFigures = parCellFigures;
        PlayerText = parPlayerText;
      }
    }

    /// <summary>
    /// Ширина игрового окна
    /// </summary>
    private const int GAME_WINDOW_WIDTH = 1200;
    /// <summary>
    /// Высота игрового окна
    /// </summary>
    private const int GAME_WINDOW_HEIGHT = 650;

    /// <summary>
    /// Толщина обводки ячейки
    /// </summary>
    private const int CELL_BORDER_THICKNESS = 2;

    /// <summary>
    /// Отступ слева и сверху от границ окна игры для игрового поля
    /// </summary>
    private const int GAME_SCREEN_MARGIN_LEFT_TOP = 50;

    /// <summary>
    /// Подсказка с допустимыми действиями
    /// </summary>
    private const string GAME_ACTIONS_HINT = "Пробел - разделение\nP (en) - пауза\nEnter - возобновить\nEscape - в меню";

    /// <summary>
    /// Окно игры
    /// </summary>
    private readonly Window _gameWindow;

    /// <summary>
    /// Панель для отображения в игровом окне
    /// </summary>
    private readonly Canvas _gameScreen = new() { Margin = new(GAME_SCREEN_MARGIN_LEFT_TOP, GAME_SCREEN_MARGIN_LEFT_TOP, 0, 0) };

    /// <summary>
    /// Генератор случайных чисел
    /// </summary>
    private readonly Random _random = new();

    /// <summary>
    /// Черная сплошная кисть
    /// </summary>
    private readonly SolidColorBrush _blackSolidBrush = Brushes.Black;

    /// <summary>
    /// Фигуры игроков
    /// </summary>
    private readonly Dictionary<Player, PlayerFigure> _playerShapes = new();

    /// <summary>
    /// Фигуры еды
    /// </summary>
    private readonly Dictionary<Cell, Ellipse> _eatShapes = new();

    /// <summary>
    /// Левая граница поля
    /// </summary>
    private readonly Line _borderLeft;
    /// <summary>
    /// Правая граница поля
    /// </summary>
    private readonly Line _borderRight;
    /// <summary>
    /// Верхняя граница поля
    /// </summary>
    private readonly Line _borderUp;
    /// <summary>
    /// Нижняя граница поля
    /// </summary>
    private readonly Line _borderDown;

    /// <summary>
    /// Фигура таблицы лидеров
    /// </summary>
    private readonly TextBlock _leaderboard;

    /// <summary>
    /// Фигура с информацией об игре
    /// </summary>
    private readonly TextBlock _gameInfo;

    /// <summary>
    /// Панель для отображения в игровом окне
    /// </summary>
    public Canvas GameScreen { get => _gameScreen; }

    /// <summary>
    /// Инициализация представления игры в WPF
    /// </summary>
    /// <param name="parWindow">Окно игры</param>
    public GameViewWPF(Window parWindow)
    {
      _gameWindow = parWindow;
      (_borderLeft, _borderRight, _borderUp, _borderDown) = CreateFieldBordersShapes();
      _leaderboard = CreateLeaderboard();
      _gameInfo = CreateGameInfo();

      GameInstance.GameField.Players.ForEach(OnPlayerCreated);
      GameInstance.GameField.Food.ForEach(OnEatCreated);

      GameInstance.GameField.PlayerCreated += OnPlayerCreated;
      GameInstance.GameField.PlayerDead += OnPlayerDead;
      GameInstance.GameField.PlayerReborn += OnPlayerReborn;
      GameInstance.GameField.EatCreated += OnEatCreated;
      GameInstance.GameField.FoodEaten += OnFoodEaten;
      GameInstance.CanRender += OnCanRender;
    }

    /// <summary>
    /// Создание блока информации об игре
    /// </summary>
    /// <returns></returns>
    private TextBlock CreateGameInfo()
    {
      const float X_OFFSET = GAME_WINDOW_WIDTH * 0.75f;
      const float Y_OFFSET = GAME_WINDOW_HEIGHT * 0.5f;
      TextBlock gameInfo = new()
      {
        Text = "[игра]\n" + GAME_ACTIONS_HINT,
        FontSize = 24,
        Foreground = Brushes.Black
      };

      Canvas.SetLeft(gameInfo, X_OFFSET);
      Canvas.SetTop(gameInfo, Y_OFFSET);
      _gameScreen.Children.Add(gameInfo);
      return gameInfo;
    }

    /// <summary>
    /// Создание блока таблицы лидеров
    /// </summary>
    /// <returns></returns>
    private TextBlock CreateLeaderboard()
    {
      const float X_OFFSET = GAME_WINDOW_WIDTH * 0.75f;
      TextBlock leaderboard = new()
      {
        Text = "Таблица лидеров",
        FontSize = 24,
        TextAlignment = TextAlignment.Right,
        Foreground = Brushes.LimeGreen
      };

      Canvas.SetLeft(leaderboard, X_OFFSET);
      Canvas.SetTop(leaderboard, 0);
      _gameScreen.Children.Add(leaderboard);
      return leaderboard;
    }

    /// <summary>
    /// Создание фигур для границ игрового поля
    /// </summary>
    /// <returns></returns>
    private (Line left, Line right, Line up, Line down) CreateFieldBordersShapes()
    {
      const int THICKNESS = 5;
      Line left = new() { StrokeThickness = THICKNESS, Stroke = _blackSolidBrush, StrokeEndLineCap = PenLineCap.Round };
      Line right = new() { StrokeThickness = THICKNESS, Stroke = _blackSolidBrush, StrokeEndLineCap = PenLineCap.Round };
      Line up = new() { StrokeThickness = THICKNESS, Stroke = _blackSolidBrush, StrokeEndLineCap = PenLineCap.Round };
      Line down = new() { StrokeThickness = THICKNESS, Stroke = _blackSolidBrush, StrokeEndLineCap = PenLineCap.Round };

      _gameScreen.Children.Add(left);
      _gameScreen.Children.Add(right);
      _gameScreen.Children.Add(up);
      _gameScreen.Children.Add(down);

      return (left, right, up, down);
    }

    /// <summary>
    /// Возвращает случайный цвет
    /// </summary>
    /// <returns></returns>
    private Color GetRandomColor()
    {
      const int MAX_RGB_CHANNEL_VALUE = 256;
      return Color.FromRgb(
        (byte)_random.Next(MAX_RGB_CHANNEL_VALUE)
        , (byte)_random.Next(MAX_RGB_CHANNEL_VALUE)
        , (byte)_random.Next(MAX_RGB_CHANNEL_VALUE));
    }

    /// <summary>
    /// Создаёт объект фигуры игрока
    /// </summary>
    /// <param name="parPlayer">Игрок</param>
    /// <returns>Фигура игрока</returns>
    private PlayerFigure CreatePlayerFigure(Player parPlayer)
    {
      SolidColorBrush randomColorBrush = new(GetRandomColor());
      List<Ellipse> playerCells = new();
      int count = parPlayer.Cells.Count;
      while (--count >= 0)
      {
        Ellipse cellFigure = new() { Fill = randomColorBrush, Stroke = _blackSolidBrush, StrokeThickness = CELL_BORDER_THICKNESS };
        playerCells.Add(cellFigure);
      }
      TextBlock playerText = new() { Text = parPlayer.Name, FontSize = 20 };
      return new(parPlayer, randomColorBrush, playerCells, playerText);
    }

    /// <summary>
    /// Обработка добавления игрока на поле
    /// </summary>
    /// <param name="parPlayer">Добавленный игрок</param>
    private void OnPlayerCreated(Player parPlayer)
    {
      // TODO
      if (_playerShapes.ContainsKey(parPlayer))
        return;

      PlayerFigure playerFigure = CreatePlayerFigure(parPlayer);
      foreach (Ellipse elCellFigure in playerFigure.CellFigures)
        _gameScreen.Children.Add(elCellFigure);
      _gameScreen.Children.Add(playerFigure.PlayerText);
      _playerShapes.Add(parPlayer, playerFigure);
    }

    /// <summary>
    /// Обработка смерти игрока
    /// </summary>
    /// <param name="parPlayer">Умерший игрок</param>
    private void OnPlayerDead(Player parPlayer)
    {
      PlayerFigure playerFigure = _playerShapes[parPlayer];
      Application.Current.Dispatcher.Invoke(() =>
      {
        playerFigure.PlayerText.Visibility = Visibility.Hidden;
        foreach (Ellipse elCellFigure in playerFigure.CellFigures)
          elCellFigure.Visibility = Visibility.Hidden;
      });
    }

    /// <summary>
    /// Обработка возрождения игрока
    /// </summary>
    /// <param name="parPlayer">Возрождённый игрок</param>
    private void OnPlayerReborn(Player parPlayer)
    {
      PlayerFigure playerFigure = _playerShapes[parPlayer];
      Application.Current.Dispatcher.Invoke(() =>
      {
        playerFigure.PlayerText.Visibility = Visibility.Visible;
        foreach (Ellipse elCellFigure in playerFigure.CellFigures)
          elCellFigure.Visibility = Visibility.Visible;
      });
    }

    /// <summary>
    /// Обработка появления еды на поле
    /// </summary>
    /// <param name="parEat">Добавленная еда</param>
    private void OnEatCreated(Cell parEat)
    {
      // TODO
      if (_eatShapes.ContainsKey(parEat))
        return;

      Application.Current.Dispatcher.Invoke(() =>
      {
        Brush randomColorBrush = new SolidColorBrush(GetRandomColor());
        Ellipse eatFigure = new() { Fill = randomColorBrush };
        _gameScreen.Children.Add(eatFigure);
        _eatShapes.Add(parEat, eatFigure);
      });
    }

    /// <summary>
    /// Обработка съедания еды
    /// </summary>
    /// <param name="parEat"></param>
    /// <param name="_"></param>
    private void OnFoodEaten(Cell parEat, Player? _)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        _eatShapes.Remove(parEat, out Ellipse? outEatShape);
        if (outEatShape != null)
          _gameScreen.Children.Remove(outEatShape);
      });
    }

    /// <summary>
    /// Настройка экрана при запуске игры
    /// </summary>
    protected override void OnStartGame()
    {
      // TODO
      _gameWindow.Width = GAME_WINDOW_WIDTH;
      _gameWindow.Height = GAME_WINDOW_HEIGHT;
      _gameWindow.Content = _gameScreen;
      _gameWindow.Focus();
    }

    /// <summary>
    /// Обновление представления при установке паузы
    /// </summary>
    protected override void OnPause()
    {
      _gameInfo.Dispatcher.Invoke(() =>
      {
        _gameInfo.Text = "[пауза]\n" + GAME_ACTIONS_HINT;
      });
      Debug.WriteLine("Paused");
    }

    /// <summary>
    /// Обновление представления при возобновлении игры
    /// </summary>
    protected override void OnResume()
    {
      _gameInfo.Dispatcher.Invoke(() =>
      {
        _gameInfo.Text = "[игра]\n" + GAME_ACTIONS_HINT;
      });
      Debug.WriteLine("Resumed");
    }

    /// <summary>
    /// Обновление представления при остановке игры
    /// </summary>
    protected override void OnStopGame()
    {
      Debug.WriteLine("Exit from game");
    }

    /// <summary>
    /// Создание камеры
    /// </summary>
    /// <returns>Камера для WPF-режима</returns>
    protected override Camera CreateCamera()
    {
      const int VERTICAL_SUBTRACT = 30;
      Camera camera = new CameraWPF(GameInstance)
      {
        ScreenWidth = GAME_WINDOW_WIDTH,
        ScreenHeight = GAME_WINDOW_HEIGHT - VERTICAL_SUBTRACT
      };
      camera.UpdateScreenWidthToHeightRatio();
      return camera;
    }

    /// <summary>
    /// Отрисовка по готовности нового кадра
    /// </summary>
    private void OnCanRender()
    {
      // TODO
      Application.Current.Dispatcher.Invoke(() =>
      {
        Render();
      });
    }

    /// <summary>
    /// Функция отрисовки
    /// </summary>
    private void Render()
    {
      // TODO
      Camera.Update();
      DrawGameField();
    }

    /// <summary>
    /// Рисование границ игрового поля
    /// </summary>
    private void DrawGameFieldBorders()
    {
      // TODO
      Camera camera = Camera;
      GameField gameField = GameInstance.GameField;
      Vector2 leftUpGameFieldCornerOnScreen = camera.CalculatePointPositionInScreen(new(0, 0));
      Vector2 rightUpGameFieldCornerOnScreen = camera.CalculatePointPositionInScreen(new(gameField.Width, 0));
      Vector2 leftDownGameFieldCornerOnScreen = camera.CalculatePointPositionInScreen(new(0, gameField.Height));
      Vector2 rightDownGameFieldCornerOnScreen = camera.CalculatePointPositionInScreen(new(gameField.Width, gameField.Height));

      _borderLeft.X1 = leftUpGameFieldCornerOnScreen.X;
      _borderLeft.Y1 = leftUpGameFieldCornerOnScreen.Y;
      _borderLeft.X2 = leftDownGameFieldCornerOnScreen.X;
      _borderLeft.Y2 = leftDownGameFieldCornerOnScreen.Y;

      _borderRight.X1 = rightUpGameFieldCornerOnScreen.X;
      _borderRight.Y1 = rightUpGameFieldCornerOnScreen.Y;
      _borderRight.X2 = rightDownGameFieldCornerOnScreen.X;
      _borderRight.Y2 = rightDownGameFieldCornerOnScreen.Y;

      _borderUp.X1 = leftUpGameFieldCornerOnScreen.X;
      _borderUp.Y1 = leftUpGameFieldCornerOnScreen.Y;
      _borderUp.X2 = rightUpGameFieldCornerOnScreen.X;
      _borderUp.Y2 = rightUpGameFieldCornerOnScreen.Y;

      _borderDown.X1 = leftDownGameFieldCornerOnScreen.X;
      _borderDown.Y1 = leftDownGameFieldCornerOnScreen.Y;
      _borderDown.X2 = rightDownGameFieldCornerOnScreen.X;
      _borderDown.Y2 = rightDownGameFieldCornerOnScreen.Y;
    }

    /// <summary>
    /// Удаление лишних фигур для клеток игрока при рисовании
    /// </summary>
    /// <param name="parPlayerFigure">Данные для рисования игрока</param>
    /// <param name="count">Количество лишних клеток</param>
    private void RemoveExcessCellFigures(PlayerFigure parPlayerFigure, int count)
    {
      while (count-- > 0)
      {
        _gameScreen.Children.Remove(parPlayerFigure.CellFigures.Last());
        parPlayerFigure.CellFigures.RemoveAt(parPlayerFigure.CellFigures.Count - 1);
      }
    }

    /// <summary>
    /// Добавление недостающих фигур для клеток игрока при рисовании
    /// </summary>
    /// <param name="parPlayerFigure">Данные для рисования игрока</param>
    /// <param name="count">Количество недостающих клеток</param>
    private void AddMissingCellFigures(PlayerFigure parPlayerFigure, int count)
    {
      while (count-- > 0)
      {
        Ellipse cellFigure = new() { Fill = parPlayerFigure.CellFillingBrush, Stroke = _blackSolidBrush, StrokeThickness = CELL_BORDER_THICKNESS };
        parPlayerFigure.CellFigures.Add(cellFigure);
        _gameScreen.Children.Add(cellFigure);
      }
    }

    /// <summary>
    /// Удаление лишних клеток на экран или добавление недостающих при отрисовке игрока
    /// </summary>
    /// <param name="parPlayerFigure">Данные для отрисовки игрока</param>
    private void CheckCellFiguresCount(PlayerFigure parPlayerFigure)
    {
      int delta = parPlayerFigure.Player.Cells.Count - parPlayerFigure.CellFigures.Count;
      if (delta == 0)
        return;

      if (delta > 0)
        AddMissingCellFigures(parPlayerFigure, delta);
      else
        RemoveExcessCellFigures(parPlayerFigure, Math.Abs(delta));
    }

    /// <summary>
    /// Рисование клетки
    /// </summary>
    /// <param name="parCell">Клетка</param>
    /// <param name="parCellFigure">Фигура клетки на экране</param>
    private void DrawCell(Cell parCell, Ellipse parCellFigure)
    {
      float cellRadiusOnScreen = Camera.CalculateLineLengthInScreen(parCell.Radius);
      parCellFigure.Width = 2 * cellRadiusOnScreen;
      parCellFigure.Height = 2 * cellRadiusOnScreen;

      Vector2 newPosition = Camera.CalculatePointPositionInScreen(parCell.Position);
      Canvas.SetLeft(parCellFigure, newPosition.X - cellRadiusOnScreen);
      Canvas.SetTop(parCellFigure, newPosition.Y - cellRadiusOnScreen);
    }

    /// <summary>
    /// Рисование еды
    /// </summary>
    /// <param name="parEat">Еда</param>
    private void DrawEat(Cell parEat)
    {
      DrawCell(parEat, _eatShapes[parEat]);
    }

    /// <summary>
    /// Рисование игрока
    /// </summary>
    /// <param name="parPlayer">Игрок</param>
    private void DrawPlayer(Player parPlayer)
    {
      PlayerFigure playerFigure = _playerShapes[parPlayer];
      CheckCellFiguresCount(playerFigure);

      // клетки
      List<MovingCell> cells = parPlayer.Cells;
      for (int i = 0; i < cells.Count; i++)
        DrawCell(cells[i], playerFigure.CellFigures[i]);

      // имя и счёт
      playerFigure.PlayerText.Text = $"{parPlayer.Name}\n{parPlayer.Score}";
      Vector2 playerTextPosition = Camera.CalculatePointPositionInScreen(cells[0].Position);
      float haldRadius = Camera.CalculateLineLengthInScreen(cells[0].Radius) / 2;
      Canvas.SetLeft(playerFigure.PlayerText, playerTextPosition.X - haldRadius);
      Canvas.SetTop(playerFigure.PlayerText, playerTextPosition.Y - haldRadius);
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
    /// Рисование еды
    /// </summary>
    private void DrawFood()
    {
      List<Cell> drawedEat = Camera.GetEatInViewport();
      foreach (Cell elEat in drawedEat)
        DrawEat(elEat);
    }

    /// <summary>
    /// Отображение игрового поля
    /// </summary>
    protected override void DrawGameField()
    {
      // TODO
      if (Camera.IsGameFieldBordersInViewPort())
        DrawGameFieldBorders();

      DrawFood();
      DrawPlayers();
      DrawLeaderboard();
    }

    /// <summary>
    /// Отображение игровой информации
    /// </summary>
    protected override void DrawGameInfo()
    {
      // реализовано в событиях
    }

    /// <summary>
    /// Отображение таблицы лидеров
    /// </summary>
    protected override void DrawLeaderboard()
    {
      List<Player> livePlayers = new();
      List<Player> deadPlayers = new();
      foreach (Player elPlayer in GameInstance.GameField.Players)
        if (elPlayer.IsAlive)
          livePlayers.Add(elPlayer);
        else
          deadPlayers.Add(elPlayer);
      livePlayers.Sort((p1, p2) => p2.Score - p1.Score);

      StringBuilder leaderboardText = new("Таблица лидеров\n");
      foreach (Player elPlayer in livePlayers)
        leaderboardText.Append(elPlayer.Name).Append(" [").Append(elPlayer.Score).Append("]\n");
      foreach (Player elPlayer in deadPlayers)
        leaderboardText.Append(elPlayer.Name).Append(" [").Append("DEAD").Append("]\n");

      _gameScreen.Dispatcher.Invoke(() => _leaderboard.Text = leaderboardText.ToString());
    }
  }
}
