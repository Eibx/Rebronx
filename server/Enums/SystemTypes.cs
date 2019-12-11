namespace Rebronx.Server.Enums
{
    public static class SystemTypes
    {
        public const byte Chat = 1;
        public const byte Combat = 2;
        public const byte Command = 3;
        public const byte Inventory = 4;
        public const byte Join = 5;
        public const byte Location = 6;
        public const byte Login = 7;
        public const byte Map = 8;
        public const byte Movement = 9;
        public const byte Store = 10;

        public const byte Ping = 255;

        public static class ChatTypes
        {
            public const byte Say = 1;
        }

        public static class LocationTypes
        {
            public const byte Chat = 1;
            public static byte PlayersUpdate = 2;
        }

        public static class CombatTypes
        {
            public const byte BeginAttack = 1;

            public const byte ChangePosition = 2;
            public const byte ChangeAttack = 3;

            public const byte Report = 4;
        }

        public static class CommandTypes
        {
            public const byte Give = 1;
        }

        public static class JoinTypes
        {
            public const byte Join = 1;
        }

        public class LoginTypes
        {
            public const byte Login = 1;
            public const byte Signup = 2;
        }

        public class MovementTypes
        {
            public const byte Move = 1;
            public const byte StartMove = 2;
            public const byte MoveDone = 3;
        }
    }
}