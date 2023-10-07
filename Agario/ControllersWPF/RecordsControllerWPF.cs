using Controllers.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewsWPF.Menu;

namespace ControllersWPF
{
  /// <summary>
  /// Контроллер экрана рекордов в WPF
  /// </summary>
  internal class RecordsControllerWPF : MenuRecordsController
  {
    /// <summary>
    /// Представление рекордов
    /// </summary>
    private readonly RecordsViewWPF _recordsView;
    /// <summary>
    /// Окно меню
    /// </summary>
    private readonly Window _window;

    /// <summary>
    /// Инициализация контроллера рекордов, создание представления
    /// </summary>
    /// <param name="parWindow">Окно меню</param>
    public RecordsControllerWPF(Window parWindow)
    {
      _window = parWindow;
      _recordsView = new(_window);
      _recordsView.GoBackSelected += GoBackCall;
    }

    /// <summary>
    /// Открытие раздела меню "Рекорды"
    /// </summary>
    public override void Start()
    {
      _recordsView.Draw();
    }
  }
}
