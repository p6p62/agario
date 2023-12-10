using System;
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
    /// Максимальное количество клеток, которое может быть у игрока
    /// </summary>
    private const int MAX_CELLS_FOR_PLAYER = 16;

    /// <summary>
    /// Минимальная масса клетки, при которой ей разрешено разделяться
    /// </summary>
    private const int MIN_WEIGHT_FOR_DIVISION = 14;

    /// <summary>
    /// Минимальный промежуток времени до слияния разделённых клеток в секундах
    /// </summary>
    private const float MIN_DURATION_BEFORE_MERGING = 6;

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
    /// Время сохранения максимального рейтинга (в секундах)
    /// </summary>
    private const int MAX_SCORE_UPDATE_TIME = 1;

    /// <summary>
    /// Соотношение площади перерытия клетки еды и другой клетки к площади всей клетки еды,
    /// чтобы она считалась съеденной накрывшей клеткой
    /// </summary>
    private const float EAT_OVERLAP_AREA_RATIO = 0.6f;

    /// <summary>
    /// Соотношение площади перекрытия клеток к площади всей клетки,
    /// чтобы она могла слиться с другой клеткой
    /// </summary>
    private const float MERGE_OVERLAP_AREA_RATIO = 0.2f;

    /// <summary>
    /// Относительная разница масс, при превышении которой клетка может съесть меньшую клетку другого
    /// игрока при перекрытии больше, чем EAT_OVERLAP_AREA_RATIO
    /// </summary>
    public const float EAT_RELATIVE_MASS_DIFFERENCE = 0.1f;

    /// <summary>
    /// Задержка перед началом движения при старте игры в секундах
    /// </summary>
    private const float START_GAME_DELAY = 1.5f;

    /// <summary>
    /// Появление новой еды
    /// </summary>
    public event Action<Cell>? EatCreated;
    /// <summary>
    /// Съедение еды. Параметры Action - еда и игрок
    /// </summary>
    public event Action<Cell, Player?>? FoodEaten;
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
    /// Оставшееся время до истечения задержки перед началом игры
    /// </summary>
    private float _startGameDelayRemainingTime = START_GAME_DELAY;

    /// <summary>
    /// Время с последнего обновления максимального рейтинга
    /// </summary>
    private float _maxScoreUpdateElapsedTime = 0;

    /// <summary>
    /// Умершие игроки и время, которое осталось до их возрождения
    /// </summary>
    private readonly Dictionary<Player, float> _deadPlayers = new();

    /// <summary>
    /// Объекты, управляющие движением игроков, которые должны управляться компьютером
    /// </summary>
    private readonly List<ComputerMovingStrategy> _playersComputerControls = new();

    /// <summary>
    /// Игроки, которые запросили разделение. Фактически разделение происходит в момент вызова Update
    /// </summary>
    private readonly List<Player> _playersWhoNeedDivide = new();

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
    public void AddPlayerOnRandomPosition(Player parPlayer, ComputerMovingStrategy? parMovingStrategy = null)
    {
      InitializePlayer(parPlayer);

      if (parMovingStrategy != null)
        _playersComputerControls.Add(parMovingStrategy);

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
    /// Обновление состояния игроков, управляемых компьютером
    /// </summary>
    /// <param name="parDeltaTime">Изменение внутреннего игрового времени в секундах</param>
    private void UpdateComputerControlledPlayers(float parDeltaTime)
    {
      foreach (ComputerMovingStrategy elPlayerControlStrategy in _playersComputerControls)
        elPlayerControlStrategy.Update(parDeltaTime);
    }

    /// <summary>
    /// Обновление максимального счёта
    /// </summary>
    /// <param name="parDeltaTime">Изменение внутреннего игрового времени в секундах</param>
    private void UpdateMaxScore(float parDeltaTime)
    {
      _maxScoreUpdateElapsedTime += parDeltaTime;
      if (_maxScoreUpdateElapsedTime >= MAX_SCORE_UPDATE_TIME)
      {
        _maxScoreUpdateElapsedTime = 0;
        foreach (Player elPlayer in Players)
          if (elPlayer.Score > elPlayer.MaxScore)
            elPlayer.MaxScore = elPlayer.Score;
      }
    }

    /// <summary>
    /// Обновление состояния игры
    /// </summary>
    /// <param name="parDeltaTime">Изменение внутреннего игрового времени в секундах</param>
    public void Update(float parDeltaTime)
    {
      if (_startGameDelayRemainingTime > 0)
      {
        _startGameDelayRemainingTime -= parDeltaTime;
        return;
      }

      foreach (Player elPlayer in _playersWhoNeedDivide)
        DividePlayer(elPlayer);
      _playersWhoNeedDivide.Clear();

      foreach (Player elPlayer in Players)
      {
        foreach (MovingCell cell in elPlayer.Cells)
          cell.TimeFromLastDivision += parDeltaTime;
        UpdatePlayerPosition(elPlayer, parDeltaTime);
      }

      UpdateEat(parDeltaTime);
      UpdateMaxScore(parDeltaTime);
      ProcessEating();
      RebornDeadPlayers(parDeltaTime);
      UpdateComputerControlledPlayers(parDeltaTime);
    }

    /// <summary>
    /// Перемещает все клетки (еду и игроков) на новое случайное место на поле.
    /// Вызов функции стоит делать только после того, как будут удалены лишние клетки игроков
    /// </summary>
    private void ShuffleSingleCells()
    {
      foreach (Cell elEat in Food)
        _cellPlaceholder.SetRandomPosition(elEat);
      foreach (Player elPlayer in Players)
        _cellPlaceholder.SetRandomPosition(elPlayer.Cells[0]);
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
    /// Возвращает поле к состоянию с начальным количеством еды
    /// </summary>
    private void ResetEat()
    {
      int delta = Food.Count - AgarioGame.START_EAT_COUNT;
      if (delta > 0)
      {
        while (--delta >= 0)
        {
          int lastIndex = Food.Count - 1;
          FoodEaten?.Invoke(Food[lastIndex], null);
          Food.RemoveAt(lastIndex);
        }
      }
      else if (delta < 0)
        CreateEat(-delta);
    }

    /// <summary>
    /// Сброс состояния игрового поля
    /// </summary>
    public void Reset()
    {
      _startGameDelayRemainingTime = START_GAME_DELAY;
      ResetEat();
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
    /// Подстройка скорости в зависимости от массы клетки
    /// </summary>
    /// <param name="parSpeedVector">Вектор скорости</param>
    /// <param name="parWeight">Масса клетки</param>
    /// <returns></returns>
    private static Vector2 FitSpeedByCellWeight(Vector2 parSpeedVector, float parWeight)
    {
      return parSpeedVector * 4 * MathF.Pow(parWeight, -0.42f);
    }

    /// <summary>
    /// Взаимное притяжение разделённых клеток игрока. Не даёт клеткам разбегаться, мягко подталкивая
    /// друг к другу через изменение направления вектора скорости
    /// </summary>
    /// <param name="parPlayer">Игрок</param>
    /// <param name="speedVector">Вектор желаемой скорости</param>
    private static void SetSpeedVectorForCellsMutualAttraction(Player parPlayer, Vector2 speedVector)
    {
      List<MovingCell> cells = parPlayer.Cells;

      // если не разделено
      if (parPlayer.Cells.Count == 1)
        cells[0].Speed = speedVector;
      else
      {
        // стремление к общему центру массы
        Vector2 massCenter = MathFunctions.CalculateMassCenter(cells);
        for (int i = 0; i < cells.Count; i++)
          cells[i].Speed = speedVector + (massCenter - cells[i].Position);

        // с отталкиванием накладывающихся клеток
        for (int i = 0; i < cells.Count; i++)
          for (int j = 0; j < cells.Count; j++)
          {
            if (i == j)
              continue;

            // отталкиваются только те клетки, которым пока не разрешено сливаться
            if (cells[i].TimeFromLastDivision < MIN_DURATION_BEFORE_MERGING
              && cells[j].TimeFromLastDivision < MIN_DURATION_BEFORE_MERGING
              && cells[i].IsIntersect(cells[j]))
            {
              Vector2 centerConnectionVector = cells[i].Position - cells[j].Position;
              cells[i].Speed += centerConnectionVector / centerConnectionVector.LengthSquared();
            }
          }
      }
    }

    /// <summary>
    /// Скорректированный вектор скорости для учёта остановки движения клетки при столкновении со стенами
    /// </summary>
    /// <param name="parCell">Клетка</param>
    /// <param name="parSpeed">Скорость до преобразования</param>
    /// <returns></returns>
    private Vector2 CorrectSpeedByWallCollisions(MovingCell parCell, Vector2 parSpeed)
    {
      Vector2 speed = parSpeed;
      if ((speed.X > 0 && parCell.Position.X + parCell.Radius >= Width)
        || (speed.X < 0 && parCell.Position.X - parCell.Radius <= 0))
        speed.X = 0;
      if ((speed.Y > 0 && parCell.Position.Y + parCell.Radius >= Height)
        || (speed.Y < 0 && parCell.Position.Y - parCell.Radius <= 0))
        speed.Y = 0;
      return speed;
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
      SetSpeedVectorForCellsMutualAttraction(parPlayer, speedVector);

      foreach (MovingCell elCell in parPlayer.Cells)
      {
        Vector2 weightCorrectedSpeed = FitSpeedByCellWeight(CalculateRealSpeedVector(elCell.Speed), elCell.Weight);
        elCell.Speed = CorrectSpeedByWallCollisions(elCell, weightCorrectedSpeed);
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
      int otherPlayerScore = parOtherPlayer.Score;
      parPlayerCell.Weight += parEatedPlayerCell.Weight;
      parPlayer.Score += parEatedPlayerCell.Weight;
      parOtherPlayer.Score -= parEatedPlayerCell.Weight;

      parOtherPlayer.Cells.Remove(parEatedPlayerCell);
      if (parOtherPlayer.Cells.Count == 0)
      {
        if (otherPlayerScore > parOtherPlayer.MaxScore)
          parOtherPlayer.MaxScore = otherPlayerScore;
        parOtherPlayer.IsAlive = false;
        _deadPlayers.Add(parOtherPlayer, 0);
        PlayerDead?.Invoke(parOtherPlayer);
      }
    }

    /// <summary>
    /// Обработка съедания игроком клеток другого игрока
    /// </summary>
    /// <param name="parPlayer"></param>
    /// <param name="parOtherEatedPlayer"></param>
    private void ProcessEatingOtherPlayer(Player parPlayer, Player parOtherEatedPlayer)
    {
      for (int j = 0; j < parPlayer.Cells.Count; j++)
      {
        MovingCell elCell = parPlayer.Cells[j];
        float elCellArea = elCell.Area;
        for (int i = parOtherEatedPlayer.Cells.Count - 1; i >= 0; i--)
        {
          MovingCell otherPlayerCell = parOtherEatedPlayer.Cells[i];
          float otherCellArea = otherPlayerCell.Area;
          if (elCellArea > otherCellArea && ((elCellArea - otherCellArea) / elCellArea > EAT_RELATIVE_MASS_DIFFERENCE))
          {
            if (MathFunctions.GetIntersectionArea(elCell, otherPlayerCell) / otherCellArea > EAT_OVERLAP_AREA_RATIO)
              PlayerEatsOtherCell(parPlayer, elCell, parOtherEatedPlayer, otherPlayerCell);
          }
        }

        if (parOtherEatedPlayer.Cells.Count == 0)
          break;
      }
    }

    /// <summary>
    /// Обработка слияния разделённых ячеек одного игрока
    /// </summary>
    /// <param name="parPlayer"></param>
    private void ProcessCellsMerging(Player parPlayer)
    {
      for (int j = 0; j < parPlayer.Cells.Count; j++)
      {
        MovingCell mergingCell = parPlayer.Cells[j];
        float mergingCellArea = mergingCell.Area;
        for (int i = parPlayer.Cells.Count - 1; i >= 0; i--)
        {
          MovingCell otherCell = parPlayer.Cells[i];
          float otherCellArea = otherCell.Area;
          if (
            mergingCellArea > otherCellArea
            && mergingCell.TimeFromLastDivision >= MIN_DURATION_BEFORE_MERGING
            && otherCell.TimeFromLastDivision >= MIN_DURATION_BEFORE_MERGING)
          {
            if (MathFunctions.GetIntersectionArea(mergingCell, otherCell) / otherCellArea > MERGE_OVERLAP_AREA_RATIO)
              PlayerEatsOtherCell(parPlayer, mergingCell, parPlayer, otherCell);
          }
        }
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
        if (elPlayer.IsAlive)
        {
          // другой игрок
          if (elPlayer != parPlayer)
          {
            if (playerRectangle.IsIntersect(elPlayer.GetBoundingRect()))
              ProcessEatingOtherPlayer(parPlayer, elPlayer);
          }
          else if (elPlayer.Cells.Count > 1)
            ProcessCellsMerging(elPlayer);
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

    /// <summary>
    /// Вычисляет положение и скорость ячеек после разделения
    /// </summary>
    /// <param name="parDividedCell">Разделившаяся ячейка</param>
    /// <param name="parNewCell">Новая ячейка</param>
    private void CalculateSpeedAndPositionAfterCellDivide(MovingCell parDividedCell, MovingCell parNewCell)
    {
      parNewCell.Speed = parDividedCell.Speed;

      Vector2 positionOffsetVector;
      float speedVectorLength = parDividedCell.Speed.Length();
      if (speedVectorLength > 0)
        positionOffsetVector = parDividedCell.Speed / speedVectorLength * (parDividedCell.Radius + parNewCell.Radius);
      else
      {
        Vector2 vectorToCenter = new Vector2(Width, Height) / 2 - parDividedCell.Position;
        positionOffsetVector = vectorToCenter / vectorToCenter.Length();
      }
      parNewCell.Position = parDividedCell.Position + positionOffsetVector;
    }

    /// <summary>
    /// Разделяет клетку <paramref name="parDividedCell"/> на 2. У <paramref name="parDividedCell"/> меняется
    /// масса. Возвращает новую появившуюся ячейку
    /// </summary>
    /// <param name="parDividedCell">Разделяемая ячейка</param>
    /// <returns>Новая ячейка, появившаяся из <paramref name="parDividedCell"/></returns>
    private MovingCell DivideCell(MovingCell parDividedCell)
    {
      int halfWeight = parDividedCell.Weight / 2;
      parDividedCell.Weight -= halfWeight;

      MovingCell newCell = new() { Weight = halfWeight };
      CalculateSpeedAndPositionAfterCellDivide(parDividedCell, newCell);
      return newCell;
    }

    /// <summary>
    /// Запрос на разделение игрока <paramref name="parPlayer"/>. Фактическое разделение произойдет при 
    /// обновлении состояния игры вызовом Update()
    /// </summary>
    /// <param name="parPlayer">Игрок, который хочет разделиться</param>
    public void DividePlayerQuery(Player parPlayer)
    {
      _playersWhoNeedDivide.Add(parPlayer);
    }

    /// <summary>
    /// Разделение игрока на части
    /// </summary>
    /// <param name="parPlayer">Игрок</param>
    private void DividePlayer(Player parPlayer)
    {
      if (!parPlayer.IsAlive
        || parPlayer.Cells.Count >= MAX_CELLS_FOR_PLAYER)
        return;

      int newCellsCount = Math.Min(parPlayer.Cells.Count, MAX_CELLS_FOR_PLAYER - parPlayer.Cells.Count);
      List<MovingCell> weightDescendingSortedCells = new(parPlayer.Cells);
      weightDescendingSortedCells.Sort((c1, c2) => c2.Weight - c1.Weight);
      while (--newCellsCount >= 0)
      {
        MovingCell dividedCell = parPlayer.Cells[newCellsCount];
        if (dividedCell.Weight < MIN_WEIGHT_FOR_DIVISION)
          continue;

        MovingCell newCell = DivideCell(dividedCell);

        dividedCell.TimeFromLastDivision = newCell.TimeFromLastDivision = 0;
        parPlayer.Cells.Add(newCell);
      }
    }
  }
}
