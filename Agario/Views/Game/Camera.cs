using AgarioModels.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static AgarioModels.Game.MathFunctions;

namespace Views.Game
{
  /// <summary>
  /// Класс для управления областью игрового поля, которая отображается на экране у игрока.
  /// Ширина и высота видимой области игрового поля, захватываемой камерой, всегда будет иметь
  /// соотношение сторон, задаваемое выражением ScreenWidth / ScreenHeight
  /// </summary>
  public abstract class Camera
  {
    /// <summary>
    /// Ширина отображаемой области игрового поля
    /// </summary>
    private float _cameraWidth;
    /// <summary>
    /// Высота отображаемой области игрового поля
    /// </summary>
    private float _cameraHeight;

    /// <summary>
    /// Ширина экрана
    /// </summary>
    private float _screenWidth;
    /// <summary>
    /// Высота экрана
    /// </summary>
    private float _screenHeight;
    /// <summary>
    /// Соотношение ширины экрана к его высоте
    /// </summary>
    private float _screenWidthToHeightRatio = 1;

    /// <summary>
    /// Смещение левого верхнего угла камеры относительно левого верхнего угла игрового поля по X-координате
    /// </summary>
    public float CameraOffsetX { get; protected set; }
    /// <summary>
    /// Смещение левого верхнего угла камеры относительно левого верхнего угла игрового поля по Y-координате
    /// </summary>
    public float CameraOffsetY { get; protected set; }
    /// <summary>
    /// Ширина отображаемой области игрового поля
    /// </summary>
    public float CameraWidth
    {
      get => _cameraWidth;
      protected set
      {
        _cameraWidth = value;
        _cameraHeight = _cameraWidth / ScreenWidthToHeightRatio;

        CameraToScreenScaleFactor = _cameraWidth / _screenWidth;
      }
    }
    /// <summary>
    /// Высота отображаемой области игрового поля
    /// </summary>
    public float CameraHeight
    {
      get => _cameraHeight;
      protected set
      {
        _cameraHeight = value;
        _cameraWidth = _cameraHeight * ScreenWidthToHeightRatio;

        CameraToScreenScaleFactor = _cameraWidth / _screenWidth;
      }
    }

    /// <summary>
    /// Коэффициент масштаба между одной из сторон камеры и соответствующей стороной экрана
    /// </summary>
    public float CameraToScreenScaleFactor { get; protected set; }

    /// <summary>
    /// Соотношение ширины экрана к его высоте
    /// </summary>
    protected float ScreenWidthToHeightRatio
    {
      get => _screenWidthToHeightRatio;
      set
      {
        float oldValue = _screenWidthToHeightRatio;
        _screenWidthToHeightRatio = value;

        /* автоматическое вычисление новых размеров области просмотра
         * так, чтобы зона видимости не уменьшалась*/
        if (value > oldValue)
          CameraHeight = _cameraHeight;
        else
          CameraWidth = _cameraWidth;
      }
    }
    /// <summary>
    /// Ширина экрана
    /// </summary>
    public float ScreenWidth
    {
      get => _screenWidth;
      set
      {
        _screenWidth = value;
      }
    }
    /// <summary>
    /// Высота экрана
    /// </summary>
    public float ScreenHeight
    {
      get => _screenHeight;
      set
      {
        _screenHeight = value;
      }
    }

    /// <summary>
    /// Экземпляр игры
    /// </summary>
    protected AgarioGame GameInstance { get; private set; }

    /// <summary>
    /// Игровое поле
    /// </summary>
    protected GameField GameField { get; private set; }

    /// <summary>
    /// Игрок, за которым следит камера
    /// </summary>
    protected Player? TrackedPlayer { get; set; }

    /// <summary>
    /// Инициализация камеры
    /// </summary>
    /// <param name="parGame">Экземпляр игры</param>
    public Camera(AgarioGame parGame)
    {
      GameInstance = parGame;
      GameField = parGame.GameField;
    }

