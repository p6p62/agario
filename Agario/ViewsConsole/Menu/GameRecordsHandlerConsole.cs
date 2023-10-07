using AgarioModels.Menu.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewsConsole.Menu
{
  /// <summary>
  /// Обработчик игровых рекордов в консоли
  /// </summary>
  public class GameRecordsHandlerConsole : GameRecordsHandler
  {
    /// <summary>
    /// Получение имени игрока
    /// </summary>
    /// <returns>Имя игрока</returns>
    protected override string GetPlayerName()
    {
      // TODO
      throw new NotImplementedException();
    }
  }
}
