using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Views.Menu;
using MenuItem = AgarioModels.Menu.MenuItem;

namespace ViewsWPF.Menu
{
  /// <summary>
  /// Представление элемента меню в WPF
  /// </summary>
  internal class MenuItemViewWPF : MenuItemView
  {
    /// <summary>
    /// Логический родитель представления
    /// </summary>
    private FrameworkElement _parent = null!;

    /// <summary>
    /// Прямоугольник для отображения элемента меню
    /// </summary>
    private readonly TextBlock _menuItemFigure = new() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center };

    /// <summary>
    /// Логический родитель представления
    /// </summary>
    public FrameworkElement Parent
    {
      get => _parent;
      set
      {
        _parent = value;
        ((IAddChild)_parent)?.AddChild(_menuItemFigure);
      }
    }

    /// <summary>
    /// Инициализация представления элемента меню
    /// </summary>
    /// <param name="parMenuItem">Элемент меню</param>
    public MenuItemViewWPF(MenuItem parMenuItem) : base(parMenuItem)
    {
      _menuItemFigure.Text = parMenuItem.Name;
      _menuItemFigure.FontSize = ViewProperties.MENU_CAPTION_SIZE;
      Grid.SetRow(_menuItemFigure, parMenuItem.ID);
    }

    /// <summary>
    /// Отображение пункта меню
    /// </summary>
    public override void Draw()
    {
      if (MenuElement.State == MenuItem.MenuItemState.Focused)
      {
        _menuItemFigure.Background = Brushes.White;
        _menuItemFigure.Foreground = Brushes.Black;
      }
      else
      {
        _menuItemFigure.Background = new SolidColorBrush(ViewProperties.BROWN_COLOR);
        _menuItemFigure.Foreground = Brushes.White;
      }
    }
  }
}
