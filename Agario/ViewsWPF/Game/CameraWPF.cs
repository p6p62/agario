using AgarioModels.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Views.Game;

namespace ViewsWPF.Game
{
  /// <summary>
  /// Камера для WPF-режима
  /// </summary>
  internal class CameraWPF : Camera
  {
    /// <summary>
    /// Инициализация камеры
    /// </summary>
    /// <param name="parGame">Экземпляр игры</param>
    public CameraWPF(AgarioGame parGame) : base(parGame)
    {
      const float ADDITIONAL_SCALE_X = 1.2f;
      const float ADDITIONAL_SCALE_Y = 1.2f;

      //TrackedPlayer = GameInstance.GameField.Players.Find(p => p.Name == AgarioGame.TEST_PLAYER_NAME);
      CameraWidth = GameField.Width * ADDITIONAL_SCALE_X;
      CameraHeight = GameField.Height * ADDITIONAL_SCALE_Y;

      CenterOnTrackedPlayer();
    }

    /// <summary>
    /// Обновление камеры
    /// </summary>
    public override void Update()
    {
      // TODO изменение масштаба камеры при отображении части игрового поля по мере роста массы игрока
      CenterOnTrackedPlayer();
    }
  }
}
