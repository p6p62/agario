using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgarioModels.Menu.Records
{
  [Serializable]
  public struct Record
  {
    /// <summary>
    /// Имя игрока
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Значение рекорда
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Создаёт рекорд с заданным именем и значением
    /// </summary>
    /// <param name="parName">Имя</param>
    /// <param name="parValue">Значение</param>
    public Record(string parName, int parValue)
    {
      Name = parName;
      Value = parValue;
    }
  }
}
