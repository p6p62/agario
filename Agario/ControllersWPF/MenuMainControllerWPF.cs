using AgarioModels.Menu;
using Controllers.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewsWPF.Menu;

namespace ControllersWPF
{
  /// <summary>
  /// Реализация контроллера меню для WPF
  /// </summary>
  public class MenuMainControllerWPF : MenuController
  {
    /// <summary>
    /// Представление меню
    /// </summary>
    private MenuViewWPF _menuView = null!;
    /// <summary>
    /// Окно меню
    /// </summary>
    private readonly Window _window;
    /// <summary>
    /// Контроллер игры в WPF
    /// </summary>
    private readonly GameControllerWPF _gameControllerWPF;
    /// <summary>
    /// Контроллер экрана "Об игре" в WPF
    /// </summary>
    private readonly AboutGameControllerWPF _aboutGameControllerWPF;
    /// <summary>
    /// Контроллер экрана "Рекорды" в WPF
    /// </summary>
    private readonly RecordsControllerWPF _recordsControllerWPF;

    /// <summary>
    /// Представление меню
    /// </summary>
    public MenuViewWPF MenuView { get => _menuView; }

    /// <summary>
    /// Инициализация контроллера главного меню
    /// </summary>
    /// <param name="parMenu">Меню</param>
    public MenuMainControllerWPF(Menu parMenu) : base(parMenu)
    {
      ConfigureMenuView(parMenu, null);
      _window = _menuView.MenuWindow;
      _gameControllerWPF = new(_window);
      _aboutGameControllerWPF = new(_window);
      _recordsControllerWPF = new(_window);

      Menu[(int)MenuMain.MenuItemCodes.StartGame].Selected += _gameControllerWPF.Start;
      Menu[(int)MenuMain.MenuItemCodes.About].Selected += _aboutGameControllerWPF.Start;
      Menu[(int)MenuMain.MenuItemCodes.Records].Selected += _recordsControllerWPF.Start;
      Menu[(int)MenuMain.MenuItemCodes.Exit].Selected += System.Diagnostics.Process.GetCurrentProcess().Kill;

      // TODO
      _gameControllerWPF.GoToBack += DrawMenu;
      _aboutGameControllerWPF.GoToBack += DrawMenu;
      _recordsControllerWPF.GoToBack += DrawMenu;
    }

    /// <summary>
    /// Запуск меню
    /// </summary>
    public override void Start()
    {
      _menuView.Draw();
    }

    /// <summary>
    /// Настройка представления меню
    /// </summary>
    /// <param name="parMenu">Меню</param>
    /// <param name="parWindow">Окно меню</param>
    private void ConfigureMenuView(AgarioModels.Menu.Menu parMenu, Window? parWindow)
    {
      _menuView = new(parMenu, parWindow);

      _menuView.PrevFocused += Menu.FocusPrevious;
      _menuView.NextFocused += Menu.FocusNext;
      _menuView.CurrentSelected += Menu.SelectFocusedElement;
      _menuView.Draw();
    }

    /// <summary>
    /// Вывод меню на экран
    /// </summary>
    private void DrawMenu()
    {
      _menuView.Draw();
    }
  }
}
