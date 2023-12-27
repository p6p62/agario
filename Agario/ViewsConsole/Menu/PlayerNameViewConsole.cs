using AgarioModels.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Views.Menu;

namespace ViewsConsole.Menu
{
  /// <summary>
  /// Представление экрана меню для настройки имени игрока
  /// </summary>
  public class PlayerNameViewConsole : MenuPlayerNameView
  {
    /// <summary>
    /// Имя игрока
    /// </summary>
    public string PlayerName { get; set; } = AgarioGame.TEST_PLAYER_NAME;

    /// <summary>
    /// Рисование
    /// </summary>
    public override void Draw()
    {
      Console.BackgroundColor = ViewsProperties.MENU_BACKGROUND_COLOR;
      Console.ForegroundColor = ViewsProperties.TEXT_COLOR;
      Console.Clear();
      Console.SetCursorPosition(0, 0);
      Console.WriteLine("Имя игрока (редактирование)");

      Console.ForegroundColor = ViewsProperties.MENU_ITEM_FOCUS_COLOR;
      Console.WriteLine(PlayerName);

      Console.ForegroundColor = ViewsProperties.TEXT_COLOR;
      Console.WriteLine("Escape - выйти");
      Console.WriteLine("Enter - сохранить и выйти");
    }
  }
}
