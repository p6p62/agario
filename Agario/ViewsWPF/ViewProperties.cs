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
    public const int MENU_CAPTION_SIZE = 48;
    /// <summary>
    /// Размер текста меню
    /// </summary>
    public const int MENU_TEXT_SIZE = 18;
    /// <summary>
    /// Коричневый цвет, используемый в игре
    /// </summary>
    public static readonly Color BROWN_COLOR = Color.FromRgb(88, 61, 51);
    /// <summary>
    /// Цвет текста экранов меню
    /// </summary>
    public static readonly Color MENU_SCREENS_TEXT_COLOR = Colors.White;
  }
}
