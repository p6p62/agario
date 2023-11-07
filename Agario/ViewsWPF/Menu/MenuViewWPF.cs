using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Views.Menu;
using MenuItem = AgarioModels.Menu.MenuItem;

namespace ViewsWPF.Menu
{
  /// <summary>
  /// Представление меню в WPF
  /// </summary>
  public class MenuViewWPF : MenuView
  {
    /// <summary>
    /// Заголовок окна
    /// </summary>
    public const string TITLE = "Agario";
    /// <summary>
    /// Высота окна
    /// </summary>
    public const int HEIGHT = 600;
    /// <summary>
    /// Ширина окна
    /// </summary>
    public const int WIDTH = 375;

    /// <summary>
    /// Выбран следующий
    /// </summary>
    public event Action? NextFocused = null;
    /// <summary>
    /// Выбран предыдущий
    /// </summary>
    public event Action? PrevFocused = null;
    /// <summary>
    /// Выбран текущий
    /// </summary>
    public event Action? CurrentSelected = null;

    /// <summary>
    /// Экран представления меню
    /// </summary>
    private readonly Grid _menuGrid = new() { Background = new SolidColorBrush(Colors.White) };

    /// <summary>
    /// Элементы меню
    /// </summary>
    public IList<MenuItemView> MenuItems => Items.Values.ToList();

    /// <summary>
    /// Окно меню
    /// </summary>
    public Window MenuWindow { get; private set; }

    /// <summary>
    /// Инициализация представления меню
    /// </summary>
    /// <param name="parMenu">Меню</param>
    /// <param name="parMenuWindow">Окно меню</param>
    public MenuViewWPF(AgarioModels.Menu.Menu parMenu, Window? parMenuWindow) : base(parMenu)
    {
      MenuWindow = parMenuWindow ?? new Window() { Title = TITLE, Height = HEIGHT, Width = WIDTH, MinWidth = 50, MinHeight = 80 };

      _menuGrid.Focusable = true;
      _menuGrid.RowDefinitions.Clear();
      foreach (MenuItemViewWPF elMenuItemViewWPF in Items.Values.Cast<MenuItemViewWPF>())
      {
        _menuGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
        elMenuItemViewWPF.Parent = _menuGrid;
      }
      _menuGrid.KeyDown += MenuKeyHandler;
    }

    /// <summary>
    /// Создание представления элемента меню
    /// </summary>
    /// <param name="parMenuItem">Элемент меню</param>
    /// <returns>Представление элемента меню</returns>
    public override MenuItemView CreateMenuItemView(MenuItem parMenuItem)
    {
      return new MenuItemViewWPF(parMenuItem);
    }

    /// <summary>
    /// Отображение меню
    /// </summary>
    public override void Draw()
    {
      MenuWindow.Width = WIDTH;
      MenuWindow.Height = HEIGHT;
      _menuGrid.Focus();
      MenuWindow.Content = _menuGrid;
      if (!MenuWindow.IsVisible)
        MenuWindow.Show();
      foreach (MenuItemViewWPF elMenuItemViewWPF in Items.Values.Cast<MenuItemViewWPF>())
      {
        elMenuItemViewWPF.Draw();
      }
    }

    /// <summary>
    /// Обработчик событий клавиатуры
    /// </summary>
    /// <param name="parSender">Отправитель</param>
    /// <param name="parEventArgs">Аргументы события</param>
    private void MenuKeyHandler(object parSender, KeyEventArgs parEventArgs)
    {
      switch (parEventArgs.Key)
      {
        case Key.Enter:
        case Key.Space:
          CurrentSelected?.Invoke();
          break;
        case Key.Down:
          NextFocused?.Invoke();
          break;
        case Key.Up:
          PrevFocused?.Invoke();
          break;
      }
    }
  }
}
