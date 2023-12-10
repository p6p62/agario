using AgarioModels.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers.Menu
{
  /// <summary>
  /// Контроллер пункта настройки имени игрока
  /// </summary>
  public abstract class MenuPlayerNameController : MenuScreenController
  {
    /// <summary>
    /// Имя игрока
    /// </summary>
    protected string PlayerName { get; set; } = string.Empty;

    /// <summary>
    /// Изменение имени игрока
    /// </summary>
    protected void ChangePlayerName()
    {
      AgarioGame game = AgarioGame.GetGameInstance();
      string oldPlayerName = game.PlayerName;
      Player? player = game.GameField.Players.Find(p => p.Name == oldPlayerName);
      if (player is not null)
        player.Name = PlayerName;
      game.PlayerName = PlayerName;
    }
  }
}
