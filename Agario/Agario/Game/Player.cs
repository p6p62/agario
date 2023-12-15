using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AgarioModels.Game
{
  /// <summary>
  /// Клетка или группа клеток, управляемая как единое целое. Класс представляет собой 
  /// просто связанную систему клеток с общим управлением и не является специфичным для
  /// "реального" игрока за компьютером
  /// </summary>
  public class Player
  {
    /// <summary>
    /// Имя игрока
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Рейтинг
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Максимальный рейтинг за время запуска приложения
    /// </summary>
    public int MaxScore { get; set; }

    /// <summary>
    /// Клетки игрока
    /// </summary>
    public List<MovingCell> Cells { get; private set; } = new();

    /// <summary>
    /// Состояние игрока (живой, мёртвый)
    /// </summary>
    public bool IsAlive { get; set; } = true;

    /// <summary>
    /// Получение минимального ограничивающего прямоугольника, включающего все клетки игрока
    /// </summary>
    /// <returns>Ограничивающий прямоугольник минимального размера</returns>
    public MathFunctions.Rectangle GetBoundingRect()
    {
      float xMin = Cells.Min(c => c.Position.X - c.Radius);
      float yMin = Cells.Min(c => c.Position.Y - c.Radius);
      float xMax = Cells.Max(c => c.Position.X + c.Radius);
      float yMax = Cells.Max(c => c.Position.Y + c.Radius);
      return new() { X1 = xMin, Y1 = yMin, X2 = xMax, Y2 = yMax };
    }
  }
}
