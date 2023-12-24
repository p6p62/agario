using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewsConsole
{
  /// <summary>
  /// Набор глобальных констант для представлений
  /// </summary>
  internal static class ViewsProperties
  {
    /// <summary>
    /// Цвет текста
    /// </summary>
    public const ConsoleColor TEXT_COLOR = ConsoleColor.White;
    /// <summary>
    /// Цвет фона элемента меню в фокусе
    /// </summary>
    public const ConsoleColor MENU_ITEM_FOCUS_COLOR = ConsoleColor.Green;
    /// <summary>
    /// Цвет фона меню
    /// </summary>
    public const ConsoleColor MENU_BACKGROUND_COLOR = ConsoleColor.Black;
    /// <summary>
    /// Цвет кнопки возврата
    /// </summary>
    public const ConsoleColor BACK_BUTTON_COLOR = ConsoleColor.DarkRed;
    /// <summary>
    /// Цвет фона игры
    /// </summary>
    public const ConsoleColor GAME_BACKGROUND_COLOR = ConsoleColor.Black;
    /// <summary>
    /// Цвет текста игры
    /// </summary>
    public const ConsoleColor GAME_TEXT_COLOR = ConsoleColor.White;
  }
}