    /// <summary>
    /// Обновляет соотношение сторон экрана
    /// </summary>
    public void UpdateScreenWidthToHeightRatio()
    {
      ScreenWidthToHeightRatio = ScreenWidth / ScreenHeight;
    }

    /// <summary>
    /// Центрирует камеру на игроке, за которым она должна следить
    /// </summary>
    protected void CenterOnTrackedPlayer()
    {
      if (TrackedPlayer is null || TrackedPlayer.Cells.Count == 0)
        return;

      // TODO сейчас выравнивается на наибольшей клетке игрока. Переделать на геометрический центр
      Cell maxCell = TrackedPlayer.Cells.MaxBy(c => c.Weight)!;
      CameraOffsetX = maxCell.Position.X - CameraWidth / 2;
      CameraOffsetY = maxCell.Position.Y - CameraHeight / 2;
    }

    /// <summary>
    /// Обновление камеры
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Получение прямоугольника, соответствующего области просмотра камеры
    /// </summary>
    /// <returns></returns>
    public Rectangle GetCameraRectangle()
    {
      return new()
      {
        X1 = CameraOffsetX,
        Y1 = CameraOffsetY,
        X2 = CameraOffsetX + CameraWidth,
        Y2 = CameraOffsetY + CameraHeight
      };
    }

    /// <summary>
    /// Определение того, видны ли на момент вызова стенки игрового поля
    /// на камере (вышла ли область обзора за границы поля)
    /// </summary>
    /// <returns>True, если на камере видны границы игрового поля</returns>
    public bool IsGameFieldBordersInViewPort()
    {
      Rectangle gameFieldRectanle = new()
      {
        X1 = 0,
        Y1 = 0,
        X2 = GameField.Width,
        Y2 = GameField.Height
      };

      // предполагается, что камера не может выйти за пределы игрового поля
      return !GetCameraRectangle().IsNestedIn(gameFieldRectanle);
    }

    /// <summary>
    /// Получение игроков, которые попали в область просмотра камеры
    /// </summary>
    /// <returns>Игроки, попавшие в область просмотра</returns>
    public List<Player> GetPlayersInViewport()
    {
      List<Player> playersInViewport = new();
      Rectangle cameraRectangle = GetCameraRectangle();
      foreach (Player elPlayer in GameField.Players)
        if (elPlayer.IsAlive && cameraRectangle.IsIntersect(elPlayer.GetBoundingRect()))
          playersInViewport.Add(elPlayer);
      return playersInViewport;
    }

    /// <summary>
    /// Получение еды, которая попала в область просмотра камеры
    /// </summary>
    /// <returns>Еда, которая попала в область просмотра камеры</returns>
    public List<Cell> GetEatInViewport()
    {
      List<Cell> eatInViewport = new();
      Rectangle cameraRectangle = GetCameraRectangle();
      foreach (Cell elEat in GameField.Food)
        if (cameraRectangle.IsIntersect(elEat))
          eatInViewport.Add(elEat);
      return eatInViewport;
    }

    /// <summary>
    /// Вычисление длины линии, чтобы отобразить её на экране в соответствии
    /// с текущими настройками камеры и параметрами экрана
    /// </summary>
    /// <param name="parLineLengthReal">Реальная длина линии</param>
    /// <returns>Длина линии в пикселях</returns>
    public float CalculateLineLengthInScreen(float parLineLengthReal)
    {
      // TODO
      return parLineLengthReal / CameraToScreenScaleFactor;
    }

    /// <summary>
    /// Вычисление положения точки на экране в соответствии с текущими настройками камеры
    /// и параметрами экрана
    /// </summary>
    /// <param name="parPoint">Точка в системе координат игрового поля</param>
    /// <returns>Координаты экрана, где должна быть нарисована эта точка</returns>
    public Vector2 CalculatePointPositionInScreen(Vector2 parPoint)
    {
      // TODO проверить при вылете объектов за экран
      Vector2 result = new(parPoint.X - CameraOffsetX, parPoint.Y - CameraOffsetY);
      result.X /= CameraToScreenScaleFactor;
      result.Y /= CameraToScreenScaleFactor;

      return result;
    }
  }
}
