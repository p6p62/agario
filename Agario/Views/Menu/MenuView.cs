using AgarioModels.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Views.Menu
{
  /// <summary>
  /// Представление меню
  /// </summary>
  public abstract class MenuView : MenuViewCommon
  {
    /// <summary>
    /// Представления элементов меню
    /// </summary>
    protected Dictionary<int, MenuItemView> Items { get; set; } = new();

    /// <summary>
    /// Создаёт представление для меню
    /// </summary>
    /// <param name="parMenu">Меню</param>
    public MenuView(AgarioModels.Menu.Menu parMenu)
    {
      foreach (MenuItem elMenuItem in parMenu.Items)
        Items.Add(elMenuItem.ID, CreateMenuItemView(elMenuItem));

      parMenu.NeedRedraw += Draw;
    }

    /// <summary>
    /// Создание представления для элемента меню
    /// </summary>
    /// <param name="parMenuItem">Элемент меню</param>
    /// <returns>Созданное представление</returns>
    public abstract MenuItemView CreateMenuItemView(MenuItem parMenuItem);
  }
}
