using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ViewsConsole
{
  /// <summary>
  /// Вспомогательный класс-прослойка между вызовами WINAPI
  /// </summary>
  internal static class ConsoleHelperUtilite
  {
    /// <summary>
    /// Флаг "Не изменять размеры окна", сохраняет автонастройку
    /// </summary>
    private const uint SWP_NOSIZE = 0x0001;

    /// <summary>
    /// Поиск дескриптора окна по классу и заголовку
    /// </summary>
    /// <param name="lpClassName">Имя класса окна</param>
    /// <param name="lpWindowName">Имя заголовка окна</param>
    /// <returns></returns>
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    /// <summary>
    /// Поиск дескриптора окна по заголовку
    /// </summary>
    /// <param name="ZeroOnly">Класс окна в более общей функции. Здесь же - всегда должно быть 0</param>
    /// <param name="lpWindowName">Точный заголовок окна</param>
    /// <returns>Дескриптор окна или ZeroPtr при отсутствии</returns>
    [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

    /// <summary>
    /// Установка положения окна
    /// </summary>
    /// <param name="hWnd">Дескриптор окна</param>
    /// <param name="hWndInsertAfter">Для управления Z-индексом</param>
    /// <param name="X">Новая позиция по X</param>
    /// <param name="Y">Новая позиция по Y</param>
    /// <param name="cx">Связано с масштабом, не используется</param>
    /// <param name="cy">Связано с масштабом, не используется</param>
    /// <param name="uFlags">Флаги для управления параметрами окна и размером</param>
    /// <returns>Флаг состояния</returns>
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    /// <summary>
    /// Установка атрибутов отображения окна
    /// </summary>
    /// <param name="hwnd">Дескриптор окна</param>
    /// <param name="crKey">Управление "цветом прозрачности", здесь не используется</param>
    /// <param name="bAlpha">Значение прозрачности</param>
    /// <param name="dwFlags">Флаги управления параметрами</param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    /// <summary>
    /// Ожидание достаточно долгого интервала времени, чтобы системные вызовы и инициализация успели сработать. 
    /// Вызывать при старте, после обновления заголовка (без этого при старте не находится дескриптор окна)
    /// </summary>
    public static void WaitSomeEmpiricalTimeInterval()
    {
      const int LONG_WAIT_MS = 100;
      Task.Delay(LONG_WAIT_MS).Wait();
    }

    /// <summary>
    /// Перемещение окна консоли
    /// </summary>
    /// <param name="parWindowName">Заголовок окна консоли</param>
    /// <param name="parPixelX">Новая X-координата</param>
    /// <param name="parPixelY">Новая Y-координата</param>
    /// <exception cref="Exception">При невозможности найти окно</exception>
    public static void MoveConsoleWindow(string parWindowName, int parPixelX, int parPixelY)
    {
      IntPtr window = FindWindowByCaption(IntPtr.Zero, parWindowName);
      if (window == IntPtr.Zero)
        throw new Exception("Нет окна!");
      SetWindowPos(window, IntPtr.Zero, parPixelX, parPixelY, 0, 0, SWP_NOSIZE);
    }

    /// <summary>
    /// Установка прозрачности окна
    /// </summary>
    /// <param name="parWindowName">Заголовок окна</param>
    /// <param name="parOpacity">Значение прозрачности [0-255]</param>
    /// <exception cref="Exception">При невозможности найти окно</exception>
    public static void SetConsoleOpacity(string parWindowName, byte parOpacity)
    {
      const uint LWA_ALPHA = 0x00000002;

      IntPtr window = FindWindowByCaption(IntPtr.Zero, parWindowName);
      if (window == IntPtr.Zero)
        throw new Exception("Нет окна!");
      SetLayeredWindowAttributes(window, 0, parOpacity, LWA_ALPHA);
    }
  }
}
