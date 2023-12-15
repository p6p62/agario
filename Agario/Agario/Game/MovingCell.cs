using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AgarioModels.Game
{
  /// <summary>
  /// Движущаяся клетка
  /// </summary>
  public class MovingCell : Cell
  {
    /// <summary>
    /// Скорость [единица расстояния на поле / секунда]
    /// </summary>
    public Vector2 Speed { get; set; }
    /// <summary>
    /// Ускорение [(единица расстояния на поле / секунда) / секунда]
    /// </summary>
    public Vector2 Acceleration { get; set; }
    /// <summary>
    /// Время с момента последнего деления в секундах
    /// </summary>
    public float TimeFromLastDivision { get; set; }
  }
}
