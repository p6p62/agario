using AgarioModels.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Views.Menu;
using static AgarioModels.Menu.MenuItem;

namespace ViewsConsole.Menu
{
  /// <summary>
  /// Представление пунктов меню в консоли
  /// </summary>
  internal class MenuItemViewConsole : MenuItemView
  {
    /// <summary>
    /// Сопоставление цветов консоли и состояний пунктов меню
    /// </summary>
    protected static Dictionary<MenuItemState, ConsoleColor> ColorByState { get; private set; } = new()
    {
      { MenuItem.MenuItemState.Normal, ViewsProperties.TEXT_COLOR },
      { MenuItem.MenuItemState.Selected, ViewsProperties.MENU_ITEM_FOCUS_COLOR },
      { MenuItem.MenuItemState.Focused, ViewsProperties.MENU_ITEM_FOCUS_COLOR }
    };

    /// <summary>
    /// Инициализация представления пункта меню
    /// </summary>
    /// <param name="parMenuItem"></param>
    public MenuItemViewConsole(MenuItem parMenuItem) : base(parMenuItem)
    {
      const int HEIGHT = 1;
      Height = HEIGHT;
      Width = parMenuItem.Name.Length;
    }

    /// <summary>
    /// Отображение пункта меню
    /// </summary>
    public override void Draw()
    {
      Console.CursorLeft = X;
      Console.CursorTop = Y;
      Console.ForegroundColor = ColorByState[MenuElement.State];
      Console.Write(MenuElement.Name);
    }
  }
}
