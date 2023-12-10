using AgarioModels.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Views.Menu;

namespace ViewsWPF.Menu
{
  /// <summary>
  /// Представление экрана меню настройки имени игрока
  /// </summary>
  public class PlayerNameViewWPF : MenuPlayerNameView
  {
    /// <summary>
    /// Возврат назад
    /// </summary>
    public event Action? GoBackSelected = null;

    /// <summary>
    /// Выбрано сохранение нового имени
    /// </summary>
    public event Action<string>? NameEditConfirmed = null;

    /// <summary>
    /// Окно приложения
    /// </summary>
    private readonly Window _window;

    /// <summary>
    /// Фигура с именем игрока
    /// </summary>
    private TextBox _playerNameTextBox = null!;

    /// <summary>
    /// Экран этого представления
    /// </summary>
    private readonly Grid _screen;

    /// <summary>
    /// Инициализация представления
    /// </summary>
    /// <param name="parWindow"></param>
    public PlayerNameViewWPF(Window parWindow)
    {
      _window = parWindow;
      _screen = GetScreenLayout();
      _screen.Focusable = true;
      _screen.KeyDown += KeyHandler;
    }

    /// <summary>
    /// Получение экрана представления
    /// </summary>
    /// <returns>Настроенный экран представления</returns>
    private Grid GetScreenLayout()
    {
      const int GRID_MARGIN = 7;

      Grid grid = new() { Background = new SolidColorBrush(ViewProperties.MENU_ITEMS_BACKGROUND_COLOR) };
      grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
      grid.RowDefinitions.Add(new RowDefinition());
      grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

      SolidColorBrush textBrush = new(ViewProperties.MENU_SCREENS_TEXT_COLOR);
      TextBlock captionTextBlock = new() { Text = "Имя игрока (редактирование):", FontSize = ViewProperties.MENU_CAPTION_SIZE, Foreground = textBrush, Margin = new(GRID_MARGIN) };
      _playerNameTextBox = new() { Text = AgarioGame.TEST_PLAYER_NAME, TextWrapping = TextWrapping.Wrap, Foreground = textBrush, FontSize = ViewProperties.MENU_TEXT_SIZE, Margin = new(GRID_MARGIN) };

      StackPanel bottomPanel = new() { Orientation = Orientation.Vertical, Margin = new(GRID_MARGIN) };
      bottomPanel.Children.Add(new TextBlock() { Text = "Escape - выйти", FontSize = ViewProperties.MENU_TEXT_SIZE, Foreground = new SolidColorBrush(ViewProperties.BACK_BUTTON_COLOR) });
      bottomPanel.Children.Add(new TextBlock() { Text = "Enter - сохранить и выйти", FontSize = ViewProperties.MENU_TEXT_SIZE, Foreground = new SolidColorBrush(ViewProperties.BACK_BUTTON_COLOR) });

      Grid.SetRow(captionTextBlock, 0);
      Grid.SetRow(_playerNameTextBox, 1);
      Grid.SetRow(bottomPanel, 2);
      grid.Children.Add(captionTextBlock);
      grid.Children.Add(_playerNameTextBox);
      grid.Children.Add(bottomPanel);
      return grid;
    }

    /// <summary>
    /// Обработчик событий клавиатуры
    /// </summary>
    /// <param name="parSender">Отправитель</param>
    /// <param name="parArgs">Аргументы события</param>
    private void KeyHandler(object parSender, KeyEventArgs parArgs)
    {
      switch (parArgs.Key)
      {
        case Key.Enter:
          NameEditConfirmed?.Invoke(_playerNameTextBox.Text);
          break;
        case Key.Escape:
          GoBackSelected?.Invoke();
          break;
        case Key.Back:
          string currentInput = _playerNameTextBox.Text;
          if (currentInput.Length > 0)
            _playerNameTextBox.Text = currentInput[0..^1];
          break;
      }
    }

    /// <summary>
    /// Рисование
    /// </summary>
    public override void Draw()
    {
      _playerNameTextBox.Text = AgarioGame.GetGameInstance().PlayerName;
      _screen.Focus();
      _window.Content = _screen;
    }
  }
}
