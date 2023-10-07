using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgarioModels.Menu
{
  /// <summary>
  /// Линейное меню без вложенных подпунктов
  /// </summary>
  public class Menu
  {
    /// <summary>
    /// Событие, возникающее при необходимости перерисовки
    /// </summary>
    public event Action? NeedRedraw = null;

    /// <summary>
    /// Элементы меню
    /// </summary>
    private readonly Dictionary<int, MenuItem> _items = new();

    /// <summary>
    /// Индекс элемента меню, находящегося в фокусе
    /// </summary>
    public int FocusedItemIndex { get; protected set; } = -1;

    /// <summary>
    /// Элементы меню
    /// </summary>
    public IList<MenuItem> Items => _items.Values.ToList();
    /// <summary>
    /// Индексатор для доступа к элементам меню
    /// </summary>
    /// <param name="parId">Индекс элемента меню</param>
    /// <returns>Элемент меню по индексу</returns>
    public MenuItem this[int parId]
    {
      get => _items[parId];
    }

    /// <summary>
    /// Добавление элемента меню
    /// </summary>
    /// <param name="parMenuItem">Элемент меню</param>
    /// <returns>Добавленный элемент</returns>
    public MenuItem AddItem(MenuItem parMenuItem)
    {
      _items.Add(parMenuItem.ID, parMenuItem);
      return parMenuItem;
    }

    /// <summary>
    /// Смена фокуса на предыдущий элемент
    /// </summary>
    public void FocusPrevious()
    {
      int oldFocusedItemIndex = FocusedItemIndex;
      FocusedItemIndex = FocusedItemIndex > 0 ? FocusedItemIndex - 1 : _items.Count - 1;

      _items[FocusedItemIndex].State = MenuItem.MenuItemState.Focused;
      _items[oldFocusedItemIndex].State = MenuItem.MenuItemState.Normal;

      NeedRedraw?.Invoke();
    }

    /// <summary>
    /// Смена фокуса на следующий элемент
    /// </summary>
    public void FocusNext()
    {
      int oldFocusedItemIndex = FocusedItemIndex;
      FocusedItemIndex = FocusedItemIndex < _items.Count - 1 ? FocusedItemIndex + 1 : 0;

      _items[FocusedItemIndex].State = MenuItem.MenuItemState.Focused;
      _items[oldFocusedItemIndex].State = MenuItem.MenuItemState.Normal;

      NeedRedraw?.Invoke();
    }

    /// <summary>
    /// Установка элемента в состоянии "В фокусе" 
    /// </summary>
    /// <param name="parID">ID элемента</param>
    public void FocusByID(int parID)
    {
      int savFocusedIndex = FocusedItemIndex;
      MenuItem menuItem = this[parID];
      FocusedItemIndex = Items.IndexOf(menuItem);

      if (savFocusedIndex != -1)
        _items[savFocusedIndex].State = MenuItem.MenuItemState.Normal;
      _items[FocusedItemIndex].State = MenuItem.MenuItemState.Focused;
      NeedRedraw?.Invoke();
    }

    /// <summary>
    /// Активация элемента, на котором стоит фокус
    /// </summary>
    public void SelectFocusedElement()
    {
      _items[FocusedItemIndex].State = MenuItem.MenuItemState.Selected;
    }
  }
}
