using Controllers.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewsWPF.Menu;

namespace ControllersWPF
{
  /// <summary>
  /// Контроллер пункта настройки имени игрока в WPF
  /// </summary>
  internal class PlayerNameControllerWPF : MenuPlayerNameController
  {
    /// <summary>
    /// Событие смены имени игрока
    /// </summary>
    public event Action<string>? PlayerNameChanged = null;

    /// <summary>
    /// Представление пункта меню
    /// </summary>
    private readonly PlayerNameViewWPF _playerNameView;

    /// <summary>
    /// Инициализация
    /// </summary>
    /// <param name="parWindow">Окно меню</param>
    public PlayerNameControllerWPF(Window parWindow)
    {
      _playerNameView = new(parWindow);
      _playerNameView.NameEditConfirmed += OnNameEditConfirmed;
      _playerNameView.GoBackSelected += GoBackCall;
    }

    /// <summary>
    /// Обработчик обновления имени игрока
    /// </summary>
    /// <param name="parNewName">Новое имя</param>
    private void OnNameEditConfirmed(string parNewName)
    {
      PlayerName = parNewName;
      ChangePlayerName();
      PlayerNameChanged?.Invoke(parNewName);
      GoBackCall();
    }

    /// <summary>
    /// Включение редактирования имени
    /// </summary>
    public override void Start()
    {
      _playerNameView.Draw();
    }
  }
}
