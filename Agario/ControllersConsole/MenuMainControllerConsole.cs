using AgarioModels.Menu;
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
  /// Контроллер главного меню в консоли
  /// </summary>
  public class MenuMainControllerConsole : MenuController
  {
    /// <summary>
    /// Представление меню
    /// </summary>
    private readonly MenuViewConsole _menuView;
    /// <summary>
    /// Контроллер игры
    /// </summary>
    private readonly GameControllerConsole _gameControllerConsole;
    /// <summary>
    /// Контроллер пункта меню "Об игре" в консоли
    /// </summary>
    private readonly AboutGameControllerConsole _aboutGameControllerConsole;
    /// <summary>
    /// Контроллер пункта "Рекорды" в консоли
    /// </summary>
    private readonly RecordsControllerConsole _recordsControllerConsole;
    /// <summary>
    /// Флаг продолжения нахождения в меню
    /// </summary>
    private volatile bool _needExit = false;

    /// <summary>
    /// Инициализация контроллера главного меню, создание дочерних компонентов
    /// </summary>
    /// <param name="parMenu">Меню</param>
    public MenuMainControllerConsole(Menu parMenu) : base(parMenu)
    {
      _menuView = new MenuViewConsole(parMenu);
      _gameControllerConsole = new();
      _aboutGameControllerConsole = new();
      _recordsControllerConsole = new();

      Menu[(int)MenuMain.MenuItemCodes.StartGame].Selected += _gameControllerConsole.Start;
      Menu[(int)MenuMain.MenuItemCodes.About].Selected += _aboutGameControllerConsole.Start;
      Menu[(int)MenuMain.MenuItemCodes.Records].Selected += _recordsControllerConsole.Start;
      Menu[(int)MenuMain.MenuItemCodes.Exit].Selected += System.Diagnostics.Process.GetCurrentProcess().Kill;

      _gameControllerConsole.GoToBack += Start;
      _aboutGameControllerConsole.GoToBack += Start;
      _recordsControllerConsole.GoToBack += Start;
    }

    /// <summary>
    /// Запуск меню
    /// </summary>
    public override void Start()
    {
      _menuView.Draw();
      _needExit = false;
      do
      {
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
        switch (keyInfo.Key)
        {
          case ConsoleKey.UpArrow:
            Menu.FocusPrevious();
            break;
          case ConsoleKey.DownArrow:
            Menu.FocusNext();
            break;
          case ConsoleKey.Enter:
          case ConsoleKey.Spacebar:
            _needExit = true;
            Menu.SelectFocusedElement();
            break;
        }
      } while (!_needExit);
    }
  }
}
