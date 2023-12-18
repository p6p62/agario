using AgarioModels.Game;
using Controllers.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Views.Game;
using ViewsConsole.Game;

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
    /// Инициализация контроллера игры, создание представления
    /// </summary>
    public GameControllerConsole()
    {
      // TODO
      GameView = CreateGameView();
      ControlledPlayer = GameInstance.GameField.Players.Find(p => p.Name == AgarioGame.TEST_PLAYER_NAME);
      //GetGameInstance().GameOver += () => _needExit = true;
    }

    /// <summary>
    /// Запуск игры
    /// </summary>
    public override void Start()
    {
      base.Start();
      // TODO
      _needExit = false;
      do
      {
        ConsoleKeyInfo key = Console.ReadKey(true);
        KeyDown?.Invoke(key);
      } while (!_needExit);
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
      // TODO player speed set reset
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
      // TODO player speed set
    }
  }
}
