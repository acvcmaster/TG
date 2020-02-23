namespace SM
{
    public enum GameState : int
    {
        InitialState = 0,
        PlayerTurn,
        Stand,
        Hit,
        DoubleDown,
        PlayerBlackjack,
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