using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewsWPF
{
  /// <summary>
  /// Глобальные константы представлений
  /// </summary>
  internal class ViewProperties
  {
    /// <summary>
    /// Размер шрифта заголовков меню
    /// </summary>
    public const int MENU_CAPTION_SIZE = 32;
    /// <summary>
    /// Размер текста меню
    /// </summary>
    public const int MENU_TEXT_SIZE = 18;
    /// <summary>
    /// Цвет текста экранов меню
    /// </summary>
    public static readonly Color MENU_SCREENS_TEXT_COLOR = Colors.Black;
    /// <summary>
    /// Цвет текста элемента в фокусе
    /// </summary>
    public static readonly Color MENU_FOCUSED_ELEMENT_TEXT_COLOR = Colors.LimeGreen;
    /// <summary>
    /// Цвет фона пунктов меню
    /// </summary>
    public static readonly Color MENU_ITEMS_BACKGROUND_COLOR = Colors.Pink;
    /// <summary>
    /// Цвет подзаголовков меню и некоторых надписей
    /// </summary>
    public static readonly Color MENU_SUBCAPTION_COLOR = Colors.LimeGreen;
    /// <summary>
    /// Цвет кнопки возврата
    /// </summary>
    public static readonly Color BACK_BUTTON_COLOR = Colors.Red;
  }
}
