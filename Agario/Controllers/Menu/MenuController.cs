using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers.Menu
{
  /// <summary>
  /// Общее для контроллеров меню
  /// </summary>
  public abstract class MenuController
  {
    /// <summary>
    /// Связанное с контроллером меню
    /// </summary>
    protected AgarioModels.Menu.Menu Menu { get; set; }

    /// <summary>
    /// Конструктор контроллера меню
    /// </summary>
    /// <param name="parMenu">Меню, для которого создаётся контроллер</param>
    public MenuController(AgarioModels.Menu.Menu parMenu)
    {
      Menu = parMenu;
    }

    /// <summary>
    /// Запуск меню
    /// </summary>
    public abstract void Start();
  }
}
