using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ViewsConsole.ConsoleHelperUtilite;

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
    /// Координаты
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Coord
    {
      /// <summary>
      /// X
      /// </summary>
      public short X;
      /// <summary>
      /// Y
      /// </summary>
      public short Y;

      /// <summary>
      /// Инициализация
      /// </summary>
      /// <param name="X"></param>
      /// <param name="Y"></param>
      public Coord(short X, short Y)
      {
        this.X = X;
        this.Y = Y;
      }
    };

    /// <summary>
    /// Представление символа
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct CharUnion
    {
      /// <summary>
      /// Поле для доступа как к символу Unicode
      /// </summary>
      [FieldOffset(0)] public char UnicodeChar;
      /// <summary>
      /// Поле для доступа как к символу Ascii
      /// </summary>
      [FieldOffset(0)] public byte AsciiChar;
    }

    /// <summary>
    /// Информация о символе
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct CharInfo
    {
      /// <summary>
      /// Символ
      /// </summary>
      [FieldOffset(0)] public CharUnion Char;
      /// <summary>
      /// Атрибуты
      /// </summary>
      [FieldOffset(2)] public short Attributes;
    }

    /// <summary>
    /// Прямоугольник
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SmallRect
    {
      /// <summary>
      /// Левая сторона
      /// </summary>
      public short Left;
      /// <summary>
      /// Верхняя сторона
      /// </summary>
      public short Top;
      /// <summary>
      /// Правая сторона
      /// </summary>
      public short Right;
      /// <summary>
      /// Нижняя сторона
      /// </summary>
      public short Bottom;
    }

    /// <summary>
    /// Точка
    /// </summary>
    public struct Point
    {
      /// <summary>
      /// X
      /// </summary>
      public int X;
      /// <summary>
      /// Y
      /// </summary>
      public int Y;
    }

    /// <summary>
    /// Перевод из координат экрана в координаты окна с дескриптором <paramref name="hWnd"/>
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="lpPoint"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    private static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

    /// <summary>
    /// Получение положения курсора
    /// </summary>
    /// <param name="lpPoint"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out Point lpPoint);

    /// <summary>
    /// Установка кодировки выхода консоли
    /// </summary>
    /// <param name="wCodePageID"></param>
    /// <returns></returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleOutputCP(uint wCodePageID);

    /// <summary>
    /// Установка кодировки консоли
    /// </summary>
    /// <param name="wCodePageID"></param>
    /// <returns></returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleCP(uint wCodePageID);

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
    /// Создание файла
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="fileAccess"></param>
    /// <param name="fileShare"></param>
    /// <param name="securityAttributes"></param>
    /// <param name="creationDisposition"></param>
    /// <param name="flags"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern SafeFileHandle CreateFile(
        string fileName,
        [MarshalAs(UnmanagedType.U4)] uint fileAccess,
        [MarshalAs(UnmanagedType.U4)] uint fileShare,
        IntPtr securityAttributes,
        [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
        [MarshalAs(UnmanagedType.U4)] int flags,
        IntPtr template);

    /// <summary>
    /// Вывод в консоль массива <paramref name="lpBuffer"/>
    /// </summary>
    /// <param name="hConsoleOutput"></param>
    /// <param name="lpBuffer"></param>
    /// <param name="dwBufferSize"></param>
    /// <param name="dwBufferCoord"></param>
    /// <param name="lpWriteRegion"></param>
    /// <returns></returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteConsoleOutput(
      SafeFileHandle hConsoleOutput,
      CharInfo[] lpBuffer,
      Coord dwBufferSize,
      Coord dwBufferCoord,
      ref SmallRect lpWriteRegion);

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

    /// <summary>
    /// Получение дескриптора окна консоли
    /// </summary>
    /// <param name="parWindowName">Имя окна консоли</param>
    /// <returns></returns>
    public static IntPtr GetConsoleWindowHandle(string parWindowName)
    {
      return FindWindowByCaption(IntPtr.Zero, parWindowName);
    }

    /// <summary>
    /// Получение дескриптора вывода консоли
    /// </summary>
    /// <returns></returns>
    public static SafeFileHandle GetConsoleOutputHandle()
    {
      return CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
    }

    /// <summary>
    /// Быстрый вывод в консоль
    /// </summary>
    /// <param name="parHandle">Дескриптор окна консоли</param>
    /// <param name="parBuffer">Буфер с данными</param>
    /// <param name="parWidth">Ширина буфера</param>
    /// <param name="parHeight">Высота буфера</param>
    public static void PrintToConsoleFast(SafeFileHandle parHandle, CharInfo[] parBuffer, int parWidth, int parHeight)
    {
      SmallRect rect = new() { Left = 0, Top = 0, Right = (short)parWidth, Bottom = (short)parHeight };
      WriteConsoleOutput(parHandle, parBuffer,
              new Coord() { X = (short)parWidth, Y = (short)parHeight },
              new Coord() { X = 0, Y = 0 },
              ref rect);
    }

    /// <summary>
    /// Получение положения курсора мыши
    /// </summary>
    /// <param name="parWindowHandler">Дескриптор окна консоли</param>
    /// <returns>Положение курсора мыши относительно левого верхнего угла содержимого консоли в пикселях</returns>
    public static Point GetCursorPosition(IntPtr parWindowHandler)
    {
      GetCursorPos(out Point position);
      ScreenToClient(parWindowHandler, ref position);
      return position;
    }

    /// <summary>
    /// Смена кодировки консоли
    /// </summary>
    public static void ChangeConsoleCP()
    {
      Console.InputEncoding = Console.OutputEncoding = Encoding.Unicode;
      //SetConsoleOutputCP(65001);
      //SetConsoleCP(65001);
    }
  }
}
