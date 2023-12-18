using AgarioModels.Menu.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Views.Menu;

namespace ViewsConsole.Menu
{
  /// <summary>
  /// Представление пункта меню "Рекорды" в консоли
  /// </summary>
  public class RecordsViewColsole : MenuRecordsView
  {
    /// <summary>
    /// Отображение
    /// </summary>
    public override void Draw()
    {
      const int TOP_OFFSET = 2;
      const string RECORDS_CAPTION = "Рекорды";
      Console.BackgroundColor = ViewsProperties.MENU_BACKGROUND_COLOR;
      Console.ForegroundColor = ViewsProperties.TEXT_COLOR;
      Console.Clear();

      Console.SetCursorPosition(0, 0);
      Console.Write(RECORDS_CAPTION);

      Console.SetCursorPosition(0, TOP_OFFSET);
      Console.ForegroundColor = ViewsProperties.MENU_ITEM_FOCUS_COLOR;
      Console.Write("Имя");

      const string SCORE_TITLE = "Рейтинг";
      int rowNumber = TOP_OFFSET;
      Console.SetCursorPosition(Console.WindowWidth - SCORE_TITLE.Length, rowNumber++);
      Console.Write(SCORE_TITLE);

      Console.ForegroundColor = ViewsProperties.TEXT_COLOR;
      foreach (Record elRecord in GameRecordsHandler.GetRecords())
      {
        Console.SetCursorPosition(0, rowNumber);
        Console.Write(elRecord.Name);
        Console.SetCursorPosition(Console.WindowWidth - elRecord.Value.ToString().Length, rowNumber);
        Console.Write(elRecord.Value);
        ++rowNumber;
      }

      Console.SetCursorPosition(0, Console.WindowHeight - 1);
      Console.ForegroundColor = ViewsProperties.BACK_BUTTON_COLOR;
      Console.Write("Назад");
    }
  }
}
