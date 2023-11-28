﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AgarioModels.Game
{
  /// <summary>
  /// Игровое поле
  /// </summary>
  public class GameField
  {
    /// <summary>
    /// Начальная масса игроков
    /// </summary>
    private const int START_SCORE = 40;

    /// <summary>
    /// Период генерации новой еды (секунды)
    /// </summary>
    private const float EAT_GENERATION_PERIOD = 3;

    /// <summary>
    /// Количество еды, создаваемой за одну генерацию
    /// </summary>
    private const int EAT_GENERATION_COUNT_PER_PERIOD = 40;

    /// <summary>
    /// Время возрождения игрока (секунды)
    /// </summary>
    private const int PLAYER_REBORN_TIME = 3;

    /// <summary>
    /// Соотношение площади перерытия клетки еды и другой клетки к площади всей клетки еды,
    /// чтобы она считалась съеденной накрывшей клеткой
    /// </summary>
    private const float EAT_OVERLAP_AREA_RATIO = 0.6f;

    /// <summary>
    /// Относительная разница масс, при превышении которой клетка может съесть меньшую клетку другого
    /// игрока при перекрытии больше, чем EAT_OVERLAP_AREA_RATIO
    /// </summary>
    private const float EAT_RELATIVE_MASS_DIFFERENCE = 0.1f;

    /// <summary>
    /// Появление новой еды
    /// </summary>
    public event Action<Cell>? EatCreated;
    /// <summary>
    /// Съедение еды. Параметры Action - еда и игрок
    /// </summary>
    public event Action<Cell, Player>? FoodEaten;
    /// <summary>
    /// Появление нового игрока
    /// </summary>
    public event Action<Player>? PlayerCreated;
    /// <summary>
    /// Возрождение игрока
    /// </summary>
    public event Action<Player>? PlayerReborn;
    /// <summary>
    /// Смерть игрока
    /// </summary>
    public event Action<Player>? PlayerDead;

    /// <summary>
    /// Расстановщик клеток
    /// </summary>
    private readonly CellPlaceholder _cellPlaceholder;

    /// <summary>
    /// Генератор случайных чисел
    /// </summary>
    private readonly Random _random = new();

    /// <summary>
    /// Время с момента прошлой генерации еды
    /// </summary>
    private float _lastEatGenerationElapsedTime = 0;

    /// <summary>
    /// Умершие игроки и время, которое осталось до их возрождения
    /// </summary>
    private readonly Dictionary<Player, float> _deadPlayers = new();

    /// <summary>
    /// Ширина
    /// </summary>
    public float Width { get; set; }
    /// <summary>
    /// Высота
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    /// Игроки
    /// </summary>
    public List<Player> Players { get; private set; } = new();

    /// <summary>
    /// Еда
    /// </summary>
    public List<Cell> Food { get; private set; } = new();

    /// <summary>
    /// Инициализация игрового поля
    /// </summary>
    public GameField()
    {
      _cellPlaceholder = new(this);
    }

    /// <summary>
    /// Инициализация и размещение игрока на поле
    /// </summary>
    /// <param name="parPlayer"></param>
    private void InitializePlayer(Player parPlayer)
    {
      parPlayer.Score = START_SCORE;
      parPlayer.Cells.Clear();
      parPlayer.Cells.Add(new() { Weight = START_SCORE });
      parPlayer.IsAlive = true;
      _cellPlaceholder.SetRandomPosition(parPlayer.Cells[0]);
    }

    /// <summary>
    /// Добавление игрока в случайное место на поле
    /// </summary>
    /// <param name="parPlayer">Добавляемый игрок</param>
    public void AddPlayerOnRandomPosition(Player parPlayer)
    {
      InitializePlayer(parPlayer);

      Players.Add(parPlayer);
      PlayerCreated?.Invoke(parPlayer);
    }

    /// <summary>
    /// Обновление положения игрока
    /// </summary>
    /// <param name="parPlayer">Обновляемый игрок</param>
    /// <param name="parDeltaTime">Временной интервал, на который надо сделать интерполяцию</param>
    private static void UpdatePlayerPosition(Player parPlayer, float parDeltaTime)
    {
      // TODO по желанию добавить клеткам инерционности
      foreach (MovingCell elCell in parPlayer.Cells)
      {
        Vector2 accelerationPerFrame = elCell.Acceleration * parDeltaTime;

        /* для правильного суммирования без накопления положительного или отрицательного остатка
         * (позволяет не зависеть от частоты кадров) */
        Vector2 halfSpeedDeltaPerFrame = elCell.Speed * parDeltaTime + accelerationPerFrame / 2;

        elCell.Position += halfSpeedDeltaPerFrame;
        elCell.Speed += accelerationPerFrame;
      }
    }

    /// <summary>
    /// Генерация новой еды через некоторый интервал времени
    /// </summary>
    /// <param name="parDeltaTime">Временной интервал, прошедший с момента последнего обновления</param>
    private void UpdateEat(float parDeltaTime)
    {
      _lastEatGenerationElapsedTime += parDeltaTime;
      if (_lastEatGenerationElapsedTime > EAT_GENERATION_PERIOD)
      {
        _lastEatGenerationElapsedTime = 0;
        CreateEat(EAT_GENERATION_COUNT_PER_PERIOD);
      }
    }

    /// <summary>
    /// Возрождение умершего игрока
    /// </summary>
    /// <param name="parPlayer"></param>
    private void RebornDeadPlayer(Player parPlayer)
    {
      InitializePlayer(parPlayer);
      _deadPlayers.Remove(parPlayer);
      PlayerReborn?.Invoke(parPlayer);
    }

    /// <summary>
    /// Возрождение умерших игроков через некоторый интервал времени
    /// </summary>
    /// <param name="parDeltaTime">Временной интервал, прошедший с момента последнего обновления</param>
    private void RebornDeadPlayers(float parDeltaTime)
    {
      Dictionary<Player, float>.KeyCollection deadPlayers = _deadPlayers.Keys;
      foreach (Player elPlayer in deadPlayers)
      {
        float newDeadTime = _deadPlayers[elPlayer] + parDeltaTime;
        if (newDeadTime >= PLAYER_REBORN_TIME)
          RebornDeadPlayer(elPlayer);
        else
          _deadPlayers[elPlayer] = newDeadTime;
      }
    }

    /// <summary>
    /// Обновление состояния игры
    /// </summary>
    /// <param name="parDeltaTime">Изменение внутреннего игрового времени в секундах</param>
    public void Update(float parDeltaTime)
    {
      // TODO
      foreach (Player elPlayer in Players)
      {
        UpdatePlayerPosition(elPlayer, parDeltaTime);
      }

      UpdateEat(parDeltaTime);
      ProcessEating();
      RebornDeadPlayers(parDeltaTime);
    }

    /// <summary>
    /// Перемещает все клетки (еду и игроков) на новое случайное место на поле.
    /// Вызов функции стоит делать только после того, как будут удалены лишние клетки игроков
    /// </summary>
    private void ShuffleSingleCells()
    {
      // TODO
    }

    /// <summary>
    /// Оставляет игроку одну клетку с начальной массой
    /// </summary>
    /// <param name="parPlayer">Игрок</param>
    private static void ResetPlayerCells(Player parPlayer)
    {
      int cellsCount = parPlayer.Cells.Count;
      if (cellsCount > 1)
        parPlayer.Cells.RemoveRange(1, cellsCount - 1);
      else if (cellsCount == 0)
        parPlayer.Cells.Add(new());

      parPlayer.Cells[0].Weight = START_SCORE;
      parPlayer.Score = START_SCORE;
    }

    /// <summary>
    /// Сброс состояния игрового поля
    /// </summary>
    public void Reset()
    {
      // TODO
      // TODO удаление старых объектов еды

      foreach (Player elPlayer in Players)
        ResetPlayerCells(elPlayer);
      ShuffleSingleCells();
    }

    /// <summary>
    /// Расчёт реальной устанавливаемой скорости клетки по вектору желаемой 
    /// скорости <paramref name="parDesiredSpeedVector"/>
    /// </summary>
    /// <param name="parDesiredSpeedVector"></param>
    /// <returns></returns>
    public static Vector2 CalculateRealSpeedVector(Vector2 parDesiredSpeedVector)
    {
      // TODO
      const float MULTIPLIER = 0.8f;
      const float MAX_SPEED_VALUE = 13;
      Vector2 result = parDesiredSpeedVector * MULTIPLIER;
      float speedVectorLength = result.Length();

      // ограничение максимальной скорости
      if (speedVectorLength > MAX_SPEED_VALUE)
        result = result / speedVectorLength * MAX_SPEED_VALUE;
      return result;
    }

    /// <summary>
    /// Установка скорости для игрока
    /// </summary>
    /// <param name="parPlayer">Игрок</param>
    /// <param name="speedVector">Вектор желаемой скорости. При превышении максимального ограничения будет 
    /// смасштабирован до соответствия ему</param>
    public void SetSpeedToPlayer(Player parPlayer, Vector2 speedVector)
    {
      // TODO добавить инерционности по желанию
      // TODO добавить разную скорость разным ячейкам по желанию
      foreach (MovingCell elCell in parPlayer.Cells)
      {
        elCell.Speed = CalculateRealSpeedVector(speedVector);
      }
    }

    /// <summary>
    /// Создаёт <paramref name="parCount"/> клеток еды в случайных местах поля
    /// </summary>
    /// <param name="parCount">Количество еды</param>
    public void CreateEat(int parCount)
    {
      const int EAT_WEIGHT_MIN = 1;
      const int EAT_WEIGHT_MAX = 6;

      for (int i = 0; i < parCount; i++)
      {
        int weight = _random.Next(EAT_WEIGHT_MIN, EAT_WEIGHT_MAX + 1);
        Cell eat = new() { Weight = weight };
        _cellPlaceholder.SetRandomPosition(eat);

        Food.Add(eat);
        EatCreated?.Invoke(eat);
      }
    }

    /// <summary>
    /// Обработка съедания клеткой <paramref name="parCell"/> игрока <paramref name="parPlayer"/>
    /// еды <paramref name="parEat"/>
    /// </summary>
    /// <param name="parPlayer"></param>
    /// <param name="parCell"></param>
    /// <param name="parEat"></param>
    private void PlayerEatsFood(Player parPlayer, MovingCell parCell, Cell parEat)
    {
      parCell.Weight += parEat.Weight;
      parPlayer.Score += parEat.Weight;
      Food.Remove(parEat);
      FoodEaten?.Invoke(parEat, parPlayer);
    }

    /// <summary>
    /// Обработка съедания клеткой <paramref name="parPlayerCell"/> игрока <paramref name="parPlayer"/>
    /// клетки <paramref name="parEatedPlayerCell"/> другого игрока <paramref name="parOtherPlayer"/>
    /// </summary>
    /// <param name="parPlayer"></param>
    /// <param name="parPlayerCell"></param>
    /// <param name="parOtherPlayer"></param>
    /// <param name="parEatedPlayerCell"></param>
    private void PlayerEatsOtherCell(Player parPlayer, MovingCell parPlayerCell, Player parOtherPlayer, MovingCell parEatedPlayerCell)
    {
      parPlayerCell.Weight += parEatedPlayerCell.Weight;
      parPlayer.Score += parEatedPlayerCell.Weight;
      parOtherPlayer.Score -= parEatedPlayerCell.Weight;

      parOtherPlayer.Cells.Remove(parEatedPlayerCell);
      if (parOtherPlayer.Cells.Count == 0)
      {
        parOtherPlayer.IsAlive = false;
        _deadPlayers.Add(parOtherPlayer, 0);
        PlayerDead?.Invoke(parOtherPlayer);
      }
    }

    /// <summary>
    /// Обработка съедания игроком клеток других игроков
    /// </summary>
    /// <param name="parPlayer"></param>
    private void ProcessEatingOtherCellsForPlayer(Player parPlayer)
    {
      MathFunctions.Rectangle playerRectangle = parPlayer.GetBoundingRect();
      foreach (Player elPlayer in Players)
      {
        if (elPlayer.IsAlive && playerRectangle.IsIntersect(elPlayer.GetBoundingRect()))
        {
          foreach (MovingCell elCell in parPlayer.Cells)
          {
            float elCellArea = elCell.Area;
            for (int i = elPlayer.Cells.Count - 1; i >= 0; i--)
            {
              MovingCell otherPlayerCell = elPlayer.Cells[i];
              float otherCellArea = otherPlayerCell.Area;
              if (elCellArea > otherCellArea && ((elCellArea - otherCellArea) / elCellArea > EAT_RELATIVE_MASS_DIFFERENCE))
              {
                if (MathFunctions.GetIntersectionArea(elCell, otherPlayerCell) / otherCellArea > EAT_OVERLAP_AREA_RATIO)
                  PlayerEatsOtherCell(parPlayer, elCell, elPlayer, otherPlayerCell);
              }
            }

            if (elPlayer.Cells.Count == 0)
              break;
          }
        }
      }
    }

    /// <summary>
    /// Обработка процесса съедания игроком еды
    /// </summary>
    /// <param name="parPlayer"></param>
    private void ProcessEatingFoodForPlayer(Player parPlayer)
    {
      MathFunctions.Rectangle playerRectangle = parPlayer.GetBoundingRect();
      for (int i = Food.Count - 1; i >= 0; i--)
      {
        Cell eat = Food[i];
        if (playerRectangle.IsIntersect(eat))
        {
          float eatArea = eat.Area;
          foreach (MovingCell elPlayerCell in parPlayer.Cells)
          {
            float intersectionArea = MathFunctions.GetIntersectionArea(eat, elPlayerCell);
            if (intersectionArea / eatArea >= EAT_OVERLAP_AREA_RATIO)
            {
              PlayerEatsFood(parPlayer, elPlayerCell, eat);
              break;
            }
          }
        }
      }
    }

    /// <summary>
    /// Обработка процесса съедания еды или клеток игроков
    /// </summary>
    private void ProcessEating()
    {
      foreach (Player elPlayer in Players)
      {
        if (!elPlayer.IsAlive)
          continue;
        ProcessEatingOtherCellsForPlayer(elPlayer);
        ProcessEatingFoodForPlayer(elPlayer);
      }
    }
  }
}