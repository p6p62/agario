using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersConsole
{
  /// <summary>
  /// Контроллер игры в консоли
  /// </summary>
  internal class GameControllerConsole
  {
    /// <summary>
    /// Представление игры в консоли
    /// </summary>
    // TODO private readonly GameViewConsole _gameView;
    /// <summary>
    /// Делегат на обработку событий клавиатуры
    /// </summary>
    private Action _keyDownAction = null!;
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
      //_gameView = new GameViewConsole();
      //GetGameInstance().PlayerScoreHandler = new GameRecordsHandlerConsole();
      //GetGameInstance().GameOver += () => _needExit = true;
    }

    /// <summary>
    /// Запуск игры
    /// </summary>
    public void StartGame()
    {
      throw new NotImplementedException();
      // TODO
      //_gameView.DrawInit();
      //_needExit = false;
      //_keyDownAction = FirstStart;
      //do
      //{
      //  Console.ReadKey();
      //  _keyDownAction();
      //} while (!_needExit);
    }
  }
}
