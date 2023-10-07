using AgarioModels.Menu;
using ControllersWPF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AgarioWPF
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    /// <summary>
    /// Обработка события Startup для запуска контроллера главного меню
    /// </summary>
    /// <param name="e">Аргументы события</param>
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      MenuMainControllerWPF menuMainController = new(MenuMain.GetMenu());
      menuMainController.Start();
    }
  }
}
