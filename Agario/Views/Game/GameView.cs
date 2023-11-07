using AgarioModels.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Views.Game
{
  /// <summary>
  /// Представление игры
  /// </summary>
  public abstract class GameView
  {
    /// <summary>
    /// Камера для отображения области игрового поля
    /// </summary>
    public Camera Camera { get; private set; }

    /// <summary>
    /// Экземпляр игры
    /// </summary>
    protected AgarioGame GameInstance { get; private set; }

    /// <summary>
    /// Инициализация представления игры
    /// </summary>
    public GameView()
    {
      GameInstance = AgarioGame.GetGameInstance();
      Camera = CreateCamera();

      GameInstance.GameStarted += OnStartGame;

      GameInstance.GamePaused += OnPause;
      GameInstance.GameResumed += OnResume;
      GameInstance.GameFinished += OnStopGame;
    }

    /// <summary>
    /// Создание камеры
    /// </summary>
    /// <returns>Камера</returns>
    protected abstract Camera CreateCamera();

    /// <summary>
    /// Настройка экрана при запуске игры
    /// </summary>
    protected abstract void OnStartGame();

    /// <summary>
    /// Обновление представления при установке паузы
    /// </summary>
    protected abstract void OnPause();

    /// <summary>
    /// Обновление представления при возобновлении игры
    /// </summary>
    protected abstract void OnResume();

    /// <summary>
    /// Обновление представления при остановке игры
    /// </summary>
    protected abstract void OnStopGame();

    /// <summary>
    /// Отображение таблицы лидеров
    /// </summary>
    protected abstract void DrawLeaderboard();

    /// <summary>
    /// Отображение игровой информации
    /// </summary>
    protected abstract void DrawGameInfo();

    /// <summary>
    /// Отображение игрового поля
    /// </summary>
    protected abstract void DrawGameField();
  }
}
