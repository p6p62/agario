using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AgarioModels.Game
{
  /// <summary>
  /// Математические функции
  /// </summary>
  public static class MathFunctions
  {
    /// <summary>
    /// Прямоугольник, задающийся двумя точками: (X1, Y1) - левый верх и (X2, Y2) - правый низ
    /// </summary>
    public struct Rectangle
    {
      /// <summary>
      /// Левая X-координата
      /// </summary>
      public float X1;
      /// <summary>
      /// Верхняя Y-координата
      /// </summary>
      public float Y1;
      /// <summary>
      /// Правая X-координата
      /// </summary>
      public float X2;
      /// <summary>
      /// Нижняя Y-координата
      /// </summary>
      public float Y2;

      /// <summary>
      /// Ширина
      /// </summary>
      public float Width => X2 - X1;
      /// <summary>
      /// Высота
      /// </summary>
      public float Height => Y2 - Y1;

      /// <summary>
      /// Проверка пересечения с другим прямоугольником
      /// </summary>
      /// <param name="parOther">Другой прямоугольник</param>
      /// <returns>True, если есть пересечение</returns>
      public bool IsIntersect(Rectangle parOther)
      {
        return MathFunctions.IsIntersect(this, parOther);
      }

      /// <summary>
      /// Проверка пересечения с клеткой
      /// </summary>
      /// <param name="parCell">Клетка</param>
      /// <returns></returns>
      public bool IsIntersect(Cell parCell)
      {
        return MathFunctions.IsIntersect(this, parCell);
      }

      /// <summary>
      /// Проверка, вложен этот прямоугольник в прямоугольник <paramref name="parOther"/> или нет
      /// </summary>
      /// <param name="parOther">Прямоугольник, вложенность в который проверяется</param>
      /// <returns>True, если прямоугольник, для которого была вызвана функция, вложен в <paramref name="parOther"/></returns>
      public bool IsNestedIn(Rectangle parOther)
      {
        return X1 > parOther.X1
        && X2 < parOther.X2
        && Y1 > parOther.Y1
        && Y2 < parOther.Y2;
      }
    }

    /// <summary>
    /// Проверка пересечения прямоугольников
    /// </summary>
    /// <param name="parR1"></param>
    /// <param name="parR2"></param>
    /// <returns></returns>
    public static bool IsIntersect(Rectangle parR1, Rectangle parR2)
    {
      bool isNotIntersect =
        (parR1.X1 > parR2.X2)
        || (parR2.X1 > parR1.X2)
        || (parR1.Y1 > parR2.Y2)
        || (parR2.Y1 > parR1.Y2);
      return !isNotIntersect;
    }

    /// <summary>
    /// Проверка пересечения прямоугольника и клетки
    /// </summary>
    /// <param name="parR"></param>
    /// <param name="parCell"></param>
    /// <returns></returns>
    public static bool IsIntersect(Rectangle parR, Cell parCell)
    {
      bool isNotIntersect =
        (parR.X1 > parCell.Position.X + parCell.Radius)
        || (parCell.Position.X - parCell.Radius > parR.X2)
        || (parR.Y1 > parCell.Position.Y + parCell.Radius)
        || (parCell.Position.Y - parCell.Radius > parR.Y2);
      return !isNotIntersect;
    }

    /// <summary>
    /// Вычисление радиуса клетки заданной массы <paramref name="parMass"/>
    /// </summary>
    /// <param name="parMass"></param>
    /// <returns></returns>
    public static float CalculateRadiusByMass(int parMass)
    {
      const float SCALE_MULTIPLIER = 0.01f;
      return MathF.Sqrt(parMass * SCALE_MULTIPLIER);
    }

    /// <summary>
    /// Вычисляет центральную точку прямоугольника
    /// </summary>
    /// <param name="parRectangle"></param>
    /// <returns></returns>
    public static Vector2 GetRectangleCenter(Rectangle parRectangle)
    {
      return new((parRectangle.X2 + parRectangle.X1) / 2, (parRectangle.Y2 + parRectangle.Y1) / 2);
    }
  }
}
