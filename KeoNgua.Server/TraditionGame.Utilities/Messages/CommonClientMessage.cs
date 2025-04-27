using TraditionGame.Utilities.Utils;

namespace TraditionGame.Utilities.Messages
{
    public class CommonClientMessage
    {
        public static readonly string UnexpectedException = "Unable to continue the game. Please leave the room";
        public static readonly string NotEnoughMoneyToContinue = "You do not have enough money to continue the game.";
        public static readonly string NoUserAction = "You have taken no action for 15 minutes.";
        public static readonly string BillingError = "The transaction is not successful, the account will be refunded.";

        public static readonly string UserRegLeaveRoom = "Register to leave the room.";
        public static readonly string OtherRegLeaveRoom = "The registered player leaves the room.";
        public static readonly string OtherDisconnected = "Player lost connection.";
        public static readonly string UserDisconnected = "You lost connection.";
        public static readonly string UserPlayOtherDevice = "Your account is playing the game on another device.";
        public static readonly string PlayerExits = "Player exits.";
        public static readonly string PleaseLogin = "Please log in.";
        public static readonly string NoRoomsFound = "Room not found.";

        public static string NotEnoughMoney(long amount, byte type)
        {
            return string.Format("You need at least {0} to continue the game. You need to recharge to play",
                amount.LongToMoneyFormat());
        }

        public static string NoActionDuring(int minute)
        {
            return string.Format("You have taken no action for {0} minutes.", minute);
        }
    }
}