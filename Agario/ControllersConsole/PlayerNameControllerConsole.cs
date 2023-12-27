using Controllers.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewsConsole.Menu;

namespace ControllersConsole
{
  /// <summary>
  /// Контроллер пункта настройки имени игрока
  /// </summary>
  internal class PlayerNameControllerConsole : MenuPlayerNameController
  {
    /// <summary>
    /// Событие смены имени игрока
    /// </summary>
    public event Action<string>? PlayerNameChanged = null;

    /// <summary>
    /// Представление пункта меню
    /// </summary>
    private readonly PlayerNameViewConsole _playerNameView;

    /// <summary>
    /// Инициализация
    /// </summary>
    public PlayerNameControllerConsole()
    {
      _playerNameView = new();
    }

    /// <summary>
    /// Открытие раздела меню
    /// </summary>
    public override void Start()
    {
      _playerNameView.Draw();
      bool needExit = false;
      do
      {
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
        switch (keyInfo.Key)
        {
          case ConsoleKey.Backspace:
            if (PlayerName.Length > 0)
              PlayerName = PlayerName.Remove(PlayerName.Length - 1);
            break;
          case ConsoleKey.Enter:
            ChangePlayerName();
            PlayerNameChanged?.Invoke(PlayerName);
            GoBackCall();
            break;
          case ConsoleKey.Escape:
            needExit = true;
            GoBackCall();
            break;
          default:
            PlayerName += keyInfo.KeyChar;
            break;
        }
        _playerNameView.PlayerName = PlayerName;
        _playerNameView.Draw();
      } while (!needExit);
    }
  }
}
