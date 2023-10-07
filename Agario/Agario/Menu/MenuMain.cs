using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgarioModels.Menu
{
  /// <summary>
  /// Стартовое меню игры
  /// </summary>
  public class MenuMain
  {
    /// <summary>
    /// Коды элементов главного меню
    /// </summary>
    public enum MenuItemCodes : int
    {
      /// <summary>
      /// Начать игру
      /// </summary>
      StartGame,
      /// <summary>
      /// Информация об игре
      /// </summary>
      About,
      /// <summary>
      /// Рекорды
      /// </summary>
      Records,
      /// <summary>
      /// Выход
      /// </summary>
      Exit
    }

    /// <summary>
    /// Получение главного меню
    /// </summary>
    /// <returns>Главное меню</returns>
    public static Menu GetMenu()
    {
      Menu menu = new();
      menu.AddItem(new MenuItem((int)MenuItemCodes.StartGame, "Играть"));
      menu.AddItem(new MenuItem((int)MenuItemCodes.About, "Об игре"));
      menu.AddItem(new MenuItem((int)MenuItemCodes.Records, "Рекорды"));
      menu.AddItem(new MenuItem((int)MenuItemCodes.Exit, "Выход"));
      menu.FocusByID((int)MenuItemCodes.StartGame);
      return menu;
    }
  }
}
