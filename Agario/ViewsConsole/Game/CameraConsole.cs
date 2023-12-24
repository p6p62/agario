using AgarioModels.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Views.Game;

namespace ViewsConsole.Game
{
  /// <summary>
  /// Камера в консоли
  /// </summary>
  internal class CameraConsole : Camera
  {
    /// <summary>
    /// Инициализация камеры
    /// </summary>
    /// <param name="parGame"></param>
    public CameraConsole(AgarioGame parGame) : base(parGame)
    {
      const float ADDITIONAL_SCALE_X = 0.2f;
      const float ADDITIONAL_SCALE_Y = 0.2f;

      TrackedPlayer = GameInstance.GameField.Players.Find(p => p.Name == AgarioGame.TEST_PLAYER_NAME);
      CameraWidth = GameField.Width * ADDITIONAL_SCALE_X;
      CameraHeight = GameField.Height * ADDITIONAL_SCALE_Y;

      CenterOnTrackedPlayer();
    }

    /// <summary>
    /// Обновление камеры
    /// </summary>
    public override void Update()
    {
      CenterOnTrackedPlayer();
    }
  }
}
