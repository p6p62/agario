﻿using AgarioModels.Game;
using Controllers.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Views.Game;
using ViewsWPF.Game;

namespace ControllersWPF
{
  /// <summary>
  /// Игровой контроллер в WPF
  /// </summary>
  internal class GameControllerWPF : GameController
  {
    /// <summary>
    /// Экземпляр игрового окна, с которым связан текущий объект GameViewWPF
    /// </summary>
    private readonly Window _gameWindow;

    /// <summary>
    /// Панель элементами игры в игровом окне
    /// </summary>
    private readonly Canvas _gameScreen;

    /// <summary>
    /// Инициализация игрового контроллера и создание представления
    /// </summary>
    /// <param name="parGameWindow">Окно игры</param>
    public GameControllerWPF(Window parGameWindow)
    {
      _gameWindow = parGameWindow;
      GameView = CreateGameView();
      _gameScreen = ((GameViewWPF)GameView).GameScreen;

      ControlledPlayer = GameInstance.GameField.Players.Find(p => p.Name == AgarioGame.TEST_PLAYER_NAME);
    }

    /// <summary>
    /// Получение представления игры
    /// </summary>
    /// <returns>Представление игры</returns>
    protected override GameView CreateGameView()
    {
      return new GameViewWPF(_gameWindow);
    }

    /// <summary>
    /// Проверка выхода игрока
    /// </summary>
    /// <param name="parSender"></param>
    /// <param name="parKeyEventArgs"></param>
    private void PlayerExitCheck(object parSender, KeyEventArgs parKeyEventArgs)
    {
      if (parKeyEventArgs.Key == Key.Escape)
        StopGame();
    }

    /// <summary>
    /// Проверка установки игроком паузы
    /// </summary>
    /// <param name="parSender"></param>
    /// <param name="parKeyEventArgs"></param>
    private void PlayerPauseCheck(object parSender, KeyEventArgs parKeyEventArgs)
    {
      if (parKeyEventArgs.Key == Key.P)
        PauseGame();
    }

    /// <summary>
    /// Обработка возобновления игры игроком
    /// </summary>
    /// <param name="parSender"></param>
    /// <param name="parKeyEventArgs"></param>
    private void PlayerResumeCheck(object parSender, KeyEventArgs parKeyEventArgs)
    {
      if (parKeyEventArgs.Key == Key.Enter)
        ResumeGame();
    }

    /// <summary>
    /// Обработка выхода игрока
    /// </summary>
    protected override void SetHandlerPlayerExited()
    {
      _gameWindow.KeyDown += PlayerExitCheck;
    }

    /// <summary>
    /// Обработка установки паузы игроком
    /// </summary>
    protected override void SetHandlerPlayerPaused()
    {
      _gameWindow.KeyDown += PlayerPauseCheck;
    }

    /// <summary>
    /// Обработка возобновления игры игроком
    /// </summary>
    protected override void SetHandlerPlayerResumed()
    {
      _gameWindow.KeyDown += PlayerResumeCheck;
    }

    /// <summary>
    /// Выключение обработки выхода игрока
    /// </summary>
    protected override void ResetHandlerPlayerExited()
    {
      _gameWindow.KeyDown -= PlayerExitCheck;
    }

    /// <summary>
    /// Выключение обработки установки паузы игроком
    /// </summary>
    protected override void ResetHandlerPlayerPaused()
    {
      _gameWindow.KeyDown -= PlayerPauseCheck;
    }

    /// <summary>
    /// Выключение обработки возобновления игры игроком
    /// </summary>
    protected override void ResetHandlerPlayerResumed()
    {
      _gameWindow.KeyDown -= PlayerResumeCheck;
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
      // TODO
      if (ControlledPlayer == null)
        return;

      Point mousePosition;
      Application.Current.Dispatcher.Invoke(() =>
      {
        mousePosition = Mouse.GetPosition(_gameScreen);
      });
      Vector2 playerCenterScreenPosition = CalculatePlayerScreenPosition();
      Vector2 speedVector = new((float)mousePosition.X - playerCenterScreenPosition.X, (float)mousePosition.Y - playerCenterScreenPosition.Y);

      // TODO починить при наличии зависимости от масштаба
      // перевод в размеры, сопоставимые с игровым полем
      const float MULTIPLIER = 3;
      speedVector *= GameView.Camera.CameraToScreenScaleFactor * MULTIPLIER;

      SetPlayerSpeed(speedVector);
    }

    /// <summary>
    /// Включание обработки управления скоростью клеток игрока
    /// </summary>
    protected override void SetHandlerPlayerSpeedSet()
    {
      GameInstance.CanRender += PlayerSpeedUpdateHandler;
    }

    /// <summary>
    /// Выключение обработки управления скоростью клеток игрока
    /// </summary>
    protected override void ResetHandlerPlayerSpeedSet()
    {
      GameInstance.CanRender -= PlayerSpeedUpdateHandler;
    }

    /// <summary>
    /// Обработка разделения игрока
    /// </summary>
    /// <param name="parSender"></param>
    /// <param name="parKeyEventArgs"></param>
    private void PlayerDivideHandler(object parSender, KeyEventArgs parKeyEventArgs)
    {
      if (parKeyEventArgs.Key == Key.Space)
        DividePlayer();
    }

    /// <summary>
    /// Включение обработки разделения игрока
    /// </summary>
    protected override void SetHandlerPlayerDivided()
    {
      _gameWindow.KeyDown += PlayerDivideHandler;
    }

    /// <summary>
    /// Выключение обработки разделения игрока
    /// </summary>
    protected override void ResetHandlerPlayerDivided()
    {
      _gameWindow.KeyDown -= PlayerDivideHandler;
    }
  }
}
