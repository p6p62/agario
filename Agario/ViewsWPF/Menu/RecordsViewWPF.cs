using AgarioModels.Menu.Records;
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
  /// Представления пункта меню "Рекорды" в WPF
  /// </summary>
  public class RecordsViewWPF : MenuRecordsView
  {
    /// <summary>
    /// Возврат назад
    /// </summary>
    public event Action? GoBackSelected = null;

    /// <summary>
    /// Таблица с рекордами
    /// </summary>
    private Grid _recordsTable = null!;
    /// <summary>
    /// Экран представления пункта меню
    /// </summary>
    private readonly Grid _menuScreen;
    /// <summary>
    /// Окно меню
    /// </summary>
    private readonly Window _window;

    /// <summary>
    /// Инициализация представления пункта меню
    /// </summary>
    /// <param name="parWindow">Окно игры</param>
    public RecordsViewWPF(Window parWindow)
    {
      _window = parWindow;
      _menuScreen = GetScreenLayout();
      _menuScreen.Focusable = true;
      _menuScreen.KeyDown += KeyHandler;
    }

    /// <summary>
    /// Получение экрана представления
    /// </summary>
    /// <returns>Настроенный экран представления пункта меню</returns>
    private Grid GetScreenLayout()
    {
      const int GRID_MARGIN = 7;

      Brush backgroundBrush = new SolidColorBrush(ViewProperties.MENU_ITEMS_BACKGROUND_COLOR);
      Brush textBrush = new SolidColorBrush(ViewProperties.MENU_SCREENS_TEXT_COLOR);
      Grid recordsScreenGrid = new() { Background = backgroundBrush };
      recordsScreenGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
      recordsScreenGrid.RowDefinitions.Add(new RowDefinition());
      recordsScreenGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

      TextBlock captionTextBlock = new() { Text = "Рекорды", FontSize = ViewProperties.MENU_CAPTION_SIZE, Foreground = textBrush, Margin = new(GRID_MARGIN) };
      _recordsTable = new() { Margin = new(GRID_MARGIN) };
      StackPanel backButtonPanel = new() { Orientation = Orientation.Horizontal };
      backButtonPanel.Children.Add(new TextBlock() { Text = "Назад", FontSize = ViewProperties.MENU_CAPTION_SIZE, Foreground = new SolidColorBrush(ViewProperties.BACK_BUTTON_COLOR) });

      Grid.SetRow(captionTextBlock, 0);
      Grid.SetRow(_recordsTable, 1);
      Grid.SetRow(backButtonPanel, 2);
      recordsScreenGrid.Children.Add(captionTextBlock);
      recordsScreenGrid.Children.Add(_recordsTable);
      recordsScreenGrid.Children.Add(backButtonPanel);

      return recordsScreenGrid;
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
        case Key.Escape:
        case Key.Space:
          GoBackSelected?.Invoke();
          break;
      }
    }

    /// <summary>
    /// Отображение пункта меню
    /// </summary>
    public override void Draw()
    {
      _menuScreen.Focus();
      _window.Content = _menuScreen;
      FillRecords();
    }

    /// <summary>
    /// Обновление рекордов на экране
    /// </summary>
    private void FillRecords()
    {
      _recordsTable.Children.Clear();
      _recordsTable.RowDefinitions.Clear();

      const int RECORD_TEXT_SIZE = 32;

      Brush subcaptionBrush = new SolidColorBrush(ViewProperties.MENU_SUBCAPTION_COLOR);
      List<Record> records = GameRecordsHandlerWPF.GetRecords();
      DockPanel captionRow = new();
      captionRow.Children.Add(new TextBlock() { Text = "Имя", Foreground = subcaptionBrush, FontSize = RECORD_TEXT_SIZE, TextAlignment = TextAlignment.Left });
      captionRow.Children.Add(new TextBlock() { Text = "Рейтинг", Foreground = subcaptionBrush, FontSize = RECORD_TEXT_SIZE, TextAlignment = TextAlignment.Right });
      Grid.SetColumn(captionRow, 0);
      _recordsTable.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
      _recordsTable.Children.Add(captionRow);

      Brush textBrush = new SolidColorBrush(ViewProperties.MENU_SCREENS_TEXT_COLOR);
      int counter = 1;
      foreach (Record elRecord in records)
      {
        _recordsTable.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
        DockPanel recordRow = new();
        recordRow.Children.Add(new TextBlock() { Text = elRecord.Name, Foreground = textBrush, FontSize = RECORD_TEXT_SIZE, TextAlignment = TextAlignment.Left });
        recordRow.Children.Add(new TextBlock() { Text = elRecord.Value.ToString(), Foreground = textBrush, FontSize = RECORD_TEXT_SIZE, TextAlignment = TextAlignment.Right });

        Grid.SetRow(recordRow, counter++);
        _recordsTable.Children.Add(recordRow);
      }
    }
  }
}
