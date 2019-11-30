class SystemTypesEnum {
    Chat: number = 1;
    Combat: number = 2;
    Command: number = 3;
    Inventory: number = 4;
    Join: number = 5;
    Location: number = 6;
    Login: number = 7;
    Map: number = 8;
    Movement: number = 9;
    Store: number = 10;

    public ChatTypes = new class {
        Say: number = 1;
    };

    public LocationTypes = new class {
        Chat: number = 1;
        PlayersUpdate: number = 2;
    };

    public CombatTypes = new class {
        Attack: number = 1;
        AttckerReport: number = 2;
        VictimReport: number = 3;
    };

    public CommandTypes = new class {
        Command: number = 1;
    };

    public JoinTypes = new class {
        Join: number = 1;
    };

    public LoginTypes = new class {
        Login: number = 1;
        Signup: number = 2;
    };

    public MovementTypes = new class {
        Move: number = 1;
        StartMove: number = 2;
        MoveDone: number = 3;
    };
};

export const SystemTypes = new SystemTypesEnum();