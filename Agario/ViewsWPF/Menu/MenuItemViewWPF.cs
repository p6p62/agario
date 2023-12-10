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
    /// Отступ элементов от боковых сторон
    /// </summary>
    public const int ITEMS_LEFT_RIGHT_MARGIN = 30;
    /// <summary>
    /// Отступ элементов по вертикали
    /// </summary>
    public const int ITEMS_UP_DOWN_MARGIN = 15;
    /// <summary>
    /// Внутренний отступ текста в элементах
    /// </summary>
    public const int ITEMS_PADDING = 7;

    /// <summary>
    /// Логический родитель представления
    /// </summary>
    private FrameworkElement _parent = null!;

    /// <summary>
    /// Прямоугольник для отображения элемента меню
    /// </summary>
    private readonly TextBlock _menuItemFigure = new()
    {
      HorizontalAlignment = HorizontalAlignment.Stretch,
      VerticalAlignment = VerticalAlignment.Center,
      TextAlignment = TextAlignment.Center,
      Margin = new(ITEMS_LEFT_RIGHT_MARGIN, ITEMS_UP_DOWN_MARGIN, ITEMS_LEFT_RIGHT_MARGIN, ITEMS_UP_DOWN_MARGIN),
      Padding = new(ITEMS_PADDING)
    };

    /// <summary>
    /// Пункт меню
    /// </summary>
    private readonly MenuItem _menuItem;

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
      _menuItem = parMenuItem;
      _menuItemFigure.Text = parMenuItem.Name;
      _menuItemFigure.FontSize = ViewProperties.MENU_CAPTION_SIZE;
      Grid.SetRow(_menuItemFigure, parMenuItem.ID);
    }

    /// <summary>
    /// Отображение пункта меню
    /// </summary>
    public override void Draw()
    {
      _menuItemFigure.Text = _menuItem.Name;
      if (MenuElement.State == MenuItem.MenuItemState.Focused)
      {
        _menuItemFigure.Background = new SolidColorBrush(ViewProperties.MENU_ITEMS_BACKGROUND_COLOR);
        _menuItemFigure.Foreground = new SolidColorBrush(ViewProperties.MENU_FOCUSED_ELEMENT_TEXT_COLOR);
      }
      else
      {
        _menuItemFigure.Background = new SolidColorBrush(ViewProperties.MENU_ITEMS_BACKGROUND_COLOR);
        _menuItemFigure.Foreground = new SolidColorBrush(ViewProperties.MENU_SCREENS_TEXT_COLOR);
      }
    }
  }
}
