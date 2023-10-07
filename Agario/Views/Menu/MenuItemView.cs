using AgarioModels.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Views.Menu
{
  /// <summary>
  /// Представление элемента меню
  /// </summary>
  public abstract class MenuItemView : MenuViewCommon
  {
    /// <summary>
    /// Элемент меню
    /// </summary>
    protected MenuItem MenuElement { get; set; }

    /// <summary>
    /// Позиция по X
    /// </summary>
    public int X { get; set; }
    /// <summary>
    /// Позиция по Y
    /// </summary>
    public int Y { get; set; }
    /// <summary>
    /// Ширина
    /// </summary>
    public int Width { get; protected set; }
    /// <summary>
    /// Высота
    /// </summary>
    public int Height { get; protected set; }

    /// <summary>
    /// Создание представления для элемента меню
    /// </summary>
    /// <param name="parMenuItem">Элемент меню</param>
    public MenuItemView(MenuItem parMenuItem)
    {
      MenuElement = parMenuItem;
    }
  }
}
