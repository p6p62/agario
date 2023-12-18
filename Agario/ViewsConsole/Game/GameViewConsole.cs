using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Views.Game;

namespace ViewsConsole.Game
{
  /// <summary>
  /// Представление игры в консоли
  /// </summary>
  public class GameViewConsole : GameView
  {
    /// <summary>
    /// Ширина игрового окна консоли
    /// </summary>
    private const int GAME_CONSOLE_WIDTH = 30;
    /// <summary>
    /// Высота игрового окна консоли
    /// </summary>
    private const int GAME_CONSOLE_HEIGHT = 20;

    /// <summary>
    /// Создание камеры
    /// </summary>
    /// <returns>Камера</returns>
    protected override Camera CreateCamera()
    {
      Camera camera = new CameraConsole(GameInstance)
      {
        ScreenWidth = GAME_CONSOLE_WIDTH,
        ScreenHeight = GAME_CONSOLE_HEIGHT
      };
      camera.UpdateScreenWidthToHeightRatio();
      return camera;
    }

    /// <summary>
    /// Отображение игрового поля
    /// </summary>
    protected override void DrawGameField()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Отображение игровой информации
    /// </summary>
    protected override void DrawGameInfo()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Отображение таблицы лидеров
    /// </summary>
    protected override void DrawLeaderboard()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Обновление представления при установке паузы
    /// </summary>
    protected override void OnPause()
    {
      Debug.WriteLine("Paused");
    }

    /// <summary>
    /// Обновление представления при возобновлении игры
    /// </summary>
    protected override void OnResume()
    {
      Debug.WriteLine("Resumed");
    }

    /// <summary>
    /// Настройка экрана при запуске игры
    /// </summary>
    protected override void OnStartGame()
    {
      if (OperatingSystem.IsWindows())
      {
        Console.WindowWidth = GAME_CONSOLE_WIDTH;
        Console.WindowHeight = GAME_CONSOLE_HEIGHT;
        Console.BufferWidth = GAME_CONSOLE_WIDTH;
        Console.BufferHeight = GAME_CONSOLE_HEIGHT;
      }
    }

    /// <summary>
    /// Обновление представления при остановке игры
    /// </summary>
    protected override void OnStopGame()
    {
      Debug.WriteLine("Exit from game");
    }
  }
}
