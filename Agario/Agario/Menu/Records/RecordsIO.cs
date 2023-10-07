using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AgarioModels.Menu.Records
{
  /// <summary>
  /// Класс-посредник между файловой системой и игровой моделью. Обеспечивает операции сохранения и получения рекордов, храня их в памяти после первого обращения для кэширования
  /// </summary>
  internal class RecordsIO
  {
    /// <summary>
    /// Класс для сравнения рекордов по убыванию значения
    /// </summary>
    private class DescendingRecordComparer : IComparer<Record>
    {
      /// <summary>
      /// Сравает 2 рекорда по убыванию значения
      /// </summary>
      /// <param name="parX">Рекорд 1</param>
      /// <param name="parY">Рекорд 2</param>
      /// <returns>Положительное число, если значение parY больше parX, отрицательное, если parY меньше parX и 0 в случае равенства</returns>
      public int Compare(Record parX, Record parY) => parY.Value - parX.Value;
    }

    /// <summary>
    /// Имя файла с рекордами
    /// </summary>
    public const string RECORDS_FILE_NAME = "records.txt";

    /// <summary>
    /// Компонент, сравнивающий рекорды для сортировки
    /// </summary>
    private static readonly IComparer<Record> _recordComparer = new DescendingRecordComparer();

    /// <summary>
    /// Количество хранимых рекордов
    /// </summary>
    private int _maxStoredRecordsCount = 10;

    /// <summary>
    /// Сохранённые в "кэше" рекорды. Значение null показывает, что рекорды ещё не были считаны из файловой системы
    /// </summary>
    private List<Record>? _records = null;

    /// <summary>
    /// Количество хранимых рекордов
    /// </summary>
    public int MaxStoredRecordsCount
    {
      get => _maxStoredRecordsCount;
      set
      {
        if (value < 0)
          throw new ArgumentException("Нельзя хранить отрицательное количество рекордов!");
        _maxStoredRecordsCount = value;
      }
    }

    /// <summary>
    /// Сохранённые рекорды. Возвращает рекорды из кэша, обновляя его из файловой системы по необходимости
    /// </summary>
    public List<Record> Records
    {
      get
      {
        _records ??= GetStoredRecords().Take(_maxStoredRecordsCount).ToList();
        return _records;
      }
    }

    /// <summary>
    /// Получение сохранённых рекордов из файловой системы
    /// </summary>
    /// <returns>Сохранённые рекорды</returns>
    private static List<Record> GetStoredRecords()
    {
      List<Record> records = new();
      try
      {
        using StreamReader inputFileStream = new(RECORDS_FILE_NAME);
        string jsonString = inputFileStream.ReadToEnd();
        Record[] recordsArray = JsonSerializer.Deserialize<Record[]>(jsonString) ?? Array.Empty<Record>();
        records.AddRange(recordsArray);
      }
      catch (Exception ex)
      {
        if (!(ex is FileNotFoundException || ex is JsonException))
          throw;
      }
      records.Sort(_recordComparer);
      return records;
    }

    /// <summary>
    /// Проверка необходимости сохранять рекорд. Возвращает true, если количество сохранённых рекордов меньше максимально разрешённого количества,
    /// или если значение рекорда превышает минимальное из уже сохранённых значений (независимо от их количества)
    /// </summary>
    /// <param name="parRecordValue">Значение рекорда, которое проверяется</param>
    /// <returns>True, если можно сохраненить, иначе false</returns>
    public bool IsNeedToStore(int parRecordValue)
    {
      Records.Sort(_recordComparer);
      return Records.Count < _maxStoredRecordsCount || Records[_maxStoredRecordsCount - 1].Value < parRecordValue;
    }

    /// <summary>
    /// Сохранение рекорда. Обновляет рекорды в кэше, сортируя по убыванию, и сразу записывет их в файловую систему
    /// </summary>
    /// <param name="parRecord">Сохраняемый рекорд</param>
    public void SaveRecord(Record parRecord)
    {
      Records.Add(parRecord);
      Records.Sort(_recordComparer);
      _records = Records.Take(_maxStoredRecordsCount).ToList();
      string jsonString = JsonSerializer.Serialize(_records.ToArray());
      using StreamWriter outFileStream = new(RECORDS_FILE_NAME);
      outFileStream.Write(jsonString);
      outFileStream.Close();
    }
  }
}
