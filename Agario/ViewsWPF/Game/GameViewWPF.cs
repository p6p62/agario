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
    private const int GAME_WINDOW_WIDTH = 1000;
    /// <summary>
    /// Высота игрового окна
    /// </summary>
    private const int GAME_WINDOW_HEIGHT = 650;

    /// <summary>
    /// Окно игры
    /// </summary>
    private readonly Window _gameWindow;

    /// <summary>
    /// Панель для отображения в игровом окне
    /// </summary>
    private readonly Canvas _gameScreen = new();

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
    /// Инициализация представления игры в WPF
    /// </summary>
    /// <param name="parWindow">Окно игры</param>
    public GameViewWPF(Window parWindow)
    {
      _gameWindow = parWindow;
      (_borderLeft, _borderRight, _borderUp, _borderDown) = CreateFieldBordersShapes();

      GameInstance.GameField.Players.ForEach(OnPlayerCreated);
      GameInstance.GameField.Food.ForEach(OnEatCreated);

      GameInstance.GameField.PlayerCreated += OnPlayerCreated;
      GameInstance.GameField.EatCreated += OnEatCreated;
      GameInstance.GameField.FoodEaten += OnFoodEaten;
      GameInstance.CanRender += OnCanRender;
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
      const int STROKE_THICKNESS = 2;
      SolidColorBrush randomColorBrush = new(GetRandomColor());
      List<Ellipse> playerCells = new();
      int count = parPlayer.Cells.Count;
      while (--count >= 0)
      {
        Ellipse cellFigure = new() { Fill = randomColorBrush, Stroke = _blackSolidBrush, StrokeThickness = STROKE_THICKNESS };
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
    private void OnFoodEaten(Cell parEat, Player _)
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
      // TODO
      Debug.WriteLine("Paused");
      Debug.WriteLine("Cam");
      Debug.WriteLine($"offset {Camera.CameraOffsetX}, {Camera.CameraOffsetY}");
      Debug.WriteLine($"size w_{Camera.CameraWidth}, h_{Camera.CameraHeight}");
    }

    /// <summary>
    /// Обновление представления при возобновлении игры
    /// </summary>
    protected override void OnResume()
    {
      // TODO
      Debug.WriteLine("Resumed");
    }

    /// <summary>
    /// Обновление представления при остановке игры
    /// </summary>
    protected override void OnStopGame()
    {
      // TODO
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
        Ellipse cellFigure = new() { Fill = parPlayerFigure.CellFillingBrush };
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
    private void DrawCell(Cell parCell)
    {
      // TODO вынести общую часть рисования
    }

    /// <summary>
    /// Рисование еды
    /// </summary>
    /// <param name="parEat">Еда</param>
    private void DrawEat(Cell parEat)
    {
      Ellipse drawedCellFigure = _eatShapes[parEat];
      float cellRadiusOnScreen = Camera.CalculateLineLengthInScreen(parEat.Radius);
      drawedCellFigure.Width = 2 * cellRadiusOnScreen;
      drawedCellFigure.Height = 2 * cellRadiusOnScreen;

      Vector2 newPosition = Camera.CalculatePointPositionInScreen(parEat.Position);
      Canvas.SetLeft(drawedCellFigure, newPosition.X - cellRadiusOnScreen);
      Canvas.SetTop(drawedCellFigure, newPosition.Y - cellRadiusOnScreen);
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
      {
        Ellipse drawedCellFigure = playerFigure.CellFigures[i];
        float cellRadiusOnScreen = Camera.CalculateLineLengthInScreen(cells[i].Radius);
        drawedCellFigure.Width = 2 * cellRadiusOnScreen;
        drawedCellFigure.Height = 2 * cellRadiusOnScreen;

        Vector2 newPosition = Camera.CalculatePointPositionInScreen(cells[i].Position);
        Canvas.SetLeft(drawedCellFigure, newPosition.X - cellRadiusOnScreen);
        Canvas.SetTop(drawedCellFigure, newPosition.Y - cellRadiusOnScreen);
      }

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
    }

    /// <summary>
    /// Отображение игровой информации
    /// </summary>
    protected override void DrawGameInfo()
    {
      // TODO
    }

    /// <summary>
    /// Отображение таблицы лидеров
    /// </summary>
    protected override void DrawLeaderboard()
    {
      // TODO
    }
  }
}
