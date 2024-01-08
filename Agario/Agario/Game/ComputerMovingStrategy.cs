using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static AgarioModels.Game.MathFunctions;

namespace AgarioModels.Game
{
  /// <summary>
  /// Логика перемещения игроков, управляемых компьютером
  /// </summary>
  public class ComputerMovingStrategy
  {
    /// <summary>
    /// Количество секунд между обновлениями
    /// </summary>
    private const float SECONDS_BETWEEN_UPDATES = 0.2f;

    /// <summary>
    /// Расстояние до клетки другого игрока большего размера, которое считается опасным (в квадрате)
    /// </summary>
    private const float DANGER_DISTANCE_TO_ESCAPE_SQUARED = 9 * 9;

    /// <summary>
    /// Расстояние до клетки другого игрока меньшего размера, при котором клетка будет пытаться его поймать (в квадрате)
    /// </summary>
    private const float MAXIMUM_CATCH_DISTANCE_SQUARED = 5 * 5;

    /// <summary>
    /// Управляемый игрок
    /// </summary>
    private readonly Player _controlledPlayer;

    /// <summary>
    /// Ограничивающий прямоугольник игрока
    /// </summary>
    private Rectangle _controlledPlayerBoundingRect;

    /// <summary>
    /// Центральная точка ограничивающего прямоугольника
    /// </summary>
    private Vector2 _boundingRectCenter;

    /// <summary>
    /// Игровое поле
    /// </summary>
    private readonly GameField _gameField;

    /// <summary>
    /// Время с момента прошлого обновления
    /// </summary>
    private float _timeFromLastUpdate = 0;

    /// <summary>
    /// Инициализация
    /// </summary>
    /// <param name="parPlayer">Управляемый игрок</param>
    /// <param name="parGameField">Игровое поле</param>
    public ComputerMovingStrategy(Player parPlayer, GameField parGameField)
    {
      _controlledPlayer = parPlayer;
      _gameField = parGameField;
    }

    /// <summary>
    /// Догнать игрока
    /// </summary>
    /// <param name="parPlayer">Игрок</param>
    /// <returns>Вектор скорости, позволяющий поймать игрока <paramref name="parPlayer"/></returns>
    private Vector2 CatchPlayer(Player parPlayer)
    {
      return GetRectangleCenter(parPlayer.GetBoundingRect()) - _boundingRectCenter;
    }

    /// <summary>
    /// Сбежать от игрока
    /// </summary>
    /// <param name="parPlayer">Игрок</param>
    /// <returns>Вектор скорости, позволяющий сбежать от игрока <paramref name="parPlayer"/></returns>
    private Vector2 EscapeFromPlayer(Player parPlayer)
    {
      return _boundingRectCenter - GetRectangleCenter(parPlayer.GetBoundingRect());
    }

    /// <summary>
    /// Вычисление вектора скорости, чтобы двигаться к ближайшей еде
    /// </summary>
    /// <returns></returns>
    private Vector2 GetSpeedByClosestEat()
    {
      Vector2 speed = new();
      Cell? closestEat = _gameField.Food.MinBy(e => (e.Position - _boundingRectCenter).LengthSquared());
      if (closestEat != null)
        speed = (closestEat.Position - _boundingRectCenter);
      return speed;
    }

    /// <summary>
    /// Оценивая положение и состояние других игроков, по необходимости вычисляет новый вектор скорости
    /// или возвращает <paramref name="parCurrentSpeedVector"/>
    /// </summary>
    /// <param name="parCurrentSpeedVector">Расчитанный вектор скорости, который вернётся, если состояние других 
    /// игроков не является существенным для изменения скорости</param>
    /// <returns></returns>
    private Vector2 GetSpeedByOtherPlayersStates(Vector2 parCurrentSpeedVector)
    {
      Vector2 speed = parCurrentSpeedVector;

      Player? closestBiggerPlayer = null;
      float? closestBiggerPlayerDistance = null;
      Player? closestSmallerPlayer = null;
      float? closestSmallerPlayerDistance = null;
      foreach (Player elPlayer in _gameField.Players)
      {
        if (!elPlayer.IsAlive || elPlayer == _controlledPlayer)
          continue;

        if (elPlayer.Score > _controlledPlayer.Score * (1 + GameField.EAT_RELATIVE_MASS_DIFFERENCE))
        {
          float distance = DistanceBetweenCentersSquared(_controlledPlayerBoundingRect, elPlayer.GetBoundingRect());
          closestBiggerPlayerDistance ??= distance;
          if (distance <= DANGER_DISTANCE_TO_ESCAPE_SQUARED && distance <= closestBiggerPlayerDistance)
          {
            closestBiggerPlayer = elPlayer;
            closestBiggerPlayerDistance = distance;
          }
        }
        else if (elPlayer.Score * (1 + GameField.EAT_RELATIVE_MASS_DIFFERENCE) < _controlledPlayer.Score)
        {
          float distance = DistanceBetweenCentersSquared(_controlledPlayerBoundingRect, elPlayer.GetBoundingRect());
          closestSmallerPlayerDistance ??= distance;
          if (distance <= MAXIMUM_CATCH_DISTANCE_SQUARED && distance <= closestSmallerPlayerDistance)
          {
            closestSmallerPlayer = elPlayer;
            closestSmallerPlayerDistance = distance;
          }
        }
      }

      if (closestBiggerPlayer != null)
        speed = EscapeFromPlayer(closestBiggerPlayer);
      else if (closestSmallerPlayer != null)
        speed = CatchPlayer(closestSmallerPlayer);
      return speed;
    }

    /// <summary>
    /// Перерасчёт направления движения
    /// </summary>
    private void RecalculateSpeedAndDirection()
    {
      const float SPEED_MULTIPLIER = 1000;
      _controlledPlayerBoundingRect = _controlledPlayer.GetBoundingRect();
      _boundingRectCenter = GetRectangleCenter(_controlledPlayerBoundingRect);

      Vector2 speed = GetSpeedByClosestEat();
      speed = GetSpeedByOtherPlayersStates(speed);

      _gameField.SetSpeedToPlayer(_controlledPlayer, speed * SPEED_MULTIPLIER);
    }

    /// <summary>
    /// Обновление, перерасчёт параметров
    /// </summary>
    /// <param name="parDeltaTime">Изменение внутреннего времени в секундах</param>
    public void Update(float parDeltaTime)
    {
      if (_controlledPlayer.IsAlive)
      {
        _timeFromLastUpdate += parDeltaTime;
        if (_timeFromLastUpdate >= SECONDS_BETWEEN_UPDATES)
        {
          _timeFromLastUpdate = 0;
          RecalculateSpeedAndDirection();
        }
      }
    }
  }
}
