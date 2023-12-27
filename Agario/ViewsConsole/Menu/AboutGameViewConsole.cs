using AgarioModels.Menu.AboutGame;
using AgarioModels.Menu.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Views.Menu;

namespace ViewsConsole.Menu
{
  /// <summary>
  /// Представление пункта меню "Об игре" в консоли
  /// </summary>
  public class AboutGameViewConsole : MenuAboutGameView
  {
    /// <summary>
    /// Смещение от верхнего края
    /// </summary>
    private const int TOP_OFFSET = 2;
    /// <summary>
    /// Смещение от нижнего края
    /// </summary>
    private const int BOTTOM_OFFSET = 3;

    /// <summary>
    /// Текст с информацией об игре
    /// </summary>
    private readonly string _aboutGameText = DataReader.GetInformationAboutGame();
    /// <summary>
    /// Разбитый на страницы текст с информацией об игре
    /// </summary>
    private readonly string[] _pageSeparatedText;
    /// <summary>
    /// Текущий номер страницы
    /// </summary>
    private int _pageNumber = 0;
    /// <summary>
    /// Количество страниц
    /// </summary>
    private readonly int _pagesCount = 0;

    /// <summary>
    /// Инициализация представления экрана
    /// </summary>
    public AboutGameViewConsole()
    {
      int symbolsOnPage = (Console.BufferHeight - TOP_OFFSET - BOTTOM_OFFSET) * Console.BufferWidth;
      _pagesCount = (int)Math.Ceiling((float)_aboutGameText.Length / symbolsOnPage);
      _pageSeparatedText = new string[_pagesCount];
      int charCounter = 0;
      string formattedInfo = _aboutGameText.Replace('\n', ' ').Replace('\r', ' ');
      for (int i = 0; i < _pageSeparatedText.Length; i++, charCounter += symbolsOnPage)
      {
        _pageSeparatedText[i] = formattedInfo.Substring(charCounter, Math.Min(symbolsOnPage, formattedInfo.Length - charCounter));
      }
    }

    /// <summary>
    /// Отрисовка экрана "Об игре"
    /// </summary>
    public override void Draw()
    {
      const string ABOUT_GAME_CAPTION = "Об игре";
      Console.Clear();
      Console.ForegroundColor = ViewsProperties.TEXT_COLOR;

      Console.SetCursorPosition(0, 0);
      Console.Write(ABOUT_GAME_CAPTION);

      Console.SetCursorPosition(0, TOP_OFFSET);
      Console.Write(_pageSeparatedText[_pageNumber]);

      Console.SetCursorPosition(0, Console.WindowHeight - 1);
      Console.ForegroundColor = ViewsProperties.BACK_BUTTON_COLOR;
      Console.Write("Назад");

      string numberHint = $"<{_pageNumber + 1}/{_pagesCount}>";
      Console.SetCursorPosition(Console.WindowWidth - numberHint.Length, Console.WindowHeight - 1);
      Console.ForegroundColor = ViewsProperties.MENU_ITEM_FOCUS_COLOR;
      Console.Write(numberHint);
      Console.ForegroundColor = ViewsProperties.TEXT_COLOR;
    }

    /// <summary>
    /// Показать следующую страницу
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
    /// Показать предыдущую страницу
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
