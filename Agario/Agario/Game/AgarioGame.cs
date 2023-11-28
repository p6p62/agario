using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AgarioModels.Game
{
  /// <summary>
  /// Игра
  /// </summary>
  public class AgarioGame
  {
    /// <summary>
    /// Состояния игры
    /// </summary>
    private enum State : int
    {
      /// <summary>
      /// Активное
      /// </summary>
      Active,
      /// <summary>
      /// Приостановленное
      /// </summary>
      Pause,
      /// <summary>
      /// Приостановленное с необходимостью сброса (при первом запуске или после перезапуска)
      /// </summary>
      StopAndNeedReset
    }

    /// <summary>
    /// Тестовое имя игрока для отладки
    /// </summary>
    public const string TEST_PLAYER_NAME = "Test";
    /// <summary>
    /// Префикс для имени клеток, управляемых компьютером
    /// </summary>
    private const string COMPUTER_PLAYER_NAME_PREFIX = "Computer";

    /// <summary>
    /// Желаемая частота обновления внутреннего состояния игры
    /// </summary>
    private const int GAME_UPDATES_PER_SECOND = 60;
    /// <summary>
    /// Время между последовательными обновлениями
    /// </summary>
    private const float UPDATE_PERIOD_SECONDS = 1f / GAME_UPDATES_PER_SECOND;
    /// <summary>
    /// При накоплении единовременной задержки больше этого значения 
    /// обновлений игрового времени происходить не будет (защита от рывка после паузы)
    /// </summary>
    private const float MAX_ELAPSED_TIME_BORDER = 1f;

    /// <summary>
    /// Начало новой игры
    /// </summary>
    public event Action? GameStarted;
    /// <summary>
    /// Приостановка игры
    /// </summary>
    public event Action? GamePaused;
    /// <summary>
    /// Возобновление игры
    /// </summary>
    public event Action? GameResumed;
    /// <summary>
    /// Завершение игры
    /// </summary>
    public event Action? GameFinished;

    /// <summary>
    /// Событие, разрешающее визуализировать состояние модели
    /// </summary>
    public event Action? CanRender;

    /// <summary>
    /// Экземпляр игры
    /// </summary>
    private static readonly AgarioGame _gameInstance = new();

    /// <summary>
    /// Текущее состояние игры
    /// </summary>
    private State _state = State.StopAndNeedReset;

    /// <summary>
    /// Игровое поле
    /// </summary>
    private readonly GameField _gameField;

    /// <summary>
    /// Поток, обновляющий игру
    /// </summary>
    private readonly Thread _gameTicker;

    /// <summary>
    /// Событие для удержания игры в приостановленном состоянии
    /// </summary>
    private readonly ManualResetEvent _gamePauseEvent = new(false);

    /// <summary>
    /// Игровое поле
    /// </summary>
    public GameField GameField { get => _gameField; }

    /// <summary>
    /// Инициализация экземпляра игры. Модификатор private для реализации шаблона singleton
    /// </summary>
    private AgarioGame()
    {
      _gameTicker = GetGameTicker();
      _gameTicker.Name = "GAME_TICKER";

      _gameField = new() { Width = 50, Height = 30 };
      InitializeGameField();
    }

    /// <summary>
    /// Получение экземпляра игры
    /// </summary>
    /// <returns>Экземпляр игры</returns>
    public static AgarioGame GetGameInstance()
    {
      return _gameInstance;
    }

    /// <summary>
    /// Добавление игрока, управляемого компьютером
    /// </summary>
    /// <param name="parNumber">Номер</param>
    private void AddComputerControlledPlayer(int parNumber)
    {
      Player player = new() { Name = $"{COMPUTER_PLAYER_NAME_PREFIX}{parNumber}" };
      ComputerMovingStrategy movingStrategy = new(player, _gameField);
      _gameField.AddPlayerOnRandomPosition(player, movingStrategy);
    }

    /// <summary>
    /// Инициализация игрового поля
    /// </summary>
    private void InitializeGameField()
    {
      const int START_EAT_COUNT = 450;

      _gameField.AddPlayerOnRandomPosition(new() { Name = TEST_PLAYER_NAME });

      AddComputerControlledPlayer(1);
      AddComputerControlledPlayer(2);
      AddComputerControlledPlayer(3);
      AddComputerControlledPlayer(4);
      AddComputerControlledPlayer(5);
      AddComputerControlledPlayer(6);

      _gameField.CreateEat(START_EAT_COUNT);
    }

    /// <summary>
    /// Запуск игры
    /// </summary>
    public void StartGame()
    {
      // TODO
      if (_state == State.Active)
        return;

      if (_state == State.StopAndNeedReset)
        _gameField.Reset();

      if (!_gameTicker.IsAlive)
        _gameTicker.Start();

      _state = State.Active;
      _gamePauseEvent.Set();
      GameStarted?.Invoke();
    }

    /// <summary>
    /// Приостановка игры
    /// </summary>
    public void Pause()
    {
      // TODO
      // приостановить можно только запущенную игру
      if (_state != State.Active)
        return;

      _gamePauseEvent.Reset();
      _state = State.Pause;
      GamePaused?.Invoke();
    }

    /// <summary>
    /// Возобновление игры
    /// </summary>
    public void Resume()
    {
      // TODO
      // возобновить можно только приостановленную игру
      if (_state != State.Pause)
        return;

      _state = State.Active;
      _gamePauseEvent.Set();
      GameResumed?.Invoke();
    }

    /// <summary>
    /// Остановка игры
    /// </summary>
    public void Stop()
    {
      // TODO
      // сброс возможен из любого состояния, повторый сброс бесполезен
      if (_state == State.StopAndNeedReset)
        return;

      _gamePauseEvent.Reset();
      _state = State.StopAndNeedReset;
      GameFinished?.Invoke();
    }

    /// <summary>
    /// Получение потока, обновляющего внутреннее время и состояние игры
    /// </summary>
    /// <returns>Поток, обновляющий игровую логику</returns>
    private static Thread GetGameTicker()
    {
      return new(() =>
      {
        Thread.CurrentThread.IsBackground = true;
        Stopwatch timer = Stopwatch.StartNew();

        double previousTime = timer.Elapsed.TotalSeconds;
        double lagSeconds = 0;

        // TODO по надобности уточнить условие цикла при выходе из игры или прерывании
        while (true)
        {
          try
          {
            _gameInstance._gamePauseEvent.WaitOne();
          }
          catch (ThreadInterruptedException)
          {
            break;
          }

          double currentTime = timer.Elapsed.TotalSeconds;
          double elapsedSeconds = currentTime - previousTime;
          previousTime = currentTime;
          lagSeconds += elapsedSeconds;

          if (lagSeconds >= MAX_ELAPSED_TIME_BORDER)
            lagSeconds = 0;

          // устранение задержки между реальным и внутренним временем модели
          while (lagSeconds >= UPDATE_PERIOD_SECONDS)
          {
            _gameInstance._gameField.Update(UPDATE_PERIOD_SECONDS);
            lagSeconds -= UPDATE_PERIOD_SECONDS;
          }

          // TODO сделать интерполяцию по остатку lagSeconds в случае "рваного" движения
          _gameInstance.CanRender?.Invoke();
        }
      });
    }
  }
}
