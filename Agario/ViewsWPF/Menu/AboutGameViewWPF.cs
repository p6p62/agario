using AgarioModels.Menu.AboutGame;
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
  /// Представление пункта "Об игре" в WPF
  /// </summary>
  public class AboutGameViewWPF : MenuAboutGameView
  {
    /// <summary>
    /// Количество слов на странице
    /// </summary>
    private const int WORDS_ON_PAGE = 40;

    /// <summary>
    /// Возврат назад
    /// </summary>
    public event Action? GoBackSelected = null;
    /// <summary>
    /// Выбор следующей страницы
    /// </summary>
    public event Action? NextPageSelected = null;
    /// <summary>
    /// Выбор предыдущей страницы
    /// </summary>
    public event Action? PrevPageSelected = null;

    /// <summary>
    /// Текст с информацией об игре
    /// </summary>
    private readonly string _aboutGameText = DataReader.GetInformationAboutGame();
    /// <summary>
    /// Разбитый на страницы текст с информацией
    /// </summary>
    private readonly string[] _pageSeparatedText;
    /// <summary>
    /// Текущий номер страницы
    /// </summary>
    private int _pageNumber = 0;
    /// <summary>
    /// Общее количество страниц
    /// </summary>
    private readonly int _pagesCount = 0;

    /// <summary>
    /// Блок вывода текста с информацией
    /// </summary>
    private TextBlock _aboutGameTextBlock = null!;
    /// <summary>
    /// Блок вывода номера страницы
    /// </summary>
    private TextBlock _pageNumberTextBlock = null!;
    /// <summary>
    /// Экран этого представления
    /// </summary>
    private readonly Grid _screen;
    /// <summary>
    /// Окно меню
    /// </summary>
    private readonly Window _window;

    /// <summary>
    /// Инциаиализация представления
    /// </summary>
    /// <param name="parWindow">Окно меню</param>
    public AboutGameViewWPF(Window parWindow)
    {
      string[] wordSeparatedText = _aboutGameText.Split(' ');
      _pagesCount = (int)Math.Ceiling((double)wordSeparatedText.Length / WORDS_ON_PAGE);

      _pageSeparatedText = new string[_pagesCount];
      for (int i = 0; i < _pagesCount; i++)
      {
        _pageSeparatedText[i] = string.Join(' ', wordSeparatedText[(i * WORDS_ON_PAGE)..Math.Min((i + 1) * WORDS_ON_PAGE, wordSeparatedText.Length)]);
      }

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
      const int BORDER_THICKNESS = 3;
      const int ARROWS_MARGIN_LEFT = 30;
      const int GRID_MARGIN = 7;

      Grid grid = new() { Background = new SolidColorBrush(ViewProperties.BROWN_COLOR) };
      grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
      grid.RowDefinitions.Add(new RowDefinition());
      grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

      SolidColorBrush textBrush = new(ViewProperties.MENU_SCREENS_TEXT_COLOR);
      TextBlock captionTextBlock = new() { Text = "Об игре", FontSize = ViewProperties.MENU_CAPTION_SIZE, Foreground = textBrush, Margin = new(GRID_MARGIN) };
      _aboutGameTextBlock = new() { Text = _pageSeparatedText[0], TextWrapping = TextWrapping.Wrap, Foreground = textBrush, FontSize = ViewProperties.MENU_TEXT_SIZE, Margin = new(GRID_MARGIN) };
      _pageNumberTextBlock = new() { FontSize = ViewProperties.MENU_TEXT_SIZE, Foreground = textBrush };
      Border backButtonBorder = new() { BorderThickness = new(BORDER_THICKNESS), BorderBrush = System.Windows.Media.Brushes.White };
      backButtonBorder.Child = new TextBlock() { Text = "Назад", FontSize = ViewProperties.MENU_CAPTION_SIZE, Foreground = textBrush };
      StackPanel bottomPanel = new() { Orientation = Orientation.Horizontal, Margin = new(GRID_MARGIN) };
      bottomPanel.Children.Add(backButtonBorder);
      bottomPanel.Children.Add(new TextBlock() { Text = "⇦ ⇨", FontSize = ViewProperties.MENU_CAPTION_SIZE, Foreground = textBrush, Margin = new(ARROWS_MARGIN_LEFT, 0, 0, 0) });
      bottomPanel.Children.Add(_pageNumberTextBlock);

      Grid.SetRow(captionTextBlock, 0);
      Grid.SetRow(_aboutGameTextBlock, 1);
      Grid.SetRow(bottomPanel, 2);
      grid.Children.Add(captionTextBlock);
      grid.Children.Add(_aboutGameTextBlock);
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
        case Key.Escape:
        case Key.Space:
          GoBackSelected?.Invoke();
          break;
        case Key.Right:
          NextPageSelected?.Invoke();
          break;
        case Key.Left:
          PrevPageSelected?.Invoke();
          break;
      }
    }

    /// <summary>
    /// Рисование пункта меню
    /// </summary>
    public override void Draw()
    {
      _screen.Focus();
      _window.Content = _screen;
      _aboutGameTextBlock.Text = _pageSeparatedText[_pageNumber];
      _pageNumberTextBlock.Text = $"{_pageNumber + 1}/{_pagesCount}";
    }

    /// <summary>
    /// Выбор следующей страницы
    /// </summary>
    public override void ShowNextPage()
    {
      if (_pageNumber < _pagesCount - 1)
      {
        _pageNumber++;
        Draw();
      }
    }

    /// <summary>
    /// Выбор предыдущей страницы
    /// </summary>
    public override void ShowPrevPage()
    {
      if (_pageNumber > 0)
      {
        _pageNumber--;
        Draw();
      }
    }
  }
}
