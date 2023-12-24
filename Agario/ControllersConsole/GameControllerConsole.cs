using AgarioModels.Game;
using Controllers.Game;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Views.Game;
using ViewsConsole;
using ViewsConsole.Game;
using ViewsConsole.Menu;
using static System.Net.Mime.MediaTypeNames;

namespace ControllersConsole
{
  /// <summary>
  /// Контроллер игры в консоли
  /// </summary>
  internal class GameControllerConsole : GameController
  {
    /// <summary>
    /// Событие нажатия клавиши в консоли
    /// </summary>
    private event Action<ConsoleKeyInfo> KeyDown = null!;

    /// <summary>
    /// Флаг продолжения работы с игрой
    /// </summary>
    private volatile bool _needExit = false;

    /// <summary>
    /// Дескриптор окна консоли
    /// </summary>
    private IntPtr _consoleWindowHandler = IntPtr.Zero;

    /// <summary>
    /// Прямоугольник окна консоли
    /// </summary>
    private Rectangle _consoleWindowRect;

    /// <summary>
    /// Множитель размера окна по X
    /// </summary>
    private int _xSizeMultiplier = 1;

    /// <summary>
    /// Множитель размера окна по Y
    /// </summary>
    private int _ySizeMultiplier = 1;

    /// <summary>
    /// Инициализация контроллера игры, создание представления
    /// </summary>
    public GameControllerConsole()
    {
      GameView = CreateGameView();
      ControlledPlayer = GameInstance.GameField.Players.Find(p => p.Name == AgarioGame.TEST_PLAYER_NAME);
    }

    /// <summary>
    /// Запуск игры
    /// </summary>
    public override void Start()
    {
      base.Start();
      _consoleWindowHandler = ConsoleHelperUtilite.GetConsoleWindowHandle(MenuViewConsole.GAME_TITLE);
      _consoleWindowRect = ConsoleHelperUtilite.GetConsoleWindowRectangle(_consoleWindowHandler);
      _xSizeMultiplier = _consoleWindowRect.Width / GameViewConsole.GAME_CONSOLE_WIDTH;
      _ySizeMultiplier = _consoleWindowRect.Height / GameViewConsole.GAME_CONSOLE_HEIGHT;

      _needExit = false;
      do
      {
        ConsoleKeyInfo key = Console.ReadKey(true);
        KeyDown?.Invoke(key);
      } while (!_needExit);
    }

    /// <summary>
    /// Возвращает положение центра игрока на экране
    /// </summary>
    /// <returns></returns>
    private Vector2 CalculatePlayerScreenPosition()
    {
      Camera camera = GameView.Camera;
      Vector2 coordinatesSum = new();
      foreach (Cell elCell in ControlledPlayer!.Cells)
        coordinatesSum += camera.CalculatePointPositionInScreen(elCell.Position);
      return coordinatesSum / ControlledPlayer!.Cells.Count;
    }

    /// <summary>
    /// Исполнитель действия по обновлению скорости игрока
    /// </summary>
    private void PlayerSpeedUpdateHandler()
    {
      if (ControlledPlayer == null)
        return;

      ConsoleHelperUtilite.Point mousePosition = ConsoleHelperUtilite.GetCursorPosition(_consoleWindowHandler);
      Vector2 playerCenterScreenPosition = CalculatePlayerScreenPosition();
      playerCenterScreenPosition.X *= _xSizeMultiplier;
      playerCenterScreenPosition.Y *= _ySizeMultiplier;
      Vector2 speedVector = new((float)mousePosition.X - playerCenterScreenPosition.X, (float)mousePosition.Y - playerCenterScreenPosition.Y);

      // TODO починить при наличии зависимости от масштаба
      // перевод в размеры, сопоставимые с игровым полем
      const float MULTIPLIER = 3;
      speedVector *= GameView.Camera.CameraToScreenScaleFactor * MULTIPLIER;

      SetPlayerSpeed(speedVector);
    }

    /// <summary>
    /// Обработка установки игроком паузы
    /// </summary>
    /// <param name="parKeyInfo">Информация о нажатии</param>
    private void PlayerPauseCheck(ConsoleKeyInfo parKeyInfo)
    {
      if (parKeyInfo.Key == ConsoleKey.P)
        PauseGame();
    }

    /// <summary>
    /// Обработка возобновления игры игроком
    /// </summary>
    /// <param name="parKeyInfo">Информация о нажатии</param>
    private void PlayerResumeCheck(ConsoleKeyInfo parKeyInfo)
    {
      if (parKeyInfo.Key == ConsoleKey.Enter)
        ResumeGame();
    }

    /// <summary>
    /// Обработка выхода игрока
    /// </summary>
    /// <param name="parKeyInfo">Информация о нажатии</param>
    private void PlayerExitCheck(ConsoleKeyInfo parKeyInfo)
    {
      if (parKeyInfo.Key == ConsoleKey.Escape)
      {
        _needExit = true;
        StopGame();
      }
    }

    /// <summary>
    /// Обработка разделения игрока
    /// </summary>
    /// <param name="parKeyInfo">Информация о нажатии</param>
    private void PlayerDivideHandler(ConsoleKeyInfo parKeyInfo)
    {
      if (parKeyInfo.Key == ConsoleKey.Spacebar)
        DividePlayer();
    }

    /// <summary>
    /// Получение представления игры
    /// </summary>
    /// <returns>Представление игры</returns>
    protected override GameView CreateGameView()
    {
      return new GameViewConsole();
    }

    /// <summary>
    /// Выключение обработки разделения игрока
    /// </summary>
    protected override void ResetHandlerPlayerDivided()
    {
      KeyDown -= PlayerDivideHandler;
    }

    /// <summary>
    /// Выключение обработки выхода игрока
    /// </summary>
    protected override void ResetHandlerPlayerExited()
    {
      KeyDown -= PlayerExitCheck;
    }

    /// <summary>
    /// Выключение обработки установки паузы игроком
    /// </summary>
    protected override void ResetHandlerPlayerPaused()
    {
      KeyDown -= PlayerPauseCheck;
    }

    /// <summary>
    /// Выключение обработки возобновления игры игроком
    /// </summary>
    protected override void ResetHandlerPlayerResumed()
    {
      KeyDown -= PlayerResumeCheck;
    }

    /// <summary>
    /// Выключение обработки управления скоростью клеток игрока
    /// </summary>
    protected override void ResetHandlerPlayerSpeedSet()
    {
      GameInstance.CanRender -= PlayerSpeedUpdateHandler;
    }

    /// <summary>
    /// Включение обработки разделения игрока
    /// </summary>
    protected override void SetHandlerPlayerDivided()
    {
      KeyDown += PlayerDivideHandler;
    }

    /// <summary>
    /// Включение обработки выхода игрока
    /// </summary>
    protected override void SetHandlerPlayerExited()
    {
      KeyDown += PlayerExitCheck;
    }

    /// <summary>
    /// Включение обработки установки паузы игроком
    /// </summary>
    protected override void SetHandlerPlayerPaused()
    {
      KeyDown += PlayerPauseCheck;
    }

    /// <summary>
    /// Включение обработки возобновления игры игроком
    /// </summary>
    protected override void SetHandlerPlayerResumed()
    {
      KeyDown += PlayerResumeCheck;
    }

    /// <summary>
    /// Включание обработки управления скоростью клеток игрока
    /// </summary>
    protected override void SetHandlerPlayerSpeedSet()
    {
      GameInstance.CanRender += PlayerSpeedUpdateHandler;
    }
  }
}
