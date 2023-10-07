using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AgarioModels.Menu.AboutGame
{
  /// <summary>
  /// Класс, оборачивающий доступ к информации об игре
  /// </summary>
  public static class DataReader
  {
    /// <summary>
    /// Получает информацию об игре из ресурсов
    /// </summary>
    /// <returns>Информация об игре</returns>
    public static string GetInformationAboutGame() => AgarioModels.Properties.Resources.AboutGameText;
  }
}
