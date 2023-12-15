using AgarioModels.Game;
using AgarioModels.Menu.Records;
using Controllers.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Views.Game;

namespace Controllers.Game
{
  /// <summary>
  /// Контроллер игры
  /// </summary>
  public abstract class GameController : MenuScreenController
  {
    /// <summary>
    /// Экземпляр игры
    /// </summary>
    private readonly AgarioGame _gameInstance;

    /// <summary>
    /// Экземпляр игры
    /// </summary>
    protected AgarioGame GameInstance
    {
      get => _gameInstance;
    }

    /// <summary>
    /// Представление игры
    /// </summary>
    protected GameView GameView { get; set; } = null!;

    /// <summary>
    /// Игрок, за действия с которым отвечает этот контроллер
    /// </summary>
    public Player? ControlledPlayer { get; set; }

    /// <summary>
    /// Инициализация контроллера игры
    /// </summary>
    public GameController()
    {
      _gameInstance = AgarioGame.GetGameInstance();
      _gameInstance.GameField.PlayerDead += UpdateRecordOnPlayerDead;
    }

    /// <summary>
    /// Обновляет значение рекорда после смерти игрока
    /// </summary>
    /// <param name="parPlayer">Игрок</param>
    private void UpdateRecordOnPlayerDead(Player parPlayer)
    {
      if (parPlayer.Name == _gameInstance.PlayerName)
        GameRecordsHandler.HandleRecordValueOnEndGame(parPlayer.MaxScore);
    }

    /// <summary>
    /// Включение обработки установки паузы игроком
    /// </summary>
    protected abstract void SetHandlerPlayerPaused();
    /// <summary>
    /// Включение обработки возобновления игры игроком
    /// </summary>
    protected abstract void SetHandlerPlayerResumed();
    /// <summary>
    /// Включение обработки выхода игрока
    /// </summary>
    protected abstract void SetHandlerPlayerExited();

    /// <summary>
    /// Включание обработки управления скоростью клеток игрока
    /// </summary>
    protected abstract void SetHandlerPlayerSpeedSet();

    /// <summary>
    /// Включение обработки разделения игрока
    /// </summary>
    protected abstract void SetHandlerPlayerDivided();

    /// <summary>
    /// Выключение обработки установки паузы игроком
    /// </summary>
    protected abstract void ResetHandlerPlayerPaused();
    /// <summary>
    /// Выключение обработки возобновления игры игроком
    /// </summary>
    protected abstract void ResetHandlerPlayerResumed();
    /// <summary>
    /// Выключение обработки выхода игрока
    /// </summary>
    protected abstract void ResetHandlerPlayerExited();

    /// <summary>
    /// Выключение обработки управления скоростью клеток игрока
    /// </summary>
    protected abstract void ResetHandlerPlayerSpeedSet();

    /// <summary>
    /// Выключение обработки разделения игрока
    /// </summary>
    protected abstract void ResetHandlerPlayerDivided();

    /// <summary>
    /// Получение представления игры
    /// </summary>
    /// <returns>Представление игры</returns>
    protected abstract GameView CreateGameView();

    /// <summary>
    /// Запуск игры
    /// </summary>
    public override void Start()
    {
      // TODO
      SetHandlerPlayerExited();
      SetHandlerPlayerPaused();
      SetHandlerPlayerResumed();

      SetHandlerPlayerSpeedSet();
      SetHandlerPlayerDivided();

      _gameInstance.StartGame();
    }

    /// <summary>
    /// Приостановка игры
    /// </summary>
    protected void PauseGame()
    {
      // TODO
      _gameInstance.Pause();
    }

    /// <summary>
    /// Возобновление игры
    /// </summary>
    protected void ResumeGame()
    {
      // TODO
      _gameInstance.Resume();
    }

    /// <summary>
    /// Остановка игры
    /// </summary>
    protected void StopGame()
    {
      // TODO
      ResetHandlerPlayerExited();
      ResetHandlerPlayerPaused();
      ResetHandlerPlayerResumed();

      ResetHandlerPlayerSpeedSet();
      ResetHandlerPlayerDivided();

      _gameInstance.Stop();
      GoBackCall();
    }

    /// <summary>
    /// Вызывает установку игроку, за которого отвечает контроллер, значения скорости,
    /// задаваемое вектором <paramref name="parSpeed"/>
    /// </summary>
    /// <param name="parSpeed">Вектор скорости</param>
    protected void SetPlayerSpeed(Vector2 parSpeed)
    {
      if (ControlledPlayer == null)
        return;
      _gameInstance.GameField.SetSpeedToPlayer(ControlledPlayer, parSpeed);
    }

    /// <summary>
    /// Разделение игрока
    /// </summary>
    protected void DividePlayer()
    {
      if (ControlledPlayer == null)
        return;
      _gameInstance.GameField.DividePlayerQuery(ControlledPlayer);
    }
  }
}
