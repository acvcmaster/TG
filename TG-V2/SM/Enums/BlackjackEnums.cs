namespace SM
{
    public enum GameState : int
    {
        InitialState = 0,
        PlayerTurn,
        FinalState
    }
    public enum BlackjackMove : int
    {
        Hit = 0,
        Stand,
        Split,
        DoubleDown
    }
}