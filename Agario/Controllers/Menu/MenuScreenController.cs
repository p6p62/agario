using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers.Menu
{
  /// <summary>
  /// Общее для контроллеров экранов меню
  /// </summary>
  public abstract class MenuScreenController
  {
    /// <summary>
    /// Событие, возникающее при возврате на предыдущий экран
    /// </summary>
    public event Action? GoToBack = null;
    /// <summary>
    /// Открытие раздела меню
    /// </summary>
    public abstract void Start();
    /// <summary>
    /// Вызов события GoToBack
    /// </summary>
    protected void GoBackCall() => GoToBack?.Invoke();
  }
}
