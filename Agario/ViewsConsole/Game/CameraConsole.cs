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
    /// Масштабирование камеры по размерам игрока
    /// </summary>
    private void AdjustToPlayerSize()
    {
      /* Минимальный и максимальный масштабный коэффициент размеров игрока относительно высоты экрана.
       * При снижении значения ниже MIN_PLAYER_TO_VIEWPORT_SCALE_FACTOR камера слегка сожмёт окно просмотра.
       * При превышении выше MAX_PLAYER_TO_VIEWPORT_SCALE_FACTOR камера расширит окно просмотра. Если размеры 
       * игрока меняются так, что масштабный коэффициент остаётся внутри интервала, то ничего не происходит*/
      const float MIN_PLAYER_TO_VIEWPORT_SCALE_FACTOR = 0.3f;
      const float MAX_PLAYER_TO_VIEWPORT_SCALE_FACTOR = 0.8f;
      const float SCALE_FACTOR_AFTER_ADJUST = 0.5f;

      if (TrackedPlayer == null || TrackedPlayer.Cells.Count == 0)
        return;

      float playerRadiusOnScreen = CalculateLineLengthInScreen(TrackedPlayer.Cells[0].Radius);
      float scaleFactor = playerRadiusOnScreen / CameraHeight;
      if (TrackedPlayer.Cells.Count == 1
        && (scaleFactor < MIN_PLAYER_TO_VIEWPORT_SCALE_FACTOR
        || scaleFactor > MAX_PLAYER_TO_VIEWPORT_SCALE_FACTOR))
      {
        CameraHeight = playerRadiusOnScreen / SCALE_FACTOR_AFTER_ADJUST;
      }
    }

    /// <summary>
    /// Обновление камеры
    /// </summary>
    public override void Update()
    {
      AdjustToPlayerSize();
      CenterOnTrackedPlayer();
    }
  }
}
