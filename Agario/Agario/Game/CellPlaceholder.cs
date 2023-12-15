using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgarioModels.Game
{
  /// <summary>
  /// Расстановщик клеток еды или игроков
  /// </summary>
  internal class CellPlaceholder
  {
    /// <summary>
    /// Игровое поле
    /// </summary>
    private readonly GameField _gameField;

    /// <summary>
    /// Генератор случайных чисел
    /// </summary>
    private readonly Random _random = new();

    /// <summary>
    /// Инициализация
    /// </summary>
    /// <param name="parGameField">Игровое поле</param>
    public CellPlaceholder(GameField parGameField)
    {
      _gameField = parGameField;
    }

    /// <summary>
    /// Устанавливает случайное положение на игровом поле для переданной клетки так, чтобы она не перекрывалась существующей
    /// на игровом поле едой или другими игроками
    /// </summary>
    /// <param name="parCell">Клетка, которой будет задано случайное положение</param>
    public void SetRandomPosition(Cell parCell)
    {
      // TODO
      // TODO проверить на столкновение с едой
      float radius = parCell.Radius;
      float lowerBound = radius;
      float upperBoundX = _gameField.Width - radius;
      float upperBoundY = _gameField.Height - radius;
      float multiplierX = upperBoundX - lowerBound;
      float multiplierY = upperBoundY - lowerBound;
      bool isOverlap = false;

      Cell tempCell = new(parCell);
      do
      {
        isOverlap = false;
        float x = lowerBound + (float)_random.NextDouble() * multiplierX;
        float y = lowerBound + (float)_random.NextDouble() * multiplierY;
        tempCell.Position = new(x, y);

        // определение занятости случайного места игроками
        for (int i = 0; !isOverlap && i < _gameField.Players.Count; i++)
        {
          Player player = _gameField.Players[i];
          for (int j = 0; !isOverlap && j < player.Cells.Count; j++)
          {
            isOverlap = tempCell.IsIntersect(player.Cells[j]);
          }
        }
      } while (isOverlap);
      parCell.Position = tempCell.Position;
    }
  }
}
