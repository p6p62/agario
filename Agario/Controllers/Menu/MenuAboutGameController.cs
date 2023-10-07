using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers.Menu
{
  /// <summary>
  /// Контроллер раздела меню "Об игре"
  /// </summary>
  public abstract class MenuAboutGameController : MenuScreenController
  {
    /// <summary>
    /// Показать следующую страницу информации
    /// </summary>
    public abstract void ShowNextPage();
    /// <summary>
    /// Показать прошлую страницу информации
    /// </summary>
    public abstract void ShowPrevPage();
  }
}
