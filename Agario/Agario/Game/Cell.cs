using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AgarioModels.Game
{
  /// <summary>
  /// Клетка
  /// </summary>
  public class Cell
  {
    /// <summary>
    /// Масса
    /// </summary>
    private int _weight;

    /// <summary>
    /// Положение
    /// </summary>
    public Vector2 Position { get; set; }
    /// <summary>
    /// Масса
    /// </summary>
    public int Weight
    {
      get => _weight;
      set
      {
        _weight = value;
        Radius = MathFunctions.CalculateRadiusByMass(value);
      }
    }
    /// <summary>
    /// Радиус
    /// </summary>
    public float Radius { get; set; }

    /// <summary>
    /// Конструктор по умолчанию
    /// </summary>
    public Cell()
    {
    }

    /// <summary>
    /// Конструктор копирования
    /// </summary>
    /// <param name="parCell">Клетка, по данным которой будет инициализирован объект</param>
    public Cell(Cell parCell)
    {
      Position = parCell.Position;
      Weight = parCell.Weight;
      Radius = parCell.Radius;
    }

    /// <summary>
    /// Проверка пересечения с клеткой <paramref name="parCell"/>
    /// </summary>
    /// <param name="parCell">Проверяемая клетка</param>
    /// <returns>true, если клетка пересекается с клеткой <paramref name="parCell"/>, иначе false</returns>
    public bool IsIntersect(Cell parCell)
    {
      // наложение есть, если расстояние между центрами меньше суммы радиусов клеток
      // возведение в квадрат для избавления от вычисления корня
      float radiusSum = Radius + parCell.Radius;
      return radiusSum * radiusSum > (parCell.Position - Position).LengthSquared();
    }
  }
}
