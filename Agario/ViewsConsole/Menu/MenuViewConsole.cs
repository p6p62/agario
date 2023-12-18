using AgarioModels.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Views.Menu;

namespace ViewsConsole.Menu
{
  /// <summary>
  /// Представление меню в консоли
  /// </summary>
  public class MenuViewConsole : MenuView
  {
    /// <summary>
    /// Ширина меню
    /// </summary>
    public const int WIDTH = 40;
    /// <summary>
    /// Высота меню
    /// </summary>
    public const int HEIGHT = 15;
    /// <summary>
    /// Заголовок окна
    /// </summary>
    public const string GAME_TITLE = "AGARIO";

    /// <summary>
    /// Инициализация представления меню
    /// </summary>
    /// <param name="parMenu">Меню</param>
    public MenuViewConsole(AgarioModels.Menu.Menu parMenu) : base(parMenu)
    {
      InitializeMenuView();
      Draw();
    }

    /// <summary>
    /// Установка размеров консоли для отображения меню
    /// </summary>
    private static void SetConsoleParametersForMenu()
    {
      if (OperatingSystem.IsWindows())
      {
        Console.WindowHeight = HEIGHT;
        Console.WindowWidth = WIDTH;
        Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
        Console.CursorVisible = false;
        Console.Title = GAME_TITLE;
      }
    }

    /// <summary>
    /// Настройка отображения консоли
    /// </summary>
    private void InitializeMenuView()
    {
      SetConsoleParametersForMenu();

      // чтобы дождаться обновления заголовка, инача вызов не находит дескриптор окна
      ConsoleHelperUtilite.WaitSomeEmpiricalTimeInterval();
      ConsoleHelperUtilite.MoveConsoleWindow(GAME_TITLE, 100, 50);
      ConsoleHelperUtilite.SetConsoleOpacity(GAME_TITLE, 0xFF);

      int menuHeight = Items.Count;
      int menuWidth = Items.Max(x => x.Value.Width);

      int x = (WIDTH - menuWidth) / 2;
      int y = 0;
      foreach (MenuItemView elMenuItemView in Items.Values)
      {
        elMenuItemView.X = x;
        elMenuItemView.Y = y++;
      }
    }

    /// <summary>
    /// Создание представления элемента меню
    /// </summary>
    /// <param name="parMenuItem">Элемент меню</param>
    /// <returns>Представление элемента меню</returns>
    public override MenuItemView CreateMenuItemView(MenuItem parMenuItem)
    {
      return new MenuItemViewConsole(parMenuItem);
    }

    /// <summary>
    /// Отображение меню на экране
    /// </summary>
    public override void Draw()
    {
      SetConsoleParametersForMenu();
      Console.BackgroundColor = ViewsProperties.MENU_BACKGROUND_COLOR;
      Console.ForegroundColor = ViewsProperties.TEXT_COLOR;
      Console.Clear();
      foreach (MenuItemView elMenuItemView in Items.Values)
      {
        elMenuItemView.Draw();
      }
    }
  }
}
