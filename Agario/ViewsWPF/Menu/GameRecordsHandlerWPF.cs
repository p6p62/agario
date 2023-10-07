using AgarioModels.Menu.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewsWPF.Menu
{
  /// <summary>
  /// Обработчик рекордов после окончания игры
  /// </summary>
  public class GameRecordsHandlerWPF : GameRecordsHandler
  {
    /// <summary>
    /// Инициализация обработчика рекордов
    /// </summary>
    /// <param name="parWindow">Окно меню</param>
    public GameRecordsHandlerWPF()
    {
    }

    /// <summary>
    /// Запрос имени игрока
    /// </summary>
    /// <returns>Имя игрока</returns>
    protected override string GetPlayerName()
    {
      throw new NotImplementedException();
      // TODO
      //_inputWaitEvent.Reset();

      //try
      //{
      //  _window.Dispatcher.Invoke(new Action(() =>
      //  {
      //    _window.Background = new SolidColorBrush(ViewProperties.BROWN_COLOR);
      //    _nameTextBox.Focus();
      //    _window.Content = _screen;
      //  }));
      //  _inputWaitEvent.WaitOne();
      //}
      //catch (ThreadInterruptedException)
      //{
      //  // пусто, даём нормально завершиться вводу
      //}

      //StringBuilder name = new();
      //_window.Dispatcher.Invoke(() => name.Append(_nameTextBox.Text));
      //return name.ToString();
    }
  }
}
