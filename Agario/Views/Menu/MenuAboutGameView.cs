using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Views.Menu
{
  /// <summary>
  /// Представление экрана меню "Об игре"
  /// </summary>
  public abstract class MenuAboutGameView : MenuViewCommon
  {
    /// <summary>
    /// Показать следующую страницу
    /// </summary>
    public abstract void ShowNextPage();
    /// <summary>
    /// Показать предыдущую страницу
    /// </summary>
    public abstract void ShowPrevPage();
  }
}
