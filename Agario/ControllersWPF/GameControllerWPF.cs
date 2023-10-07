using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ControllersWPF
{
  /// <summary>
  /// Игровой контроллер в WPF
  /// </summary>
  internal class GameControllerWPF
  {
    /// <summary>
    /// Экземпляр игрового окна, с которым связан текущий объект GameViewWPF
    /// </summary>
    private readonly Window _gameWindow;
    /// <summary>
    /// Представление игры
    /// </summary>
    // TODO private readonly GameViewWPF _gameView = null!;

    /// <summary>
    /// Инициализация игрового контроллера и создание представления
    /// </summary>
    /// <param name="parGameWindow">Окно игры</param>
    public GameControllerWPF(Window parGameWindow)
    {
      _gameWindow = parGameWindow;
      // TODO
      //_gameView = new(parGameWindow);

      //_gameWindow.Closed += (s, e) => GetGameInstance().StopGame(true);

      //// настройка обработки рекордов
    }

    /// <summary>
    /// Запуск игры
    /// </summary>
    public void StartGame()
    {
      throw new NotImplementedException();
      // TODO
      //_gameView.Draw();
      //_gameWindow.KeyDown += FirstStart;
    }
  }
}
