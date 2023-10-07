using Controllers.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewsConsole.Menu;

namespace ControllersConsole
{
  /// <summary>
  /// Контроллер раздела меню "Об игре"
  /// </summary>
  internal class AboutGameControllerConsole : MenuAboutGameController
  {
    /// <summary>
    /// Представление раздела
    /// </summary>
    private readonly AboutGameViewConsole _aboutGameView;

    /// <summary>
    /// Инициализация контроллера пункта меню, создание представления
    /// </summary>
    public AboutGameControllerConsole()
    {
      _aboutGameView = new AboutGameViewConsole();
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

    /// <summary>
    /// Открытие раздела меню "Об игре"
    /// </summary>
    public override void Start()
    {
      _aboutGameView.Draw();
      bool needExit = false;
      do
      {
        ConsoleKeyInfo keyInfo = Console.ReadKey();
        switch (keyInfo.Key)
        {
          case ConsoleKey.LeftArrow:
            ShowPrevPage();
            break;
          case ConsoleKey.RightArrow:
            ShowNextPage();
            break;
          case ConsoleKey.Enter:
          case ConsoleKey.Spacebar:
          case ConsoleKey.Escape:
            needExit = true;
            GoBackCall();
            break;
        }
      } while (!needExit);
    }
  }
}
