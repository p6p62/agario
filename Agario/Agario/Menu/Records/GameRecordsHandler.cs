using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgarioModels.Menu.Records
{
  /// <summary>
  /// Обработчик игровых рекордов. Управляет запросом имени пользователя при сохранении после окончания игры
  /// </summary>
  public abstract class GameRecordsHandler
  {
    /// <summary>
    /// Количество рекордов, хранящихся в файловой системе
    /// </summary>
    public const int MAX_STORED_RECORDS_COUNT = 5;

    /// <summary>
    /// Класс-посредник, обеспеспечивающий обращение к файловой системе для операций с рекордами и их кэширование
    /// </summary>
    private static readonly RecordsIO _recordsDataIO = new() { MaxStoredRecordsCount = MAX_STORED_RECORDS_COUNT };

    /// <summary>
    /// Получение набора рекордов из _recordsDataIO
    /// </summary>
    /// <returns>Сохраненные игровые рекорды</returns>
    public static List<Record> GetRecords() => _recordsDataIO.Records;

    /// <summary>
    /// Обработка значения рекорда после окончания игры. Сохраняет рекорд по необходимости, запрашивая при этом имя игрока
    /// </summary>
    /// <param name="parRecordValue">Значение рекорда, набранного игроком</param>
    public static void HandleRecordValueOnEndGame(int parRecordValue)
    {
      if (_recordsDataIO.IsNeedToStore(parRecordValue))
      {
        string playerName = AgarioModels.Game.AgarioGame.GetGameInstance().PlayerName;
        _recordsDataIO.SaveRecord(new(playerName, parRecordValue));
      }
    }
  }
}
