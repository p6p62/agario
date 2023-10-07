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
  /// Контроллер раздела меню "Об игре" в WPF
  /// </summary>
  internal class AboutGameControllerWPF : MenuAboutGameController
  {
    /// <summary>
    /// Представление раздела "Об игре"
    /// </summary>
    private AboutGameViewWPF _aboutGameView = null!;
    /// <summary>
    /// Окно приложения
    /// </summary>
    private readonly Window _window;

    /// <summary>
    /// Создание раздела меню "Об игре" и его представления
    /// </summary>
    /// <param name="parWindow">Окно приложения</param>
    public AboutGameControllerWPF(Window parWindow)
    {
      _window = parWindow;
      ConfigureView();
    }

    /// <summary>
    /// Открытие раздела меню "Об игре"
    /// </summary>
    public override void Start()
    {
      _aboutGameView.Draw();
    }

    /// <summary>
    /// Настройка представления
    /// </summary>
    private void ConfigureView()
    {
      _aboutGameView = new(_window);
      _aboutGameView.GoBackSelected += GoBackCall;
      _aboutGameView.NextPageSelected += ShowNextPage;
      _aboutGameView.PrevPageSelected += ShowPrevPage;
    }

    /// <summary>
    /// Показать следующую страницу информации
    /// </summary>
    public override void ShowNextPage()
    {
      _aboutGameView.ShowNextPage();
    }

    /// <summary>
    /// Показать прошлую страницу информации
    /// </summary>
    public override void ShowPrevPage()
    {
      _aboutGameView.ShowPrevPage();
    }
  }
}
