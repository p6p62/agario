using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgarioModels.Menu
{
  /// <summary>
  /// Элемент меню
  /// </summary>
  public class MenuItem
  {
    /// <summary>
    /// Состояния пункта меню
    /// </summary>
    public enum MenuItemState
    {
      /// <summary>
      /// Стандартное
      /// </summary>
      Normal,
      /// <summary>
      /// В фокусе
      /// </summary>
      Focused,
      /// <summary>
      /// Выбран
      /// </summary>
      Selected
    }

    /// <summary>
    /// Событие, возникающее при выборе пункта меню
    /// </summary>
    public event Action? Selected = null;

    /// <summary>
    /// Состояние пункта меню
    /// </summary>
    private MenuItemState _state = MenuItemState.Normal;

    /// <summary>
    /// ID пункта меню
    /// </summary>
    public int ID { get; private set; }
    /// <summary>
    /// Имя пункта меню
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// Состояние пункта меню
    /// </summary>
    public MenuItemState State
    {
      get => _state;
      set
      {
        _state = value;
        if (_state == MenuItemState.Selected)
        {
          Selected?.Invoke();
          _state = MenuItemState.Focused;
        }
      }
    }
    /// <summary>
    /// Создаёт элемент меню с заданным именем и ID
    /// </summary>
    /// <param name="parID">ID элемента меню</param>
    /// <param name="parName">Имя элемента меню</param>
    public MenuItem(int parID, string parName)
    {
      ID = parID;
      Name = parName;
    }
  }
}
