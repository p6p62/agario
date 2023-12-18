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
  /// Контроллер игровых рекордов в консоли
  /// </summary>
  internal class RecordsControllerConsole : MenuRecordsController
  {
    /// <summary>
    /// Представление экрана рекордов
    /// </summary>
    private readonly RecordsViewColsole _recordsView = new();

    /// <summary>
    /// Запуск перехода на экран рекордов
    /// </summary>
    public override void Start()
    {
      _recordsView.Draw();
      bool needExit = false;
      do
      {
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
        switch (keyInfo.Key)
        {
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
